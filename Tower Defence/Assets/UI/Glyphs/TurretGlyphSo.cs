using UnityEngine;

namespace UI.Glyphs
{
    /// <summary>
    /// Stores the settings for a glyph
    /// </summary>
    [CreateAssetMenu(fileName = "NewTurretGlyph", menuName = "TurretGlyph", order=1)]
    public class TurretGlyphSo : ScriptableObject
    {
        [Tooltip("The sprite of the glyph")]
        public Sprite glyph;
        [Tooltip("The main colour of the sprite")]
        public Color body = new Color(255, 255, 255, 100);
        [Tooltip("The shade to apply to the colour to obtain a secondary colour")]
        public Color shade = new Color(0, 0, 0, 0.25f);
    }
}
