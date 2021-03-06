﻿using CelesteEngine;

namespace SpaceCardGame
{
    /// <summary>
    /// The beam turret fires beams and implements the creation of them when firing.
    /// </summary>
    public class BeamTurret : Turret
    {
        public BeamTurret(string turretDataAsset) :
            base(turretDataAsset)
        {

        }

        #region Virtual Functions

        /// <summary>
        /// Create a beam for firing.
        /// </summary>
        protected override void CreateBullet(GameObject target)
        {
            ScreenManager.Instance.CurrentScreen.AddInGameUIObject(new Beam(target, WorldPosition, BulletData), true, true);
        }

        #endregion
    }
}
