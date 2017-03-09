// <copyright file="RockPegasusClient.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    using Hearthrock.Contracts;
    using Hearthrock.Engine;
    using PegasusShared;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public class RockPegasus : IRockPegasus
    {

        RockEngineTracer tracer;

        public RockPegasus(RockEngineTracer tracer)
        {
            this.tracer = tracer;
        }

        public void SetActive()
        {
            InactivePlayerKicker ipk = InactivePlayerKicker.Get();
            if (ipk == null)
            {
                // InactivePlayerKicker seesm to be used to make sure use is alive, but still need to confirm this.
                this.tracer.Verbose("InactivePlayerKicker is not available.");
                return;
            }

            FieldInfo fieldinfo = ipk.GetType().GetField("m_activityDetected", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldinfo.SetValue(ipk, true);
        }

        public void TrySetSceneMode(SceneMgr.Mode mode)
        {
            SceneMgr.Get().SetNextMode(mode);
        }


        public void EndTurn()
        {
            InputManager.Get().DoEndTurnButton();
        }


        public void TryFinishEndGame()
        {
            if (EndGameScreen.Get() != null)
            {
                try
                {
                    EndGameScreen.Get().m_hitbox.TriggerRelease();
                }
                catch { }
            }
        }


        public void SelectPracticeOpponent(int index)
        {
            tracer.Verbose(GetPrivateField<PracticeAIButton>(PracticePickerTrayDisplay.Get(), "m_selectedPracticeAIButton")?.name);

            List<PracticeAIButton> m_practiceAIButtons = GetPrivateField<List<PracticeAIButton>>(PracticePickerTrayDisplay.Get(), "m_practiceAIButtons");
            m_practiceAIButtons[0].TriggerRelease();


            tracer.Verbose(GetPrivateField<PracticeAIButton>(PracticePickerTrayDisplay.Get(), "m_selectedPracticeAIButton")?.name);
            PracticePickerTrayDisplay.Get().m_playButton.TriggerRelease();
        }


        private static T GetPrivateField<T>(object obj, string field)
        {
            FieldInfo fieldinfo = obj.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            T m_practiceAIButtons = (T)fieldinfo.GetValue(obj);

            return m_practiceAIButtons;
        }


        public bool TryCloseDialog()
        {
            if (DialogManager.Get() == null)
            {
                return false;
            }

            if (DialogManager.Get().ShowingDialog())
            {
                DialogManager.Get().GoBack();
                return true;
            }

            return false;
        }



        public bool TryCloseQuests()
        {
            WelcomeQuests wq = WelcomeQuests.Get();
            if (wq != null)
            {
                wq.m_clickCatcher.TriggerRelease();
                return true;
            }

            return false;
        }

        public RockPegasusState GetSceneMode()
        {
            if (WelcomeQuests.Get() != null)
            {
                return RockPegasusState.QuestsDialog;
            }

            if (DialogManager.Get() != null)
            {
                if (DialogManager.Get().ShowingDialog())
                {
                    return RockPegasusState.GeneralDialog;
                }
            }

            if (Network.Get().IsFindingGame())
            {
                return RockPegasusState.BlockingSceneMode;
            }

            if (GameMgr.Get().IsTransitionPopupShown())
            {
                return RockPegasusState.BlockingSceneMode;
            }


            var sceneMode = SceneMgr.Get().GetMode();

            var pegasusState = RockPegasusHelper.GetPegasusState(sceneMode);

            if (pegasusState == RockPegasusState.GamePlay)
            {
                if (GameState.Get() == null)
                {
                    return RockPegasusState.BlockingSceneMode;
                }
            }

            return pegasusState;

        }
        public RockPegasusGameState GetPegasusGameState()
        {
            GameState state = GameState.Get();

            if (state.IsBlockingPowerProcessor())
            {
                return RockPegasusGameState.Blocking;
            }
            if (state.IsBusy())
            {
                return RockPegasusGameState.Blocking;
            }
            else if (state.IsMulliganPhase())
            {
                if (state.IsMulliganManagerActive() == false || MulliganManager.Get() == null || MulliganManager.Get().GetMulliganButton() == null || MulliganManager.Get().GetMulliganButton().IsEnabled() == false)
                {
                    return RockPegasusGameState.Blocking;
                }

                FieldInfo filedinfo = MulliganManager.Get().GetType().GetField("m_waitingForUserInput", BindingFlags.NonPublic | BindingFlags.Instance);
                bool iswaiting = (bool)filedinfo.GetValue(MulliganManager.Get());
                if (!iswaiting)
                {
                    return RockPegasusGameState.Blocking;
                }

                return RockPegasusGameState.WaitForMulligan;
            }
            else if (state.IsMulliganPhasePending())
            {
                return RockPegasusGameState.Blocking;
            }
            else if (state.IsGameOver())
            {
                return RockPegasusGameState.GameOver;
            }
            else if (state.IsFriendlySidePlayerTurn() == true)
            {
                if (EndTurnButton.Get().IsInWaitingState())
                {
                    return RockPegasusGameState.Blocking;
                }

                return RockPegasusGameState.WaitForAction;
            }

            return RockPegasusGameState.None;
        }

        public void NavigateToHub()
        {
            SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        }

        public void NavigateToTournament()
        {
            SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
        }

        public void NavigateToAdventure()
        {
            SceneMgr.Get().SetNextMode(SceneMgr.Mode.ADVENTURE);
        }

        /// <summary>
        /// The method to FindGame.
        /// </summary>
        /// <param name="gameMode"></param>
        /// <param name="deckId"></param>
        /// <param name="missionId"></param>
        public static void FindGame(RockGameMode gameMode, long deckId, int missionId)
        {
            Options.Get().SetBool(Option.HAS_PLAYED_EXPERT_AI, true);
            GameType gameType = RockPegasusHelper.GetGameType(gameMode);
            FormatType formatType = RockPegasusHelper.GetFormatType(gameMode);

            GameMgr.Get().FindGame(gameType, formatType, missionId, deckId, 0L);
        }
    }
}
