using System;
using System.Collections.Generic;
using Turrets;
using Turrets.Choker;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Laser;
using Turrets.Shooter;
using Turrets.Smasher;
using UI.Glyphs;
using UnityEngine;

namespace Abstract
{
    /// <summary>
    /// A class to reference between turret types and turret glyphs
    /// </summary>
    [Serializable]
    public class TypeSpriteLookup
    {
        private static readonly List<Type> Types = new()
        {
            typeof(Turret), // Represents no specific turret type
            typeof(Shooter),
            typeof(Laser),
            typeof(Smasher),
            typeof(Gunner),
            typeof(Lancer),
            typeof(Choker)
        };
        
        [Tooltip("The list of TurretGlyphs to use")]
        [SerializeField]
        private List<TurretGlyphSo> sprites;
        
        /// <summary>
        /// Get all the types the game currently has stored
        /// </summary>
        /// <returns>All the types current stored</returns>
        public static List<Type> GetAllTypes()
        {
            return Types;
        }
        
        /// <summary>
        /// Gets the glyph for a specific turret type
        /// </summary>
        /// <param name="t">The turret type to get the glyph for</param>
        /// <returns>The turret's glyph</returns>
        public TurretGlyphSo GetForType(Type t)
        {
            try
            {
                return sprites[Types.IndexOf(t)]; 
            }
            catch (Exception ex)
            {
                if (ex is not IndexOutOfRangeException && ex is not ArgumentOutOfRangeException) throw;
            
                Debug.LogError("Can't find sprite of type " + t);
                return sprites[0];

            }
        } 
    }
}