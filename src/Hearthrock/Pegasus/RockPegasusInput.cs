using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Hearthrock.Pegasus
{
    public static class RockPegasusInput
    {

        public static void ClickCard(Card card)
        {
            InputManager inputManager = InputManager.Get();
            MethodInfo method = inputManager.GetType().GetMethod("HandleClickOnCard", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(inputManager, new object[] { card.gameObject, true });
        }

        public static void DropCard()
        {
            InputManager inputManager = InputManager.Get();
            inputManager.DropHeldCard();
        }

        public static void DisableInput()
        {
            InputManager inputManager = InputManager.Get();
            inputManager.DisableInput();
        }

        public static void EnableInput()
        {
            InputManager inputManager = InputManager.Get();
            inputManager.EnableInput();
        }
    }
}
