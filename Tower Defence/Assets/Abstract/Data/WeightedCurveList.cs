using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Abstract.Data
{
    /// <summary>
    /// A list of items and their weight.
    /// Can get a random item and total weight of the values
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    [Serializable]
    public struct WeightedCurveList<T>
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
        /// Gets a random item from the list using the weights at a certain time
        /// </summary>
        /// <param name="time">The time on the animation curve to get the weight from</param>
        /// <returns>A random item, if all weights are 0, selects a random with equal weighting</returns>
        /// <exception cref="NullReferenceException">The list is empty</exception>
        public T GetRandomItem(float time)
        {
            if (list.Count == 0) throw new NullReferenceException("WeightedList is empty");

            float total = list.Sum(t => t.Value.Evaluate(time));
            total = Random.Range(0f, total);

            if (total == 0) return list[Random.Range(0, list.Count)].item;

            var i = 0;
            while (total >= 0 && i < list.Count)
            {
                float iWeight = list[i].Value.Evaluate(time);
                if (total < iWeight && iWeight != 0) return list[i].item;

                total -= iWeight;
                i++;
            }
        
            // It's not perfect, but it's better than nothing
            if (total == 0) return list[Random.Range(0, list.Count)].item;
        
            throw new NullReferenceException("Cannot return a random item from the WeightedList");
        }
    
        /// <summary>
        /// Gets the total weight of all elements in the list combined at a certain time
        /// </summary>
        /// <param name="time">The time on the animation curve to get the weight from</param>
        /// <returns>The total overall weight</returns>
        /// <exception cref="NullReferenceException">The list is empty</exception>
        public float GetTotalWeight(float time)
        {
            if (list.Count == 0) throw new NullReferenceException("WeightedList is empty");
        
            float total = list.Sum(t => t.Value.Evaluate(time));

            return total;
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
            weightedList.list.RemoveAt(0);
            foreach (WeightedCurvedReference<T> item in list.Where(item => item.Value.Evaluate(time) > 0))
            {
                weightedList.list.Add(new WeightedItem<T>(item.item, item.Value.Evaluate(time)));
            }

            return weightedList;
        }
    }
}
