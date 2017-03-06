// <copyright file="IRockRobot.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Robot
{
    using System.Collections.Generic;
    using Hearthrock.Contracts;

    public interface IRockRobot
    {
        List<RockCard> PickCards(List<RockCard> cards);

        RockAction GetAction(RockScene scene);
    }
}
