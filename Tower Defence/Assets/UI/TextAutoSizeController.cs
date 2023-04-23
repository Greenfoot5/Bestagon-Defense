using TMPro;
using UnityEngine;

namespace UI
{
    public class TextAutoSizeController : MonoBehaviour
    {
        public TMP_Text[] textObjects;
 
        private void Awake()
        {
            if (textObjects == null || textObjects.Length == 0)
                return;
 
            // Iterate over each of the text objects in the array to find the smallest font size
            float minSize = textObjects[0].fontSize;
            
            
            foreach (TMP_Text t in textObjects)
            {
                float fontSize = t.fontSize;
                Debug.Log(fontSize);
                if (fontSize < minSize && fontSize > t.minWidth)
                {
                    minSize = fontSize;
                }
            }
            Debug.Log(minSize);

            // Iterate over all other text objects to set the point size
            foreach (TMP_Text t in textObjects)
            {
                t.enableAutoSizing = false;
                t.fontSize = minSize;
            }
        }
    }
}