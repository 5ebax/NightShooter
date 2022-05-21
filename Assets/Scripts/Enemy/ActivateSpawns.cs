using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/** Author: Sebastián Jiménez Fernández.
 * Class for Active the spawns in zones.
 * */
public class ActivateSpawns : MonoBehaviour
{
    private Spawner[] spawner;

    private void Awake()
    {
        spawner = GetComponentsInChildren<Spawner>();
    }

    //Recorerá todos los spawns de la zona a la que entró y activa a los enemigos.
    IEnumerator Spawn()
    {
        foreach (var spawn in spawner)
        {
            spawn.ObjSpawn();
        }
        yield return new WaitForSeconds(60); //En 60seg podrán volver a respawnear si sale y entra de la zona(para que no se instancien instántaneo si entras y sales repetidamente).
        foreach (var spawn in spawner)
        {
            spawn.canSpawn = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Si el jugador entra a la zona, los enemigos spawnean.
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Spawn());
        }
    }
}
