// <copyright file="RockPegasus.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    using Hearthrock.Contracts;
    using Hearthrock.Engine;
    using PegasusShared;
    using System;
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


            if (index <= 0 || index > m_practiceAIButtons.Count)
            {
                Random r = new Random();
                index = r.Next(0, m_practiceAIButtons.Count);
            } else
            {
                index -= 1;
            }
            m_practiceAIButtons[index].TriggerRelease();


            tracer.Verbose(GetPrivateField<PracticeAIButton>(PracticePickerTrayDisplay.Get(), "m_selectedPracticeAIButton")?.name);
 
        }

        public void PlayPractice()
        {
            PracticePickerTrayDisplay.Get().m_playButton.TriggerRelease();
        }



        public long GetSelectedDeckID()
        {
            // DeckPickerTrayDisplay is used on both Tournament and Practice
            return DeckPickerTrayDisplay.Get().GetSelectedDeckID();
        }


        public void ChooseDeck(int index)
        {
            AdventureSubScenes currentSubScene = AdventureConfig.Get().GetCurrentSubScene();
            if (currentSubScene == AdventureSubScenes.Practice)
            {
                PracticePickerTrayDisplay.Get().Show();
            }

        }


        public void ConfigTournament(bool ranked, bool wild)
        {
            bool is_ranked = Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE);
            if (is_ranked != ranked)
            {
                Options.Get().SetBool(Option.IN_RANKED_PLAY_MODE, ranked);
            }

            bool is_wild = Options.Get().GetBool(Option.IN_WILD_MODE);
            if (is_wild != wild)
            {
                Options.Get().SetBool(Option.IN_RANKED_PLAY_MODE, wild);
            }

        }

        public void PlayTournament()
        {

            DeckPickerTrayDisplay.Get().m_playButton.TriggerRelease();
        }



        public void ChoosePracticeMode(bool expert)
        {
            AdventureDbId adventureId = Options.Get().GetEnum<AdventureDbId>(Option.SELECTED_ADVENTURE, AdventureDbId.PRACTICE);
            AdventureModeDbId modeId = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.NORMAL);
            if (expert)
            {
                modeId = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.EXPERT);
            }

            if (AdventureConfig.Get().CanPlayMode(adventureId, modeId))
            {
                AdventureConfig.Get().SetSelectedAdventureMode(adventureId, modeId);
                AdventureConfig.Get().ChangeSubSceneToSelectedAdventure();
            }
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



        public RockPegasusSubsceneState GetPegasusSubsceneState(RockPegasusSceneState sceneState)
        {
            switch(sceneState)
            {
                case RockPegasusSceneState.Adventure:
                    return GetPegasusAdventureSubsceneState();
                case RockPegasusSceneState.Tournament:
                    return GetPegasusTournamentSubsceneState();
                default:
                    return RockPegasusSubsceneState.None;
            }
        }

        private RockPegasusSubsceneState GetPegasusAdventureSubsceneState()
        {
            if (AdventureConfig.Get() == null)
            {
                return RockPegasusSubsceneState.None;
            }

            AdventureSubScenes currentSubScene = AdventureConfig.Get().GetCurrentSubScene();

            if (currentSubScene == AdventureSubScenes.Chooser)
            {
                return RockPegasusSubsceneState.WaitChooseMode;
            }

            if (currentSubScene == AdventureSubScenes.Practice)
            {
                if (PracticePickerTrayDisplay.Get().IsShown() == false)
                {
                    return RockPegasusSubsceneState.WaitChooseDeck;
                }

                if (GetPrivateField<PracticeAIButton>(PracticePickerTrayDisplay.Get(), "m_selectedPracticeAIButton") == null)
                {
                    return RockPegasusSubsceneState.WaitChooseOpponent;
                }
                else
                {
                    return RockPegasusSubsceneState.Ready;
                }
            }

            return RockPegasusSubsceneState.None;
        }

        private RockPegasusSubsceneState GetPegasusTournamentSubsceneState()
        {
            if (DeckPickerTrayDisplay.Get() == null)
            {
                return RockPegasusSubsceneState.None;
            } else
            {
                return RockPegasusSubsceneState.Ready;
            }
        }

        public RockPegasusSceneState GetPegasusSceneState()
        {
            if (WelcomeQuests.Get() != null)
            {
                return RockPegasusSceneState.QuestsDialog;
            }

            if (DialogManager.Get() != null)
            {
                if (DialogManager.Get().ShowingDialog())
                {
                    return RockPegasusSceneState.GeneralDialog;
                }
            }

            if (Network.Get().IsFindingGame())
            {
                return RockPegasusSceneState.BlockingSceneMode;
            }

            if (GameMgr.Get().IsTransitionPopupShown())
            {
                return RockPegasusSceneState.BlockingSceneMode;
            }


            var sceneMode = SceneMgr.Get().GetMode();

            var pegasusState = RockPegasusHelper.GetPegasusState(sceneMode);

            if (pegasusState == RockPegasusSceneState.GamePlay)
            {
                if (GameState.Get() == null)
                {
                    return RockPegasusSceneState.BlockingSceneMode;
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


        public void ClickObject(int rockId)
        {
            RockPegasusInput.ClickCard(GetObject(rockId).PegasusCard);
        }


        public void DropObject()
        {
            RockPegasusInput.DropCard();
        }


        public RockPegasusObject GetObject(int rockId)
        {
            var card = GetCard(GameState.Get(), rockId);
            if (card == null)
            {
                return null;
            }
            return new RockPegasusObject(card);
        }

        public static Card GetCard(GameState gameState, int rockId)
        {
            return GameState.Get().GetEntity(rockId)?.GetCard();
        }

        public static Entity GetEntity(GameState gameState, int rockId)
        {
            return GameState.Get().GetEntity(rockId);
        }
    }
}
