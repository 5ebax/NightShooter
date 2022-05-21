using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/** Author: Sebastián Jiménez Fernández.
 * Class for controlling the player.
 * */
public class PlayerController : MonoBehaviour
{
    [Header("Vidas")]
    public int actualHealth;
    public int maxHealth;
    public HealthBar healthBar;

    [Header("Movement")]
    public float speed;
    public float jumpForce;
    public float gravity = -9.81f;
    public Vector3 velocity;
    public CharacterController characterController;


    [Header("Ground")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Light flashlight;

    private int score;
    private int skulls;
    private bool isGrounded;
    public GameManager gm;
    private Vector3 respawn;
    private AudioManager audioM;
    private Animator anim;

    private void Awake()
    {
        score = 0;
        gm = GameManager.Instance;
        anim = GetComponentInChildren<Animator>();
        audioM = AudioManager.Instance;
        flashlight = GetComponentInChildren<Light>();
        if (maxHealth <= 0) maxHealth = 5;
        skulls = PlayerPrefs.GetInt("skulls");
        FindObjectOfType<UITexts>().skullTxt.text = "Skulls: " + skulls + "/" + 5;
    }

    private void Start()
    {   //Al guardarse en el disco y el Start() realizarse en cada LoadScene, spawneará donde lo dejó o al inicio si es partida nueva.
        FindObjectOfType<MenuController>().SpawnPlayer();

        //Setteamos con cuidado los datos guardados del jugador, si tiene.
        actualHealth = PlayerPrefs.GetInt("health");
        IncreasePlayerScore(PlayerPrefs.GetInt("score"));
        if(actualHealth <= 0)
            actualHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(actualHealth);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    #region Movements and actions
    private void Movement()
    {
        speed = 5F;
        Run();
        Down();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
            respawn = transform.position;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else if (!isGrounded && velocity.y < 0.5F) //Para una caída un poco más realista y agradable. 
            velocity.y -= 10F * Time.deltaTime;

        float x = Input.GetAxisRaw("Horizontal");
        bool movX = Input.GetButton("Horizontal");
        bool movZ = Input.GetButton("Vertical");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 dir = transform.right * x + transform.forward * z;

        characterController.Move(dir * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        if ((movX || movZ) && isGrounded && !Run()) audioM.PlayOneAtTime("StepsWalk"); else audioM.Stop("StepsWalk");
        if (Run()) { audioM.PlayOneAtTime("StepsRun"); anim.SetBool("Run", true); } else { audioM.Stop("StepsRun"); anim.SetBool("Run", false); }

        Jump();
        Flashlight();
    }

    private bool Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0 && isGrounded && !Down())
        {
            speed = 6.5F;
            return true;
        }
        else return false;
    }
    public bool Down()
    {
        if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            anim.SetBool("Down",true);
            speed = 4F;
            return true;
        }
        else anim.SetBool("Down",false); return false;
    }


    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void Flashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            audioM.PlayOneShot("SwitchLight");
            if(flashlight.enabled)
                flashlight.enabled = false;
            else
                flashlight.enabled = true;
        }
    }
    #endregion

    #region Otros

    public void ReducePlayerHealth(int hit)
    {
        actualHealth -= hit;
        PlayerPrefs.SetInt("health", actualHealth);
        healthBar.SetHealth(actualHealth);

        //Si el jugador se queda sin vidas acaba el juego.
        if (actualHealth <= 0)
        {
            GameOver();
        }
    }

    public void IncreasePlayerHealth(int healthPoints)
    {
        actualHealth = Mathf.Clamp(actualHealth + healthPoints, 0, maxHealth);
        PlayerPrefs.SetInt("health", actualHealth);
        healthBar.SetHealth(actualHealth);
    }
    public void IncreasePlayerScore(int scorePoints)
    {
        score += scorePoints;
        PlayerPrefs.SetInt("score", score);
        FindObjectOfType<UITexts>().scoreTxt.text = "Score: "+ score;
    }
    public void ResetPlayerScore()
    {
        score = 0;
        PlayerPrefs.SetInt("score", score);
        FindObjectOfType<UITexts>().scoreTxt.text = "Score: " + score;
    }
    public void IncreasePlayerSkulls()
    {
        skulls += 1;
        PlayerPrefs.SetInt("skulls", skulls);
        FindObjectOfType<UITexts>().skullTxt.text = "Skulls: " + skulls+"/"+5;
        if (skulls == 5)
            StartCoroutine(Win());
    }

    IEnumerator Win()
    {
        actualHealth = 10000;
        Time.timeScale = 0.2F;
        yield return new WaitForSecondsRealtime(5F);
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("End");
    }

    private void GameOver()
    {
        ResetPlayerScore();
        FindObjectOfType<MenuController>().GameOver();
    }

    #endregion

    #region Collisions
    //Si cae al río o sale de los límites, perderá vida y volverá a la posición anterior.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("River"))
        {
            ReducePlayerHealth(2);
            transform.position = respawn;
            Physics.SyncTransforms();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Limits"))
        {
            ReducePlayerHealth(1);
            transform.position = respawn;
            Physics.SyncTransforms();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.tag) //Usamos el switch par prevenir que no use el tag de Limits, de River, u otro.
        {                   //El orden es importante, ya que en caso de estar en dos a la vez, priorizará la zona anterior (Así no avanza a la siguiente si lo matan al borde de una).
            case "ParkingZone":
                gm.SpawnZone = other.tag;
                break;
            case "CityZone":
                gm.SpawnZone = other.tag;
                break;
            case "AirportZone":
                gm.SpawnZone = other.tag;
                break;
            case "FarmZone":
                gm.SpawnZone = other.tag;
                break;
            case "ForestZone":
                gm.SpawnZone = other.tag;
                break;
        }
    }
    #endregion
}
