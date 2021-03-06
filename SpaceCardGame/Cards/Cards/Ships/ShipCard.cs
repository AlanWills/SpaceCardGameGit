﻿using CelesteEngine;
using SpaceCardGameData;

namespace SpaceCardGame
{
    /// <summary>
    /// A class used to represent a shield in our game.
    /// </summary>
    public abstract class ShipCard : Card
    {
        public ShipCard(Player player, CardData shipCardData) :
            base(player, shipCardData)
        {
        }

        #region Virtual Functions

        /// <summary>
        /// Ship cards create a CardShipPair
        /// </summary>
        /// <returns></returns>
        public override CardObjectPair CreateCardObjectPair()
        {
            return new CardShipPair(this);
        }

        /// <summary>
        /// Can only lay this ship card if we have sufficient resources
        /// </summary>
        /// <param name="player"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public override bool CanLay(Player player, ref string error)
        {
            bool shipSlotsLeft = player.CurrentShipsPlaced < Player.MaxShipNumber;

            return shipSlotsLeft && base.CanLay(player, ref error);
        }

        /// <summary>
        /// The valid target for a ship card is only another ship card, so return whether the input is a CardShipPair with non-zero health.
        /// </summary>
        /// <param name="pairToValidate"></param>
        /// <returns></returns>
        public override bool CanUseOn(CardObjectPair pairToValidate)
        {
            if (pairToValidate is CardShipPair)
            {
                // If we are targeting another ship, it is valid if it not dead
                return !(pairToValidate as CardShipPair).Ship.DamageModule.Dead;
            }

            // Otherwise the target is invalid
            return false;
        }

        /// <summary>
        /// A function for calculating the damage being done to this ship.
        /// By default is just a straight pass through
        /// Override this to perform custom damage calculation for ships.
        /// </summary>
        /// <param name="objectDoingTheDamage"></param>
        /// <param name="inputDamage"></param>
        public virtual float CalculateDamageDoneToThis(BaseObject objectDoingTheAttacking, float inputDamage)
        {
            return inputDamage;
        }

        /// <summary>
        /// A function which calculates this ship's attack.
        /// By default returns the attack in the ship data, but if we have another turret attached, returns that turret's damage
        /// </summary>
        /// <returns></returns>
        public virtual float CalculateAttack(BaseObject targetBaseObject)
        {
            CardShipPair cardShipPair = GetCardObjectPair<CardShipPair>();

            if (cardShipPair.Ship.Turret.IsDefaultTurret)
            {
                return cardShipPair.Ship.ShipData.Attack;
            }
            else
            {
                return cardShipPair.Ship.Turret.BulletData.Damage;
            }
        }

        #endregion
    }
}
