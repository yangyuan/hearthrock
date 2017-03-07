// <copyright file="IRockRobot.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;
    using Hearthrock.Contracts;

    public interface IRockBot
    {
        List<RockCard> PickCards(List<RockCard> cards);

        RockAction GetAction(RockScene scene);
    }
}
