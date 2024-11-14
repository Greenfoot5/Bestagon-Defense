using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Abstract.EnvironmentVariables
{
    /// <summary>
    /// Allows us to store data and get it depending on if the game is in the editor, stable or development build.
    /// </summary>
    [Serializable]
    public struct EnvironmentVariable
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private string stable;
        [SerializeField]
        private string betaBuild;
        [SerializeField]
        private string alphaBuild;
        [SerializeField]
        [FormerlySerializedAs("nightlyBuild")] private string devBuild;
        [SerializeField]
        private string editor;
        
        /// <summary>
        /// Gets the data for the correct version (Editor/Development Build/Release)
        /// </summary>
        /// <returns>The data for the version</returns>
        public string GetData()
        {
#if UNITY_EDITOR
            return editor;
#else
            if (Application.version.ToLower().Contains("alpha")) 
                return alphaBuild;
            else if (Application.version.ToLower().Contains("dev"))
                return devBuild;
            else if (Application.version.ToLower().Contains("beta"))
                return betaBuild;
            return stable;
#endif
        }
        
        /// <summary>
        /// Sets the data for the current version
        /// </summary>
        public void SetData()
        {
            Environment.SetEnvironmentVariable(name, GetData());
        }
    }
}
