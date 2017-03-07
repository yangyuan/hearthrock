// <copyright file="RockUnity.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Unity Component of Hearthrock
    /// </summary>
    class RockUnity : MonoBehaviour
    {
        const string Title = "Hearthrock";
        const double UIScale = 1.5;

        const int HearthrockWidth = (int)(120 * UIScale);
        const int HearthrockHeight = (int)(96 * UIScale);

        // dynamic values
        private int HearthrockState = 0;
        private int HearthrockMode = 0;

        // static values
        const int UnityPadding = (int)(8 * UIScale);
        const int UnitySpacing = (int)(4 * UIScale);
        const int UnityTitleHeight = (int)(15 * UIScale);
        const int UnityBorderSize = (int)(2 * UIScale);
        const int UnityButtonHeight = (int)(22 * UIScale);

        private string[] HearthrockStates = { "Ready", "Running", "Paused" };
        private string[] HearthrockModes = { "RANK", "PVP", "PVE N", "PVE X" };
        private HearthrockEngine Engine = new HearthrockEngine();

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
            Rect rect = new Rect(Screen.width - HearthrockWidth - UnityPadding, UnityPadding, HearthrockWidth, HearthrockHeight);
            GUI.ModalWindow(0, rect, OnHearthrockWindow, Title);
        }

        /// <summary>
        /// Main loop of Hearthrock.
        /// </summary>
        /// <returns>IEnumerator of Unity async tasks.</returns>
        private IEnumerator RockRoutine()
        {
            while (true)
            {
                Engine.SwitchMode(HearthrockMode);
                Engine.Tick();
                double delay = 1;
                if (HearthrockState == 1)
                {
                    delay = Engine.Update();
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
            int width = HearthrockWidth - UnitySpacing * 2 - UnityBorderSize * 2;
            int offset_left = UnitySpacing + UnityBorderSize;
            int button_offset_top = UnityBorderSize + UnitySpacing + UnityTitleHeight;
            int select_offset_top = button_offset_top + UnityButtonHeight + UnitySpacing;
            int select_height = HearthrockHeight - select_offset_top - UnityBorderSize - UnitySpacing;

            HearthrockMode = GUI.SelectionGrid(new Rect(offset_left, select_offset_top, width, select_height), HearthrockMode, HearthrockModes, 2);
            

            if (GUI.Button(new Rect(offset_left, button_offset_top, width, UnityButtonHeight), HearthrockStates[HearthrockState]))
            {
                if (HearthrockState == 0 || HearthrockState == 2)
                {
                    HearthrockState = 1;
                    Engine.Reload();
                    Engine.RockInfo("Hearthrock Started");
                }
                else
                {
                    HearthrockState = 2;
                    Engine.Reload();
                    Engine.RockInfo("Hearthrock Paused");
                }
                Engine.Clear();
            }
        }
    }
}
