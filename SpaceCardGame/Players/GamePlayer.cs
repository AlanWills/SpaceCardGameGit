﻿using CardGameEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpaceCardGame
{
    public enum ChargeType
    {
        kCharge,
        kRefund
    }

    public class GamePlayer : Player
    {
        public delegate void NewTurnHandler(GamePlayer newActivePlayer);

        #region Properties and Fields

        /// <summary>
        /// An array of resource card lists.
        /// Allows us to influence the UI by manipulating the resources.
        /// </summary>
        public List<ResourceCard>[] Resources { get; private set; }

        /// <summary>
        /// A cap to limit the number of resource cards we can lay
        /// </summary>
        public int ResourceCardsPlacedThisTurn { get; set; }

        /// <summary>
        /// A cap to limit the number of ships the player can have placed at any one time
        /// </summary>
        public int CurrentShipsPlaced { get; set; }

        /// <summary>
        /// An event which will be fired when we begin a new turn
        /// </summary>
        public event NewTurnHandler OnNewTurn;

        public const int ResourceCardsCanLay = 10;
        public const int MaxShipNumber = 6;

        #endregion

        public GamePlayer(Deck chosenDeck) :
            base(chosenDeck)
        {
            ResourceCardsPlacedThisTurn = 0;
            CurrentShipsPlaced = 0;

            // Set up our resources array
            Resources = new List<ResourceCard>[(int)ResourceType.kNumResourceTypes]
            {
                new List<ResourceCard>(),
                new List<ResourceCard>(),
                new List<ResourceCard>(),
                new List<ResourceCard>()
            };
        }

        #region Virtual Functions

        /// <summary>
        /// Calls our new turn event handler.
        /// Called after our CurrentActivePlayer is updated in our screen.
        /// </summary>
        public override void NewTurn()
        {
            base.NewTurn();

            ResourceCardsPlacedThisTurn = 0;

            // Refresh all our resources so they are all available for use now
            for (ResourceType resourceIndex = ResourceType.Crew; resourceIndex != ResourceType.kNumResourceTypes; resourceIndex++)
            {
                foreach (ResourceCard card in Resources[(int)resourceIndex])
                {
                    card.Used = false;
                }
            }

            OnNewTurn?.Invoke(this);
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Draws a card from the deck of a certain type - useful for the beginning of the game to enforce certain cards appear in the player's hand.
        /// Also useful in certain abilities which add a card type to your hand.
        /// For example, to add a MissileBarrageAbilityCard to your hand, use DrawCard("MissileBarrageAbilityCard").
        /// </summary>
        /// <param name="resourceType"></param>
        public void DrawCard(string cardTypeName)
        {
            Debug.Assert(ChosenDeck.Exists(x => (x as GameCardData).CardTypeName == cardTypeName));
            CardData cardData = ChosenDeck.Find(x => (x as GameCardData).CardTypeName == cardTypeName);
            ChosenDeck.Remove(cardData);

            TriggerDrawCardEvents(cardData);
        }

        /// <summary>
        /// A function which obtains the data for the station in the player's deck and removes it from the deck so that we cannot draw it.
        /// Used at the start of the game to get the station data so we can add it to our board straightaway.
        /// </summary>
        /// <returns></returns>
        public ShipCardData GetStationData()
        {
            // This assert should NEVER trigger.  If it does, it means that the player has no station chosen.
            // This quite simply CANNOT happen otherwise we have no basis for a game!
            Debug.Assert(ChosenDeck.Exists(x => x is ShipCardData && (x as ShipCardData).Type == "Station"));
            ShipCardData stationData = ChosenDeck.Find(x => x is ShipCardData && (x as ShipCardData).Type == "Station") as ShipCardData;
            ChosenDeck.Remove(stationData);

            return stationData;
        }

        /// <summary>
        /// A function which works out whether we have enough resources to lay the inputted card data.
        /// Outputs an error message based on what resource was lacking.
        /// </summary>
        /// <param name="cardData"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool HaveSufficientResources(CardData cardData, ref string error)
        {
            for (int i = 0; i < (int)ResourceType.kNumResourceTypes; i++)
            {
                if (Resources[i].FindAll(x => !x.Used).Count < cardData.ResourceCosts[i])
                {
                    // We do not have enough of the current resource we are analysing to lay this card so return false
                    error = "Insufficient " + Enum.GetNames(typeof(ResourceType))[i];
                    return false;
                }
            }

            // We have enough of each resource type for this card
            return true;
        }

        /// <summary>
        /// Alters the current resources by the amount specified in the card data.
        /// Either charges them or refunds them based on the input enum
        /// </summary>
        /// <param name="cardData"></param>
        /// <param name="charge"></param>
        public void AlterResources(CardData cardData, ChargeType charge)
        {
            for (int typeIndex = 0; typeIndex < (int)ResourceType.kNumResourceTypes; typeIndex++)
            {
                AlterResources((ResourceType)typeIndex, cardData.ResourceCosts[typeIndex], charge);
            }
        }

        /// <summary>
        /// Alters the current resources of the inputted type by the inputted amount.
        /// Either charges them or refunds them based on the input enum
        /// </summary>
        /// <param name="cardData"></param>
        /// <param name="charge"></param>
        public void AlterResources(ResourceType resourceType, int amount, ChargeType charge)
        {
            int typeIndex = (int)resourceType;
            int numAvailableResources = Resources[typeIndex].Count;

            Debug.Assert(numAvailableResources >= amount);

            for (int i = 0; i < amount; ++i)
            {
                // If the 'charge' enum is set to kRefund, we are refunding so the resource should not be used
                // If the 'charge' enum is set to kCharge, we are charging the player the resources, so they should be used
                Resources[typeIndex][i].Used = charge == ChargeType.kCharge ? true : false;
            }
        }

        #endregion
    }
}
