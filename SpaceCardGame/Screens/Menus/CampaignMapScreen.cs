﻿using CelesteEngine;
using Microsoft.Xna.Framework;
using SpaceCardGameData;
using System.Collections.Generic;

namespace SpaceCardGame
{
    /// <summary>
    /// The screen in which we will choose missions - these missions will dictate the deck that our computer AI will have
    /// </summary>
    public class CampaignMapScreen : MenuScreen
    {
        #region Virtual Functions

        private GridControl MissionGridControl { get; set; }
        private MissionInfoImage CurrentSelectedMission { get; set; }

        #endregion

        public CampaignMapScreen() :
            base("CampaignMapScreen")
        {

        }

        #region Virtual Functions

        /// <summary>
        /// Set up our buttons for the campaign thumbnails
        /// </summary>
        protected override void AddInitialUI()
        {
            base.AddInitialUI();

            MissionGridControl = AddScreenUIObject(new GridControl(4, new Vector2(ScreenDimensions.X * 0.5f, ScreenDimensions.Y), new Vector2(ScreenCentre.X * 0.5f, ScreenCentre.Y)));
            MissionGridControl.Padding = new Vector2(60, 60);

            List<MissionData> allMissionData = AssetManager.GetAllDataOfType<MissionData>();
            foreach (MissionData missionData in allMissionData)
            {
                ClickableImage clickImage = MissionGridControl.AddChild(new ClickableImage(Vector2.Zero, missionData.MissionThumbnailTextureAsset));
                clickImage.ClickableModule.OnLeftClicked += OnMissionLeftClicked;
                clickImage.StoredObject = missionData;

                if (missionData.MissionNumber > PlayerDataRegistry.Instance.PlayerData.CurrentLevel)
                {
                    clickImage.Hide();
                }
            }

            CurrentSelectedMission = AddScreenUIObject(new MissionInfoImage(allMissionData[PlayerDataRegistry.Instance.PlayerData.CurrentLevel - 1]));
        }

        protected override void GoToPreviousScreen()
        {
            Transition(new LobbyMenuScreen());
        }

        #endregion

        #region Callbacks

        private void OnMissionLeftClicked(BaseObject clickedObject)
        {
            // Cleanup the old mission info
            CurrentSelectedMission.Die();

            // Add the new mission info image
            CurrentSelectedMission = AddScreenUIObject(new MissionInfoImage((clickedObject as UIObject).StoredObject as MissionData), true, true);
        }

        #endregion
    }
}