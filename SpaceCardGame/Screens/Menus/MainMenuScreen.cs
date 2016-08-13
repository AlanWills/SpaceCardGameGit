﻿using _2DEngine;
using Microsoft.Xna.Framework;
using SpaceCardGameData;
using System;
using System.IO;

namespace SpaceCardGame
{
    public class MainMenuScreen : MenuScreen
    {
        private const float shipSpawnTime = 2;
        private float currentShipSpawnTimer = shipSpawnTime;

        private const float missileSpawnTime = 0.5f;
        private float currentMissileSpawnTime = missileSpawnTime;

        public MainMenuScreen(string screenDataAsset = "Screens\\MainMenuScreen.xml") :
            base(screenDataAsset)
        {

        }

        #region Virtual Functions

        /// <summary>
        /// Add Buttons to our MainMenuScreen
        /// </summary>
        protected override void AddInitialUI()
        {
            base.AddInitialUI();

            ListControl buttonControl = AddScreenUIObject(new ListControl(new Vector2(ScreenDimensions.X * 0.75f, ScreenDimensions.Y * 0.4f)));
            buttonControl.UseScissorRectangle = false;

            Button newGameButton = buttonControl.AddChild(new Button("New Campaign", Vector2.Zero));
            newGameButton.AddModule(new ToolTipModule("Begin a new campaign"));
            newGameButton.ClickableModule.OnLeftClicked += OnNewGameButtonLeftClicked;

            Button continueGameButton = buttonControl.AddChild(new Button("Continue Campaign", Vector2.Zero));
            continueGameButton.ClickableModule.OnLeftClicked += OnContinueButtonLeftClicked;

            Button optionsButton = buttonControl.AddChild(new Button("Options", Vector2.Zero));
            optionsButton.ClickableModule.OnLeftClicked += OnOptionsButtonClicked;

#if DEBUG
            // If in debug add the hardpoint screen option
            Button hardpointButton = buttonControl.AddChild(new Button("Hardpoint Calculator", Vector2.Zero));
            hardpointButton.ClickableModule.OnLeftClicked += OnHardpointButtonClicked;
#endif

            Button exitGameButton = buttonControl.AddChild(new Button("Exit", Vector2.Zero));
            exitGameButton.ClickableModule.OnLeftClicked += OnExitGameButtonClicked;

            // Add static string on each card for their data asset?
        }

        /// <summary>
        /// Add our ships here so that we can load and initialise them immediately to fix up sizes etc.
        /// </summary>
        public override void Initialise()
        {
            CheckShouldInitialise();

            base.Initialise();

            AddShips();
        }

        public override void Update(float elapsedGameTime)
        {
            base.Update(elapsedGameTime);

            currentShipSpawnTimer += elapsedGameTime;
            if (currentShipSpawnTimer > shipSpawnTime)
            {
                currentShipSpawnTimer = 0;
                SpawnSmallShip();
            }

            currentMissileSpawnTime += elapsedGameTime;
            if (currentMissileSpawnTime > missileSpawnTime)
            {
                currentMissileSpawnTime = 0;
                FireAtSmallShips();
            }
        }

        #endregion

        #region Animated Ship UI Sugar

        private void AddShips()
        {
            GameObject eagleFrigate = AddGameObject(new GameObject(new Vector2(ScreenDimensions.X * 0.25f, ScreenDimensions.Y * 0.75f), "Cards\\Ships\\EagleFrigate\\EagleFrigateObject.xml"), true, true);
            eagleFrigate.Name = "Eagle Frigate";
            float maxDimension = Math.Max(eagleFrigate.Size.X, eagleFrigate.Size.Y);

            Image eagleShield = eagleFrigate.AddChild(new Image(new Vector2(1.75f * maxDimension), Vector2.Zero, "Cards\\Shields\\PhaseEnergyShield\\PhaseEnergyShield"), true, true);
        }

        private void SpawnSmallShip()
        { 
            GameObject smallShip = AddGameObject(new GameObject(new Vector2(ScreenCentre.X, -100), "Cards\\Ships\\BlazeInterceptor\\BlazeInterceptorObject.xml"), true, true);
            smallShip.LocalRotation = MathHelper.Pi;
            smallShip.Name = "Target";
            smallShip.AddModule(new MoveToDestinationModule(new Vector2(ScreenCentre.X, ScreenDimensions.Y + 30), 375), true, true);
        }

        // Change this so it doesn't fire missiles, but instead fires gatling bullets in a straight line - one bullet for each ship
        // Use the RigidBodyModule
        // Also, add the Board background onto the main menu screen - not sure how we add the background per-se but it's awesome - Screen Module?  YES TRY IT

        private void FireAtSmallShips()
        {
            GameObject eagleFrigate = FindGameObject<GameObject>(x => x.Name == "Eagle Frigate");
            DebugUtils.AssertNotNull(eagleFrigate);

            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.Name == "Target")
                {
                    Missile missile = AddGameObject(new Missile(gameObject, eagleFrigate.WorldPosition, AssetManager.GetData<ProjectileData>("Cards\\Weapons\\Missile\\VulcanMissileTurret\\VulcanMissileTurretBullet.xml")), true, true);
                }
            }
        }

        #endregion

        #region Event callbacks for main menu screen buttons

        /// <summary>
        /// The callback to execute when we press the 'Play' button
        /// </summary>
        /// <param name="baseObject">The baseObject that was clicked</param>
        private void OnNewGameButtonLeftClicked(BaseObject baseObject)
        {
            // Need to load assets before we transition to the next screen
            PlayerDataRegistry.Instance.LoadAssets(PlayerDataRegistry.startingDataRegistryDataAsset);

            // Reset the player's current level to 1
            PlayerDataRegistry.Instance.PlayerData.CurrentLevel = 1;
            Transition(new LobbyMenuScreen());
        }

        /// <summary>
        /// The callback to execute when we press the 'Continue' button
        /// </summary>
        /// <param name="baseObject"></param>
        private void OnContinueButtonLeftClicked(BaseObject baseObject)
        {
            // Need to load assets before we transition to the next screen
            PlayerDataRegistry.Instance.LoadAssets(PlayerDataRegistry.playerDataRegistryDataAsset);
            Transition(new LobbyMenuScreen());
        }

        /// <summary>
        /// The callback to execute when we press the 'Options' button
        /// </summary>
        /// <param name="baseObject">The image that was clicked</param>
        private void OnOptionsButtonClicked(BaseObject baseObject)
        {
            Transition(new GameOptionsScreen());
        }

#if DEBUG

        /// <summary>
        /// The callback to execute where we transition to the hardpoint screen
        /// </summary>
        /// <param name="baseObject"></param>
        private void OnHardpointButtonClicked(BaseObject baseObject)
        {
            Transition(new HardpointScreen());
        }

#endif

        /// <summary>
        /// The callback to execute when we press the 'Exit' button
        /// </summary>
        /// <param name="baseObject">Unused</param>
        private void OnExitGameButtonClicked(BaseObject baseObject)
        {
            ScreenManager.Instance.EndGame();
        }

        #endregion
    }
}