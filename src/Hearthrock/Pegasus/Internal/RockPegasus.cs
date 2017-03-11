// <copyright file="RockPegasus.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Hearthrock.Contracts;
    using Hearthrock.Diagnostics;
    using PegasusShared;

    /// <summary>
    /// The implementation of IRockPegasus
    /// </summary>
    public class RockPegasus : IRockPegasus
    {
        /// <summary>
        /// The RockTracer.
        /// </summary>
        private RockTracer tracer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockPegasus" /> class.
        /// </summary>
        /// <param name="tracer">The RockTracer.</param>
        public RockPegasus(RockTracer tracer)
        {
            this.tracer = tracer;
        }

        /// <summary>
        /// Trigger some activity to make user looks active.
        /// </summary>
        public void TriggerUserActive()
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

        /// <summary>
        /// Close a general dialog if there is one.
        /// </summary>
        /// <returns>return false if there is no general dialog.</returns>
        public bool DoCloseGeneralDialog()
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

        /// <summary>
        /// Close a quests dialog if there is one.
        /// </summary>
        /// <returns>return false if there is no quests dialog.</returns>
        public bool DoCloseQuestsDialog()
        {
            WelcomeQuests wq = WelcomeQuests.Get();
            if (wq != null)
            {
                wq.m_clickCatcher.TriggerRelease();
                return true;
            }

            return false;
        }

        /// <summary>
        /// End current turn.
        /// </summary>
        public void DoEndTurn()
        {
            InputManager.Get().DoEndTurnButton();
        }

        /// <summary>
        /// End a finished game.
        /// </summary>
        public void DoEndFinishedGame()
        {
            if (EndGameScreen.Get() != null)
            {
                try
                {
                    EndGameScreen.Get().m_hitbox.TriggerRelease();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Get current Pegasus Scene State.
        /// </summary>
        /// <returns>The RockPegasusSceneState.</returns>
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
                return RockPegasusSceneState.BlockingScene;
            }

            if (GameMgr.Get().IsTransitionPopupShown())
            {
                return RockPegasusSceneState.BlockingScene;
            }

            var sceneMode = SceneMgr.Get().GetMode();

            var pegasusState = RockPegasusHelper.GetPegasusSceneState(sceneMode);

            if (pegasusState == RockPegasusSceneState.GamePlay)
            {
                if (GameState.Get() == null)
                {
                    return RockPegasusSceneState.BlockingScene;
                }
            }

            return pegasusState;
        }

        /// <summary>
        /// Get current Pegasus Subscene State.
        /// </summary>
        /// <param name="sceneState">The current RockPegasusSceneState.</param>
        /// <returns>The RockPegasusSubsceneState.</returns>
        public RockPegasusSubsceneState GetPegasusSubsceneState(RockPegasusSceneState sceneState)
        {
            switch (sceneState)
            {
                case RockPegasusSceneState.AdventureScene:
                    return this.GetPegasusAdventureSubsceneState();
                case RockPegasusSceneState.TournamentScene:
                    return this.GetPegasusTournamentSubsceneState();
                default:
                    return RockPegasusSubsceneState.None;
            }
        }

        /// <summary>
        /// Get current Pegasus Game State.
        /// </summary>
        /// <returns>The RockPegasusGameState.</returns>
        public RockPegasusGameState GetPegasusGameState()
        {
            GameState state = GameState.Get();

            if (state.IsBusy())
            {
                return RockPegasusGameState.Blocking;
            }
            else if (state.IsGameOver())
            {
                return RockPegasusGameState.GameOver;
            }
            else if(state.IsResponsePacketBlocked())
            {
                return RockPegasusGameState.Blocking;
            }
            else if (state.IsBlockingPowerProcessor())
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
            else if (state.IsFriendlySidePlayerTurn() == true)
            {
                if (EndTurnButton.Get().IsInWaitingState())
                {
                    return RockPegasusGameState.Blocking;
                }

                return RockPegasusGameState.WaitForPlay;
            }

            return RockPegasusGameState.None;
        }

        /// <summary>
        /// Navigate to Hub Scene.
        /// </summary>
        public void NavigateToHubScene()
        {
            SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        }

        /// <summary>
        /// Navigate to Tournament Scene.
        /// </summary>
        public void NavigateToTournamentScene()
        {
            SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
        }

        /// <summary>
        /// Navigate to Adventure Scene.
        /// </summary>
        public void NavigateToAdventureScene()
        {
            SceneMgr.Get().SetNextMode(SceneMgr.Mode.ADVENTURE);
        }

        /// <summary>
        /// Start a practice (PVE) game.
        /// </summary>
        public void PlayPracticeGame()
        {
            PracticePickerTrayDisplay.Get().m_playButton.TriggerRelease();
        }

        /// <summary>
        /// Start a tournament (PVP) game.
        /// </summary>
        public void PlayTournamentGame()
        {
            DeckPickerTrayDisplay.Get().m_playButton.TriggerRelease();
        }

        /// <summary>
        /// Config deck for a game.
        /// </summary>
        /// <param name="index">The index of deck.</param>
        public void ConfigDeck(int index)
        {
            AdventureSubScenes currentSubScene = AdventureConfig.Get().GetCurrentSubScene();
            if (currentSubScene == AdventureSubScenes.Practice)
            {
                PracticePickerTrayDisplay.Get().Show();
            }
        }

        /// <summary>
        /// Config opponent for practice game.
        /// </summary>
        /// <param name="index">The index of opponent.</param>
        public void ConfigPracticeOpponent(int index)
        {
            this.tracer.Verbose(GetPrivateField<PracticeAIButton>(PracticePickerTrayDisplay.Get(), "m_selectedPracticeAIButton")?.name);

            List<PracticeAIButton> m_practiceAIButtons = GetPrivateField<List<PracticeAIButton>>(PracticePickerTrayDisplay.Get(), "m_practiceAIButtons");

            if (index <= 0 || index > m_practiceAIButtons.Count)
            {
                Random r = new Random();
                index = r.Next(0, m_practiceAIButtons.Count);
            }
            else
            {
                index -= 1;
            }

            m_practiceAIButtons[index].TriggerRelease();

            this.tracer.Verbose(GetPrivateField<PracticeAIButton>(PracticePickerTrayDisplay.Get(), "m_selectedPracticeAIButton")?.name);
        }

        /// <summary>
        /// Config mode for practice game.
        /// </summary>
        /// <param name="expert">If play expert mode.</param>
        public void ConfigPracticeMode(bool expert)
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

        /// <summary>
        /// Config mode for tournament game.
        /// </summary>
        /// <param name="ranked">If play ranked mode.</param>
        /// <param name="wild">If play wild format.</param>
        public void ConfigTournamentMode(bool ranked, bool wild)
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

        /// <summary>
        /// Get a rock object with RockId.
        /// </summary>
        /// <param name="rockId">The RockId.</param>
        /// <returns>The pegasus object.</returns>
        public IRockObject GetObject(int rockId)
        {
            var card = GetCard(GameState.Get(), rockId);
            if (card == null)
            {
                return null;
            }

            return new RockPegasusObject(card);
        }

        /// <summary>
        /// Click a rock object with RockId.
        /// </summary>
        /// <param name="rockId">The RockId.</param>
        public void ClickObject(int rockId)
        {
            RockPegasusInputHelper.ClickCard(((RockPegasusObject)this.GetObject(rockId)).PegasusCard);
        }

        /// <summary>
        /// Drop a holding pegasus object.
        /// </summary>
        public void DropObject()
        {
            RockPegasusInputHelper.DropCard();
        }

        /// <summary>
        /// Snapshot current scene.
        /// </summary>
        /// <returns>The RockScene.</returns>
        public RockScene SnapshotScene()
        {
            return RockPegasusSnapshotHelper.SnapshotScene();
        }

        /// <summary>
        /// Get selected DeckID
        /// </summary>
        /// <returns>The DeckId.</returns>
        public long GetSelectedDeckID()
        {
            // DeckPickerTrayDisplay is used on both Tournament and Practice
            return DeckPickerTrayDisplay.Get().GetSelectedDeckID();
        }

        /// <summary>
        /// Get a private field of an object
        /// </summary>
        /// <typeparam name="T">The field type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="field">The field name.</param>
        /// <returns>The field value.</returns>
        private static T GetPrivateField<T>(object obj, string field)
        {
            FieldInfo fieldinfo = obj.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            T m_practiceAIButtons = (T)fieldinfo.GetValue(obj);

            return m_practiceAIButtons;
        }

        /// <summary>
        /// Get a Card with RockId.
        /// </summary>
        /// <param name="gameState">The GameState.</param>
        /// <param name="rockId">The RockId.</param>
        /// <returns>The card.</returns>
        private static Card GetCard(GameState gameState, int rockId)
        {
            return GameState.Get().GetEntity(rockId)?.GetCard();
        }

        /// <summary>
        /// Get a Entity with RockId.
        /// </summary>
        /// <param name="gameState">The GameState.</param>
        /// <param name="rockId">The RockId.</param>
        /// <returns>The entity.</returns>
        private static Entity GetEntity(GameState gameState, int rockId)
        {
            return GameState.Get().GetEntity(rockId);
        }

        /// <summary>
        /// Get the SubsceneState in AdventureScene
        /// </summary>
        /// <returns>The RockPegasusSubsceneState.</returns>
        private RockPegasusSubsceneState GetPegasusAdventureSubsceneState()
        {
            if (AdventureConfig.Get() == null)
            {
                return RockPegasusSubsceneState.None;
            }

            AdventureSubScenes currentSubScene = AdventureConfig.Get().GetCurrentSubScene();

            if (currentSubScene == AdventureSubScenes.Chooser)
            {
                return RockPegasusSubsceneState.WaitForChooseMode;
            }

            if (currentSubScene == AdventureSubScenes.Practice)
            {
                if (PracticePickerTrayDisplay.Get().IsShown() == false)
                {
                    return RockPegasusSubsceneState.WaitForChooseDeck;
                }

                if (GetPrivateField<PracticeAIButton>(PracticePickerTrayDisplay.Get(), "m_selectedPracticeAIButton") == null)
                {
                    return RockPegasusSubsceneState.WaitForChooseOpponent;
                }
                else
                {
                    return RockPegasusSubsceneState.Ready;
                }
            }

            return RockPegasusSubsceneState.None;
        }

        /// <summary>
        /// Get the SubsceneState in TournamentScene
        /// </summary>
        /// <returns>The RockPegasusSubsceneState.</returns>
        private RockPegasusSubsceneState GetPegasusTournamentSubsceneState()
        {
            if (DeckPickerTrayDisplay.Get() == null)
            {
                return RockPegasusSubsceneState.None;
            }
            else
            {
                return RockPegasusSubsceneState.Ready;
            }
        }

        //// private static void FindGame(RockGameMode gameMode, long deckId, int missionId)
        //// {
        ////     Options.Get().SetBool(Option.HAS_PLAYED_EXPERT_AI, true);
        ////     GameType gameType = RockPegasusHelper.GetGameType(gameMode);
        ////     FormatType formatType = RockPegasusHelper.GetFormatType(gameMode);
        //// 
        ////     GameMgr.Get().FindGame(gameType, formatType, missionId, deckId, 0L);
        //// }
    }
}
