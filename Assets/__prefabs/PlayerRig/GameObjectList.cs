using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameObjectList : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public GameObject[] gameObjects;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Dictionary<string, GameObject> GetGameObjectDictionary()
    {
        Dictionary<string, GameObject> gameObjectDict = new Dictionary<string, GameObject>();
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null && !gameObjectDict.ContainsKey(obj.name))
            {
                gameObjectDict.Add(obj.name, obj);
            }
        }
        return gameObjectDict;
    }

    void OnValidate()
    {
        // ensure no duplicates when adding in editor
        if (this.gameObjects != null)
        {
            this.gameObjects = this.gameObjects.DistinctBy(obj => obj.name).ToArray();
        }
    }
}
