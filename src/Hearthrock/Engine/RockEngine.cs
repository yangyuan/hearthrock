// <copyright file="RockEngine.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using System;
    using System.IO;

    using Hearthrock.Communication;
    using Hearthrock.Contracts;
    using Hearthrock.Pegasus;

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
        /// Context for action.
        /// </summary>
        private RockActionContext rockActionContext;

        /// <summary>
        /// Context for mulligan.
        /// </summary>
        private RockMulliganContext rockMulliganContext;

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
                    case RockPegasusState.BlockingSceneMode:
                        return 1;
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
                                //// Tournament.Get().NotifyOfBoxTransitionStart();
                                break;
                            default:
                                break;
                        }

                        break;
                    case RockPegasusState.Adventure:
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                                return this.OnRockPraticeMode(false);
                            case RockGameMode.ExpertPractice:
                                return this.OnRockPraticeMode(true);
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
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                            case RockGameMode.ExpertPractice:
                                this.pegasus.NavigateToHub();
                                break;
                            case RockGameMode.Casual:
                            case RockGameMode.WildCasual:
                                return this.OnRockTournamentMode(false);
                            case RockGameMode.Ranked:
                            case RockGameMode.WildRanked:
                                return this.OnRockTournamentMode(true);
                            default:
                                this.pegasus.NavigateToHub();
                                break;
                        }

                        break;
                    case RockPegasusState.GamePlay:
                        return this.OnRockGamePlay();
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

        /// <summary>
        /// On GamePlay state
        /// </summary>
        /// <returns>Seconds to be delayed before next call.</returns>
        private double OnRockGamePlay()
        {
            var pegasusGameState = this.pegasus.GetPegasusGameState();
            switch (pegasusGameState)
            {
                case RockPegasusGameState.Blocking:
                    return 1;
                case RockPegasusGameState.GameOver:
                    ShowRockInfo("Game Over");
                    this.pegasus.TryFinishEndGame();
                    return 5;
                case RockPegasusGameState.WaitForAction:
                    return OnRockAction();
                case RockPegasusGameState.WaitForMulligan:
                    return OnRockMulligan();
                case RockPegasusGameState.None:
                default:
                    return 1;
            }
        }

        /// <summary>
        /// On WaitForAction state
        /// </summary>
        /// <returns>Seconds to be delayed before next call.</returns>
        private double OnRockAction()
        {
            if (EndTurnButton.Get().HasNoMorePlays())
            {
                ShowRockInfo("Job's Done");
                this.pegasus.EndTurn();
                this.rockActionContext = null;
                return 3;
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
                    ShowRockInfo("Job's Done");
                    this.pegasus.EndTurn();
                    return 3;
                }
            }

            this.rockActionContext.Apply(GameState.Get(), this);
            return 1;
        }

        /// <summary>
        /// On WaitForMulligan state
        /// </summary>
        /// <returns>Seconds to be delayed before next call.</returns>
        private double OnRockMulligan()
        {
            if (this.rockMulliganContext == null)
            {
                ShowRockInfo("Mulligan");
                var scene = RockSnapshotter.SnapshotScene(GameState.Get());
                var mulliganedCards = this.bot.GetMulligan(scene);

                this.rockMulliganContext = new RockMulliganContext(mulliganedCards);
            }

            if (this.rockMulliganContext.IsDone())
            {
                MulliganManager.Get().GetMulliganButton().TriggerRelease();
                return 5;
            }

            this.rockMulliganContext.Apply(GameState.Get());
            return 1;
        }


        private double OnRockTournamentMode(bool ranked)
        {
            if (DeckPickerTrayDisplay.Get() == null)
            {
                return 1;
            }
            long deck = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
            if (deck == 0)
            {
                return 1;
            }

            bool is_ranked = Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE);
            if (is_ranked != ranked)
            {
                Options.Get().SetBool(Option.IN_RANKED_PLAY_MODE, ranked);
            }

            long selectedDeckID = DeckPickerTrayDisplay.Get().GetSelectedDeckID();


            PegasusShared.GameType type;
            if (ranked)
            {
                type = PegasusShared.GameType.GT_RANKED;
            }
            else
            {
                type = PegasusShared.GameType.GT_CASUAL;
            }

            GameMgr.Get().FindGame(type, FormatType.FT_STANDARD, 2, selectedDeckID, 0L);

            Enum[] args = new Enum[] { PresenceStatus.PLAY_QUEUE };
            PresenceMgr.Get().SetStatus(args);

            return 1;
        }

        private double OnRockPraticeMode(bool expert)
        {
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

            this.pegasus.SelectPracticeOpponent(1);
            return 1;
        }
    }
}
