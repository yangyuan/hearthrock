// <copyright file="RockPegasusObject.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus.Internal
{
    using Hearthrock.Contracts;

    /// <summary>
    /// Pegasus Object
    /// </summary>
    public class RockPegasusObject : IRockObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RockPegasusObject" /> class.
        /// </summary>
        /// <param name="card">The Pegasus Card.</param>
        public RockPegasusObject(Card card)
        {
            this.PegasusCard = card;
        }

        /// <summary>
        /// Gets the Pegasus Card.
        /// </summary>
        public Card PegasusCard { get; }

        /// <summary>
        /// Gets the Pegasus Entity.
        /// </summary>
        public Entity PegasusEntity
        {
            get
            {
                return this.PegasusCard.GetEntity();
            }
        }

        /// <summary>
        /// Gets the CardId.
        /// </summary>
        public string CardId
        {
            get
            {
                return this.PegasusEntity.GetCardId();
            }
        }

        /// <summary>
        /// Gets the RockId.
        /// </summary>
        public int RockId
        {
            get
            {
                return this.PegasusEntity.GetEntityId();
            }
        }

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.PegasusEntity.GetName();
            }
        }
    }
}
