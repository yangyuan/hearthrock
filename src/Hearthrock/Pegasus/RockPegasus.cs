// <copyright file="RockPegasusClient.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    using Hearthrock.Contracts;
    using Hearthrock.Engine;
    using PegasusShared;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public class RockPegasus : IRockPegasus
    {

        RockEngineTracer tracer;

        public RockPegasus (RockEngineTracer tracer)
        {
            this.tracer = tracer;
        }

        public void SetActive()
        {
            InactivePlayerKicker ipk = InactivePlayerKicker.Get();
            if (ipk == null)
            {
                // InactivePlayerKicker seesm to be used to make sure use is alive, but still need to confirm this.
                this.tracer.Verbose("InactivePlayerKicker is not available.");
                return;
            }

            FieldInfo fieldinfo = ipk.GetType().GetField("m_activityDetected", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldinfo.SetValue(ipk, true);
        }

        public void TrySetSceneMode(SceneMgr.Mode mode)
        {
            SceneMgr.Get().SetNextMode(mode);
        }


        public bool TryCloseDialog()
        {
            if (DialogManager.Get() == null)
            {
                return false;
            }

            if (DialogManager.Get().ShowingDialog())
            {
                DialogManager.Get().GoBack();
                return true;
            }

            return false;
        }



        public bool TryCloseQuests()
        {
            WelcomeQuests wq = WelcomeQuests.Get();
            if (wq != null)
            {
                wq.m_clickCatcher.TriggerRelease();
                return true;
            }

            return false;
        }

        public SceneMgr.Mode GetSceneMode()
        {
            return SceneMgr.Get().GetMode();
        }

        /// <summary>
        /// The method to FindGame.
        /// </summary>
        /// <param name="gameMode"></param>
        /// <param name="deckId"></param>
        /// <param name="missionId"></param>
        public static void FindGame(RockGameMode gameMode, long deckId, int missionId)
        {
            Options.Get().SetBool(Option.HAS_PLAYED_EXPERT_AI, true);
            GameType gameType = RockPegasusHelper.GetGameType(gameMode);
            FormatType formatType = RockPegasusHelper.GetFormatType(gameMode);

            GameMgr.Get().FindGame(gameType, formatType, missionId, deckId, 0L);
        }
    }
}
