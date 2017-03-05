
namespace Hearthrock.Robot
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
            rockHero.RockId = heroEntity.GetEntityId();
            rockHero.CanAttack = heroEntity.CanAttack();
            rockHero.Damage = heroEntity.GetATK();
            rockHero.HasWeapon = player.HasWeapon();
            rockHero.Health = heroEntity.GetRealTimeRemainingHP();

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

            rockMinion.BaseHealth = minion.GetHealth();
            rockMinion.CanAttack = minion.CanAttack();
            rockMinion.CanBeAttacked = minion.CanBeAttacked();
            rockMinion.Damage = minion.GetATK();
            rockMinion.HasTaunt = minion.HasTaunt();
            rockMinion.HasWindfury = minion.HasWindfury();
            rockMinion.Health = minion.GetRealTimeRemainingHP();

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

            return rockCard;
        }
    }
}
