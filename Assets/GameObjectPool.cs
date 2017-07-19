using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour {
    //The instance of the prefab that is returned by the object pool
    public GameObject prefab;
    //Store inactive instances of the prefab into a collection
    private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

    // Returns an instance of the prefab
    public GameObject GetGameObject()
    {
        GameObject convertGameObject;

        if (inactiveInstances.Count > 0)
        { 
            //Use inactive instance of the prefab if exist
            convertGameObject = inactiveInstances.Pop();
        }
        else
        {
            //Otherwise, create a new instance
            convertGameObject = GameObject.Instantiate(prefab) as GameObject;
        }
  
        //Set up game object
        convertGameObject.transform.SetParent(null);
        convertGameObject.SetActive(true);

        return convertGameObject;
    }

    // Return an instance of the prefab to the pool
    public void PutToStack(GameObject gameObject)
    {
        //Make the instance a child of this and disable it
        gameObject.transform.SetParent(transform);
        gameObject.SetActive(false);
        //Push the instance to stack
        inactiveInstances.Push(gameObject);
    }
}

