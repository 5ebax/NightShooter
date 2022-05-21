using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
/** Author: Sebastián Jiménez Fernández.
 * Class GameManager.
 * */
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    instance = new GameObject("GameManager").AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    public string SpawnZone { get => PlayerPrefs.GetString("spawn"); set => PlayerPrefs.SetString("spawn",value); }

    private string spawnZone; //Con esto podremos "Guardar la partida", y al cargarla recoger la información y cargarla en la zona adecuada.

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }


}
