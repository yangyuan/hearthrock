using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HearthRock
{
    class HearthRock : MonoBehaviour
    {
        private int RockState = 0;
        private int RockMode = 0;
        private DateTime delay_start = DateTime.Now;
        private long delay_length = 0;

        public HearthRock()
        {
            RockState = 0;
            RockMode = 0;
            HearthRockEngine.RockGameMode = ROCK_GAME_MODE.PRACTICE_NORMAL;
        }
        public static void Hook()
        {
            GameObject sceneObject = SceneMgr.Get().gameObject;
            sceneObject.AddComponent<HearthRock>();
        }

        private String[] RockModes = { "PRACTICE NM", "PRACTICE EX", "PLAY UNRANK", "PLAY RANKED" };
        void OnGUI()
        {
            Rect windowRect = new Rect(Screen.width - 136, 8, 128, 164);
            GUI.ModalWindow(0, windowRect, OnHearthRockWindow, "HearthRock");
        }
        void OnHearthRockWindow(int windowID)
        {
            RockMode = GUI.SelectionGrid(new Rect(10, 24, 108, 100), RockMode, RockModes, 1);
            HearthRockEngine.RockGameMode = (ROCK_GAME_MODE) RockMode;
            String buttontext = (RockState == 0)? "Paused": "Running";
            if (GUI.Button(new Rect(10, 128, 108, 26), buttontext))
            {
                if (RockState == 0)
                {
                    RockState = 1;
                    UIStatus.Get().AddInfo("HearthRock Started " + RockModes[RockMode]);
                }
                else
                {
                    RockState = 0;
                    UIStatus.Get().AddInfo("HearthRock Paused");
                }
                ClearUI();
            }
        }

        private void HoldBack(long msec)
        {
            delay_start = DateTime.Now;
            delay_length = msec;
        }

        public void NoInactivePlayerKicker()
        {
            InactivePlayerKicker ipk = InactivePlayerKicker.Get();
            FieldInfo fieldinfo = ipk.GetType().GetField("m_activityDetected", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldinfo.SetValue(ipk, true);
        }

        private void Log(string message)
        {
            print(DateTime.Now + ": " + message);
        }

        private void Notify(string message)
        {
            Log(message);
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
            if (RockState == 0) return;
            if (SingletonOnUpdate == true) return;
            SingletonOnUpdate = true;

            NoInactivePlayerKicker();


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
                        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                        break;
                    case SceneMgr.Mode.INVALID:
                    case SceneMgr.Mode.FATAL_ERROR:
                    case SceneMgr.Mode.RESET:
                    default:
                        break;
                    case SceneMgr.Mode.HUB:
                        HoldBack(3000);
                        ClearUI();
                        switch (HearthRockEngine.RockGameMode)
                        {
                            case ROCK_GAME_MODE.PRACTICE_NORMAL:
                            case ROCK_GAME_MODE.PRACTICE_EXPERT:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.PRACTICE);
                                break;
                            case ROCK_GAME_MODE.PLAY_UNRANKED:
                            case ROCK_GAME_MODE.PLAY_RANKED:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
                                Tournament.Get().NotifyOfBoxTransitionStart();
                                break;
                            default:
                                break;
                        }
                        break;
                    case SceneMgr.Mode.PRACTICE:
                        HoldBack(3000);
                        ClearGameState();
                        ClearUIQuest();
                        switch (HearthRockEngine.RockGameMode)
                        {
                            case ROCK_GAME_MODE.PRACTICE_NORMAL:
                                OnRockPraticeMode(false);
                                break;
                            case ROCK_GAME_MODE.PRACTICE_EXPERT:
                                OnRockPraticeMode(true);
                                break;
                            case ROCK_GAME_MODE.PLAY_UNRANKED:
                            case ROCK_GAME_MODE.PLAY_RANKED:
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
                        switch (HearthRockEngine.RockGameMode)
                        {
                            case ROCK_GAME_MODE.PRACTICE_NORMAL:
                            case ROCK_GAME_MODE.PRACTICE_EXPERT:
                                SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                                break;
                            case ROCK_GAME_MODE.PLAY_UNRANKED:
                                OnRockTournamentMode(false);
                                break;
                            case ROCK_GAME_MODE.PLAY_RANKED:
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
                Notify("BlockingServer");
            }
            else if (state.IsMulliganPhase())
            {
                OnRockMulligan();
                TurnReady = false;
            }
            else if (state.IsGameOver())
            {
                ClearUI();
                OnRockGameOver();
            }
            else if (state.IsLocalPlayerTurn() == true)
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
            HearthRockEngine.RockEnd();
            InputManager.Get().DoEndTurnButton();
            //
            OnRocking = false;
        }

        private void ClearUI()
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
                

                RockClickCard(action.card1);
                action.step = 1;
            }
            else if (action.step == 1)
            {
                int delay = r.Next(300, 600);
                if (action.type == ROCK_ACTION_TYPE.ATTACK)
                {
                    delay += 400;
                }
                HoldBack(delay);
                if (action.type == ROCK_ACTION_TYPE.PLAY)
                {
                    InputManager input_mgr = InputManager.Get();
                    MethodInfo dynMethod = input_mgr.GetType().GetMethod("DropHeldCard", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
                    dynMethod.Invoke(input_mgr, new object[] { });
                    action.step = -1;
                }
                else if (action.type == ROCK_ACTION_TYPE.ATTACK)
                {
                    RockClickCard(action.card2);
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

                HoldBack(250);
                if (EndTurnButton.Get().HasNoMorePlays())
                {
                    OnRockTurnEnd();
                    return;
                }

                RockAction action = HearthRockEngine.RockIt();
                if (action.type == ROCK_ACTION_TYPE.PLAY)
                {
                    SingletonEndTurn = false;
                    Notify("Play: " + action.card1.GetEntity().GetName());
                    ActionRocking = action;
                }
                else if (action.type == ROCK_ACTION_TYPE.ATTACK)
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
                Card[] cards = GameState.Get().GetLocalPlayer().GetHandZone().GetCards().ToArray();
                foreach (Card current in cards)
                {
                    if (current.GetEntity().GetCardId() == "GAME_005") continue;
                    //Log(current.GetEntity().GetName());
                    if (current.GetEntity().GetCost() > 4)
                    {
                        RockClickCard(current);
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

            if (SceneMgr.Get().IsInGame() || Network.IsMatching())
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
            MissionID missionID = MissionID.MULTIPLAYER_1v1;
            GameMode mode = ranked ? GameMode.RANKED_PLAY : GameMode.UNRANKED_PLAY;
            GameMgr.Get().SetNextGame(mode, missionID);
            if (ranked)
            {
                Network.TrackClient(Network.TrackLevel.LEVEL_INFO, Network.TrackWhat.TRACK_PLAY_TOURNAMENT_WITH_CUSTOM_DECK);
                Network.RankedMatch(selectedDeckID);
            }
            else
            {
                Network.TrackClient(Network.TrackLevel.LEVEL_INFO, Network.TrackWhat.TRACK_PLAY_CASUAL_WITH_CUSTOM_DECK);
                Network.UnrankedMatch(selectedDeckID);
            }
            FriendChallengeMgr.Get().OnEnteredMatchmakerQueue();
            GameMgr.Get().UpdatePresence();
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

            DeckPickerTrayDisplay.Get().GetSelectedDeckID();

            HoldBack(5000);
            MissionID mission = HearthRockEngine.RandomAIMissionID(expert);

            Notify("PraticeMode: Deck " + deck + "Mission " + mission);
            GameMgr.Get().StartGame(GameMode.PRACTICE, mission, deck);
            GameMgr.Get().UpdatePresence();
        }


        public static void RockClickCard(Card card)
        {
            InputManager input = InputManager.Get();
            MethodInfo method = input.GetType().GetMethod("HandleClickOnCard", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(input, new object[] { card.gameObject });
        }

    }

}
