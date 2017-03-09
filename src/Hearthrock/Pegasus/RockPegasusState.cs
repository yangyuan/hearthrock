using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hearthrock.Pegasus
{
    /// <summary>
    /// Why not use original SceneState and GameState
    /// SceneState and GameState are designed to manage game states, so Pegasus can know what to showup.
    /// But for hearthrock, we need to decide what to do, so we need a list of states to know what can be done.
    /// SceneState does not fit the requirement so I designed 2 new states.
    /// </summary>
    public enum RockPegasusState
    {
        None,
        InvalidSceneMode,
        BlockingSceneMode,
        CancelableSceneMode,
        Hub,
        Dialog,
        GamePlay,
        QuestsDialog,
        GeneralDialog,
        Tournament,
        Adventure,
    }
}
