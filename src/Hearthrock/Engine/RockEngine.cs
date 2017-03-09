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

            //// var pegasusState = this.pegasus.GetSceneMode();
            //// this.tracer.Verbose(pegasusState.ToString());

            try
            {
                AdventureSubScenes currentSubScene = AdventureConfig.Get().GetCurrentSubScene();
                this.tracer.Verbose(currentSubScene.ToString());

                this.tracer.Verbose(PracticePickerTrayDisplay.Get().IsShown().ToString());
            }
            catch
            {
            }
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
                var pegasusState = this.pegasus.GetPegasusSceneState();

                switch (pegasusState)
                {
                    case RockPegasusSceneState.BlockingSceneMode:
                        return 1;
                    case RockPegasusSceneState.QuestsDialog:
                        this.pegasus.TryCloseQuests();
                        return 2;
                    case RockPegasusSceneState.GeneralDialog:
                        this.pegasus.TryCloseDialog();
                        return 2;
                    case RockPegasusSceneState.CancelableSceneMode:
                        this.pegasus.NavigateToHub();
                        break;
                    case RockPegasusSceneState.Hub:
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
                    case RockPegasusSceneState.Adventure:
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                                return this.OnRockPracticeMode(false);
                            case RockGameMode.ExpertPractice:
                                return this.OnRockPracticeMode(true);
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
                    case RockPegasusSceneState.Tournament:
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                            case RockGameMode.ExpertPractice:
                                this.pegasus.NavigateToHub();
                                break;
                            case RockGameMode.Casual:
                                return this.OnRockTournamentMode(false, false);
                            case RockGameMode.WildCasual:
                                return this.OnRockTournamentMode(false, true);
                            case RockGameMode.Ranked:
                                return this.OnRockTournamentMode(true, false);
                            case RockGameMode.WildRanked:
                                return this.OnRockTournamentMode(true, true);
                            default:
                                this.pegasus.NavigateToHub();
                                break;
                        }

                        break;
                    case RockPegasusSceneState.GamePlay:
                        return this.OnRockGamePlay();
                    case RockPegasusSceneState.InvalidSceneMode:
                    case RockPegasusSceneState.None:
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
                    this.ShowRockInfo("Game Over");
                    this.pegasus.TryFinishEndGame();
                    return 5;
                case RockPegasusGameState.WaitForAction:
                    return this.OnRockAction();
                case RockPegasusGameState.WaitForMulligan:
                    return this.OnRockMulligan();
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
                this.ShowRockInfo("Job's Done");
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
                    this.ShowRockInfo(this.rockActionContext.Interpretion(GameState.Get()));
                }
                else
                {
                    this.ShowRockInfo("Job's Done");
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
                this.ShowRockInfo("Mulligan");
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

        /// <summary>
        /// On Tournament state
        /// </summary>
        /// <param name="ranked">if play in rank mode.</param>
        /// <param name="wild">if play in wild format.</param>
        /// <returns>Seconds to be delayed before next call.</returns>
        private double OnRockTournamentMode(bool ranked, bool wild)
        {
            RockPegasusSubsceneState subscene = this.pegasus.GetPegasusSubsceneState(RockPegasusSceneState.Tournament);

            switch (subscene)
            {
                case RockPegasusSubsceneState.Ready:
                    this.pegasus.ConfigTournament(ranked, wild);
                    this.pegasus.PlayTournament();
                    return 1;
                default:
                case RockPegasusSubsceneState.WaitChooseMode:
                case RockPegasusSubsceneState.WaitChooseDeck:
                case RockPegasusSubsceneState.WaitChooseOpponent:
                case RockPegasusSubsceneState.None:
                    return 0.5;
            }
        }

        /// <summary>
        /// On Practice state
        /// </summary>
        /// <param name="expert">if play in expert mode.</param>
        /// <returns>Seconds to be delayed before next call.</returns>
        private double OnRockPracticeMode(bool expert)
        {
            RockPegasusSubsceneState subscene = this.pegasus.GetPegasusSubsceneState(RockPegasusSceneState.Adventure);

            switch (subscene)
            {
                case RockPegasusSubsceneState.WaitChooseMode:
                    this.pegasus.ChoosePracticeMode(expert);
                    return 1;
                case RockPegasusSubsceneState.WaitChooseDeck:
                    this.pegasus.ChooseDeck(this.configuration.DeckIndex);
                    return 1;
                case RockPegasusSubsceneState.WaitChooseOpponent:
                    this.pegasus.SelectPracticeOpponent(this.configuration.OpponentIndex);
                    return 1;
                case RockPegasusSubsceneState.Ready:
                    this.pegasus.PlayPractice();
                    return 1;
                default:
                case RockPegasusSubsceneState.None:
                    return 0.5;
            }
        }
    }
}
