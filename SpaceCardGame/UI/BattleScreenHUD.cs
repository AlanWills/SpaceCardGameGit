﻿using _2DEngine;
using CardGameEngine;
using CardGameEngineData;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace SpaceCardGame
{
    public class BattleScreenHUD : UIObjectContainer
    {
        #region Properties and Fields

        /// <summary>
        /// A reference to our player
        /// </summary>
        private Player Player { get; set; }

        /// <summary>
        /// A reference to the player's deck UI
        /// </summary>
        private PlayerDeckUI PlayerDeckUI { get; set; }

        /// <summary>
        /// A button which will end the current player's turn
        /// </summary>
        private Button EndTurnButton { get; set; }

        private float padding = 35;

        #endregion

        public BattleScreenHUD(string hudTextureAsset) :
            base(ScreenManager.Instance.ScreenDimensions, ScreenManager.Instance.ScreenCentre, hudTextureAsset)
        {
            Player = BattleScreen.Player;
        }

        #region Virtual Functions

        /// <summary>
        /// Sets up callbacks for when we draw a card
        /// </summary>
        public override void LoadContent()
        {
            CheckShouldLoad();

            Player.OnCardDraw += AddPlayerHandCardUI;

            PlayerDeckUI = AddObject(new PlayerDeckUI(Player, new Vector2(ScreenManager.Instance.ScreenDimensions.X * 0.45f, ScreenManager.Instance.ScreenDimensions.Y * 0.4f)));

            EndTurnButton = AddObject(new Button("End Turn", Vector2.Zero));
            EndTurnButton.OnLeftClicked += OnEndTurnButtonClicked;

            base.LoadContent();
        }

        /// <summary>
        /// Fixup some UI
        /// </summary>
        public override void Initialise()
        {
            CheckShouldInitialise();

            base.Initialise();

            EndTurnButton.LocalPosition += new Vector2((Size.X - EndTurnButton.Size.X) * 0.5f - 10, 0);
        }

        /// <summary>
        /// Do some size fixup - when we resize cards this may not be necessary
        /// </summary>
        public override void Begin()
        {
            // Do this before we call base.Begin, because we need the new size before we call the begin function in there
            PlayerDeckUI.Size *= 0.5f;

            base.Begin();
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// A callback every time we draw a card.
        /// Adds appropriate UI for the newly drawn card.
        /// </summary>
        /// <param name="drawnCard"></param>
        private void AddPlayerHandCardUI(CardData drawnCard)
        {
            Vector2 screenDimensions = ScreenManager.Instance.ScreenDimensions;
            Vector2 size = new Vector2(screenDimensions.X * 0.1f, screenDimensions.Y * 0.15f);

            PlayerHandCardThumbnail cardUI = AddObject(new PlayerHandCardThumbnail(drawnCard, size, Vector2.Zero), true, true);
            size = cardUI.Size;
            cardUI.LocalPosition = new Vector2((size.X + padding) * (Player.CurrentHand.Count - 1) + (size.X - screenDimensions.X) * 0.5f + padding, (screenDimensions.Y - size.Y) * 0.5f - padding);

            cardUI.OnLeftClicked += AddCardToGame;
            cardUI.OnRightClicked += AddInfoImage;
        }

        #endregion

        #region Click Callbacks

        /// <summary>
        /// Nothing more than a callback wrapper around the NewPlayerTurn - but this is specifically for callbacks.
        /// Want to keep the original function too, so that we can end the turn after a certain amount of time too.
        /// </summary>
        /// <param name="clickable"></param>
        private void OnEndTurnButtonClicked(IClickable clickable)
        {
            Debug.Assert(ScreenManager.Instance.CurrentScreen is BattleScreen);

            (ScreenManager.Instance.CurrentScreen as BattleScreen).NewPlayerTurn();
        }

        /// <summary>
        /// A callback which adds a card in our hand to the game
        /// </summary>
        /// <param name="clickable"></param>
        private void AddCardToGame(IClickable clickable)
        {
            Debug.Assert(clickable is PlayerHandCardThumbnail);

            ScriptManager.Instance.AddObject(new PlaceCardScript(clickable as PlayerHandCardThumbnail), true, true);
        }

        /// <summary>
        /// A callback which displays an image with more in depth information about a card, bound to a click dismiss script (which will remove it as soon as we click anywhere)
        /// </summary>
        /// <param name="clickable"></param>
        private void AddInfoImage(IClickable clickable)
        {
            Debug.Assert(clickable is UIObject);

            string cardTextureAsset = (clickable as UIObject).TextureAsset;
            Vector2 screenDimensions = ScreenManager.Instance.ScreenDimensions;

            Image infoImage = AddObject(new Image(new Vector2(screenDimensions.X * 0.5f, screenDimensions.Y * 0.5f), Vector2.Zero, cardTextureAsset), true, true);
            ScriptManager.Instance.AddObject(new ClickDismissScript(infoImage), true, true);
        }

        #endregion
    }
}