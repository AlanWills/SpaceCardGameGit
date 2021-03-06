﻿using CelesteEngine;
using Microsoft.Xna.Framework;

namespace SpaceCardGame
{
    /// <summary>
    /// A wrapper class to allow us to transition between screens
    /// </summary>
    public class DeckEditingScreen : MenuScreen
    {
        #region Properties and Fields

        /// <summary>
        /// The deck we will be editing.
        /// </summary>
        private Deck Deck { get; set; }

        #endregion

        public DeckEditingScreen(Deck deck) :
            base("DeckEditingScreen")
        {
            Deck = deck;
        }

        #region Virtual Functions

        /// <summary>
        /// Adds the UI for editing our deck
        /// </summary>
        protected override void AddInitialUI()
        {
            base.AddInitialUI();

            float tabControlHeight = ScreenDimensions.Y * 0.035f;

            TabControl tabControl = AddScreenUIObject(new TabControl(new Vector2(ScreenDimensions.X, tabControlHeight), new Vector2(ScreenCentre.X, tabControlHeight * 0.5f)));

            // Add a DeckCardTypeControl for each resource type
            foreach (string cardType in CentralCardRegistry.CardTypes)
            {
                tabControl.AddChild(new DeckCardTypeControl(Deck, cardType, new Vector2(ScreenDimensions.X, ScreenDimensions.Y - tabControlHeight), new Vector2(0, ScreenCentre.Y)));
            }
        }

        /// <summary>
        /// Want to return to our deck manager screen
        /// </summary>
        /// <returns></returns>
        protected override void GoToPreviousScreen()
        {
            PlayerDataRegistry.Instance.SaveAssets();
            Transition(new DeckManagerScreen());
        }

        #endregion
    }
}
