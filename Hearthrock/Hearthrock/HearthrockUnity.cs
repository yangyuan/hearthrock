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
        // configuration values
        private string HearthrockName = "Hearthrock";
        private string[] HearthrockStates = { "Ready", "Running", "Paused" };
        private string[] HearthrockModes = { "RANK", "PVP", "PVE N", "PVE X" };
        private int HearthrockWidth = 120;
        private int HearthrockHeight = 96;
        // dynamic values
        private int HearthrockState = 0;
        private int HearthrockMode = 0;
        private HearthrockEngine Engine = new HearthrockEngine();
        // static values
        private int UnityPadding = 8;
        private int UnitySpacing = 4;
        private int UnityTitleHeight = 15;
        private int UnityBorderSize = 2;
        private int UnityButtonHeight = 22;
        public static void Hook()
        {
            GameObject sceneObject = SceneMgr.Get().gameObject;
            sceneObject.AddComponent<HearthrockUnity>();
        }
        public void OnGUI()
        {
            Rect rect = new Rect(Screen.width - HearthrockWidth - UnityPadding, UnityPadding, HearthrockWidth, HearthrockHeight);
            GUI.ModalWindow(0, rect, OnHearthrockWindow, HearthrockName);
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
