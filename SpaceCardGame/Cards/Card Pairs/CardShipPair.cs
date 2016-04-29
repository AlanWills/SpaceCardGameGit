﻿using _2DEngine;
using Microsoft.Xna.Framework;
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

        /// <summary>
        /// A reference to the player who owns this ship - we will be using this to keep track of the number of ships the player has
        /// </summary>
        protected GamePlayer Player { get; set; }

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
        /// Add our ShipHoverCardInfo module to the card ship pair.
        /// </summary>
        public override void LoadContent()
        {
            CheckShouldLoad();

            if (AddHoverInfoModule)
            {
                HoverInfoModule = AddModule(new ShipHoverCardInfoModule(this));
            }

            base.LoadContent();
        }

        /// <summary>
        /// Fixup the size of the ship so that we don't create one larger than our card.
        /// Do this in begin, because we usually do extra size fixup before we start updating so we want to capture those changes.
        /// Also, do it before so that the colliders for our card and card object get the latest size too.
        /// </summary>
        public override void Begin()
        {
            Vector2 scale = Vector2.Min(Vector2.One, Vector2.Divide(Card.Size, CardObject.Size));
            Ship.Scale(scale);

            base.Begin();
        }

        /// <summary>
        /// When we add a ship to the game board.
        /// Want to update the player's ships placed and set up event callbacks for when this ship dies
        /// </summary>
        /// <param name="gameBoard"></param>
        /// <param name="player"></param>
        public override void WhenAddedToGameBoard(GameBoardSection gameBoard, GamePlayer player)
        {
            Player = player;
            Debug.Assert(Player.CurrentShipsPlaced < GamePlayer.MaxShipNumber);

            Reparent(gameBoard.ShipCardControl);                       // Reparent this under the card ship control rather than the game board which it was initially added to

            player.CurrentShipsPlaced++;
        }

        /// <summary>
        /// Cannot add ships to other ships
        /// </summary>
        /// <param name="cardShipPair"></param>
        public override void AddToCardShipPair(CardShipPair cardShipPair)
        {
            Debug.Fail("Cannot add ships to other ships");
        }
        
        /// <summary>
        /// When this ship dies we want to reduce the number of player ships by one
        /// </summary>
        public override void Die()
        {
            base.Die();

            Player.CurrentShipsPlaced--;
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// A utility function for setting up the default weapon on this ship
        /// </summary>
        private void AddDefaultWeapon()
        {
            WeaponCardData defaultWeaponCardData = AssetManager.GetData<WeaponCardData>(WeaponCardData.defaultWeaponCardDataAsset);
            CardWeaponPair defaultWeapon = AddChild(new CardWeaponPair(defaultWeaponCardData));

            defaultWeapon.AddToCardShipPair(this);
        }

        #endregion
    }
}
