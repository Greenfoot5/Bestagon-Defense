using System;
using System.Collections.Generic;
using Abstract.Data;
using Turrets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Abstract.Saving
{
    /// <summary>
    /// Represents a level's save data
    /// </summary>
    public class SaveLevel
    {
        [Serializable]
        public struct NodeData
        {
            public string uuid;
            public TurretBlueprint turretBlueprint;
            public List<ModuleChainHandler> moduleChainHandlers;
            public Quaternion turretRotation;
        }
        public List<NodeData> nodes;
        
        public int energy;
        public int cells;
        public int lives;
        public int waveIndex;
        public Random.State random;
        public int shopCost;

        public List<TurretBlueprint> turretInventory;
        public List<ModuleChainHandler> moduleInventory;

        public readonly string version = Application.version;
        
        /// <summary>
        /// Translates the class into json format
        /// </summary>
        /// <returns>This class in json format</returns>
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        
        /// <summary>
        /// Loads this class from jsom
        /// </summary>
        /// <param name="json">The json to load from</param>
        public void LoadFromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
    
    /// <summary>
    /// Handles the loading and populating of the save data
    /// Designed to be implemented by MonoBehaviours
    /// </summary>
    public interface ISaveableLevel
    {
        void PopulateSaveData(SaveLevel saveData);
        void LoadFromSaveData(SaveLevel saveData);
    }
}