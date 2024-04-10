using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project
{
    public static partial class TransformExtensions
    {
        public static Transform[] GetChildrenArray(this Transform trans)
        {
            Transform[] array = new Transform[trans.childCount];

            for (int i = 0; i < trans.childCount; i++)
                array[i] = trans.GetChild(i);

            return array;
        }

        public static Component[] GetComponentInShallowChildren(this Transform trans, Type type, Type[] typesToBreak)
        {
            List<Component> result = new List<Component>();
            Queue<Transform> queue = new Queue<Transform>();

            for (int i = 0; i < trans.childCount; i++)
                queue.Enqueue(trans.GetChild(i));

            while (queue.Count > 0)
            {
                var t = queue.Dequeue();
                var component = t.GetComponent(type);

                if (component != null)
                    result.Add(component);

                bool skip = typesToBreak
                    .Select(x => t.GetComponent(x))
                    .Any(x => x != null);

                if (skip)
                    continue;

                for (int i = 0; i < t.childCount; i++)
                    queue.Enqueue(t.GetChild(i));
            }

            return result.ToArray();
        }

        public static Component[] GetComponentInShallowChildren(this Transform trans, Type type) =>
            GetComponentInShallowChildren(trans, type, new Type[] { type });

        public static T[] GetComponentInShallowChildren<T>(this Transform trans, Type[] otherTypes) where T : Component =>
            GetComponentInShallowChildren(trans, typeof(T), otherTypes)
                .Select(x => x as T)
                .ToArray();

        public static T[] GetComponentInShallowChildren<T>(this Transform trans) where T : Component =>
            GetComponentInShallowChildren(trans, typeof(T))
                .Select(x => x as T)
                .ToArray();
    }
}