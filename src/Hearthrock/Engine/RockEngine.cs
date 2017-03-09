// <copyright file="RockEngine.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using System;
    using System.IO;
    using System.Reflection;

    using Hearthrock.Contracts;
    using Hearthrock.Pegasus;
    using Hearthrock.Communication;

    using PegasusShared;

    /// <summary>
    /// This class is the bridge between Pegasus and RockBot.
    /// </summary>
    public class RockEngine
    {
        /// <summary>
        /// The RockConfiguration.
        /// </summary>
        private RockConfiguration configuration;

        /// <summary>
        /// The RockBotClient
        /// </summary>
        private RockBotClient bot;

        /// <summary>
        /// The RockEngineTracer
        /// </summary>
        private RockEngineTracer tracer;

        /// <summary>
        /// The IRockPegasus
        /// </summary>
        private IRockPegasus pegasus;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockEngine" /> class.
        /// </summary>
        public RockEngine()
        {
            this.Reload();
        }

        /// <summary>
        /// Gets current GameMode
        /// </summary>
        public RockGameMode GameMode
        {
            get
            {
                return this.configuration.GameMode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether use local as trace output.
        /// </summary>
        public bool UseLocalTrace
        {
            get
            {
                return string.IsNullOrEmpty(this.configuration.TraceEndpoint);
            }
        }

        /// <summary>
        /// Gets a value indicating whether use built-in bot.
        /// </summary>
        public bool UseBuiltinBot
        {
            get
            {
                return string.IsNullOrEmpty(this.configuration.BotEndpoint);
            }
        }

        /// <summary>
        /// Reload the configuration of RockEngine.
        /// Warning: not thread safe.
        /// </summary>
        public void Reload()
        {
            var configurationString = File.ReadAllText(RockEngineConstants.ConfigurationFilePath);
            this.configuration = RockJsonSerializer.Deserialize<RockConfiguration>(configurationString);
            this.bot = new RockBotClient(this.configuration);
            this.tracer = new RockEngineTracer(this.configuration);
            this.pegasus = new RockPegasus(this.tracer);
        }

        /// <summary>
        /// called every frame
        /// </summary>
        public void Tick()
        {
            this.pegasus.SetActive();

            var pegasusState = this.pegasus.GetSceneMode();
            this.tracer.Verbose(pegasusState.ToString());
        }

        /// <summary>
        /// Display trace info to screen.
        /// </summary>
        /// <param name="message">The trace info.</param>
        public void ShowRockInfo(string message)
        {
            UIStatus.Get().AddInfo(message);
            this.tracer.Info(message);
        }

        /// <summary>
        /// The update method of engine.
        /// </summary>
        /// <returns>Seconds to be delayed before next call.</returns>
        public double Update()
        {
            try
            {
                var pegasusState = this.pegasus.GetSceneMode();

                switch (pegasusState)
                {
                    case RockPegasusState.QuestsDialog:
                        this.pegasus.TryCloseQuests();
                        return 2;
                    case RockPegasusState.GeneralDialog:
                        this.pegasus.TryCloseDialog();
                        return 2;
                    case RockPegasusState.CancelableSceneMode:
                        this.pegasus.NavigateToHub();
                        break;
                    case RockPegasusState.Hub:
                        Clear();
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                            case RockGameMode.ExpertPractice:
                                this.pegasus.NavigateToAdventure();
                                break;
                            case RockGameMode.Casual:
                            case RockGameMode.Ranked:
                            case RockGameMode.WildCasual:
                            case RockGameMode.WildRanked:
                                this.pegasus.NavigateToTournament();
                                Tournament.Get().NotifyOfBoxTransitionStart();
                                break;
                            default:
                                break;
                        }
                        break;
                    case RockPegasusState.Adventure:
                        ClearGameState();
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                                return OnRockPraticeMode(false);
                            case RockGameMode.ExpertPractice:
                                return OnRockPraticeMode(true);
                            case RockGameMode.Casual:
                            case RockGameMode.Ranked:
                            case RockGameMode.WildCasual:
                            case RockGameMode.WildRanked:
                                this.pegasus.NavigateToHub();
                                break;
                            default:
                                this.pegasus.NavigateToHub();
                                break;
                        }
                        break;
                    case RockPegasusState.Tournament:
                        ClearGameState();
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                            case RockGameMode.ExpertPractice:
                                this.pegasus.NavigateToHub();
                                break;
                            case RockGameMode.Casual:
                            case RockGameMode.WildCasual:
                                return OnRockTournamentMode(false);
                            case RockGameMode.Ranked:
                            case RockGameMode.WildRanked:
                                return OnRockTournamentMode(true);
                            default:
                                this.pegasus.NavigateToHub();
                                break;
                        }
                        break;
                    case RockPegasusState.GamePlay:
                        return OnRockGamePlay();
                    case RockPegasusState.InvalidSceneMode:
                    case RockPegasusState.None:
                    default:
                        break;
                }

                return 1;
            }
            catch (Exception e)
            {
                this.tracer.Error(e.ToString());

                return 1;
            }
        }


        bool TurnReady = false;
        private double OnRockGamePlay()
        {
            if (GameMgr.Get().IsTransitionPopupShown())
            {
                this.tracer.Verbose("IsTransitionPopupShown OnRockGamePlay");
                return 1;
            }

            GameState state = GameState.Get();
            if (state == null)
            {
                return 1;
            }
            
            if (state.IsBlockingPowerProcessor())
            {
                this.tracer.Verbose("BlockingServer");
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
                ShowRockInfo("MulliganPhasePending");
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
                var rockAction = this.bot.GetAction(scene);
                if (rockAction != null)
                {
                    this.rockActionContext = new RockActionContext(rockAction);
                    ShowRockInfo(this.rockActionContext.Interpretion(GameState.Get()));
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
            ShowRockInfo("Job's Done!");
            TurnReady = false;
            InputManager.Get().DoEndTurnButton();
            return 3;
        }

        public void Clear()
        {
            ClearGameState();
        }


        private void ClearGameState()
        {
            MulliganState = 0;
            TurnReady = false;
        }

        private double OnRockGameOver()
        {
            if (EndGameScreen.Get() != null)
            {
                ShowRockInfo("Game Over");
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
                ShowRockInfo("Mulligan");
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

            if (GameMgr.Get().IsTransitionPopupShown())
            {
                this.tracer.Verbose("IsTransitionPopupShown");
                return 1;
            }

            if (DeckPickerTrayDisplay.Get() == null)
            {
                this.tracer.Verbose("DeckPickerTrayDisplay.Get() NULL");
                AdventureDbId adventureId = Options.Get().GetEnum<AdventureDbId>(Option.SELECTED_ADVENTURE, AdventureDbId.PRACTICE);
                AdventureModeDbId modeId = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.NORMAL);
                if (expert)
                {
                    modeId = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.EXPERT);
                }
                this.tracer.Verbose("AdventureConfig.Get().GetSelectedMode " + AdventureConfig.Get().GetSelectedMode());

                if (AdventureConfig.Get().CanPlayMode(adventureId, modeId))
                {
                    AdventureConfig.Get().SetSelectedAdventureMode(adventureId, modeId);
                    AdventureConfig.Get().ChangeSubSceneToSelectedAdventure();
                }
                else
                {
                    this.tracer.Verbose("AdventureConfig.Get().CanPlayMode FALSE");
                }

                return 1;
            }
            long deck = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
            if (deck == 0)
            {
                this.tracer.Verbose("DeckPickerTrayDisplay.Get() 0");
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


            ShowRockInfo("FindGame GT_VS_AI");

            this.pegasus.SelectPracticeOpponent(1);
            //GameMgr.Get().FindGame(PegasusShared.GameType.GT_VS_AI, FormatType.FT_WILD, mission, deck, 0L);
            return 1;
        }


        public static void HearthstoneClickCard(Card card)
        {
            InputManager input = InputManager.Get();
            MethodInfo method = input.GetType().GetMethod("HandleClickOnCard", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(input, new object[] { card.gameObject , true});
        }

    }
}
