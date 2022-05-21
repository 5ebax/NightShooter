using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IA_ControlEnemy : MonoBehaviour
{
    [HideInInspector] public GameObject[] checkpointsPatrol;
    NavMeshAgent agente;
    Animator anim;
    bool wait;
    int index;


    // Start is called before the first frame update
    void Start()
    {
        index = 0;

        agente = GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();

        agente.SetDestination( checkpointsPatrol[index].transform.position);
        
        float vm = Random.Range(0.5f, 1.25f);
        agente.speed *= vm;

    }    
    void Update()
    {
        if(!wait)
            StartCoroutine(Next());
    }

    IEnumerator Next()
    {
        if (agente.remainingDistance < 7)
        {
            wait = true;
            anim.SetBool("Walk", false);

            yield return new WaitForSeconds(Random.Range(2.0F, 5.0F));
            index++;
            anim.SetBool("Walk", true);
            if(index >= checkpointsPatrol.Length)
                index = 0;
            agente.SetDestination(checkpointsPatrol[index].transform.position);
            wait = false;
        }
    }

}
