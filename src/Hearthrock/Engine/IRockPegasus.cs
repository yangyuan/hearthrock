using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hearthrock.Engine
{
    public interface IRockPegasus
    {
        void SetActive();

        bool TryCloseDialog();

        bool TryCloseQuests();


        SceneMgr.Mode GetSceneMode();

        void TrySetSceneMode(SceneMgr.Mode mode);
    }
}
