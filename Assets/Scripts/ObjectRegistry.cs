using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks active Unity components without duplicate entries. OOP principle: generic reusable code.
/// </summary>
public sealed class ObjectRegistry<T> where T : MonoBehaviour
{
    private readonly List<T> objects = new List<T>();
    private readonly HashSet<T> uniqueObjects = new HashSet<T>();

    // Gets the active objects after removing components Unity has destroyed. Algorithm: linear cleanup.
    public IReadOnlyList<T> ActiveObjects
    {
        get
        {
            PruneDestroyedObjects();
            return objects;
        }
    }

    // Gets the number of registered objects. OOP principle: encapsulation through a read-only property.
    public int Count
    {
        get
        {
            PruneDestroyedObjects();
            return objects.Count;
        }
    }

    // Adds an object only if it is valid and was not already registered. Algorithm: hash-based duplicate detection.
    public bool Register(T item)
    {
        if (item == null || !uniqueObjects.Add(item))
        {
            return false;
        }

        objects.Add(item);
        return true;
    }

    // Removes an object from the registry. OOP principle: encapsulation of collection updates.
    public bool Unregister(T item)
    {
        if (item == null || !uniqueObjects.Remove(item))
        {
            return false;
        }

        objects.Remove(item);
        return true;
    }

    // Removes Unity-destroyed components while preserving the active list. Algorithm: in-place linear cleanup.
    public void PruneDestroyedObjects()
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if (objects[i] == null)
            {
                uniqueObjects.Remove(objects[i]);
                objects.RemoveAt(i);
            }
        }
    }
}
