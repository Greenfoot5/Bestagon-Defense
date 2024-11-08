using UnityEngine;
using UnityEngine.Localization;

namespace Turrets
{
    public abstract class PlacedObject : MonoBehaviour
    {
        [Tooltip("The display name of the object")]
        [HideInInspector]
        public LocalizedString displayName;

        /// <summary>
        /// Called when the object is selected
        /// </summary>
        public abstract void Selected();

        /// <summary>
        /// Called when the object is deselected
        /// </summary>
        public abstract void Deselected();
    }
}
