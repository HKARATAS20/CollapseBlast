using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class will allow us to instantiate as many objects as we will need
 * while the scene is being set up so that we don't slow down the game by
 * calling Instantiate during gameplay.
 * 
 * It is preferred, but optional, to call PoolObjects first with a specified amount
 */
public abstract class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
{
    [SerializeField] protected T prefab;

    private List<T> pooledObjects;
    private int amount;
    private bool isReady;

    public void PoolObjects(int amount = 0)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException("Amount to pool must be non-negative.");

        this.amount = amount;

        pooledObjects = new List<T>(amount);

        GameObject newObject;

        for (int i = 0; i != amount; ++i)
        {
            newObject = Instantiate(prefab.gameObject, transform);
            newObject.SetActive(false);

            pooledObjects.Add(newObject.GetComponent<T>());
        }
        isReady = true;
    }

    public T GetPooledObject()
    {
        if (!isReady)
            PoolObjects(1);

        for (int i = 0; i != amount; ++i)
            if (!pooledObjects[i].isActiveAndEnabled)
                return pooledObjects[i];

        GameObject newObject = Instantiate(prefab.gameObject, transform);
        newObject.SetActive(false);
        pooledObjects.Add(newObject.GetComponent<T>());
        ++amount;

        return newObject.GetComponent<T>();
    }

    public virtual void ReturnObjectToPool(T toBeReturned)
    {
        if (toBeReturned == null)
            return;

        if (!isReady)
        {
            PoolObjects();
            pooledObjects.Add(toBeReturned);
            ++amount;
        }
        toBeReturned.gameObject.SetActive(false);
    }
}