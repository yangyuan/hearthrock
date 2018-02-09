// <copyright file="RockUnity.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Hooks
{
    using System.Collections;

    using Hearthrock.Engine;
    using UnityEngine;

    /// <summary>
    /// Unity Component of Hearthrock
    /// </summary>
    public class RockUnity : MonoBehaviour
    {
        /// <summary>
        /// Is RockUnity enabled.
        /// </summary>
        public static bool IsRockEnabled = false;

        /// <summary>
        /// The RockEngine instance.
        /// </summary>
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
            // Init RockRoutine.
            this.StartCoroutine(this.RockRoutine());
        }

        /// <summary>
        /// For OnGUI Message of MonoBehaviour.
        /// </summary>
        public void OnGUI()
        {
            int windowContentHeight = (RockUnityConstants.RockStatusHeight * 4)
                + RockUnityConstants.WindowTitleHeight
                + RockUnityConstants.RockButtonHeight
                + (RockUnityConstants.RockSpacing * 2);

            var windowWidth = RockUnityConstants.WindowContentWidth
                + ((RockUnityConstants.RockSpacing + RockUnityConstants.WindowBorderSize) * 2);
            var windowHeight = windowContentHeight
                + ((RockUnityConstants.RockSpacing + RockUnityConstants.WindowBorderSize) * 2);

            Rect rect = new Rect(Screen.width - windowWidth - RockUnityConstants.WindowPadding, RockUnityConstants.WindowPadding, windowWidth, windowHeight);
         
            GUI.ModalWindow(0, rect, this.OnRockWindow, RockUnityConstants.Title);
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

                this.rockEngine.Tick();

                if (IsRockEnabled)
                {
                    delay = this.rockEngine.Update();
                }

                yield return new WaitForSeconds((float)delay);
            }
        }

        /// <summary>
        /// Main function of HearthrockUnity ModalWindow.
        /// </summary>
        /// <param name="windowID">The window id of an Unity Window.</param>
        private void OnRockWindow(int windowID)
        {
            int contentOffsetLeft = RockUnityConstants.RockSpacing + RockUnityConstants.WindowBorderSize;
            int buttonOffsetTop = RockUnityConstants.WindowBorderSize + RockUnityConstants.RockSpacing + RockUnityConstants.WindowTitleHeight;
            int statusOffsetTop = buttonOffsetTop + RockUnityConstants.RockButtonHeight + RockUnityConstants.RockSpacing;

            // The main rock button.
            if (GUI.Button(
                new Rect(contentOffsetLeft, buttonOffsetTop, RockUnityConstants.WindowContentWidth, RockUnityConstants.RockButtonHeight),
                RockUnityConstants.RockButtonTitle))
            {
                if (IsRockEnabled)
                {
                    this.isRockEnabled = false;
                    RockGameHooks.EnableLockMousePosition = false;

                    this.rockEngine.Reload();
                    this.rockEngine.ShowRockInfo("Hearthrock Paused");
                }
                else
                {
                    this.isRockEnabled = true;
                    RockGameHooks.EnableLockMousePosition = true;

                    this.rockEngine.Reload();
                    this.rockEngine.ShowRockInfo("Hearthrock Started");
                }
            }

            // The status texts.
            string statusRockState = $"Status: " + (IsRockEnabled ? "Running" : "Paused");
            string statusGameMode = $"Mode: " + this.rockEngine.GameMode.ToString();
            string statusTrace = $"Trace: " + (this.rockEngine.UseBuiltinTrace ? "Builtin" : "Remote");
            string statusBot = $"Bot: " + (this.rockEngine.UseBuiltinBot ? "Builtin" : "Remote");

            int currentOffsetTop = statusOffsetTop;
            GUI.Label(new Rect(contentOffsetLeft, currentOffsetTop, RockUnityConstants.WindowContentWidth, RockUnityConstants.RockStatusHeight), statusRockState);
            currentOffsetTop += RockUnityConstants.RockStatusHeight;
            GUI.Label(new Rect(contentOffsetLeft, currentOffsetTop, RockUnityConstants.WindowContentWidth, RockUnityConstants.RockStatusHeight), statusGameMode);
            currentOffsetTop += RockUnityConstants.RockStatusHeight;
            GUI.Label(new Rect(contentOffsetLeft, currentOffsetTop, RockUnityConstants.WindowContentWidth, RockUnityConstants.RockStatusHeight), statusTrace);
            currentOffsetTop += RockUnityConstants.RockStatusHeight;
            GUI.Label(new Rect(contentOffsetLeft, currentOffsetTop, RockUnityConstants.WindowContentWidth, RockUnityConstants.RockStatusHeight), statusBot);
        }
    }
}
