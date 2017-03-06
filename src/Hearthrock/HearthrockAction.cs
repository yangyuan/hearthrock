// <copyright file="DirectoryAsync.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock
{
    /// <summary>
    /// Action of Hearthrock
    /// </summary>
    public class RockActionInternal
    {
        public RockActionTypeInternal type;
        public int step;
        public string msg;
        public Card card1;
        public Card card2;
        public RockActionInternal()
        {
            type = RockActionTypeInternal.None;
            step = 0;
            msg = "";
            card1 = null;
            card2 = null;
        }
    }

    /// <summary>
    /// Action Type of Hearthrock
    /// </summary>
    public enum RockActionTypeInternal
    {
        None,
        Play,
        Attack,
    }

}
