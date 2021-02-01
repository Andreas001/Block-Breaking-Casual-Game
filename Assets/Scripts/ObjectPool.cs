using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] int amountToInitialize = 100;

    [Header("Object Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Objects in Pool")]
    [SerializeField] List<GameObject> activeObjects;
    [SerializeField] List<GameObject> inactiveObjects;

    void Awake() {
        activeObjects = new List<GameObject>();
        inactiveObjects = new List<GameObject>();

        for (int index = 0; index < amountToInitialize; index++) {
            prefab.SetActive(false);
            GameObject newObject = Instantiate(prefab);
            newObject.transform.parent = gameObject.transform;
            inactiveObjects.Add(newObject);
        }
    }

    public GameObject GetObject() {
        if (inactiveObjects.Count > 0) {
            GameObject toRemoveFromInactive = null;

            foreach (GameObject gameObject in inactiveObjects) {
                if (!gameObject.activeInHierarchy && gameObject.transform.parent != null) {                
                    toRemoveFromInactive = gameObject;                    
                    break;
                }
            }

            if (toRemoveFromInactive) {
                toRemoveFromInactive.transform.parent = null;
                activeObjects.Add(toRemoveFromInactive);
                inactiveObjects.Remove(toRemoveFromInactive);

                return toRemoveFromInactive;
            }           
        }

        //If code reaches here it means there are not enough bullets so more will be instantiated
        GameObject newObject = Instantiate(prefab);

        newObject.transform.parent = null;
        activeObjects.Add(newObject);

        return newObject;
    }

    public void ReturnObject(GameObject gameObject) {
        gameObject.SetActive(false);
        gameObject.transform.parent = transform;
        activeObjects.Remove(gameObject);
        inactiveObjects.Add(gameObject);
    }

    public void ReturnAllActiveObject() {
        foreach(GameObject gameObject in activeObjects) {
            gameObject.transform.parent = transform;
            gameObject.SetActive(false);
            inactiveObjects.Add(gameObject);
        }

        activeObjects.Clear();
    }
}
