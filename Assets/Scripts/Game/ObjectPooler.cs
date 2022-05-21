using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
/** Author:
 * Sebastián Jiménez Fernández
 */

public class ObjectPoolItem {
  public GameObject objectToPool;
  public int amountToPool;
  public bool shouldExpand;
}

public class ObjectPooler : MonoBehaviour {

  public static ObjectPooler Instance;
  public List<ObjectPoolItem> itemsToPool;
  public List<GameObject> pooledObjects;

	void Awake() {
		Instance = this;

        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    //Cuando carga la escena.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game" || scene.name == "Base")
        {
            pooledObjects = new List<GameObject>(); //Necesita pisar la lista al cargar la escena para que exista de nuevo.
        }
    }
    

        public GameObject GetPooledObject(string tag) {
    for (int i = 0; i < pooledObjects.Count; i++) {
      if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag) {
        return pooledObjects[i];
      }
    }
    foreach (ObjectPoolItem item in itemsToPool) {
      if (item.objectToPool.tag == tag) {
        if (item.shouldExpand) {
          GameObject obj = (GameObject)Instantiate(item.objectToPool);
          obj.SetActive(false);
          pooledObjects.Add(obj);
          return obj;
        }
      }
    }
    return null;
  }
}
