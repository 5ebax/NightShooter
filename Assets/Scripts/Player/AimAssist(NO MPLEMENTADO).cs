using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/** Author: Sebastián Jiménez Fernández.
 * No termina de funcionar correctamente.
 * */
public class AimAssist : MonoBehaviour
{
    [SerializeField] bool aimAssist;
    [SerializeField] float aimAssistSize = 1f;
    public Transform aimPosition;
    public GameObject currentTarget;
    public float distance = 20f;
    private Vector3 collision;

    // Update is called once per frame
    void Update()
    {
        CheckTarget();

        if (aimAssist)
            AutoAiming();
    }

    private void CheckTarget()
    {
        RaycastHit hit;

        if(Physics.SphereCast(aimPosition.position, aimAssistSize, aimPosition.forward, out hit, distance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (!aimAssist)
                    Debug.Log("Enemigo encontrado!");

                collision = hit.point;
                currentTarget = hit.transform.gameObject;
                aimAssist = true;
            }
            else
            {
                currentTarget = null;
                aimAssist=false;
            }
        }
    }

    private void AutoAiming()
    {
        Camera.main.transform.LookAt(collision);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(collision, aimAssistSize);
    }
}
