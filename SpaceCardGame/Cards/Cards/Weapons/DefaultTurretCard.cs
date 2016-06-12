﻿using System.Diagnostics;

namespace SpaceCardGame
{
    /// <summary>
    /// The weapon card for the default turret every ship has when first created.
    /// Creates a kinetic turret.
    /// </summary>
    public class DefaultTurretCard : WeaponCard
    {
        public DefaultTurretCard(WeaponCardData weaponCardData) :
            base(weaponCardData)
        {

        }

        #region Virtual Functions

        /// <summary>
        /// Creates a kinetic turret
        /// </summary>
        /// <param name="weaponObjectDataAsset"></param>
        /// <returns></returns>
        public override Turret CreateTurret(string weaponObjectDataAsset)
        {
            return new KineticTurret(weaponObjectDataAsset);
        }

        /// <summary>
        /// The default turret card is never played conventionally like a normal card - it is to fit in with our system only.
        /// Therefore it doesn't matter what we return here - it is never going to be called.
        /// </summary>
        /// <returns></returns>
        public override AICardWorthMetric CalculateAIMetric()
        {
            Debug.Fail("This should never be called");
            return AICardWorthMetric.kShouldNotPlayAtAll;
        }

        #endregion
    }
}
