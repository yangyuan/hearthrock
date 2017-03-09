using Hearthrock.Contracts;
using Hearthrock.Pegasus;
using System.Collections.Generic;

namespace Hearthrock.Engine
{
    class RockMulliganContext
    {
        bool applied;
        List<RockCard> cards;

        public RockMulliganContext(List<RockCard> c)
        {
            applied = false;

            if (c == null)
            {
                cards = new List<RockCard>();
            }
            else
            {
                cards = c;
            }
        }

        public bool IsDone()
        {
            return applied;
        }

        public void Apply(GameState gameState)
        {

            foreach (RockCard card in this.cards)
            {
                if (card.CardId == "GAME_005") continue;
                RockPegasusInput.ClickCard(GetCard(gameState, card.RockId));
            }
            applied = true;
        }

        public static Card GetCard(GameState gameState, int rockId)
        {
            return GameState.Get().GetEntity(rockId)?.GetCard();
        }

        public static Entity GetEntity(GameState gameState, int rockId)
        {
            return GameState.Get().GetEntity(rockId);
        }
    }
}
