﻿using _2DEngineData;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CardGameEngineData
{
    public class CardData : GameObjectData
    {
        /// <summary>
        /// The object on the card's texture asset - potentially empty if not used in game
        /// </summary>
        public string ObjectTextureAsset { get; set; }

        /// <summary>
        /// The type of this card
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The name of the card (for UI purposes)
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// A string to indicate the rarity of this card
        /// </summary>
        public string Rarity { get; set; }

        /// <summary>
        /// A list of the resources required to lay this card
        /// </summary>
        [XmlArrayItem(ElementName = "Resource")]
        public List<int> ResourceCosts { get; set; }
    }
}