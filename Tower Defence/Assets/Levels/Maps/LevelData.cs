using Abstract.Data;
using Turrets;
using UnityEngine;

namespace Levels.Maps
{
    /// <summary>
    /// Allows us to save data to allow levels to be different
    /// </summary>
    [CreateAssetMenu(fileName = "LevelName", menuName = "Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        [Header("InitialSelection")]
        [Tooltip("These chances overwrite base chances, they are only present of the player's first selection")]
        public WeightedList<TurretBlueprint> initialTurretSelection;
        [Tooltip("What type of duplicate check to perform, if any, against the rest of the selection")]
        public DuplicateTypes initialDuplicateCheck = DuplicateTypes.None;

        [Header("Selection")]
        [Tooltip("The weighted chance of picking a turret card to appear in the selection")]
        public CurvedReference turretOptionWeight;
        [Tooltip("The weighted chance of picking a module card to appear in the selection")]
        public CurvedReference moduleOptionWeight;
        [Tooltip("The weighted chance of pickup an additional life")]
        public CurvedReference lifeOptionWeight;
        [Tooltip("How many lives the life card will grant")]
        public int lifeCount;
        [Tooltip("What turrets can appear and their weighted chance of appearing on a turret selection card")]
        public WeightedCurveList<TurretBlueprint> turrets;
        [Tooltip("What type of duplicate check to perform, if any, against the rest of the selection")]
        public DuplicateTypes turretDuplicateCheck = DuplicateTypes.ByName;
        [Tooltip("What module can appear and their weighted chance of appearing on a module selection card")]
        public WeightedCurveList<ModuleChainHandler> moduleHandlers;
        [Tooltip("What type of duplicate check to perform, if any, against the rest of the selection")]
        public DuplicateTypes moduleDuplicateCheck = DuplicateTypes.ByType;

        [Header("Selection Counts")]
        [Tooltip("How many times should the initial selection be shown?")]
        public int initialSelectionCount = 1;
        [Tooltip("How many options to display in the first selection")]
        public int initialChoices = 3;
        [Tooltip("How many options to display in selections after the initial")]
        public int selectionChoices = 3;
        
        [Header("Costs")]
        [Tooltip("Shows how costs scale per wave.\nMust be valid formula for ExpressionEvaluator.Evaluate\n\"x\" is the input")]
        public string selectionCostFormula;

        [Header("Wave Scaling")]
        [Tooltip("What to multiply the enemy health by for every wave")]
        public CurvedReference health;
        [Tooltip("What to multiply the enemy count by for every wave")]
        public CurvedReference enemyCount;

        [Header("Refund Amounts")]
        [Tooltip("How many lives to gain back when picking revitalise")]
        public int revitaliseLives;
        [Tooltip("What percentage of the shop cost to refund when picking refund")]
        public float refundPercentage;
        [Tooltip("Lives to reroll ration for the selection" +
                 "\n\nUse an integer to take 1+ hearts per reroll" +
                 "\nUse a decimal to grant extra free rerolls per purchase")]
        public float rerollCost;
        
        [Tooltip("What percentage of the shop cost to refund when selling")]
        public float sellPercentage;
    }
}
