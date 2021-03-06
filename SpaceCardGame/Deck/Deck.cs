﻿using SpaceCardGameData;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpaceCardGame
{
    /// <summary>
    /// A class representing our current usable deck - cards chosen from the player's registry
    /// </summary>
    public class Deck
    {
        #region Properties and Fields

        /// <summary>
        /// The cards in this deck
        /// </summary>
        public List<CardData> Cards { get; private set; }

        /// <summary>
        /// The name of the deck
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A flag to indicate that this deck has been created.
        /// All decks are constructed at startup so that we can pass references, but we still need them to be 'created' in the manager screen.
        /// </summary>
        public bool IsCreated { get; private set; }

        #endregion

        public Deck()
        {
            Name = "";
            Cards = new List<CardData>();
        }

        #region Creation/State methods

        /// <summary>
        /// The function to call to create a deck and mark it as created.
        /// </summary>
        public void Create()
        {
            // Our cards should be empty, otherwise something bad has happened
            Debug.Assert(Cards.Count == 0);
            Debug.Assert(!IsCreated);
            IsCreated = true;
        }

        /// <summary>
        /// Creates a list and loads the card data assets we have inputted.
        /// </summary>
        /// <param name="cards">The initial card data assets to add to this deck</param>
        public void Create(List<string> cards)
        {
            Create();

            Cards.AddRange(CentralCardRegistry.ConvertToDataList(cards));
        }

        /// <summary>
        /// Creates a list and adds the inputted cards to it.
        /// </summary>
        /// <param name="cards">The initial cards to add to this deck</param>
        public void Create(List<CardData> cards)
        {
            Create();

            Cards.AddRange(cards);
        }

        /// <summary>
        /// The function to call to remove a deck and mark it as not created.
        /// </summary>
        public void Delete()
        {
            // Clear our deck and mark it as not created.
            Cards.Clear();
            IsCreated = false;
        }

        #endregion
    }
}
