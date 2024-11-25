using System;
using System.Collections.Generic;
using Abstract.Data;
using Modules;
using Turrets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
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
            public List<string> moduleNames;
            public List<int> moduleTiers;
            public DynamicTurret.TargetingMethod targetingMethod;
            public Quaternion turretRotation;
            public string blueprintName;
        }
        public List<NodeData> Nodes;
        
        public int Energy;
        public int Powercells;
        public int Lives;
        public int WaveIndex;
        public int TotalCellsCollected;
        
        public Random.State RandomState;
        public int RandomSeed;
        public int ShopRandomN;

        public List<TurretBlueprint> TurretInventory;
        public List<ModuleChainHandler> ModuleInventory;

        private readonly string _version = Application.version;

        public static readonly Dictionary<string, TurretBlueprint> Blueprints = new();
        public static readonly Dictionary<string, ModuleChain> Chains = new();
        private static AsyncOperationHandle<IList<TurretBlueprint>> _turretOp;
        private static AsyncOperationHandle<IList<ModuleChain>> _chainOp;
        
        /// <summary>
        /// Translates the class into json format
        /// </summary>
        /// <returns>This class in json format</returns>
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        
        /// <summary>
        /// Loads json without loading addressables
        /// </summary>
        /// <param name="json">The json to load from</param>
        /// <returns>Save's version</returns>
        public string LoadVersion(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);

            return _version;
        }
        
        /// <summary>
        /// Loads this class from json with addressables loaded into memory
        /// </summary>
        /// <param name="json">The json to load from</param>
        public void LoadFromJson(string json)
        {
            // Matches the labels assigned to the addressables
            _turretOp = Addressables.LoadAssetsAsync<TurretBlueprint>(new List<string> { "TurretBlueprint" },
                addressable => { Blueprints.Add(addressable.name, addressable); },
                Addressables.MergeMode.Union);
            _turretOp.WaitForCompletion();
            _chainOp = Addressables.LoadAssetsAsync<ModuleChain>(new List<string> { "ModuleChain" },
                addressable =>
                {
                    Chains.Add(addressable.name, addressable);
                },
                Addressables.MergeMode.Union);
            _chainOp.WaitForCompletion();
            
            JsonUtility.FromJsonOverwrite(json, this);
            
            // We can release everything when we exit the level
            SceneManager.sceneLoaded += ReleaseAll;
        }

        private static void ReleaseAll(Scene scene, LoadSceneMode mode)
        {
            _turretOp.Release();
            _chainOp.Release();
            
            Blueprints.Clear();
            Chains.Clear();
            SceneManager.sceneLoaded -= ReleaseAll;
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