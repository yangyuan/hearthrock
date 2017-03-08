// <copyright file="RockEngine.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using System;
    using System.IO;
    using System.Reflection;

    using PegasusShared;

    using Hearthrock.Contracts;
    using Hearthrock.Serialization;
    using Hearthrock.Pegasus;

    /// <summary>
    /// This class is the bridge between Pegasus and RockBot.
    /// </summary>
    class RockEngine
    {
        /// <summary>
        /// The RockConfiguration.
        /// </summary>
        private RockConfiguration configuration;

        /// <summary>
        /// The RockBotClient
        /// </summary>
        private RockBotClient botClient;

        /// <summary>
        /// The RockEngineTracer
        /// </summary>
        private RockEngineTracer engineTracer;

        public RockEngine()
        {
            this.Reload();
        }

        public void Reload()
        {
            var configurationString = File.ReadAllText("Hearthstone_Data\\Managed\\hearthrock.json");
            this.configuration = RockJsonSerializer.Deserialize<RockConfiguration>(configurationString);
            this.botClient = new RockBotClient(this.configuration);
            this.engineTracer = new RockEngineTracer();
        }

        public RockGameMode GameMode
        {
            get
            {
                return this.configuration.GameMode;
            }
        }

        public bool UseLocalTrace
        {
            get
            {
                return string.IsNullOrEmpty(this.configuration.TraceEndpoint);
            }
        }

        public bool UseBuildinBot
        {
            get
            {
                return string.IsNullOrEmpty(this.configuration.BotEndpoint);
            }
        }

        /// <summary>
        /// called every frame
        /// </summary>
        public void Tick()
        {
            InactivePlayerKicker ipk = InactivePlayerKicker.Get();
            if (ipk == null)
            {
                Trace("InactivePlayerKicker NA");
                return;
            }
            FieldInfo fieldinfo = ipk.GetType().GetField("m_activityDetected", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldinfo.SetValue(ipk, true);
        }

        /// <summary>
        /// Display trace info to screen.
        /// </summary>
        /// <param name="message">The trace info.</param>
        public void RockInfo(string message)
        {
            UIStatus.Get().AddInfo(message);
            this.Trace(message);
        }

        /// <summary>
        /// Display trace info to screen.
        /// </summary>
        /// <param name="message">The trace info.</param>
        public void Trace(string message)
        {
            this.botClient.SendTrace(message);
        }

        /// <summary>
        /// The update method of engine.
        /// Each time the engine will check the state and do responding behavior.
        /// Cannot use yield because user may change states at any time.
        /// </summary>
        public double Update()
        {
            RockGameMode gameMode = this.configuration.GameMode;

            try
            {
                SceneMgr.Mode sceneMode = SceneMgr.Get().GetMode();
                engineTracer.TraceSceneMode(sceneMode);

                switch (sceneMode)
                {
                    case SceneMgr.Mode.STARTUP:
                    case SceneMgr.Mode.LOGIN:
                        return 1;
                    case SceneMgr.Mode.DRAFT:
                        Trace("disable for bug");
                        return 1;
                    case SceneMgr.Mode.COLLECTIONMANAGER:
                    case SceneMgr.Mode.PACKOPENING:
                    case SceneMgr.Mode.FRIENDLY:
                    case SceneMgr.Mode.CREDITS:
                        Trace("NEXT MODE" + sceneMode);
                        {
                            if (DialogManager.Get().ShowingDialog())
                            {
                                Trace("ShowingDialog");
                                DialogManager.Get().GoBack();
                            }
                        }
                        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                        break;
                    case SceneMgr.Mode.INVALID:
                    case SceneMgr.Mode.FATAL_ERROR:
                    case SceneMgr.Mode.RESET:
                    default:
                        break;
                    case SceneMgr.Mode.HUB:
                        Clear();
                        switch (gameMode)
                        {
                            case RockGameMode.NormalPractice:
                            case RockGameMode.ExpertPractice:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.ADVENTURE);
                                break;
                            case RockGameMode.Casual:
                            case RockGameMode.Ranked:
                            case RockGameMode.WildCasual:
                            case RockGameMode.WildRanked:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
                                Tournament.Get().NotifyOfBoxTransitionStart();
                                break;
                            default:
                                break;
                        }
                        break;
                    case SceneMgr.Mode.ADVENTURE:
                        ClearGameState();
                        ClearUIQuest();
                        switch (gameMode)
                        {
                            case RockGameMode.NormalPractice:
                                return OnRockPraticeMode(false);
                            case RockGameMode.ExpertPractice:
                                return OnRockPraticeMode(true);
                            case RockGameMode.Casual:
                            case RockGameMode.Ranked:
                            case RockGameMode.WildCasual:
                            case RockGameMode.WildRanked:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                                break;
                            default:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                                break;
                        }
                        break;
                    case SceneMgr.Mode.TOURNAMENT:
                        ClearGameState();
                        ClearUIQuest();
                        switch (gameMode)
                        {
                            case RockGameMode.NormalPractice:
                            case RockGameMode.ExpertPractice:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                                break;
                            case RockGameMode.Casual:
                            case RockGameMode.WildCasual:
                                return OnRockTournamentMode(false);
                            case RockGameMode.Ranked:
                            case RockGameMode.WildRanked:
                                return OnRockTournamentMode(true);
                            default:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                                break;
                        }
                        break;
                    case SceneMgr.Mode.GAMEPLAY:
                        return OnRockGamePlay();
                }
            }
            catch (Exception e)
            {
                RockInfo(e.ToString());
            }
            finally
            {
            }

            return 1;
        }


        bool TurnReady = false;
        private double OnRockGamePlay()
        {
            GameState state = GameState.Get();
            if (state == null)
            {
                return 1;
            }
            
            if (state.IsBlockingPowerProcessor())
            {
                Trace("BlockingServer");
                return 0.75;
            }
            else if (state.IsMulliganPhase())
            {
                TurnReady = false;
                return OnRockMulligan();
            }
            else if (state.IsMulliganPhasePending())
            {
                // which means some pending about mulligan
                RockInfo("MulliganPhasePending");
                return 0.75;
            }
            else if (state.IsGameOver())
            {
                Clear();
                return OnRockGameOver();
            }
            else if (state.IsFriendlySidePlayerTurn() == true)
            {
                if (TurnReady)
                {
                    return OnRockAI2();
                }
                else
                {
                    return OnRockTurnStart();
                }
            }
            else
            {
                TurnReady = false;
                return 1;
            }
        }

        private RockActionContext rockActionContext;

        private double OnRockAI2()
        {
            if (EndTurnButton.Get().HasNoMorePlays())
            {
                OnRockTurnEnd();
                this.rockActionContext = null;
                return 0.25;
            }

            if (this.rockActionContext == null || this.rockActionContext.IsDone() || this.rockActionContext.IsInvalid(GameState.Get()))
            {
                var scene = RockSnapshotter.SnapshotScene(GameState.Get());
                var rockAction = this.botClient.GetAction(scene);
                if (rockAction != null)
                {
                    this.rockActionContext = new RockActionContext(rockAction);
                    RockInfo(this.rockActionContext.Interpretion(GameState.Get()));
                }
                else
                {
                    OnRockTurnEnd();
                    return 0.25;
                }
            }

            this.rockActionContext.Apply(GameState.Get(), this);
            return 1;
        }

        private float OnRockTurnStart()
        {
            TurnReady = true;

            return 5;
        }
        private float OnRockTurnEnd()
        {
            RockInfo("Job's Done!");
            TurnReady = false;
            InputManager.Get().DoEndTurnButton();
            return 3;
        }

        public void Clear()
        {
            ClearGameState();
            ClearUIQuest();
        }


        private void ClearGameState()
        {
            MulliganState = 0;
            TurnReady = false;
        }

        private void ClearUIQuest()
        {
            try
            {
                WelcomeQuests wq = WelcomeQuests.Get();
                if (wq != null) wq.m_clickCatcher.TriggerRelease();
            }
            catch (Exception e)
            {
                RockInfo(e.ToString());
            }
        }

        private double OnRockGameOver()
        {
            if (EndGameScreen.Get() != null)
            {
                RockInfo("Game Over");
                // EndGameScreen.Get().ContinueEvents();
                try
                {
                    EndGameScreen.Get().m_hitbox.TriggerRelease();
                }
                catch { }
            }

            return 5;
        }
        

        int MulliganState = 0;
        private double OnRockMulligan()
        {
            if (GameState.Get().IsMulliganManagerActive() == false || MulliganManager.Get() == null || MulliganManager.Get().GetMulliganButton() == null || MulliganManager.Get().GetMulliganButton().IsEnabled() == false)
            {
                return 0.5;
            }

            FieldInfo filedinfo = MulliganManager.Get().GetType().GetField("m_waitingForUserInput", BindingFlags.NonPublic | BindingFlags.Instance);
            bool iswaiting = (bool)filedinfo.GetValue(MulliganManager.Get());
            if (!iswaiting)
            {
                return 0.5;
            }
            
            if (MulliganState <= 0)
            {
                RockInfo("Mulligan");
                Card[] cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards().ToArray();
                foreach (Card current in cards)
                {
                    if (current.GetEntity().GetCardId() == "GAME_005") continue;
                    if (current.GetEntity().GetCost() > 4)
                    {
                        HearthstoneClickCard(current);
                    }
                }
                MulliganState = 1;
                return 1;
            }
            if (MulliganState <= 1)
            {
                MulliganManager.Get().GetMulliganButton().TriggerRelease();
                MulliganState = 2;
                return 5;
            }
            return 0.1;
        }

        private double OnRockTournamentMode(bool ranked)
        {
            if (SceneMgr.Get().IsInGame() || Network.Get().IsFindingGame())
            {
                return 1;
            }
            if (DeckPickerTrayDisplay.Get() == null)
            {
                return 1;
            }
            long deck = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
            if (deck == 0)
            {
                return 1;
            }
            /*
            DeckPickerTrayDisplay.Get().GetSelectedDeckID();

            HoldBack(5000);
            MissionID mission = HearthRockEngine.RandomAIMissionID(expert);

            Notify("PraticeMode: Deck " + deck + "Mission " + mission);
            GameMgr.Get().StartGame(GameMode.PRACTICE, mission, deck);
            GameMgr.Get().UpdatePresence();
             * */

            bool is_ranked = Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE);
            if (is_ranked != ranked)
            {
                Options.Get().SetBool(Option.IN_RANKED_PLAY_MODE, ranked);
            }

            long selectedDeckID = DeckPickerTrayDisplay.Get().GetSelectedDeckID();


            //Network.TrackWhat what;
            PegasusShared.GameType type;
            if (ranked)
            {
                //what = Network.TrackWhat.TRACK_PLAY_TOURNAMENT_WITH_CUSTOM_DECK;
                type = PegasusShared.GameType.GT_RANKED;
            }
            else
            {
                //what = Network.TrackWhat.TRACK_PLAY_CASUAL_WITH_CUSTOM_DECK;
                type = PegasusShared.GameType.GT_CASUAL;
            }
            //Network.TrackClient(Network.TrackLevel.LEVEL_INFO, what);


            GameMgr.Get().FindGame(type, FormatType.FT_STANDARD, 2, selectedDeckID, 0L);

            Enum[] args = new Enum[] { PresenceStatus.PLAY_QUEUE };
            PresenceMgr.Get().SetStatus(args);

            return 1;
        }

        private double OnRockPraticeMode(bool expert)
        {
            if (SceneMgr.Get().IsInGame())
            {
                return 1;
            }
            if (DeckPickerTrayDisplay.Get() == null)
            {
                Trace("DeckPickerTrayDisplay.Get() NULL");
                AdventureDbId adventureId = Options.Get().GetEnum<AdventureDbId>(Option.SELECTED_ADVENTURE, AdventureDbId.PRACTICE);
                AdventureModeDbId modeId = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.NORMAL);
                if (expert)
                {
                    modeId = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.EXPERT);
                }
                Trace("AdventureConfig.Get().GetSelectedMode " + AdventureConfig.Get().GetSelectedMode());

                if (AdventureConfig.Get().CanPlayMode(adventureId, modeId))
                {
                    AdventureConfig.Get().SetSelectedAdventureMode(adventureId, modeId);
                    AdventureConfig.Get().ChangeSubSceneToSelectedAdventure();
                }
                else
                {
                    Trace("AdventureConfig.Get().CanPlayMode FALSE");
                }

                return 1;
            }
            long deck = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
            if (deck == 0)
            {
                Trace("DeckPickerTrayDisplay.Get() 0");
                return 1;
            }


            AdventureSubScenes currentSubScene = AdventureConfig.Get().GetCurrentSubScene();
            if (currentSubScene == AdventureSubScenes.Practice)
            {
                PracticePickerTrayDisplay.Get().Show();
            }
     //       if (currentSubScene == AdventureSubScenes.MissionDeckPicker)
     //       {
     //           GameMgr.Get().FindGame(GameType.GT_VS_AI, formatType, (int)adventureConfig.GetMission(), selectedDeckID3, 0L);
     //       }


            int mission = RockPegasusHelper.GetPracticeMissionId(0);


            RockInfo("Mulligan");

            GameMgr.Get().FindGame(PegasusShared.GameType.GT_VS_AI, FormatType.FT_STANDARD, mission, deck, 0L);
            return 5;
        }


        public static void HearthstoneClickCard(Card card)
        {
            InputManager input = InputManager.Get();
            MethodInfo method = input.GetType().GetMethod("HandleClickOnCard", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(input, new object[] { card.gameObject , true});
        }

    }
}
