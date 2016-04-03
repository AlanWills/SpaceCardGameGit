﻿using _2DEngine;
using _2DEngineData;
using Microsoft.Xna.Framework;
using SpaceCardGameData;

namespace SpaceCardGame
{
    public class Ship : GameObjectContainer, IDamageable
    {
        #region Properties and Fields

        /// <summary>
        /// A reference to the data for the ship this object represents
        /// </summary>
        private ShipData ShipData { get; set; }

        /// <summary>
        /// A float to represent the health of this ship
        /// </summary>
        private float health;
        public float Health
        {
            get { return health; }
            private set
            {
                health = value;
                HandleCurrentHealth();
            }
        }

        /// <summary>
        /// A bool to use to indicate that this object is dead, without having to change it's IsAlive property.
        /// </summary>
        private bool dead;
        public bool Dead
        {
            get { return dead; }
            private set { dead = value; }
        }

        /// <summary>
        /// A reference to the turret for our ship.
        /// We will create a default turret and then can override it by adding a turret card to our ship
        /// </summary>
        public Turret Turret { get; private set; }

        #endregion

        // The ship is tied to the card, so it's position will be amended when the card is added to the screen
        public Ship(ShipCardData cardData) :
            base(Vector2.Zero, cardData.ObjectDataAsset)
        {
            
        }

        #region Virtual Functions

        /// <summary>
        /// Loads the ship object data
        /// </summary>
        /// <returns></returns>
        protected override GameObjectData LoadGameObjectData()
        {
            return AssetManager.LoadData<ShipData>(DataAsset);
        }

        /// <summary>
        /// Loads the ship data and sets up it's stats
        /// </summary>
        public override void LoadContent()
        {
            CheckShouldLoad();

            ShipData = Data as ShipData;
            DebugUtils.AssertNotNull(ShipData);

            Health = ShipData.Defence;

            Turret = AddObject(new Turret(ShipData.Attack, Vector2.Zero));

            base.LoadContent();
        }

        /// <summary>
        /// If the ship is selected we trigger a script to handle the attacking of other ships
        /// </summary>
        /// <param name="elapsedGameTime"></param>
        /// <param name="mousePosition"></param>
        public override void HandleInput(float elapsedGameTime, Vector2 mousePosition)
        {
            base.HandleInput(elapsedGameTime, mousePosition);

            DebugUtils.AssertNotNull(Collider);
            if (Collider.IsClicked && Turret.ShotsLeft > 0)
            {
                ScriptManager.Instance.AddObject(new AttackOpponentShipScript(this), true, true);
            }
        }

        /// <summary>
        /// Kills our parent which will kill us and the card we are attached too
        /// </summary>
        public override void Die()
        {
            base.Die();

            DebugUtils.AssertNotNull(GetParent());
            GetParent().Die();
        }

        #endregion

        #region IDamagable Interface Functions

        /// <summary>
        /// Damages the ship by reducing it's health by the inputted amount
        /// </summary>
        /// <param name="damage"></param>
        public void Damage(float damage)
        {
            Health -= damage;
        }

        /// <summary>
        /// Analyses the ship's current health and indicates it's dead
        /// </summary>
        public void HandleCurrentHealth()
        {
            if (Health <= 0)
            {
                DebugUtils.AssertNotNull(GetParent());
                Dead = true;
            }
        }

        #endregion
    }
}
