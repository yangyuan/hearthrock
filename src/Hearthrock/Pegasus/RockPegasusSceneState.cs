// <copyright file="RockPegasusSceneState.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    /// <summary>
    /// SceneState for Pegasus.
    /// </summary>
    public enum RockPegasusSceneState
    {
        /// <summary>
        /// The None.
        /// </summary>
        None,

        /// <summary>
        /// Invalid Scene, can do nothing.
        /// </summary>
        InvalidScene,

        /// <summary>
        /// Blocking Scene, should wait.
        /// </summary>
        BlockingScene,

        /// <summary>
        /// A not support scene but can be canceled to hub.
        /// </summary>
        CancelableScene,

        /// <summary>
        /// A quests dialog is showing.
        /// </summary>
        QuestsDialog,

        /// <summary>
        /// A general dialog is showing.
        /// </summary>
        GeneralDialog,

        /// <summary>
        /// The hub scene.
        /// </summary>
        HubScene,

        /// <summary>
        /// The adventure (PVE) scene.
        /// </summary>
        AdventureScene,

        /// <summary>
        /// The tournament (PVP) scene.
        /// </summary>
        TournamentScene,

        /// <summary>
        /// Playing the game.
        /// </summary>
        GamePlay
    }
}
