using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/** Author: Sebastián Jiménez Fernández.
 * Class Bullets.
 * */
public class Bullet : MonoBehaviour
{
    public GameObject explosionParticle;
    public GameObject bloodParticle;
    public int healthPointsDmg;
    private int instakill = 100;

    private AudioManager audioM;
    private void Awake()
    {
        audioM = AudioManager.Instance;
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            //Bala a golpeado al enemigo
            case "EnemySwat":
                other.gameObject.GetComponentInParent<EnemyController>().ReduceEnemyHealth(healthPointsDmg, other);
                audioM.PlayOneShot("BodyHit", 0.3F);
                EndBullet(bloodParticle);
                break;
            case "EnemyFront":
                other.gameObject.GetComponentInParent<EnemyController>().ReduceEnemyHealth(healthPointsDmg, other);
                audioM.PlayOneShot("BodyHit", 0.3F);
                EndBullet(bloodParticle);
                break;
            case "EnemyBack":
                other.gameObject.GetComponentInParent<EnemyController>().ReduceEnemyHealth(healthPointsDmg, other);
                audioM.PlayOneShot("BodyHit", 0.3F);
                EndBullet(bloodParticle);
                break;
            case "EnemyHead":
                other.gameObject.GetComponentInParent<EnemyController>().ReduceEnemyHealth(instakill, other);
                audioM.PlayOneShot("Headshot");
                EndBullet(bloodParticle);
                break;
            case "EnemyBackHead":
                other.gameObject.GetComponentInParent<EnemyController>().ReduceEnemyHealth(instakill, other);
                audioM.PlayOneShot("Headshot");
                EndBullet(bloodParticle);
                break;
        }

        if (other.gameObject.layer == 3) //Suelo
        {
            //Cuando la bola colisione tenemos que ponerla a false para que se pueda reutilizar
            audioM.PlayOneShot("HitGround");
            EndBullet(explosionParticle);
        }else if (other.gameObject.layer == 6) //Obstaculos
        {
            //Cuando la bola colisione tenemos que ponerla a false para que se pueda reutilizar
            audioM.PlayOneShot("HitGround");
            EndBullet(explosionParticle);
        }else if (other.gameObject.CompareTag("Player"))
        {
            //Bala a golpeado al jugador
            other.gameObject.GetComponent<PlayerController>().ReducePlayerHealth(healthPointsDmg);
            audioM.PlayOneShot("BodyHit", 0.3F);
            EndBullet(bloodParticle);

        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            audioM.PlayOneShot("HitGround");
            EndBullet(explosionParticle);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Limits"))
        {
            gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("Limits"))
        {
            gameObject.SetActive(false);
        }
    }

    void EndBullet(GameObject particle)
    {
        gameObject.SetActive(false);

        //Creamos las partículas de explosión
        GameObject particulas = Instantiate(particle, transform.position, Quaternion.identity);

        //Se destruye cuando pase la duración del sistema de partículas
        Destroy(particulas, particulas.GetComponent<ParticleSystem>().main.duration);
    }

}
