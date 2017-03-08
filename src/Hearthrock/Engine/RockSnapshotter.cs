// <copyright file="RockSnapshotter.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using Hearthrock.Contracts;
    using System.Collections.Generic;

    class RockSnapshotter
    {
        public static RockScene SnapshotScene(GameState state)
        {
            var rockScene = new RockScene();

            Player self = GameState.Get().GetFriendlySidePlayer();
            Player opponent = GameState.Get().GetFirstOpponentPlayer(GameState.Get().GetFriendlySidePlayer());

            rockScene.Self = SnapshotPlayer(self);
            rockScene.Opponent = SnapshotPlayer(opponent);
            return rockScene;
        }

        private static RockPlayer SnapshotPlayer(Player player)
        {
            var rockPlayer = new RockPlayer();
            rockPlayer.Resources = player.GetNumAvailableResources();
            rockPlayer.Hero = SnapshotHero(player);
            rockPlayer.Power = SnapshotPower(player);
            rockPlayer.Minions = SnapshotMinions(player);
            rockPlayer.Cards = SnapshotCards(player);

            return rockPlayer;
        }

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
            rockHero.Damage = heroEntity.GetATK();
            rockHero.CanAttack = heroEntity.CanAttack();
            rockHero.IsExhausted = heroEntity.IsExhausted();
            rockHero.Health = heroEntity.GetRealTimeRemainingHP();
            rockHero.HasWeapon = player.HasWeapon();
            if (rockHero.HasWeapon)
            {
                rockHero.WeaponRockId = heroEntity.GetWeaponCard().GetEntity().GetEntityId();
                rockHero.WeaponCanAttack = heroEntity.GetWeaponCard().GetEntity().CanAttack();
            }
            else
            {
                rockHero.WeaponRockId = 0;
                rockHero.WeaponCanAttack = false;
            }

            return rockHero;
        }

        private static RockCard SnapshotPower(Player player)
        {
            return SnapshotCard(player.GetHeroPower());
        }

        private static List<RockMinion> SnapshotMinions(Player player)
        {
            var rockMinions = new List<RockMinion>();

            List<Card> minions = player.GetBattlefieldZone().GetCards();
            foreach(var minion in minions)
            {
                rockMinions.Add(SnapshotMinion(minion.GetEntity()));
            }

            return rockMinions;
        }

        private static RockMinion SnapshotMinion(Entity minion)
        {
            var rockMinion = new RockMinion();

            rockMinion.RockId = minion.GetEntityId();
            rockMinion.Health = minion.GetRealTimeRemainingHP();
            rockMinion.BaseHealth = minion.GetHealth();
            rockMinion.CanAttack = minion.CanAttack();
            rockMinion.CanBeAttacked = minion.CanBeAttacked();
            rockMinion.Damage = minion.GetATK();
            rockMinion.HasTaunt = minion.HasTaunt();
            rockMinion.HasWindfury = minion.HasWindfury();
            rockMinion.HasDivineShield = minion.HasDivineShield();
            rockMinion.IsStealthed = minion.IsStealthed();
            rockMinion.IsExhausted = minion.IsExhausted();
            rockMinion.IsFrozen = minion.IsFrozen();
            rockMinion.IsAsleep = minion.IsAsleep();

            return rockMinion;
        }


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

        private static RockCard SnapshotCard(Entity card)
        {
            var rockCard = new RockCard();

            rockCard.RockId = card.GetEntityId();
            rockCard.CardId = card.GetCardId();
            rockCard.Cost = card.GetCost();
            rockCard.IsMinion = card.IsMinion();
            rockCard.IsSpell = card.IsSpell();
            rockCard.IsWeapon = card.IsWeapon();
            rockCard.HasTaunt = card.HasTaunt();
            rockCard.HasCharge = card.HasCharge();

            return rockCard;
        }
    }
}
