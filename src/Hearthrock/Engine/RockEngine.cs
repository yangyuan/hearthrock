// <copyright file="RockEngine.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using System;
    using System.IO;

    using Hearthrock.Communication;
    using Hearthrock.Contracts;
    using Hearthrock.Diagnostics;
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
        /// The IRockBot
        /// </summary>
        private IRockBot bot;

        /// <summary>
        /// The RockEngineTracer
        /// </summary>
        private RockTracer tracer;

        /// <summary>
        /// The IRockPegasus
        /// </summary>
        private IRockPegasus pegasus;

        /// <summary>
        /// Context for action.
        /// </summary>
        private RockEngineAction currentAction;

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
        public bool UseBuiltinTrace
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
            this.tracer = new RockTracer(this.configuration);
            this.bot = new RockEngineBot(this.configuration, this.tracer);
            this.pegasus = RockPegasusFactory.CreatePegasus(this.tracer);
        }

        /// <summary>
        /// called every frame
        /// </summary>
        public void Tick()
        {
            this.pegasus.TriggerUserActive();

            //// try
            //// {
            ////     var pegasusState = this.pegasus.GetSceneMode();
            ////     this.tracer.Verbose(pegasusState.ToString());
            ////     RockPegasusGameState state = this.pegasus.GetPegasusGameState();
            ////     this.tracer.Verbose(state.ToString());
            //// }
            //// catch
            //// {
            //// }
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
                    case RockPegasusSceneState.BlockingScene:
                        return 1;
                    case RockPegasusSceneState.QuestsDialog:
                        this.pegasus.DoCloseQuestsDialog();
                        return 2;
                    case RockPegasusSceneState.GeneralDialog:
                        this.pegasus.DoCloseGeneralDialog();
                        return 2;
                    case RockPegasusSceneState.CancelableScene:
                        this.pegasus.NavigateToHubScene();
                        break;
                    case RockPegasusSceneState.HubScene:
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                            case RockGameMode.ExpertPractice:
                                this.pegasus.NavigateToAdventureScene();
                                break;
                            case RockGameMode.Casual:
                            case RockGameMode.Ranked:
                            case RockGameMode.WildCasual:
                            case RockGameMode.WildRanked:
                                this.pegasus.NavigateToTournamentScene();
                                //// Tournament.Get().NotifyOfBoxTransitionStart();
                                break;
                            default:
                                break;
                        }

                        break;
                    case RockPegasusSceneState.AdventureScene:
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
                                this.pegasus.NavigateToHubScene();
                                break;
                            default:
                                this.pegasus.NavigateToHubScene();
                                break;
                        }

                        break;
                    case RockPegasusSceneState.TournamentScene:
                        switch (this.GameMode)
                        {
                            case RockGameMode.NormalPractice:
                            case RockGameMode.ExpertPractice:
                                this.pegasus.NavigateToHubScene();
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
                                this.pegasus.NavigateToHubScene();
                                break;
                        }

                        break;
                    case RockPegasusSceneState.GamePlay:
                        return this.OnRockGamePlay();
                    case RockPegasusSceneState.InvalidScene:
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
                    this.pegasus.DoEndFinishedGame();
                    return 5;
                case RockPegasusGameState.WaitForPlay:
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
                this.pegasus.DoEndTurn();
                this.currentAction = null;
                return 3;
            }

            if (this.currentAction == null || this.currentAction.IsDone() || !this.currentAction.IsValid())
            {
                var scene = this.pegasus.SnapshotScene();
                var rockAction = this.bot.GetPlayAction(scene);
                if (rockAction != null)
                {
                    var rockActionContext = new RockEngineAction(this.pegasus, rockAction);
                    if (rockActionContext.IsValid())
                    {
                        this.currentAction = rockActionContext;
                        this.ShowRockInfo(this.currentAction.Interpretation);
                    }
                    else
                    {
                        this.tracer.Warning("Invalid rockAction");
                    }
                }
                else
                {
                    this.ShowRockInfo("Job's Done");
                    this.pegasus.DoEndTurn();
                    return 3;
                }
            }

            if (this.currentAction != null && this.currentAction.IsValid())
            {
                this.currentAction.Apply();
            }

            return 1;
        }

        /// <summary>
        /// On WaitForMulligan state
        /// </summary>
        /// <returns>Seconds to be delayed before next call.</returns>
        private double OnRockMulligan()
        {
            if (this.currentAction == null)
            {
                this.ShowRockInfo("Mulligan");
                var scene = this.pegasus.SnapshotScene();
                var mulliganedCards = this.bot.GetMulliganAction(scene);

                this.currentAction = new RockEngineAction(this.pegasus, mulliganedCards);
            }

            if (this.currentAction.IsDone())
            {
                MulliganManager.Get().GetMulliganButton().TriggerRelease();
                return 5;
            }

            this.currentAction.ApplyAll();
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
            RockPegasusSubsceneState subscene = this.pegasus.GetPegasusSubsceneState(RockPegasusSceneState.TournamentScene);

            switch (subscene)
            {
                case RockPegasusSubsceneState.Ready:
                    this.pegasus.ConfigTournamentMode(ranked, wild);
                    this.pegasus.PlayTournamentGame();
                    return 1;
                default:
                case RockPegasusSubsceneState.WaitForChooseMode:
                case RockPegasusSubsceneState.WaitForChooseDeck:
                case RockPegasusSubsceneState.WaitForChooseOpponent:
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
            RockPegasusSubsceneState subscene = this.pegasus.GetPegasusSubsceneState(RockPegasusSceneState.AdventureScene);

            switch (subscene)
            {
                case RockPegasusSubsceneState.WaitForChooseMode:
                    this.pegasus.ConfigPracticeMode(expert);
                    return 1;
                case RockPegasusSubsceneState.WaitForChooseDeck:
                    this.pegasus.ConfigDeck(this.configuration.DeckIndex);
                    return 1;
                case RockPegasusSubsceneState.WaitForChooseOpponent:
                    this.pegasus.ConfigPracticeOpponent(this.configuration.OpponentIndex);
                    return 1;
                case RockPegasusSubsceneState.Ready:
                    this.pegasus.PlayPracticeGame();
                    return 1;
                default:
                case RockPegasusSubsceneState.None:
                    return 0.5;
            }
        }
    }
}
