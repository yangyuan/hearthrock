// <copyright file="IRockObject.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    public interface IRockObject
    {
        int RockId { get; }
        string Name { get; }
        string CardId { get; }
    }
}
