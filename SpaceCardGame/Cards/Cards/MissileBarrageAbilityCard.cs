﻿using _2DEngine;
using SpaceCardGameData;

namespace SpaceCardGame
{
    /// <summary>
    /// The missile barrage ability card.
    /// Deals one damage to all enemies with a combined defence and speed of 5 or less when laid.
    /// </summary>
    public class MissileBarrageAbilityCard : AbilityCard
    {
        public MissileBarrageAbilityCard(AbilityCardData abilityCardData) :
            base(abilityCardData)
        {

        }

        #region Virtual Functions

        /// <summary>
        /// When we lay this card, we do damage all the opponent's ships with defence + speed <= 5
        /// </summary>
        public override void OnLay(Board board, GamePlayer player)
        {
            base.OnLay(board, player);

            foreach (CardShipPair pair in board.NonActivePlayerBoardSection.GameBoardSection.ShipCardControl)
            {
                // Add up the ship's speed and defence and if it is less than or equal to 5, we do one damage to it
                if (pair.Ship.ShipData.Defence + pair.Ship.ShipData.Speed <= 5)
                {
                    pair.Ship.DamageModule.Damage(1);
                    SpawnBulletAtTarget(pair.Ship);
                }
            }

            // Kill our parent which will kill us and the object we are with too
            Parent.Die();
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Spawns a bullet (not parented under a turret, but just as effect) which fires at the inputted target ship
        /// </summary>
        /// <param name="targetShip"></param>
        private void SpawnBulletAtTarget(Ship targetShip)
        {
            BulletData bulletData = AssetManager.GetData<BulletData>(Bullet.defaultBulletDataAsset);
            DebugUtils.AssertNotNull(bulletData);

            ScreenManager.Instance.CurrentScreen.AddInGameUIObject(new Bullet(targetShip, WorldPosition, bulletData), true, true);
        }

        #endregion
    }
}