using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {

    public GameObject prototype; //the prototype object
    List<GameObject> pool; //list of objects
    public bool canGrow; //if we can grow
    private int lastSpawned; //last index spawned for circular list

    //Constructor, instantiates prototypes and sets default values
	public ObjectPool(GameObject prefab, bool resizeable, int count)
    {
        //set defaults
        prototype = prefab;
        canGrow = resizeable;
        pool = new List<GameObject>(count);
        lastSpawned = -1;
        //instantiate objects, deactivate them, and add them
        for(int i = 0; i < count; i++)
        {
            GameObject g = GameObject.Instantiate(prototype);
            g.SetActive(false);
            pool.Add(g);
        }
    }

    //Returns an inactive object
    public GameObject GetObject()
    {
        //check list for inactive prefab
        for (int i = (lastSpawned + 1) % pool.Count; i != lastSpawned; i = (i + 1) % pool.Count)
        {
            if (!pool[i].activeInHierarchy)
            {
                lastSpawned = i;
                return pool[i];
            }
        }
        //not in list, check if can grow
        if (canGrow)
        {
            GameObject instance = GameObject.Instantiate(prototype, new Vector2(-20,-20), Quaternion.identity);
            pool.Add(instance);
            pool[pool.Count - 1].SetActive(false);
            lastSpawned = pool.Count - 1;
            return pool[pool.Count - 1];
        }
        //not in list, can't grow
        return null;
    }

    public List<GameObject> GetRawPool()
    {
        return pool;
    }
}
