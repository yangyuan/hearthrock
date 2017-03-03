using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Hearthrock
{
    class HearthrockUnity : MonoBehaviour
    {
        const string HearthrockTitle = "Hearthrock";
        const double HearthrockScale = 1.5;

        private string[] HearthrockStates = { "Ready", "Running", "Paused" };
        private string[] HearthrockModes = { "RANK", "PVP", "PVE N", "PVE X" };
        const int HearthrockWidth = (int)(120 * HearthrockScale);
        const int HearthrockHeight = (int)(96 * HearthrockScale);
        // dynamic values
        private int HearthrockState = 0;
        private int HearthrockMode = 0;
        private HearthrockEngine Engine = new HearthrockEngine();
        // static values
        const int UnityPadding = (int)(8 * HearthrockScale);
        const int UnitySpacing = (int)(4 * HearthrockScale);
        const int UnityTitleHeight = (int)(15 * HearthrockScale);
        const int UnityBorderSize = (int)(2 * HearthrockScale);
        const int UnityButtonHeight = (int)(22 * HearthrockScale);
        public static void Hook()
        {
            GameObject sceneObject = SceneMgr.Get().gameObject;
            sceneObject.AddComponent<HearthrockUnity>();
        }
        public void OnGUI()
        {
            Rect rect = new Rect(Screen.width - HearthrockWidth - UnityPadding, UnityPadding, HearthrockWidth, HearthrockHeight);
            GUI.ModalWindow(0, rect, OnHearthrockWindow, HearthrockTitle);
        }

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
                    HearthrockEngine.Message("Hearthrock Started");
                }
                else
                {
                    HearthrockState = 2;
                    HearthrockEngine.Message("Hearthrock Paused");
                }
                Engine.Clear();
            }
        }

        public void Update()
        {
            Engine.SwitchMode(HearthrockMode);
            Engine.Tick();
            if (HearthrockState == 1)
            {
                Engine.Update();
            }
        }
    }
}
