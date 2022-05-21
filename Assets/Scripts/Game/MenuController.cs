using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/** Author: Sebastián Jiménez Fernández.
 * Class for Menu/Buttons control.
 * */
public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    private AudioManager audioM;
    private Scene actualSecene;
    private Transform respawn;
    private PlayerController playerController;
    public GameObject[] playerSpawns;

    private void Awake()
    {
        audioM = AudioManager.Instance;
    }

    private void Update()
    {
        PauseEscape();
    }


    #region OnSceneLaoded

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //Cuando carga la escena.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        Time.timeScale = 1f;
        if (scene.name == "Game")
        {
            playerController = FindObjectOfType<PlayerController>();
            audioM.StopAll();
            audioM.PlayOneAtTime("GamePlay");
        }
        if (scene.name == "Base")
        {
            Cursor.lockState = CursorLockMode.None;
            audioM.StopAll();
            audioM.PlayOneAtTime("Base");
        }
        if (scene.name == "Menu" || scene.name == "End")
        {
            Cursor.lockState = CursorLockMode.None;
            audioM.StopAll();
            audioM.PlayOneAtTime("Menu");
        }
        actualSecene = scene;
    }
    #endregion

    #region Buttons
    public void NewGame()
    {
        PlayerPrefs.DeleteAll(); //Borrará nuestros datos.
        PlayerPrefs.SetInt("ammo", 6);
        PlayerPrefs.SetInt("totalAmmo", 12);
        SceneManager.LoadScene("Base");
    }
    public void ContinueGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("Scene"));

        if (PlayerPrefs.GetString("Scene") == "Base")
        {
            PlayerPrefs.SetInt("ammo", 6);
            PlayerPrefs.SetInt("totalAmmo", 12);
        }
    }

    public void ContinueFromRespawn()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("totalAmmo", 12);
        PlayerPrefs.SetInt("ammo", 6);
        Cursor.lockState = CursorLockMode.Locked;
        gameOverMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SpawnPlayer();
    }
    
    public void SaveScore()
    {
        PlayerPrefs.Save();
        FindObjectOfType<UITexts>().scoreSavedTxt.SetActive(true);
    }

    public void ResumePauseGame()
    {
        FindObjectOfType<UITexts>().scoreSavedTxt.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1F;
        pauseMenu.SetActive(false);
    }
    public void QuitGame()
    {
        PlayerPrefs.SetString("Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
    #endregion

    #region Others
    public void GameOver()
    {
        gameOverMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");
    }

    private void PauseEscape()
    {
        if (SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Base")
            if (Input.GetKeyDown(KeyCode.Escape) && !gameOverMenu.activeSelf)
            {
                if (!pauseMenu.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Time.timeScale = 0f;
                    pauseMenu.SetActive(true);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Time.timeScale = 1f;
                    pauseMenu.SetActive(false);
                }
            }

    }

    //Podría estar en un sitio mejor.
    public void SpawnPlayer()
    {
        if (GameManager.Instance.SpawnZone == "")
        {
            playerController.transform.SetPositionAndRotation(playerSpawns[0].transform.position, playerSpawns[0].transform.rotation);
            Physics.SyncTransforms();
        }
        else
        {
            foreach (var spawn in playerSpawns)
            {
                if (spawn.gameObject.CompareTag(GameManager.Instance.SpawnZone))
                {
                    playerController.transform.SetPositionAndRotation(spawn.transform.position, spawn.transform.rotation);
                    Physics.SyncTransforms();
                }
            }
        }
    }
    #endregion
}
