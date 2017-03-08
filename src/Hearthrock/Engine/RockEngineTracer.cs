// <copyright file="RockEngineTracer.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{

    /// <summary>
    /// Rock Engine Tracer
    /// </summary>
    class RockEngineTracer
    {

        SceneMgr.Mode scenemgr_mode = SceneMgr.Mode.INVALID;
        public void TraceSceneMode(SceneMgr.Mode mode)
        {
            if (scenemgr_mode != mode)
            {
                scenemgr_mode = mode;
                // Trace(scenemgr_mode.ToString());
            }
        }
    }
}
