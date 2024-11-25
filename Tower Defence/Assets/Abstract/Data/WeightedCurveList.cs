using System;
using System.Collections.Generic;
using System.Linq;

namespace Abstract.Data
{
    /// <summary>
    /// A list of items and their weight.
    /// Can get a random item and total weight of the values
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    [Serializable]
    public struct WeightedCurveList<T> where T : ISubtypeable
    {
        public List<WeightedCurvedReference<T>> list;
    
        /// <summary>
        /// Basic constructor for the list
        /// </summary>
        /// <param name="list">The list to create</param>
        public WeightedCurveList(List<WeightedCurvedReference<T>> list)
        {
            this.list = list;
        }
    
        /// <summary>
        /// Empties/Cleans the list of all elements
        /// </summary>
        public void Clear()
        {
            list = new List<WeightedCurvedReference<T>>();
        }
        
        /// <summary>
        /// Converts the WeightedCurveList to a WeightedList at a certain time
        /// </summary>
        /// <param name="time">The time to get the weight from the AnimationCurves</param>
        /// <returns>The WeightedList for a specific time</returns>
        public WeightedList<T> ToWeightedList(float time)
        {
            var weightedList = new WeightedList<T>(new List<WeightedItem<T>> {new(list[0].item, list[0].Value.Evaluate(time))});
            weightedList.RemoveAt(0);
            
            foreach (WeightedCurvedReference<T> item in list.Where(item => item.Value.Evaluate(time) > 0))
            {
                weightedList.Add(new WeightedItem<T>(item.item, item.Value.Evaluate(time)));
            }

            return weightedList;
        }
    }
}
