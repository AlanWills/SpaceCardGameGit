﻿using _2DEngine;
using CardGameEngineData;
using Microsoft.Xna.Framework;
using System;

namespace CardGameEngine
{
    /// <summary>
    /// A class for editting a certain card type between our registry and our current deck.
    /// Contains two list controls, one for the cards in our registry, the other for the cards in our deck.
    /// </summary>
    public class DeckCardTypeControl : UIObject
    {
        #region Properties and Fields

        /// <summary>
        /// A reference to the CardListControl for our deck.
        /// </summary>
        private CardGridControl DeckCardListControl { get; set; }

        /// <summary>
        /// A reference to the CardListControl for the cards in our registry not in our deck.
        /// </summary>
        private CardGridControl RegistryCardListControl { get; set; }

        /// <summary>
        /// A reference to the deck which this UI is representing.
        /// </summary>
        private Deck Deck { get; set; }

        /// <summary>
        /// The type of card this control represents.
        /// </summary>
        private string CardType { get; set; }

        // UI fields
        private int deckColumns = 4;
        private int registryColumns = 6;
        private float ratio;

        #endregion

        public DeckCardTypeControl(Deck deck, string cardType, Vector2 localPosition, string textureAsset = AssetManager.DefaultEmptyPanelTextureAsset) :
            this(deck, cardType, Vector2.Zero, localPosition, textureAsset)
        {

        }

        public DeckCardTypeControl(Deck deck, string cardType, Vector2 size, Vector2 localPosition, string textureAsset = AssetManager.DefaultEmptyPanelTextureAsset) :
            base(size, localPosition, textureAsset)
        {
            Deck = deck;
            CardType = cardType;
            Name = cardType + "s";

            ratio = deckColumns / (float)(deckColumns + registryColumns);
        }

        #region Virtual Functions

        /// <summary>
        /// Create our 
        /// </summary>
        public override void Initialise()
        {
            base.Initialise();

            DeckCardListControl = new CardGridControl(Deck.Cards, deckColumns, new Vector2(Size.X * ratio, Size.Y), new Vector2(Size.X * (0.5f - 0.5f * ratio), 0));
            // Add all the cards in our deck that are of our type
            DeckCardListControl.IncludePredicate = new Predicate<CardData>(x => x.Type == CardType);
            DeckCardListControl.OnRightClicked += RemoveFromDeck;

            // Do this here because we need to add the IncludePredicate before we initialise the control.
            AddChild(DeckCardListControl, true, true);

            // Add a save button here which will serialise the deck to XML
            // Don't parent to list control because otherwise it will move when we scroll
            // For now have the accelerator as the escape key so we automatically save when we exit the screen
            /*Button saveButton = AddObject(new Button("Save Deck", Size * 0.5f), true, true);
            saveButton.LeftClickAccelerator = Keys.Escape;
            saveButton.LocalPosition -= saveButton.Size * 0.5f;
            saveButton.OnLeftClicked += SaveDeck;*/

            RegistryCardListControl = new CardGridControl(PlayerCardRegistry.Instance.AvailableCards, registryColumns, new Vector2(Size.X * (1 - ratio), Size.Y), new Vector2(-ratio * 0.5f * Size.X, 0));
            // Find all cards of our type that are also not in our deck already
            RegistryCardListControl.IncludePredicate = new Predicate<CardData>(x => x.Type == CardType);
            RegistryCardListControl.OnLeftClicked += AddToDeck;

            // Do this here because we need to add the IncludePredicate before we initialise the control.
            AddChild(RegistryCardListControl, true, true);
        }

        #endregion

        #region Click Callbacks

        /// <summary>
        /// The function to call when one of our cards is right clicked in the deck list control.
        /// Removes it from the deck and adds it to our player registry list control.
        /// </summary>
        /// <param name="baseObject">The image we clicked on</param>
        private void RemoveFromDeck(BaseObject baseObject)
        {
            UIObject image = baseObject as UIObject;
            DebugUtils.AssertNotNull(image);

            image.Die();

            DebugUtils.AssertNotNull(image.StoredObject);
            CardData cardData = image.StoredObject as CardData;
            DebugUtils.AssertNotNull(cardData);

            Deck.Cards.Remove(cardData);
            PlayerCardRegistry.Instance.AvailableCards.Add(cardData);

            RegistryCardListControl.AddCard(cardData);
        }

        /// <summary>
        /// The function to call when one of our cards is left clicked in the registry card list control.
        /// Removes it from the the player registry list control and adds it to the deck.
        /// </summary>
        /// <param name="baseObject"></param>
        private void AddToDeck(BaseObject baseObject)
        {
            UIObject image = baseObject as UIObject;
            DebugUtils.AssertNotNull(image);

            image.Die();

            DebugUtils.AssertNotNull(image.StoredObject);
            CardData cardData = image.StoredObject as CardData;
            DebugUtils.AssertNotNull(cardData);

            Deck.Cards.Add(cardData);
            PlayerCardRegistry.Instance.AvailableCards.Remove(cardData);

            DeckCardListControl.AddCard(cardData);
        }

        /// <summary>
        /// The function to call when our save button is clicked.
        /// Saves the deck to XML.
        /// </summary>
        /// <param name="clickable"></param>
        private void SaveDeck(IClickable clickable)
        {
            //Deck.Save();
            PlayerCardRegistry.Instance.Decks[0] = Deck;
        }

        #endregion
    }
}
