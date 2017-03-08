// <copyright file="RockEntity.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    class RockEntity
    {
        public bool ReferencesWindfury { get; set; }

        public bool CanAttack { get; set; }

        public bool CanBeAttacked { get; set; }

        public bool CanBeTargetedByBattlecries { get; set; }

        public bool CanBeTargetedByHeroPowers { get; set; }

        public bool CanBeTargetedByOpponents { get; set; }

        public bool CanBeTargetedBySpells { get; set; }

        public bool CannotAttackHeroes { get; set; }

        public bool DontShowImmune { get; set; }

        public int GetArmor { get; set; }

        public int GetATK { get; set; }

        public int GetAttached { get; set; }
        
        public int GetControllerId { get; set; }

        public int GetCost { get; set; }

        public int GetCreatorId { get; set; }

        public int GetDamage { get; set; }

        public int GetDisplayedCreatorId { get; set; }

        public int GetDurability { get; set; }

        public int GetEntityId { get; set; }

        public int GetExtraAttacksThisTurn { get; set; }

        public int GetFatigue { get; set; }

        public int GetHealth { get; set; }
        
        public int GetNumAttacksThisTurn { get; set; }

        public int GetNumTurnsInPlay { get; set; }
        
        public int GetSpellPower { get; set; }

        public int GetWindfury { get; set; }

        public bool HasAura { get; set; }

        public bool HasBattlecry { get; set; }

        public bool HasCharge { get; set; }

        public bool HasCombo { get; set; }

        public bool HasCustomKeywordEffect { get; set; }

        public bool HasDeathrattle { get; set; }

        public bool HasDivineShield { get; set; }

        public bool HasHealthMin { get; set; }

        public bool HasHeroPowerDamage { get; set; }

        public bool HasInspire { get; set; }

        public bool HasOverload { get; set; }
        
        public bool HasSpellPower { get; set; }

        public bool HasSpellPowerDouble { get; set; }

        public bool HasWindfury { get; set; }

        public bool IsAffectedBySpellPower { get; set; }

        public bool IsAsleep { get; set; }

        public bool IsAttached { get; set; }

        public bool IsBasicCardUnlock { get; set; }

        public bool IsCharacter { get; set; }

        public bool IsElite { get; set; }

        public bool IsEnraged { get; set; }

        public bool IsExhausted { get; set; }

        public bool IsFreeze { get; set; }

        public bool IsFrozen { get; set; }

        public bool IsGame { get; set; }

        public bool IsHero { get; set; }

        public bool IsHeroPower { get; set; }

        public bool IsImmune { get; set; }

        public bool IsItem { get; set; }

        public bool IsMagnet { get; set; }

        public bool IsMinion { get; set; }
        
        public bool IsObfuscated { get; set; }

        public bool IsPlayer { get; set; }

        public bool IsPoisonous { get; set; }

        public bool IsRecentlyArrived { get; set; }

        public bool IsSecret { get; set; }

        public bool IsSilenced { get; set; }

        public bool IsSpell { get; set; }

        public bool IsStealthed { get; set; }

        public bool IsToken { get; set; }

        public bool IsWeapon { get; set; }

        public bool ReferencesAutoAttack { get; set; }

        public bool ReferencesBattlecry { get; set; }

        public bool ReferencesCharge { get; set; }

        public bool ReferencesDeathrattle { get; set; }

        public bool ReferencesDivineShield { get; set; }

        public bool ReferencesImmune { get; set; }

        public bool ReferencesSecret { get; set; }

        public bool ReferencesSpellPower { get; set; }

        public bool ReferencesStealth { get; set; }

        public bool ReferencesTaunt { get; set; }
    }
}
