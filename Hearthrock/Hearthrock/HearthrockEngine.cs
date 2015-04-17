using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Hearthrock
{
    /// <summary>
    /// This class is the bridge between Hearthstone and Hearthrock.
    /// HearthrockEngine will used to do the Artificial Intelligence
    /// </summary>
    class HearthrockEngine
    {
        /// <summary>
        /// ranked, vs_ai and so on
        /// </summary>
        private HEARTHROCK_GAMEMODE GameMode;
        private DateTime delay_start = DateTime.Now;
        private long delay_length = 0;

        public HearthrockEngine()
        {
        }

        public void SwitchMode(int mode)
        {
            // do a simple map
            GameMode = (HEARTHROCK_GAMEMODE) mode;
        }

        /// <summary>
        /// called every frame
        /// </summary>
        public void Tick()
        {
            InactivePlayerKicker ipk = InactivePlayerKicker.Get();
            if (ipk == null)
            {
                Log("InactivePlayerKicker NA");
                return;
            }
            FieldInfo fieldinfo = ipk.GetType().GetField("m_activityDetected", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldinfo.SetValue(ipk, true);
        }

        /// <summary>
        /// this function is called when you want you check sth later
        /// make sure you returned after call this
        /// </summary>
        /// <param name="msec"></param>
        private void HoldBack(long msec)
        {
            delay_start = DateTime.Now;
            delay_length = msec;
        }


        public static void Log(string message)
        {
            Console.WriteLine(DateTime.Now + ": " + message);
        }

        private void Notify(string message)
        {
            Log(message);
            UIStatus.Get().AddInfo(message);
        }

        public static void Message(string message)
        {
            UIStatus.Get().AddInfo(message);
        }

        SceneMgr.Mode scenemgr_mode = SceneMgr.Mode.INVALID;
        private void LogSceneMode(SceneMgr.Mode mode)
        {
            if (scenemgr_mode != mode)
            {
                scenemgr_mode = mode;
                Log(scenemgr_mode.ToString());
            }
        }

        bool SingletonOnUpdate = false;
        public void Update()
        {
            if (SingletonOnUpdate == true) return;
            SingletonOnUpdate = true;

            try
            {
                SceneMgr.Mode mode = SceneMgr.Get().GetMode();
                LogSceneMode(mode);

                DateTime current_time = DateTime.Now;
                TimeSpan time_since_delay = current_time - delay_start;
                if (time_since_delay.TotalMilliseconds < delay_length)
                {
                    SingletonOnUpdate = false;
                    return;
                }
                switch (mode)
                {
                    case SceneMgr.Mode.STARTUP:
                    case SceneMgr.Mode.LOGIN:
                        HoldBack(1000);
                        break;
                    case SceneMgr.Mode.DRAFT:
                        Log("disable for bug");
                        HoldBack(1000);
                        break;
                    case SceneMgr.Mode.COLLECTIONMANAGER:
                    case SceneMgr.Mode.PACKOPENING:
                    case SceneMgr.Mode.FRIENDLY:
                    case SceneMgr.Mode.CREDITS:
                        Log("NEXT MODE" + mode);
                        {
                            if (DialogManager.Get().ShowingDialog())
                            {
                                Log("ShowingDialog");
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
                        HoldBack(3000);
                        Clear();
                        switch (GameMode)
                        {
                            case HEARTHROCK_GAMEMODE.PRACTICE_NORMAL:
                            case HEARTHROCK_GAMEMODE.PRACTICE_EXPERT:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.ADVENTURE);
                                break;
                            case HEARTHROCK_GAMEMODE.PLAY_UNRANKED:
                            case HEARTHROCK_GAMEMODE.PLAY_RANKED:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
                                Tournament.Get().NotifyOfBoxTransitionStart();
                                break;
                            default:
                                break;
                        }
                        break;
                    case SceneMgr.Mode.ADVENTURE:
                        HoldBack(3000);
                        ClearGameState();
                        ClearUIQuest();
                        switch (GameMode)
                        {
                            case HEARTHROCK_GAMEMODE.PRACTICE_NORMAL:
                                OnRockPraticeMode(false);
                                break;
                            case HEARTHROCK_GAMEMODE.PRACTICE_EXPERT:
                                OnRockPraticeMode(true);
                                break;
                            case HEARTHROCK_GAMEMODE.PLAY_UNRANKED:
                            case HEARTHROCK_GAMEMODE.PLAY_RANKED:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                                break;
                            default:
                                break;
                        }
                        break;
                    case SceneMgr.Mode.TOURNAMENT:
                        HoldBack(3000);
                        ClearGameState();
                        ClearUIQuest();
                        switch (GameMode)
                        {
                            case HEARTHROCK_GAMEMODE.PRACTICE_NORMAL:
                            case HEARTHROCK_GAMEMODE.PRACTICE_EXPERT:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                                break;
                            case HEARTHROCK_GAMEMODE.PLAY_UNRANKED:
                                OnRockTournamentMode(false);
                                break;
                            case HEARTHROCK_GAMEMODE.PLAY_RANKED:
                                OnRockTournamentMode(true);
                                break;
                            default:
                                break;
                        }
                        break;
                    case SceneMgr.Mode.GAMEPLAY:
                        SingletonOnGameRequest = false;
                        OnRockGamePlay();
                        break;
                }
            }
            catch (Exception e)
            {
                Notify(e.ToString());
            }
            finally
            {
                SingletonOnUpdate = false;
            }
        }

        bool SingletonOnGameRequest = false;

        bool TurnReady = false;
        private void OnRockGamePlay()
        {
            GameState state = GameState.Get();
            if (state == null) return;
            
            if (state.IsBlockingServer())
            {
                HoldBack(750);
                Log("BlockingServer");
            }
                /*
            else if (state.IsMulliganPhase())
            {
                OnRockMulligan();
                TurnReady = false;
            }
                 * */
            
            else if (state.IsMulliganPhase())
            {
                OnRockMulligan();
                TurnReady = false;
            }
            else if (state.IsMulliganPhasePending())
            {
                // which means some pending about mulligan
                HoldBack(750);
                Notify("MulliganPhasePending");
            }
            else if (state.IsGameOver())
            {
                Clear();
                OnRockGameOver();
            }
            else if (state.IsFriendlySidePlayerTurn() == true)
            {
                if (TurnReady)
                {
                    OnRockAI();
                }
                else
                {
                    OnRockTurnStart();
                }
            }
            else
            {
                TurnReady = false;
            }
        }

        private void OnRockTurnStart()
        {
            HoldBack(5000);
            ActionRocking = null;
            TurnReady = true;
            SingletonEndTurn = false;
            //
            OnRocking = false;
        }
        private void OnRockTurnEnd()
        {
            if (SingletonEndTurn) return;
            SingletonEndTurn = true;
            HoldBack(3000);
            Notify("Job's Done!");
            ActionRocking = null;
            TurnReady = false;
            HearthrockRobot.RockEnd();
            InputManager.Get().DoEndTurnButton();
            //
            OnRocking = false;
        }

        public void Clear()
        {
            SingletonOnUpdate = false;
            SingletonOnGameRequest = false;
            ClearGameState();
            ClearUIQuest();
        }


        private void ClearGameState()
        {
            MulliganState = 0;
            TurnReady = false;
            ActionRocking = null;
            OnRocking = false;
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
                Notify(e.ToString());
            }
        }

        private void OnRockGameOver()
        {
            HoldBack(5000);
            if (EndGameScreen.Get() != null)
            {
                HoldBack(5000);
                Notify("Game Over");
                // EndGameScreen.Get().ContinueEvents();
                try
                {
                    EndGameScreen.Get().m_hitbox.TriggerRelease();
                }
                catch { }
            }
        }
        
        RockAction ActionRocking = null;
        private void OnAction(RockAction action)
        {
            System.Random r = new System.Random();
            
            if (action.step == 0)
            {
                int delay = r.Next(400, 600);
                HoldBack(delay);
                /*
                Spell spell = action.card1.GetBattlecrySpell();
                if (spell != null)
                {
                }
                else
                {
                    Notify("No Spell ");
                }
                */

                //bool x = GameState.Get().HasResponse(action.card1.GetEntity());
                

                HearthstoneClickCard(action.card1);
                action.step = 1;
            }
            else if (action.step == 1)
            {
                int delay = r.Next(300, 600);
                if (action.type == HEARTHROCK_ACTIONTYPE.ATTACK)
                {
                    delay += 400;
                }
                HoldBack(delay);
                if (action.type == HEARTHROCK_ACTIONTYPE.PLAY)
                {
                    InputManager input_mgr = InputManager.Get();
                    input_mgr.DropHeldCard();
                    //MethodInfo dynMethod = input_mgr.GetType().GetMethod("DropHeldCard", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
                    //dynMethod.Invoke(input_mgr, new object[] { });
                    action.step = 2;
                }
                else if (action.type == HEARTHROCK_ACTIONTYPE.ATTACK)
                {
                    HearthstoneClickCard(action.card2);
                    action.step = -1;
                }
            }
            else if (action.step == 2)
            {
                action.step = -1;
                return;
                // maybe can do sth to deal with card with spell
                if (InputManager.Get().heldObject == null)
                {
                    action.step = -1;
                    return;
                }
                int delay = r.Next(300, 600);
                HoldBack(delay);
                if (action.type == HEARTHROCK_ACTIONTYPE.PLAY)
                {
                    action.step = -1;
                }
            }
        }


        bool SingletonEndTurn = false;
        bool OnRocking = false;
        private void OnRockAI()
        {
            if (OnRocking) return;
            OnRocking = true;

            try
            {

                if (ActionRocking != null && ActionRocking.step == -1)
                {
                    ActionRocking = null;
                }
                if (ActionRocking != null)
                {
                    try
                    {
                        OnAction(ActionRocking);
                    }
                    catch
                    {
                        ActionRocking = null;
                    }
                    return;
                }

                // ActionRocking = should be null;

                HoldBack(250);
                if (EndTurnButton.Get().HasNoMorePlays())
                {
                    OnRockTurnEnd();
                    return;
                }

                RockAction action = HearthrockRobot.RockIt();
                if (action.type == HEARTHROCK_ACTIONTYPE.PLAY)
                {
                    SingletonEndTurn = false;
                    Notify("Play: " + action.card1.GetEntity().GetName());
                    ActionRocking = action;
                }
                else if (action.type == HEARTHROCK_ACTIONTYPE.ATTACK)
                {
                    SingletonEndTurn = false;
                    Notify("Attack: " + action.card1.GetEntity().GetName() + " > " + action.card2.GetEntity().GetName());
                    ActionRocking = action;
                }
                else
                {
                    OnRockTurnEnd();
                }

                
            }
            catch (Exception e){
                Log("OnRockAI ex " + e.ToString());
            }
            finally
            {
                OnRocking = false;
            }
        }


        int MulliganState = 0;
        private void OnRockMulligan()
        {
            if (GameState.Get().IsMulliganManagerActive() == false || MulliganManager.Get() == null || MulliganManager.Get().GetMulliganButton() == null || MulliganManager.Get().GetMulliganButton().IsEnabled() == false)
            {
                HoldBack(500);
                return;
            }

            FieldInfo filedinfo = MulliganManager.Get().GetType().GetField("m_waitingForUserInput", BindingFlags.NonPublic | BindingFlags.Instance);
            bool iswaiting = (bool)filedinfo.GetValue(MulliganManager.Get());
            if (!iswaiting)
            {
                HoldBack(500);
                return;
            }
            
            if (MulliganState <= 0)
            {
                Notify("Mulligan");
                HoldBack(1000);
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
                return;
            }
            if (MulliganState <= 1)
            {
                MulliganManager.Get().GetMulliganButton().TriggerRelease();
                MulliganState = 2;
                HoldBack(5000);
                return;
            }
            return;
        }

        private void OnRockTournamentMode(bool ranked)
        {
            if (SingletonOnGameRequest) return;
            SingletonOnGameRequest = true;

            if (SceneMgr.Get().IsInGame() || Network.Get().IsFindingGame())
            {
                HoldBack(1000);
                return;
            }
            if (DeckPickerTrayDisplay.Get() == null)
            {
                HoldBack(1000);
                SingletonOnGameRequest = false;
                return;
            }
            long deck = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
            if (deck == 0)
            {
                HoldBack(1000);
                SingletonOnGameRequest = false;
                return;
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
                SingletonOnGameRequest = false;
                return;
            }

            long selectedDeckID = DeckPickerTrayDisplay.Get().GetSelectedDeckID();


            Network.TrackWhat what;
            PegasusShared.GameType type;
            if (ranked)
            {
                what = Network.TrackWhat.TRACK_PLAY_TOURNAMENT_WITH_CUSTOM_DECK;
                type = PegasusShared.GameType.GT_RANKED;
            }
            else
            {
                what = Network.TrackWhat.TRACK_PLAY_CASUAL_WITH_CUSTOM_DECK;
                type = PegasusShared.GameType.GT_UNRANKED;
            }
            Network.TrackClient(Network.TrackLevel.LEVEL_INFO, what);


            GameMgr.Get().FindGame(type, 2, selectedDeckID, 0L);

            Enum[] args = new Enum[] { PresenceStatus.PLAY_QUEUE };
            PresenceMgr.Get().SetStatus(args);
        }

        private void OnRockPraticeMode(bool expert)
        {
            if (SingletonOnGameRequest) return;
            SingletonOnGameRequest = true;

            if (SceneMgr.Get().IsInGame())
            {
                HoldBack(1000);
                return;
            }
            if (DeckPickerTrayDisplay.Get() == null)
            {
                HoldBack(1000);
                Log("DeckPickerTrayDisplay.Get() NULL");
                SingletonOnGameRequest = false;
                AdventureDbId adventureId = Options.Get().GetEnum<AdventureDbId>(Option.SELECTED_ADVENTURE, AdventureDbId.PRACTICE);
                AdventureModeDbId modeId = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.NORMAL);
                if (expert)
                {
                    modeId = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.EXPERT);
                }
                Log("AdventureConfig.Get().GetSelectedMode " + AdventureConfig.Get().GetSelectedMode());

                if (AdventureConfig.Get().CanPlayMode(adventureId, modeId))
                {
                    AdventureConfig.Get().SetSelectedAdventureMode(adventureId, modeId);
                    AdventureConfig.Get().ChangeSubSceneToSelectedAdventure();
                }
                else
                {
                    Log("AdventureConfig.Get().CanPlayMode FALSE");
                }

                return;
            }
            long deck = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
            if (deck == 0)
            {
                HoldBack(1000);
                Log("DeckPickerTrayDisplay.Get() 0");
                SingletonOnGameRequest = false;
                return;
            }

            HoldBack(5000);
            ScenarioDbId mission = HearthrockUtils.RandomPracticeMission();
            GameMgr.Get().FindGame(PegasusShared.GameType.GT_VS_AI, (int)mission, deck, 0L);
        }


        public static void HearthstoneClickCard(Card card)
        {
            InputManager input = InputManager.Get();
            MethodInfo method = input.GetType().GetMethod("HandleClickOnCard", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(input, new object[] { card.gameObject , true});
        }
    }
}
