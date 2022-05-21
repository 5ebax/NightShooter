using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/** Author: Sebastián Jiménez Fernández.
 * Class Spawner, spawn enemies, objt, etc.
 * */
public class Spawner : MonoBehaviour
{
    public GameObject objPrefab;
    public bool canSpawn;
    public bool isStop; //Si se quedará quieto en el sitio al vernos o no (Snippers sobretodo).
    public float atkRange; //El rango de ataque que queramos ponerle a los enemigos.
    public bool isPatrol;
    public GameObject[] checkpoints;

    private GameObject objToSpawn;
    private IA_ControlEnemy iaControl;
    private ObjectPooler objPooler;

    private void Start()
    {
        canSpawn = true;
        objPooler = FindObjectOfType<ObjectPooler>();
    }

    //Comprobará si puede spawnear, y el enemigo no existe o está esactivado(por el Pooler), lo instancia en la posición y dirección del Spawner.
    public void ObjSpawn()
    {
        if (canSpawn)
        {
            if (objToSpawn == null || !objToSpawn.activeSelf)
            {
                canSpawn = false;
                objToSpawn = objPooler.GetPooledObject(objPrefab.tag);
                if (objToSpawn != null)
                {
                    objToSpawn.transform.position = transform.position;
                    objToSpawn.transform.rotation = Quaternion.LookRotation(transform.forward);
                    if (objToSpawn.CompareTag("Enemy") || objToSpawn.CompareTag("EnemySwat"))
                    {
                        objToSpawn.GetComponent<EnemyController>().Respawn();
                        objToSpawn.GetComponent<EnemyController>().actualAtkRange = atkRange;
                        objToSpawn.GetComponent<NavMeshAgent>().isStopped = isStop;
                        objToSpawn.GetComponent<EnemyController>().controlEactive = isPatrol;
                        iaControl = objToSpawn.GetComponent<IA_ControlEnemy>();
                    }
                    else objToSpawn.SetActive(true);
                }
            }
        }
        if (isPatrol)
        {
            iaControl.enabled = true;
            iaControl.checkpointsPatrol = checkpoints;
        }
    }
}
