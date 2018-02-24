// <copyright file="RockPegasusSnapshotHelper.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus.Internal
{
    using System.Collections.Generic;

    using Hearthrock.Contracts;

    /// <summary>
    /// Pegasus Snapshot Helper
    /// </summary>
    internal class RockPegasusSnapshotHelper
    {
        /// <summary>
        /// Snapshot Scene
        /// </summary>
        /// <param name="sessionId">The Session Id.</param>
        /// <param name="actionId">The Action Id.</param>
        /// <returns>The RockScene.</returns>
        public static RockScene SnapshotScene(string sessionId, int actionId)
        {
            var rockScene = new RockScene();

            Player self = GameState.Get().GetFriendlySidePlayer();
            Player opponent = GameState.Get().GetFirstOpponentPlayer(GameState.Get().GetFriendlySidePlayer());

            rockScene.Self = SnapshotPlayer(self);
            rockScene.Opponent = SnapshotPlayer(opponent);
            rockScene.PlayOptions = SnapshotOptions();
            rockScene.Turn = GameState.Get().GetTurn();
            rockScene.SessionId = sessionId;
            rockScene.ActionId = actionId;
            return rockScene;
        }

        /// <summary>
        /// Snapshot a Player.
        /// </summary>
        /// <param name="player">The Player.</param>
        /// <returns>The RockPlayer.</returns>
        private static RockPlayer SnapshotPlayer(Player player)
        {
            var rockPlayer = new RockPlayer();
            rockPlayer.Resources = player.GetNumAvailableResources();
            rockPlayer.PermanentResources = player.GetNumPermanentResources();
            rockPlayer.TemporaryResources = player.GetNumTemporaryResources();
            rockPlayer.Hero = SnapshotHero(player);
            rockPlayer.Power = SnapshotPower(player);
            rockPlayer.Minions = SnapshotMinions(player);
            rockPlayer.Cards = SnapshotCards(player);
            rockPlayer.Choices = SnapshotChoices(player);
            rockPlayer.PowerAvailable = !player.GetHeroPower().IsExhausted();

            rockPlayer.HasWeapon = player.HasWeapon();
            if (rockPlayer.HasWeapon)
            {
                rockPlayer.Weapon = SnapshotCard(player.GetHero().GetWeaponCard().GetEntity());
            }
            else
            {
                rockPlayer.Weapon = null;
            }

            return rockPlayer;
        }

        /// <summary>
        /// Snapshot a Hero.
        /// </summary>
        /// <param name="player">The Player.</param>
        /// <returns>The RockHero.</returns>
        private static RockHero SnapshotHero(Player player)
        {
            var rockHero = new RockHero();

            var heroEntity = player.GetHero();
            switch (player.GetHeroCard().GetEntity().GetClass())
            {
                case TAG_CLASS.WARLOCK:
                    rockHero.Class = RockHeroClass.Warlock;
                    break;
                case TAG_CLASS.HUNTER:
                    rockHero.Class = RockHeroClass.Hunter;
                    break;
                case TAG_CLASS.DRUID:
                    rockHero.Class = RockHeroClass.Druid;
                    break;
                case TAG_CLASS.PALADIN:
                    rockHero.Class = RockHeroClass.Paladin;
                    break;
                case TAG_CLASS.ROGUE:
                    rockHero.Class = RockHeroClass.Rogue;
                    break;
                case TAG_CLASS.SHAMAN:
                    rockHero.Class = RockHeroClass.Shaman;
                    break;
                case TAG_CLASS.WARRIOR:
                    rockHero.Class = RockHeroClass.Warrior;
                    break;
                case TAG_CLASS.PRIEST:
                    rockHero.Class = RockHeroClass.Priest;
                    break;
                case TAG_CLASS.MAGE:
                    rockHero.Class = RockHeroClass.Mage;
                    break;
                default:
                    rockHero.Class = RockHeroClass.None;
                    break;
            }

            rockHero.RockId = heroEntity.GetEntityId();
            rockHero.Name = heroEntity.GetName();
            rockHero.CardId = heroEntity.GetCardId();
            rockHero.Damage = heroEntity.GetATK();
            rockHero.CanAttack = heroEntity.CanAttack();
            rockHero.IsExhausted = heroEntity.IsExhausted();
            rockHero.Health = heroEntity.GetRealTimeRemainingHP();
            rockHero.IsQuest = heroEntity.IsQuest();
            rockHero.IsSecret = heroEntity.IsSecret();

            return rockHero;
        }

        /// <summary>
        /// Snapshot a Hero Power.
        /// </summary>
        /// <param name="player">The Player.</param>
        /// <returns>The RockCard.</returns>
        private static RockCard SnapshotPower(Player player)
        {
            return SnapshotCard(player.GetHeroPower());
        }

        /// <summary>
        /// Snapshot minions.
        /// </summary>
        /// <param name="player">The Player.</param>
        /// <returns>The list of RockMinion.</returns>
        private static List<RockMinion> SnapshotMinions(Player player)
        {
            var rockMinions = new List<RockMinion>();

            List<Card> minions = player.GetBattlefieldZone().GetCards();
            foreach (var minion in minions)
            {
                rockMinions.Add(SnapshotMinion(minion.GetEntity()));
            }

            return rockMinions;
        }

        /// <summary>
        /// Snapshot a minion.
        /// </summary>
        /// <param name="minion">The Entity.</param>
        /// <returns>The RockMinion.</returns>
        private static RockMinion SnapshotMinion(Entity minion)
        {
            var rockMinion = new RockMinion();

            rockMinion.RockId = minion.GetEntityId();
            rockMinion.Name = minion.GetName();
            rockMinion.CardId = minion.GetCardId();
            rockMinion.Health = minion.GetRealTimeRemainingHP();
            rockMinion.BaseHealth = minion.GetHealth();
            rockMinion.CanAttack = minion.CanAttack();
            rockMinion.CanBeAttacked = minion.CanBeAttacked();
            rockMinion.Damage = minion.GetATK();
            rockMinion.HasTaunt = minion.HasTaunt();
            rockMinion.HasWindfury = minion.HasWindfury();
            rockMinion.HasDivineShield = minion.HasDivineShield();
            rockMinion.HasAura = minion.HasAura();
            rockMinion.IsStealthed = minion.IsStealthed();
            rockMinion.IsExhausted = minion.IsExhausted();
            rockMinion.IsFrozen = minion.IsFrozen();
            rockMinion.IsAsleep = minion.IsAsleep();
            rockMinion.HasDeathrattle = minion.HasDeathrattle();
            rockMinion.HasInspire = minion.HasInspire();
            rockMinion.HasTriggerVisual = minion.HasTriggerVisual();
            rockMinion.HasLifesteal = minion.HasLifesteal();
            rockMinion.IsPoisonous = minion.IsPoisonous();
            rockMinion.IsEnraged = minion.IsEnraged();
            rockMinion.HasBattlecry = minion.HasBattlecry();
            rockMinion.Race = (RockRace)(int)minion.GetRace();
            rockMinion.IsSilenced = minion.IsSilenced();
            rockMinion.NumAttacksThisTurn = minion.GetNumAttacksThisTurn() == 0;

            return rockMinion;
        }

        /// <summary>
        /// Snapshot cards.
        /// </summary>
        /// <param name="player">The Player.</param>
        /// <returns>The list of RockCard.</returns>
        private static List<RockCard> SnapshotCards(Player player)
        {
            var rockCards = new List<RockCard>();

            List<Card> cards = player.GetHandZone().GetCards();
            foreach (var card in cards)
            {
                rockCards.Add(SnapshotCard(card.GetEntity()));
            }

            return rockCards;
        }

        /// <summary>
        /// Snapshot card choices.
        /// </summary>
        /// <param name="player">The Player.</param>
        /// <returns>The list of RockCard.</returns>
        private static List<RockCard> SnapshotChoices(Player player)
        {
            var rockCards = new List<RockCard>();

            var choices = GameState.Get()?.GetEntityChoices(player.GetPlayerId());

            if (choices != null)
            {
                foreach (var entityId in choices.Entities)
                {
                    rockCards.Add(SnapshotCard(GameState.Get()?.GetEntity(entityId)));
                }
            }

            return rockCards;
        }

        /// <summary>
        /// Snapshot a card.
        /// </summary>
        /// <param name="card">The Entity.</param>
        /// <returns>The RockCard.</returns>
        private static RockCard SnapshotCard(Entity card)
        {
            var rockCard = new RockCard();

            rockCard.RockId = card.GetEntityId();
            rockCard.Name = card.GetName();
            rockCard.CardId = card.GetCardId();
            rockCard.Cost = card.GetCost();
            if (card.IsMinion())
            {
                rockCard.CardType = RockCardType.Minion;
            }
            else if (card.IsSpell())
            {
                rockCard.CardType = RockCardType.Spell;
            }
            else if (card.IsEnchantment())
            {
                rockCard.CardType = RockCardType.Enchantment;
            }
            else if (card.IsWeapon())
            {
                rockCard.CardType = RockCardType.Weapon;
            }
            else
            {
                rockCard.CardType = RockCardType.None;
            }

            rockCard.Damage = card.GetATK();
            rockCard.Health = card.IsWeapon() ? card.GetCurrentDurability() : card.GetHealth();
            rockCard.HasTaunt = card.HasTaunt();
            rockCard.HasCharge = card.HasCharge();
            rockCard.Options = new List<RockCard>();

            if (card.HasSubCards())
            {
                foreach (var subCardID in card.GetSubCardIDs())
                {
                    rockCard.Options.Add(SnapshotCard(GameState.Get().GetEntity(subCardID)));
                }
            }

            return rockCard;
        }

        /// <summary>
        /// Snapshot play options.
        /// </summary>
        /// <returns>The play options.</returns>
        private static List<List<int>> SnapshotOptions()
        {
            var ret = new List<List<int>>();

            var options = GameState.Get()?.GetOptionsPacket();
            if (options == null || options.List == null)
            {
                var choices = GameState.Get()?.GetFriendlyEntityChoices();
                if (choices != null)
                {
                    foreach (var entityId in choices.Entities)
                    {
                        ret.Add(new List<int> { entityId });
                    }
                }

                return ret;
            }

            foreach (var option in options.List)
            {
                foreach (var subOption in SnapshotOptions(option))
                {
                    ret.Add(subOption);
                }
            }

            return ret;
        }

        /// <summary>
        /// Snapshot play options.
        /// </summary>
        /// <param name="option">The Network Option.</param>
        /// <returns>The play options.</returns>
        private static IEnumerable<List<int>> SnapshotOptions(Network.Options.Option option)
        {
            if (option.Subs != null && option.Subs.Count > 0)
            {
                foreach (var sub in option.Subs)
                {
                    foreach (var subOption in SnapshotSubOptions(sub))
                    {
                        var ret = new List<int> { option.Main.ID };
                        ret.AddRange(subOption);
                        yield return ret;
                    }
                }
            }
            else
            {
                foreach (var mainOption in SnapshotSubOptions(option.Main))
                {
                    yield return mainOption;
                }
            }
        }

        /// <summary>
        /// Snapshot play sub options.
        /// </summary>
        /// <param name="subOption">The Network SubOption.</param>
        /// <returns>The play sub options.</returns>
        private static IEnumerable<List<int>> SnapshotSubOptions(Network.Options.Option.SubOption subOption)
        {
            if (!subOption.HasValidTarget())
            {
                if (subOption.PlayErrorInfo.IsValid())
                {
                    if (subOption.ID == 0)
                    {
                        yield break;
                    }
                    else
                    {
                        yield return new List<int> { subOption.ID };
                    }
                }
            }
            else
            {
                foreach (var target in subOption.Targets)
                {
                    if (target.PlayErrorInfo.IsValid())
                    {
                        yield return new List<int> { subOption.ID, target.ID };
                    }
                }
            }
        }
    }
}