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
        public List<NodeData> Nodes;
        
        public int Energy;
        public int Powercells;
        public int Lives;
        public int WaveIndex;
        public Random.State RandomState;
        public int ShopCost;

        public List<TurretBlueprint> TurretInventory;
        public List<ModuleChainHandler> ModuleInventory;

        public readonly string Version = Application.version;
        
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