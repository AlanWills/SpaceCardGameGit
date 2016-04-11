﻿using _2DEngine;
using System.Diagnostics;

namespace SpaceCardGame
{
    /// <summary>
    /// A class which uses the CardObjectPair class but is specifically used with Ships.
    /// </summary>
    public class CardShipPair : CardObjectPair
    {
        #region Properties and Fields

        /// <summary>
        /// A reference to the stored card as a ShipCard (saves casting elsewhere)
        /// </summary>
        public ShipCard ShipCard { get; private set; }

        /// <summary>
        /// A reference to the stored game object as a Ship (saves casting elsewhere)
        /// </summary>
        public Ship Ship { get; private set; }

        #endregion

        public CardShipPair(ShipCardData shipCardData) :
            base(shipCardData)
        {
            Ship = AddChild(new Ship(shipCardData.ObjectDataAsset));
            CardObject = Ship;

            Debug.Assert(Card is ShipCard);
            ShipCard = Card as ShipCard;

            AddDefaultWeapon();
        }

        #region Virtual Functions

        /// <summary>
        /// Cannot add ships to other ships
        /// </summary>
        /// <param name="cardShipPair"></param>
        public override void AddToCardShipPair(CardShipPair cardShipPair)
        {
            Debug.Fail("Cannot add ships to other ships");
        }

        /// <summary>
        /// Calls this function iteratively on all CardObjectPairs parented under this ship card.
        /// </summary>
        public override void MakeReadyForCardPlacement()
        {
            base.MakeReadyForCardPlacement();

            foreach (CardObjectPair pair in Children.GetChildrenOfType<CardObjectPair>())
            {
                pair.MakeReadyForCardPlacement();
            }
        }

        /// <summary>
        /// Calls this function iteratively on all CardObjectPairs parented under this ship card.
        /// </summary>
        public override void MakeReadyForBattle()
        {
            base.MakeReadyForBattle();

            foreach (CardObjectPair pair in Children.GetChildrenOfType<CardObjectPair>())
            {
                pair.MakeReadyForBattle();
            }
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// A utility function for setting up the default weapon on this ship
        /// </summary>
        private void AddDefaultWeapon()
        {
            WeaponCardData defaultWeaponCardData = AssetManager.LoadData<WeaponCardData>(WeaponCardData.defaultWeaponCardDataAsset);
            CardWeaponPair defaultWeapon = AddChild(new CardWeaponPair(defaultWeaponCardData));

            defaultWeapon.AddToCardShipPair(this);
        }

        #endregion
    }
}