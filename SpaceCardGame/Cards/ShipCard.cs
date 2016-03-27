﻿using SpaceCardGameData;

namespace SpaceCardGame
{
    public class ShipCard : GameCard
    {
        public ShipCard(ShipCardData shipCardData) :
            base(shipCardData)
        {

        }

        #region Virtual Functions

        /// <summary>
        /// Looks at how many ships we have placed already and the resource costs of the card we wish to lay
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool CanLay(GamePlayer player)
        {
            // Return if we have no more room!
            if (player.CurrentShipsPlaced >= GamePlayer.MaxShipNumber) { return false; }

            // Check resource requirements
            return base.CanLay(player);
        }

        #endregion
    }
}