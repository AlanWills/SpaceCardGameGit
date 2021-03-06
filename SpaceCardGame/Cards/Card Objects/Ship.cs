﻿using CelesteEngine;
using CelesteEngineData;
using Microsoft.Xna.Framework;
using SpaceCardGameData;
using System.Diagnostics;

namespace SpaceCardGame
{
    public class Ship : CardObject
    {
        #region Properties and Fields

        /// <summary>
        /// A reference to the data for the ship this object represents
        /// </summary>
        public ShipData ShipData { get; private set; }

        /// <summary>
        /// A reference to our damageable object module - useful for reference to it's health etc.
        /// </summary>
        public DamageableObjectModule DamageModule { get; private set; }

        /// <summary>
        /// A reference to the turret for our ship.
        /// We will create a default turret from the ship data and then can override it by adding a turret card to the ship
        /// </summary>
        private Turret turret;
        public Turret Turret
        {
            get { return turret; }
            set
            {
                if (turret != null)
                {
                    turret.Die();
                }

                turret = value;
            }
        }

        /// <summary>
        /// A reference to the shield for our ship.
        /// By default this will not be set to anything, but can be set by adding a shield card to the ship
        /// </summary>
        private Shield shield;
        public Shield Shield
        {
            get { return shield; }
            set
            {
                if (shield != null)
                {
                    shield.Die();
                }

                shield = value;
            }
        }

        /// <summary>
        /// A fixed size aray of references to the engines on our ship.
        /// Used mainly for fancy animation.
        /// </summary>
        public Engine[] Engines { get; set; }

        /// <summary>
        /// A fixed size array of the ui we use to signify damage
        /// </summary>
        public DamageUI[] DamageUI { get; private set; }

        /// <summary>
        /// A reference to our explosion that is triggered when this ship dies.
        /// </summary>
        private CustomSoundEffect ShipDeathExplosionSFX { get; set; }

        #endregion

        // The ship is tied to the card, so it's position will be amended when the card is added to the screen
        public Ship(string shipDataAsset) :
            base(Vector2.Zero, shipDataAsset)
        {

        }

        #region Virtual Functions

        /// <summary>
        /// Loads the ship data and sets up it's stats
        /// </summary>
        public override void LoadContent()
        {
            CheckShouldLoad();

            ShipData = Data as ShipData;
            DebugUtils.AssertNotNull(ShipData);

            ShipDeathExplosionSFX = new CustomSoundEffect(ShipData.ExplosionSFXAsset);

            DamageModule = AddModule(new DamageableObjectModule(ShipData.Defence));
            DamageModule.CalculateDamage += GetCardObjectPair<CardShipPair>().ShipCard.CalculateDamageDoneToThis;

            // Add engine UI from our data
            Engines = new Engine[ShipData.EngineHardpoints.Count];
            Debug.Assert(ShipData.EngineHardpoints.Count > 0);

            for (int i = 0; i < ShipData.EngineHardpoints.Count; ++i)
            {
                Engines[i] = AddChild(new Engine(ShipData.Speed, ShipData.EngineHardpoints[i]));
            }

            Debug.Assert(ShipData.Defence > 0);
            Debug.Assert(ShipData.DamageHardpoints.Count >= ShipData.Defence - 1);
            DamageUI = new DamageUI[ShipData.Defence - 1];

            for (int i = 0; i < ShipData.Defence - 1; i++)
            {
                DamageUI damageUI = AddChild(new DamageUI(ShipData.DamageHardpoints[i]));
                damageUI.Hide();            // Initially hide all the damage UI
                DamageUI[i] = damageUI;
            }

            base.LoadContent();
        }

        /// <summary>
        /// If the ship is selected and ready, we trigger a script to handle the attacking of other ships
        /// </summary>
        /// <param name="elapsedGameTime"></param>
        /// <param name="mousePosition"></param>
        public override void HandleInput(float elapsedGameTime, Vector2 mousePosition)
        {
            base.HandleInput(elapsedGameTime, mousePosition);

            CardShipPair cardShipPair = GetCardObjectPair<CardShipPair>();

            DebugUtils.AssertNotNull(Collider);

            // Check to see that the player who is interacting is the owner of this card
            bool isOwningPlayersTurn = cardShipPair.Card.Player == ScreenManager.Instance.GetCurrentScreenAs<BattleScreen>().ActivePlayer;
            if (isOwningPlayersTurn && Collider.IsClicked && Turret.CanFire && cardShipPair.IsReady)
            {
                DebugUtils.AssertNotNull(Parent);
                Debug.Assert(Parent is CardShipPair);
                CommandManager.Instance.AddChild(new AttackOpponentShipCommand(Parent as CardShipPair), true, true);
            }

            // Updates the colour of the ship if our mouse is over it
            // Green if we can attack - red if we cannot
            Color hoverColour = (Turret.CanFire && cardShipPair.IsReady) ? Color.LightGreen : Color.Red;
            Parent.Colour = Collider.IsMouseOver ? hoverColour : Color.White;
        }

        /// <summary>
        /// Show the damage for this ship
        /// </summary>
        /// <param name="showChildren"></param>
        public override void MakeReadyForBattle()
        {
            base.MakeReadyForBattle();

            ShowDamage();
        }

        /// <summary>
        /// When this object collides with another, we check to see if we have collided with a bullet.
        /// If we have, we add various pieces of damage UI and an explosion.
        /// If we are dead we call Die() to clean ourselves up from the scene.
        /// </summary>
        /// <param name="collidedObject"></param>
        public override void OnCollisionWith(GameObject collidedObject)
        {
            if (collidedObject is Projectile)
            {
                // Adds debris
                ScreenManager.Instance.CurrentScreen.AddGameObject(new Debris(WorldPosition), true, true);

                // Show our ship damage
                ShowDamage();

                if (DamageModule.Dead)
                {
                    ShipDeathExplosionSFX.Play();
                    Die();
                }
            }   
        }

        #endregion

        #region Event Callbacks

        /// <summary>
        /// Shows damage on the ship corresponding to how much we have taken
        /// </summary>
        private void ShowDamage()
        {
            if (DamageModule.Health <= 0)
            {
                // Don't add damage if we have no health - in the end we will implement the damage happening when our bullet hits so it won't matter that we skip this
                return;
            }

            // Hide any damage we no longer need to show
            for (int i = 0; i < (int)DamageModule.Health - 1; i++)
            {
                Debug.Assert(i < DamageUI.Length);
                DamageUI[i].Hide();
            }

            // Turn on damage equal to the damage we have taken
            for (int i = (int)DamageModule.Health - 1; i < ShipData.Defence - 1; ++i)
            {
                Debug.Assert(i < DamageUI.Length);
                DamageUI[i].Show();
            }
        }

        #endregion
    }
}