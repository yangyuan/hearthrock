// <copyright file="RockPegasusInput.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    using System.Reflection;

    public static class RockPegasusInput
    {

        public static void ClickCard(Card card)
        {
            InputManager inputManager = InputManager.Get();
            MethodInfo method = inputManager.GetType().GetMethod("HandleClickOnCard", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(inputManager, new object[] { card.gameObject, true });

            //RockInputManager.DisableInput();
            // InputManager.Get().DoNetworkResponse(GetCard(gameState, this.rockAction.Source).GetEntity(), true);
            //RockInputManager.EnableInput();
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
