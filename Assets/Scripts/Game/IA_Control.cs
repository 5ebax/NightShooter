using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IA_Control : MonoBehaviour
{
    public GameObject[] localizacionMetas;
    NavMeshAgent agente;
    Animator anim;
    bool wait;


    // Start is called before the first frame update
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();

        agente.SetDestination( localizacionMetas[Random.Range(0, localizacionMetas.Length)].transform.position);
        
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
        if (agente.remainingDistance < Random.Range(0.0F, 1.5F))
        {
            wait = true;
            anim.SetBool("Walk", false);

            yield return new WaitForSeconds(Random.Range(0.0F, 3.0F));

            anim.SetBool("Walk", true);
            agente.SetDestination(localizacionMetas[Random.Range(0,localizacionMetas.Length)].transform.position);
            wait = false;
        }
    }

}
