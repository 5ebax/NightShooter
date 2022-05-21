using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPoint : MonoBehaviour
{
    public Transform target;
    public LineRenderer laserPoint;
    
    // Start is called before the first frame update
    void Awake()
    {
        laserPoint = GetComponent<LineRenderer>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void ActiveLaserPoint()
    {
        laserPoint.enabled = true;
        laserPoint.SetPosition(0, transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, target.position - transform.position, out hit, 50F))
            {
                laserPoint.SetPosition(1, hit.point);
            }
            else
            {
                laserPoint.SetPosition(1, transform.position + (transform.forward * 50F));
            }
    }
    public void DesactiveLaserPoint()
    {
        laserPoint.enabled = false;
    }
}
