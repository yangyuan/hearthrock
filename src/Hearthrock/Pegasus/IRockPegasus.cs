// <copyright file="IRockPegasus.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    using Hearthrock.Contracts;

    /// <summary>
    /// The interface to interact with Pegasus.
    /// </summary>
    public interface IRockPegasus
    {
        /// <summary>
        /// Trigger some activity to make user looks active.
        /// </summary>
        void TriggerUserActive();

        /// <summary>
        /// Close a general dialog if there is one.
        /// </summary>
        /// <returns>return false if there is no general dialog.</returns>
        bool DoCloseGeneralDialog();

        /// <summary>
        /// Close a quests dialog if there is one.
        /// </summary>
        /// <returns>return false if there is no quests dialog.</returns>
        bool DoCloseQuestsDialog();

        /// <summary>
        /// End current turn.
        /// </summary>
        void DoEndTurn();

        /// <summary>
        /// End a finished game.
        /// </summary>
        void DoEndFinishedGame();

        /// <summary>
        /// Get current Pegasus Scene State.
        /// </summary>
        /// <returns>The RockPegasusSceneState.</returns>
        RockPegasusSceneState GetPegasusSceneState();

        /// <summary>
        /// Get current Pegasus Subscene State.
        /// </summary>
        /// <param name="sceneState">The current RockPegasusSceneState.</param>
        /// <returns>The RockPegasusSubsceneState.</returns>
        RockPegasusSubsceneState GetPegasusSubsceneState(RockPegasusSceneState sceneState);

        /// <summary>
        /// Get current Pegasus Game State.
        /// </summary>
        /// <returns>The RockPegasusGameState.</returns>
        RockPegasusGameState GetPegasusGameState();

        /// <summary>
        /// Navigate to Hub Scene.
        /// </summary>
        void NavigateToHubScene();

        /// <summary>
        /// Navigate to Tournament Scene.
        /// </summary>
        void NavigateToTournamentScene();

        /// <summary>
        /// Navigate to Adventure Scene.
        /// </summary>
        void NavigateToAdventureScene();

        /// <summary>
        /// Start a practice (PVE) game.
        /// </summary>
        void PlayPracticeGame();

        /// <summary>
        /// Start a tournament (PVP) game.
        /// </summary>
        void PlayTournamentGame();

        /// <summary>
        /// Config deck for a game.
        /// </summary>
        /// <param name="index">The index of deck.</param>
        void ConfigDeck(int index);

        /// <summary>
        /// Config opponent for practice game.
        /// </summary>
        /// <param name="index">The index of opponent.</param>
        void ConfigPracticeOpponent(int index);

        /// <summary>
        /// Config mode for practice game.
        /// </summary>
        /// <param name="expert">If play expert mode.</param>
        void ConfigPracticeMode(bool expert);

        /// <summary>
        /// Config mode for tournament game.
        /// </summary>
        /// <param name="ranked">If play ranked mode.</param>
        /// <param name="wild">If play wild format.</param>
        void ConfigTournamentMode(bool ranked, bool wild);

        /// <summary>
        /// Get a rock object with RockId.
        /// </summary>
        /// <param name="rockId">The RockId.</param>
        /// <returns>The pegasus object.</returns>
        IRockObject GetObject(int rockId);

        /// <summary>
        /// Click a rock object with RockId.
        /// </summary>
        /// <param name="rockId">The RockId.</param>
        void ClickObject(int rockId);

        /// <summary>
        /// Drop a holding pegasus object.
        /// </summary>
        void DropObject();

        /// <summary>
        /// Snapshot current scene.
        /// </summary>
        /// <returns>The RockScene.</returns>
        RockScene SnapshotScene();
    }
}
