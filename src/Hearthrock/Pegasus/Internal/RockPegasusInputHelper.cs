// <copyright file="RockPegasusInputHelper.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus.Internal
{
    using System.Reflection;

    /// <summary>
    /// Pegasus Input Helper
    /// </summary>
    internal static class RockPegasusInputHelper
    {
        /// <summary>
        /// Click a card
        /// </summary>
        /// <param name="card">The card.</param>
        public static void ClickCard(Card card)
        {
            InputManager inputManager = InputManager.Get();
            MethodInfo method = inputManager.GetType().GetMethod("HandleClickOnCard", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(inputManager, new object[] { card.gameObject, true });

            // A few APIs which might be useful in future.
            //// InputManager.Get().DisableInput();
            //// InputManager.Get().DoNetworkResponse(GetCard(gameState, this.rockAction.Source).GetEntity(), true);
            //// InputManager.Get().EnableInput();
        }

        /// <summary>
        /// Drop current card
        /// </summary>
        public static void DropCard()
        {
            InputManager inputManager = InputManager.Get();
            inputManager.DropHeldCard();
        }
    }
}
