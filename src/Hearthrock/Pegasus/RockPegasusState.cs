using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hearthrock.Pegasus
{
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
        Adventure
    }
}
