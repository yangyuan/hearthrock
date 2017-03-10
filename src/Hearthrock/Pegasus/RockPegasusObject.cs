using Hearthrock.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hearthrock.Pegasus
{
    public class RockPegasusObject : RockObject
    {
        public Card PegasusCard { get; }

        public Entity PegasusEntity
        {
            get
            {
                return this.PegasusCard.GetEntity();
            }
        }

        public string CardId
        {
            get
            {
                return this.PegasusEntity.GetCardId();
            }
        }

        public override int RockId
        {
            get
            {
                return this.PegasusEntity.GetEntityId();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                return this.PegasusEntity.GetName();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public RockPegasusObject(Card card)
        {
            this.PegasusCard = card;
        }
    }
}
