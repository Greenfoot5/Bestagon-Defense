using Abstract.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TurretStats
{
    /// <summary>
    /// Displays a single stat on a turret's shop card
    /// </summary>
    public class TurretStat : MonoBehaviour
    {
        [Tooltip("The icon of the turret stat to set the colour of")]
        [SerializeField]
        private Image icon;
        [Tooltip("The text to fill with the turret's stat")]
        [SerializeField]
        private TextMeshProUGUI text;
        
        /// <summary>
        /// Sets the colour of the icon
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            icon.color = color;
        }
        
        /// <summary>
        /// Sets the text's content for the stat
        /// </summary>
        /// <param name="data"></param>
        private void SetData(string data)
        {
            text.text = data;
        }
        
        /// <summary>
        /// Sets all data for the stat
        /// </summary>
        /// <param name="data">The data to set</param>
        public void SetData(object data) => SetData(data.ToString());
    }
}
