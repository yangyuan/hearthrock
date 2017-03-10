using Hearthrock.Contracts;
using Hearthrock.Pegasus;
using System.Collections.Generic;

namespace Hearthrock.Engine
{
    class RockMulliganContext
    {
        bool applied;
        List<int> cards;
        IRockPegasus pegasus;

        public RockMulliganContext(List<int> cards, IRockPegasus pegasus)
        {
            applied = false;
            this.pegasus = pegasus;

            if (cards == null)
            {
                this.cards = new List<int>();
            }
            else
            {
                this.cards = cards;
            }
        }

        public bool IsDone()
        {
            return applied;
        }

        public void Apply()
        {
            foreach (int cardId in this.cards)
            {
                var card = this.pegasus.GetObject(cardId);
                if (card.CardId == "GAME_005") continue;
                this.pegasus.ClickObject(card.RockId);
            }
            applied = true;
        }
    }
}
