﻿using _2DEngine;
using CardGameEngine;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace SpaceCardGame
{
    /// <summary>
    /// A class to handle the UI objects in the game board for a player
    /// </summary>
    public class UIBoardSection : UIObject
    {
        #region Properties and Fields

        /// <summary>
        /// A reference to the battle screen
        /// </summary>
        private BattleScreen BattleScreen { get; set; }

        /// <summary>
        /// A reference to the player which this UI section is for
        /// </summary>
        private GamePlayer Player { get; set; }

        /// <summary>
        /// A reference to this player's deck UI
        /// </summary>
        public DeckUI DeckUI { get; private set; }

        /// <summary>
        /// A reference to this player's hand UI
        /// </summary>
        public HandUI HandUI { get; private set; }

        #endregion

        public UIBoardSection(GamePlayer player, Vector2 localPosition, string textureAsset = AssetManager.DefaultEmptyPanelTextureAsset) :
            base(new Vector2(ScreenManager.Instance.ScreenDimensions.X, ScreenManager.Instance.ScreenDimensions.Y * 0.5f), localPosition, textureAsset)
        {
            Player = player;

            DeckUI = AddChild(new DeckUI(Player, Vector2.Zero));
            HandUI = AddChild(new HandUI(Player, new Vector2(Size.X * 0.8f, Size.Y * 0.25f), new Vector2(0, Size.Y * 0.4f)));

            Debug.Assert(ScreenManager.Instance.CurrentScreen is BattleScreen);
            BattleScreen = ScreenManager.Instance.CurrentScreen as BattleScreen;

            // Removed this at the moment because when we quickly transition between states we get flickering - not actually sure we want to hide anything anyway
            //BattleScreen.OnCardPlacementStateStarted += SetupUIForCardPlacement;
            //BattleScreen.OnBattleStateStarted += SetupUIForBattle;
        }

        #region Virtual Functions

        /// <summary>
        /// Fixup the position the Deck
        /// </summary>
        public override void Initialise()
        {
            CheckShouldInitialise();

            base.Initialise();

            // Do this before we call base.Begin, because we need the new size before we call the begin function in there
            // In the Begin function for the board section we change this local position, so we must set it here
            DeckUI.Size *= 0.5f;

            Vector2 offset = new Vector2(MathHelper.Max(DeckUI.Size.X, DeckUI.DeckCountLabel.Size.X) + 10, DeckUI.Size.Y + 25);
            DeckUI.LocalPosition = (Size - offset) * 0.5f;
            DeckUI.DeckCountLabel.LocalPosition = new Vector2(0, -(DeckUI.Size.Y + DeckUI.DeckCountLabel.Size.Y) * 0.5f - 10);  // Can't do this in DeckUI because of fixup which occurs elsewhere
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Changes the UI and objects in our game for the card placement phase.
        /// </summary>
        private void SetupUIForBattle()
        {
            DeckUI.Hide();
            HandUI.Hide();
        }

        /// <summary>
        /// Changes the UI and objects in our game for the battle phase.
        /// </summary>
        private void SetupUIForCardPlacement()
        {
            DeckUI.Show();
            HandUI.Show();
        }

        #endregion
    }
}