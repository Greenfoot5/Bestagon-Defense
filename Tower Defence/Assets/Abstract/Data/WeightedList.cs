using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abstract.Data
{
    /// <summary>
    /// A way to disable duplicates in random selections
    /// </summary>
    public enum DuplicateTypes
    {
        None,
        ByName,
        ByType
    }
    
    /// <summary>
    /// A list of items and their weight.
    /// Can get a random item and total weight of the values
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    [Serializable]
    public struct WeightedList<T> where T : ISubtypeable
    {
        [SerializeField]
        private List<WeightedItem<T>> list;
    
        /// <summary>
        /// Basic constructor for the list
        /// </summary>
        /// <param name="list">The list to create</param>
        public WeightedList(List<WeightedItem<T>> list)
        {
            this.list = new List<WeightedItem<T>>(list);
            RemoveUnweighted();
        }

        public WeightedList(WeightedList<T> list)
        {
            this.list = new List<WeightedItem<T>>(list.list);
            RemoveUnweighted();
        }
        
        /// <summary>
        /// Gets random items from the list using the weights
        /// </summary>
        /// <param name="duplicateType">Which duplication type to use when checking against previously picked</param>
        /// <param name="rng">The random generator to use</param>
        /// <param name="previousPicks">The previous picks to check against</param>
        /// <returns>A random item</returns>
        /// <exception cref="NullReferenceException">The list isn't suitable to grant all items</exception>
        public T GetRandomItem(DuplicateTypes duplicateType = DuplicateTypes.None, Squirrel3 rng = null, ICollection<T> previousPicks = null)
        {
            rng ??= new Squirrel3();
            previousPicks ??= Array.Empty<T>();
            var items = new List<WeightedItem<T>>(list);
            
            foreach (T pick in previousPicks)
            {
                switch (duplicateType)
                {
                    case DuplicateTypes.ByName:
                        for (var k = 0; k < items.Count; k++)
                        {
                            if (items[k].item.ToString() == pick.ToString())
                                items.RemoveAt(k);
                        }

                        break;
                    case DuplicateTypes.ByType:
                        for (var k = 0; k < items.Count; k++)
                        {
                            if (items[k].item.GetSubtype() == pick.GetSubtype())
                            {
                                items.RemoveAt(k);
                            }
                        }

                        break;
                    case DuplicateTypes.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(duplicateType), duplicateType, null);
                }
            }

            float total = items.Where(item => item.weight > 0).Sum(item => item.weight);
            if (items.Count < 1) throw new NullReferenceException("WeightedList is not large enough");
            if (total == 0) throw new NullReferenceException("Total Weight is 0");

            // Downgrade duplicateTypes if we can't generate
            if (total == 0)
            {
                Debug.LogWarning("Could not use " + duplicateType + " duplicate checking, downgrading and regenerating.");
                duplicateType = duplicateType switch
                {
                    DuplicateTypes.ByName => DuplicateTypes.ByType,
                    DuplicateTypes.ByType => DuplicateTypes.None,
                    _ => duplicateType
                };
                return GetRandomItem(duplicateType, rng, previousPicks);
            }
            
            float picked = rng.Next() * total;
            var j = 0;
            while (picked >= 0)
            {
                if (picked < items[j].weight)
                {
                    return items[j].item;
                }

                picked -= items[j].weight;
                j++;
            }

            throw new NullReferenceException("Failed to pick an item");
        }

        /// <summary>
        /// Gets random items from the list using the weights
        /// </summary>
        /// <param name="count">How many to pick</param>
        /// <param name="duplicateType">Which duplication type to use when checking against previously picked</param>
        /// <param name="rng">The random generator to use</param>
        /// <returns>A random item</returns>
        /// <exception cref="NullReferenceException">The list isn't suitable to grant all items</exception>
        public T[] GetRandomItems(int count, DuplicateTypes duplicateType = DuplicateTypes.None, Squirrel3 rng = null)
        {
            rng ??= new Squirrel3();
            Math.Clamp(count, 0, int.MaxValue);
            float total = GetTotalWeight();
            if (list.Count < count) throw new NullReferenceException("WeightedList is not large enough");
            if (total == 0) throw new NullReferenceException("Total Weight is 0");
            
            var output = new T[count];
            // Make a copy we can remove items from
            var items = new List<WeightedItem<T>>(list);

            for (var i = 0; i < count; i++)
            {
                // Downgrade duplicateTypes if we can't generate
                if (total == 0)
                {
                    Debug.LogWarning("Could not use " + duplicateType + " duplicate checking, downgrading and regenerating.");
                    duplicateType = duplicateType switch
                    {
                        DuplicateTypes.ByName => DuplicateTypes.ByType,
                        DuplicateTypes.ByType => DuplicateTypes.None,
                        _ => duplicateType
                    };
                    return GetRandomItems(count, duplicateType, rng);
                }
                
                float picked = rng.Next() * total;
                var j = 0;
                while (picked >= 0 && output[i] == null)
                {
                    if (picked < items[j].weight)
                    {
                        output[i] = items[j].item;
                    }
                    else
                    {
                        picked -= items[j].weight;
                        j++;
                    }
                }


                switch (duplicateType)
                {
                    case DuplicateTypes.ByName:
                        for (var k = 0; k < items.Count; k++)
                        {
                            if (items[k].item.ToString() == output[i].ToString())
                                items.RemoveAt(k);
                        }
                        break;
                    case DuplicateTypes.ByType:
                        for (var k = 0; k < items.Count; k++)
                        {
                            if (items[k].item.GetType() == output[i].GetType())
                                items.RemoveAt(k);
                        }
                        break;
                    case DuplicateTypes.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(duplicateType), duplicateType, null);
                }
                total = items.Where(item => item.weight > 0).Sum(item => item.weight);
            }

            return output;
        }
    
        /// <summary>
        /// Gets the total weight of all elements in the list combined
        /// </summary>
        /// <returns>The total overall weight</returns>
        /// <exception cref="NullReferenceException">The list is empty</exception>
        public float GetTotalWeight()
        {
            if (list.Count == 0) throw new NullReferenceException("WeightedList is empty");

            return list.Where(item => item.weight > 0).Sum(item => item.weight);
        }

        public void RemoveItem(T item)
        {
            var i = 0;
            while (i < list.Count)
            {
                if (list[i].item.Equals(item))
                    list.RemoveAt(i);
                else
                    i++;
            }
        }
        
        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public void Add(WeightedItem<T> item)
        {
            list.Add(item);
        }
        
        public WeightedItem<T> this[int key]
        {
            get => list[key];
            set => list[key] = value;
        }


        public int Count => list.Count;

        public void RemoveUnweighted()
        {
            // We can remove everything if the total weight is 0
            if (GetTotalWeight() == 0)
                Clear();
            
            var i = 0;
            while (i < list.Count)
            {
                if (list[i].weight <= 0)
                    list.RemoveAt(i);
                else
                    i++;
            }
        }
    
        /// <summary>
        /// Empties/Cleans the list of all elements
        /// </summary>
        public void Clear()
        {
            list = new List<WeightedItem<T>>();
        }

        public bool IsEmpty()
        {
            return list.Count == 0;
        }
    }
}
