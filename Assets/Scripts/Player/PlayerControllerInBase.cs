using UnityEngine;
/** Author: Sebastián Jiménez Fernández.
 * Class for controlling the player.
 * */
public class PlayerControllerInBase : MonoBehaviour
{
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

    private bool isGrounded;
    private AudioManager audioM;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        audioM = AudioManager.Instance;
        flashlight = GetComponentInChildren<Light>();
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
        if (Run()) { audioM.PlayOneAtTime("Running"); anim.SetBool("Run",true); } else { audioM.Stop("Running"); anim.SetBool("Run",false); }

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


    #region Collisions
    #endregion
}
