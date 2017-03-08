// <copyright file="RockUnity.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Unity
{
    using System.Collections;
    using UnityEngine;

    using Hearthrock.Engine;

    /// <summary>
    /// Unity Component of Hearthrock
    /// </summary>
    class RockUnity : MonoBehaviour
    {
        private bool isRockEnabled = false;

        const double UIScale = 1.75;

        // static values
        const int RockSpacing = (int)(4 * UIScale);
        const int RockButtonHeight = (int)(22 * UIScale);
        const int RockStatusHeight = 20;
        const int WindowPadding = (int)(8 * UIScale);
        const int WindowBorderSize = 2;
        const int WindowTitleHeight = 15;
        const int WindowContentWidth = 150;
        const int WindowContentHeight = 80 + WindowTitleHeight + RockButtonHeight + RockSpacing * 2;

        private RockEngine rockEngine = new RockEngine();

        /// <summary>
        /// The method to inject RockUnity.
        /// </summary>
        public static void Hook()
        {
            GameObject sceneObject = SceneMgr.Get().gameObject;
            sceneObject.AddComponent<RockUnity>();
        }

        /// <summary>
        /// For Start Message of MonoBehaviour.
        /// </summary>
        public void Start()
        {
            // Init RockRoutine
            StartCoroutine(RockRoutine());
        }

        /// <summary>
        /// For OnGUI Message of MonoBehaviour.
        /// </summary>
        public void OnGUI()
        {
            var WindowWidth = WindowContentWidth + (RockSpacing + WindowBorderSize) * 2;
            var WindowHeight = WindowContentHeight + (RockSpacing + WindowBorderSize) * 2;

            Rect rect = new Rect(Screen.width - WindowWidth - WindowPadding, WindowPadding, WindowWidth, WindowHeight);
         
            GUI.ModalWindow(0, rect, OnHearthrockWindow, RockUnityConstants.Title);
        }

        /// <summary>
        /// Main loop of Hearthrock.
        /// </summary>
        /// <returns>IEnumerator of Unity async tasks.</returns>
        private IEnumerator RockRoutine()
        {
            while (true)
            {
                double delay = 1;
                if (this.isRockEnabled)
                {
                    rockEngine.Tick();
                    delay = rockEngine.Update();
                }

                yield return new WaitForSeconds((float)delay);
            }
        }

        /// <summary>
        /// Main function of HearthrockUnity ModalWindow
        /// </summary>
        /// <param name="windowID"></param>
        private void OnHearthrockWindow(int windowID)
        {
            int contentOffsetLeft = RockSpacing + WindowBorderSize;
            int buttonOffsetTop = WindowBorderSize + RockSpacing + WindowTitleHeight;
            int statusOffsetTop = buttonOffsetTop + RockButtonHeight + RockSpacing;

            if (GUI.Button(new Rect(contentOffsetLeft, buttonOffsetTop, WindowContentWidth, RockButtonHeight), "Run / Pause"))
            {
                if (this.isRockEnabled)
                {
                    this.isRockEnabled = false;

                    rockEngine.Reload();
                    rockEngine.RockInfo("Hearthrock Paused");
                }
                else
                {
                    this.isRockEnabled = true;

                    rockEngine.Reload();
                    rockEngine.RockInfo("Hearthrock Started");
                }
                rockEngine.Clear();
            }

            string statusRockState = $"Status: " + (this.isRockEnabled?"Running": "Paused");
            string statusGameMode = $"Mode: " + this.rockEngine.GameMode.ToString();
            string statusTrace = $"Trace: " + (this.rockEngine.UseLocalTrace?"Local":"Remote");
            string statusBot = $"Bot: " + (this.rockEngine.UseBuildinBot ? "Buildin" : "Remote");

            int currentOffsetTop = statusOffsetTop;
            GUI.Label(new Rect(contentOffsetLeft, currentOffsetTop, WindowContentWidth, RockStatusHeight), statusRockState);
            currentOffsetTop += RockStatusHeight;
            GUI.Label(new Rect(contentOffsetLeft, currentOffsetTop, WindowContentWidth, RockStatusHeight), statusGameMode);
            currentOffsetTop += RockStatusHeight;
            GUI.Label(new Rect(contentOffsetLeft, currentOffsetTop, WindowContentWidth, RockStatusHeight), statusTrace);
            currentOffsetTop += RockStatusHeight;
            GUI.Label(new Rect(contentOffsetLeft, currentOffsetTop, WindowContentWidth, RockStatusHeight), statusBot);
        }
    }
}
