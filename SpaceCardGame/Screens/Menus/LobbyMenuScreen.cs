﻿using _2DEngine;
using CardGameEngine;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace SpaceCardGame
{
    /// <summary>
    /// A class representing the screen after our MainMenuScreen, where players can edit their decks and play games
    /// </summary>
    public class LobbyMenuScreen : MenuScreen
    {
        public LobbyMenuScreen(string screenDataAsset = "Content\\Data\\Screens\\LobbyMenuScreen.xml") :
            base(screenDataAsset)
        {

        }

        #region Properties and Fields

        /// <summary>
        /// Adds our buttons for playing or managing decks
        /// </summary>
        protected override void AddInitialUI()
        {
            base.AddInitialUI();

            float padding = ScreenDimensions.Y * 0.1f;
            UIObject parent = null;

            Button playGameButton = AddScreenUIObject(new Button("Play", new Vector2(ScreenDimensions.X * 0.5f, ScreenDimensions.Y * 0.40f)));
            playGameButton.OnLeftClicked += OnPlayGameButtonLeftClicked;
            parent = playGameButton;

            // Disable the play button if we have no decks to choose from
            if (PlayerCardRegistry.Instance.AvailableDecks == 0)
            {
                playGameButton.Disable();
            }

            Button deckManagerButton = AddScreenUIObject(new Button("Decks", new Vector2(0, padding)));
            deckManagerButton.SetParent(parent);
            deckManagerButton.OnLeftClicked += OnDeckManagerButtonClicked;
            parent = deckManagerButton;
        }

        /// <summary>
        /// Transition back to our main menu screen
        /// </summary>
        protected override void GoToPreviousScreen()
        {
            Transition(new MainMenuScreen());
        }

        #endregion

        #region Click Callbacks

        /// <summary>
        /// The callback to execute when we press the 'Play' button
        /// </summary>
        /// <param name="clickable"></param>
        private void OnPlayGameButtonLeftClicked(IClickable clickable)
        {
            // Have to do this separately so we get the callbacks added to our objects during load
            ChooseDeckBox chooseDeckBox = AddScreenUIObject(new ChooseDeckBox("Choose Deck", ScreenCentre), true, true);
            chooseDeckBox.OnLeftClicked += ChooseDeckBoxClicked;
        }

        /// <summary>
        /// The callback to execute when we press the 'Decks' button
        /// </summary>
        /// <param name="image"></param>
        private void OnDeckManagerButtonClicked(IClickable clickable)
        {
            Transition(new GameDeckManagerScreen());
        }

        /// <summary>
        /// The callback to execute when we choose our deck
        /// </summary>
        /// <param name="image"></param>
        private void ChooseDeckBoxClicked(IClickable clickable)
        {
            Debug.Assert(clickable is ClickableImage);
            ClickableImage clickableImage = clickable as ClickableImage;

            DebugUtils.AssertNotNull(clickableImage.StoredObject);
            Debug.Assert(clickableImage.StoredObject is Deck);

            Transition(new BattleScreen(clickableImage.StoredObject as Deck, "Content\\Data\\Screens\\BattleScreen.xml"));
        }

        #endregion
    }
}