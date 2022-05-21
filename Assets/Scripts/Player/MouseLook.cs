using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/** Author: Sebastián Jiménez Fernández.
 * Class for Mouse Look.
 * */
public class MouseLook : MonoBehaviour
{

    [Header("Camera")]
    public float mouseSensitivityX;
    public float mouseSensitivityY;
    public float rotationX;
    public float minViewX;
    public float maxViewX;
    public Transform playerBody;
    private Quaternion rotation;


    [SerializeField] bool aimAssist;
    [SerializeField] float aimAssistSize = 1f;
    public Transform aimPosition;
    public Vector3 currentTarget;
    public float distance = 20f;
    private Vector3 collision;

    private void Awake()
    {
        rotationX = 0f;
        if (mouseSensitivityX <= 0f) mouseSensitivityX = 2F;
        if (mouseSensitivityY <= 0f) mouseSensitivityY = 2F;
        if (minViewX <= 0f) minViewX = -90f;
        if (maxViewX <= 0f) maxViewX = 90f;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        //Rotacion de la camara con el jugador
        CameraView();
    }


    private void CameraView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minViewX, maxViewX);

        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        playerBody.Rotate(Vector3.up * mouseX);
    }
     
    public Quaternion GetRotationMouseLook()
    {
        rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        return rotation;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(collision, aimAssistSize);
    }
}
