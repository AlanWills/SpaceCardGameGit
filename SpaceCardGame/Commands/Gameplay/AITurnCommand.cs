﻿using _2DEngine;
using CardGameEngine;
using System;
using System.Diagnostics;

namespace SpaceCardGame
{
    /// <summary>
    /// A command which runs decision algorithms for an AI player
    /// </summary>
    public class AITurnCommand : Command
    {
        #region Properties and Fields

        /// <summary>
        /// A reference to the AI player we will be controlling in this command
        /// </summary>
        private GamePlayer AIPlayer { get; set; }

        /// <summary>
        /// A reference to the board section we will be controlling in this command
        /// </summary>
        private PlayerBoardSection BoardSection { get; set; }

        /// <summary>
        /// A reference to our battle screen
        /// </summary>
        private BattleScreen BattleScreen { get; set; }

        private float timeBetweenCardLays = 1;
        private float timeBetweenAttacks = 1;
        private float timeUntilTurnEnd = 2.5f;
        private float currentTimeBetweenCardLays = 0;
        private float currentTimeBetweenAttacks = 0;
        private float currentTimeUntilTurnEnd = 0;

        #endregion

        public AITurnCommand(GamePlayer player, PlayerBoardSection playerBoardSection) :
            base()
        {
            AIPlayer = player;
            BoardSection = playerBoardSection;
        }

        #region Virtual Functions

        /// <summary>
        /// Set up the reference to our battle screen and hides the Progress button 
        /// </summary>
        public override void Begin()
        {
            base.Begin();

            Debug.Assert(ParentScreen is BattleScreen);
            BattleScreen = ParentScreen as BattleScreen;

            BattleScreen.ProgressTurnButton.Disable();
        }

        /// <summary>
        /// Updates our player's state based on the turn state and various other factors
        /// </summary>
        /// <param name="elapsedGameTime"></param>
        public override void Update(float elapsedGameTime)
        {
            base.Update(elapsedGameTime);

            if (BattleScreen.TurnState == TurnState.kPlaceCards)
            {
                // Updates our AI player when in the card placement state
                OnPlaceCardsState(elapsedGameTime);
            }
            else
            {
                OnBattleState(elapsedGameTime);
            }
        }

        #endregion

        #region Turn State Functions

        /// <summary>
        /// Lay as many resource cards as possible and add appropriate ships/weapons etc.
        /// </summary>
        private void OnPlaceCardsState(float elapsedGameTime)
        {
            currentTimeBetweenCardLays += elapsedGameTime;

            // Check to see if we have laid the resource cards we can this turn and we have resources in our hand we can lay
            if (AIPlayer.CurrentHand.Exists(GetCardLayPredicate<ResourceCardData>()))
            {
                // Lay a resource card
                CardData resourceCardData = AIPlayer.CurrentHand.Find(GetCardLayPredicate<ResourceCardData>());

                if (currentTimeBetweenCardLays >= timeBetweenCardLays)
                {
                    // TODO Can improve this by analysing the resource costs of the other cards and working out what cards would be best to lay
                    LayCard(resourceCardData);
                }
            }
            // Check to see if we have laid the ships we can and we have ships in our hand we can lay
            else if (AIPlayer.CurrentShipsPlaced < GamePlayer.MaxShipNumber && AIPlayer.CurrentHand.Exists(GetCardLayPredicate<ShipCardData>()))
            {
                if (currentTimeBetweenCardLays >= timeBetweenCardLays)
                {
                    // Lay a ship card
                    CardData shipCardData = AIPlayer.CurrentHand.Find(GetCardLayPredicate<ShipCardData>());

                    LayCard(shipCardData);
                }
            }
            else
            {
                // Move to battle phase
                ChangeState();

                // If we cannot actually do anything in our battle phase we should skip it and move to the player's turn
                if (!ContinueBattlePhase())
                {
                    ChangeState();
                }
            }
        }

        /// <summary>
        /// Attack the opponent ships
        /// </summary>
        private void OnBattleState(float elapsedGameTime)
        {
            currentTimeBetweenAttacks += elapsedGameTime;

            // See if we have valid conditions to carry on fighting
            if (ContinueBattlePhase())
            {
                // Space out attacks so they don't all happen at once
                if (currentTimeBetweenAttacks > timeBetweenAttacks)
                {
                    // Loop through each of our ships
                    foreach (CardShipPair pair in BoardSection.GameBoardSection.ShipCardControl)
                    {
                        // Make sure we can fire
                        if (GetReadyShipAndCanFirePredicate().Invoke(pair))
                        {
                            // TODO Can improve this by analysing the best opponent ship to attack
                            AttackOpponentShip(pair.Ship);
                        }
                    }
                }
            }
            else
            {
                currentTimeUntilTurnEnd += elapsedGameTime;
                if (currentTimeUntilTurnEnd > timeUntilTurnEnd)
                {
                    // Change state after a certain amount of time has passed
                    ChangeState();
                }
            }
        }

