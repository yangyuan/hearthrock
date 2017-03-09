using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hearthrock.Pegasus
{
    public enum RockPegasusGameState
    {
        None,
        GameOver,
        WaitForMulligan,
        WaitForAction,
        Blocking
    }
}