        /// <summary>
        /// Shows the ProgressTurnButton again
        /// </summary>
        public override void Die()
        {
            base.Die();

            BattleScreen.ProgressTurnButton.Show();
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Changes the state of our current turn and kills this script when our turn is over.
        /// </summary>
        private void ChangeState()
        {
            currentTimeBetweenCardLays = 0;
            currentTimeBetweenAttacks = 0;
            currentTimeUntilTurnEnd = 0;

            // If we are currently in a battle state and about to change to the next player's turn then we should kill this script
            if (BattleScreen.TurnState == TurnState.kBattle)
            {
                Die();
                BattleScreen.ProgressTurnButton.Enable();
            }

            BattleScreen.ProgressTurnButton.ClickableModule.ForceClick();
        }

        /// <summary>
        /// Creates and adds a card using the inputted card data and resets the timer so we have spacing between adding cards.
        /// </summary>
        /// <param name="cardData"></param>
        private void LayCard(CardData cardData)
        {
            Debug.Assert(currentTimeBetweenCardLays >= timeBetweenCardLays);

            BaseUICard cardThumbnail = BoardSection.UIBoardSection.HandUI.FindCardThumbnail(cardData);
            cardThumbnail.Die();

            // Set the position of the card so that when we add it to the game board section it will be added to a slot
            BoardSection.GameBoardSection.AddCard(cardData, cardThumbnail.Size);

            currentTimeBetweenCardLays = 0;
        }

        /// <summary>
        /// A function which analyses lots of small sub conditions for whether the AI can still make a move.
        /// These include:
        /// It has ships
        /// The opponent has ships
        /// It's ships are ready
        /// It's ships have shots left
        /// </summary>
        /// <returns></returns>
        private bool ContinueBattlePhase()
        {
            // The AI has no ships, or the player has no ships, so return false
            if (AIPlayer.CurrentShipsPlaced == 0 || BattleScreen.Player.CurrentShipsPlaced == 0)
            {
                return false;
            }
            
            // If we cannot find a ship which is ready, then return false
            if (!BoardSection.GameBoardSection.ShipCardControl.Exists(GetReadyShipPredicate()))
            {
                return false;
            }

            // If we cannot find a ship which has a turret with shots left, then return false
            if (!BoardSection.GameBoardSection.ShipCardControl.Exists(GetReadyShipAndCanFirePredicate()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Searches for a ship to attack, attacks it and resets our timer for between attacks.
        /// Inputted ship guaranteed to be able to fire at this point.
        /// </summary>
        /// <param name="attackingShip"></param>
        private void AttackOpponentShip(Ship attackingShip)
        {
            foreach (CardShipPair pair in BattleScreen.Board.NonActivePlayerBoardSection.GameBoardSection.ShipCardControl)
            {
                DebugUtils.AssertNotNull(pair.Ship.DamageModule);
                if (pair.Ship.DamageModule.Health > 0)
                {
                    float targetAngle = MathUtils.AngleBetweenPoints(attackingShip.WorldPosition, pair.CardObject.WorldPosition);
                    attackingShip.Turret.LocalRotation = targetAngle - attackingShip.WorldRotation;

                    // Attack the selected ship
                    attackingShip.Turret.Attack(pair.Ship);
                    break;
                }
            }

            currentTimeBetweenAttacks = 0;
        }

        #endregion

        #region Logical Predicates

        /// <summary>
        /// Returns a predicate for finding all the cards of the inputted type that can also be laid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Predicate<CardData> GetCardLayPredicate<T>() where T : CardData
        {
            string error = "";
            return new Predicate<CardData>(x => (x is T) && (x as T).CanLay(AIPlayer, ref error));
        }

        /// <summary>
        /// A predicate to find a ship which is ready
        /// </summary>
        /// <returns></returns>
        private Predicate<BaseObject> GetReadyShipPredicate()
        {
            return new Predicate<BaseObject>(x => (x is CardShipPair) && (x as CardShipPair).IsReady);
        }

        /// <summary>
        /// A predicate to find a ship which is ready and has turrets with shots left
        /// </summary>
        /// <returns></returns>
        private Predicate<BaseObject> GetReadyShipAndCanFirePredicate()
        {
            return new Predicate<BaseObject>(x => (x is CardShipPair) && (x as CardShipPair).IsReady && (x as CardShipPair).Ship.Turret.CanFire);
        }

        #endregion
    }
}