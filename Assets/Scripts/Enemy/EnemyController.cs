using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
/** Author: Sebasti�n Jim�nez Fern�ndez.
 * Class for controlling the enemies.
 * */
public enum EnemyType
{
    Soldier,
    Snipper
}

public class EnemyController : MonoBehaviour
{
    [Header("Statistics")]
    public int health;
    public int maxHealth;
    public EnemyType enemyType;
    public float maxShootRange;
    public float minShootRange;

    [Header("Movement")]
    public float speed;
    public float atkRange;
    public float atkSpeed;

    [Header("Components")]
    public Rigidbody weapon;
    public Transform firePoint;
    public NavMeshAgent enemy;
    public Animator animEnemy;
    public Transform targetPlayer;

    private int enemyPoints;
    private bool reload;
    public bool startShooting;
    private bool dead;
    public float atkRangeMin;
    public float atkRangeIfPlayerSneak;
    public float atkRangeIfPlayerShot;
    public float actualAtkRange;
    private Shooting shoot;
    private Shooting playerShoot;
    private IA_ControlEnemy controlE;
    [HideInInspector] public bool controlEactive;
    private LaserPoint laser;
    private AudioManager audioM;

    private Vector3 collision;
    private Vector3 redirection = Vector3.down;
    private Vector3 lastTargetPosition;

    private void Awake()
    {
        speed = 3.5F;
        if (minShootRange < 0.2) minShootRange = 1f;
        if (maxShootRange < 0.2) maxShootRange = 1f;
        audioM = AudioManager.Instance;
        shoot = GetComponentInChildren<Shooting>();
        animEnemy = GetComponent<Animator>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;

        actualAtkRange = atkRange;
        health = maxHealth;
        laser = firePoint.gameObject.GetComponent<LaserPoint>();
        playerShoot = targetPlayer.GetComponentInChildren<Shooting>();
    }

    private void Start()
    {
        if (controlEactive)
            controlE = GetComponent<IA_ControlEnemy>();

        startShooting = true;
        dead = false;

        switch (enemyType)
        {
            case EnemyType.Soldier: enemyPoints = 1; if (actualAtkRange <= 0) actualAtkRange = 20f; break;
                case EnemyType.Snipper: enemyPoints = 5; if (actualAtkRange <= 0) actualAtkRange = 30f; break;
        }
        //Colocamos los rangos de ataque de los enemigos, seg�n el dado.
        atkRangeIfPlayerShot = actualAtkRange + 10;
        atkRangeIfPlayerSneak = actualAtkRange - 12;
        atkRangeMin = actualAtkRange - 5;
    }

    private void Update()
    {
        MovementAndActions();
    }

    #region Movements, ranges and actions.
    private void MovementAndActions()
    {
        if (!dead)
        {
            AtkRangeChanges();

            Animations();

            //Si no est� en rango, mirar� a la �ltima posici�n en la que vi� al jugador.
            //ADEM�S, la mira, sigue apuntado al jugador, para detectarlo a menos que est� en su espalda con el Raycast.
            if (!InRange())
            {
                startShooting = true; //Se vuelve true, para cada vez que pierda en rango y vuelva a verlo, no ataque instant�neo.

                if (controlE != null)
                    controlE.enabled = true;

                if (lastTargetPosition != Vector3.zero)
                    transform.LookAt(lastTargetPosition + redirection);
                firePoint.LookAt(targetPlayer.position);
            }
            //Si est� en rango, se apuntar� al jugador y se crear� su ruta hacia �l siempre que lo "vea".
            if (InRange())
            {
                if(controlE != null)
                    controlE.enabled = false;

                transform.LookAt(targetPlayer.position + redirection);
                firePoint.LookAt(targetPlayer.position);
                enemy.SetDestination(targetPlayer.position);

                switch (enemyType)
                {
                    case EnemyType.Soldier: StartCoroutine(Soldier()); break;
                    case EnemyType.Snipper: StartCoroutine(Snipper()); break;
                }
                lastTargetPosition = targetPlayer.position; //Recoge la �ltima posici�n del jugador.
            }
        }
        else if (dead && enemyType == EnemyType.Snipper) laser.DesactiveLaserPoint(); //Asegurar que se desactiva el l�ser al morir.
    }

    private void AtkRangeChanges()
    {
        //Si el jugador tiene la linterna apagada, esta agachado o ambos, les costar� m�s verlo a los enemigos.
        if (targetPlayer.GetComponent<PlayerController>().Down() && !targetPlayer.GetComponentInChildren<Light>().enabled)
            atkRange = atkRangeIfPlayerSneak;
        else if (targetPlayer.GetComponent<PlayerController>().Down() || !targetPlayer.GetComponentInChildren<Light>().enabled)
            atkRange = atkRangeMin;
        else atkRange = actualAtkRange;

        //Si el jugador dispara, los enemigos mirar�n hacia la direcci�n del disparo y podr�n detectarlo desde un poco m�s lejos.
        if (!playerShoot.canShoot && !InRange())
        {
            transform.LookAt(targetPlayer.position + redirection);
            atkRange = atkRangeIfPlayerShot;
        }
    }
    private void Animations()
    {
        //Si se mueve, se anima.
        if (enemy.velocity.magnitude > 0)
        {
            animEnemy.SetBool("Walk", true);
        }
        else
            animEnemy.SetBool("Walk", false);
    }
    #endregion

    #region Coroutines
    //Para que los enemigos nada m�s verte no disparen inst�ntaneamente.
    IEnumerator Soldier()
    {
        if (startShooting)
            yield return new WaitForSeconds(0.5F);

        startShooting = false;
        if (InShootRange() && shoot.canShoot)
        {
            audioM.PlayOneShot("ShootPistol", 0.3F);
            StartCoroutine(shoot.ShootTime(atkSpeed));
        }
    }

    IEnumerator Snipper()
    {
        if(startShooting)
            yield return new WaitForSeconds(2F);

        startShooting = false;
        if (shoot.canShoot)
        {
            audioM.PlayOneShot("RifleShoot", 0.3F);
            StartCoroutine(Reload());
            StartCoroutine(shoot.ShootTime(atkSpeed));
        }
    }

    //La recarga del Snipper y su animaci�n, deshabilitando el l�ser ya que no est� apuntando.
    IEnumerator Reload()
    {
        if (!dead)
        {
            yield return new WaitForSeconds(0.5F);
            animEnemy.SetTrigger("Reload");
            reload = true;
            yield return new WaitForSeconds(2F);
            animEnemy.ResetTrigger("Reload");
            reload = false;
        }
    }

    #endregion

    #region Others
    //Rango de disparo del enemigo.
    //Si esta a menos de la distancia de freno disparar� Y si est� a cierta distancia entre el jugador y el punto de Stop.
    public bool InShootRange()
    {
        if (enemy.destination != null)
        {
            if(enemy.remainingDistance <= enemy.stoppingDistance && enemy.remainingDistance > 0)
                return true;
            else if (enemy.remainingDistance <= enemy.stoppingDistance + maxShootRange && enemy.remainingDistance >= enemy.stoppingDistance - minShootRange)
                return true;
        }
        return false;
    }

    //Detecta al jugador si entra en su rango de ataque.
    public bool InRange()
    {
        RaycastHit hit;
        Ray ray = new Ray(firePoint.transform.position, firePoint.transform.forward);
        if(Physics.Raycast(ray, out hit, atkRange))
        {
            collision = hit.point;
        }

        if (hit.collider != null && hit.collider.CompareTag("Player") && !reload)
        {
            if (enemyType == EnemyType.Snipper) //Si es el Snipper, deshabilita o habilita la mira.
            { laser.ActiveLaserPoint(); }
            return true;
        }
        else
        {
            if (enemyType == EnemyType.Snipper)
                laser.DesactiveLaserPoint();
            return false; 
        }
    }

    //Reduce la vida del enemigo y recibe la zona donde ha sido golpeado.
    public void ReduceEnemyHealth(int quantity, Collision zoneHit)
    {
        if (!dead)
        {
            health -= quantity;
            enemy.SetDestination(targetPlayer.position);
            //Si el enemigo se queda sin vidas lo destruyo
            if (health <= 0)
            {
                GameObject.FindObjectOfType<PlayerController>().IncreasePlayerScore(enemyPoints);
                dead = true;
                enemy.isStopped = true;
                switch (zoneHit.gameObject.tag)
                {
                    case "EnemyFront":
                        animEnemy.SetTrigger("FrontDeath");
                        break;
                    case "EnemyBack":
                        animEnemy.SetTrigger("BackDeath");
                        break;
                    case "EnemyHead":
                        audioM.Play("Headshot");
                        animEnemy.SetTrigger("FrontHeadshoot");
                        break;
                    case "EnemyBackHead":
                        animEnemy.SetTrigger("BackHeadshoot");
                        break;
                    case "EnemySwat":
                        animEnemy.SetTrigger("Death");
                        break;
                }
                Invoke("Dead", 15F);
            }
        }
    }

    private void Dead()
    {
        gameObject.SetActive(false);
    }
    public void Respawn()
    {
        gameObject.SetActive(true);
        dead = false;
        if(enemy.isStopped)
            enemy.isStopped = false;
    }


    //Simplemente usado de gu�a para ver si chocaba el Raycast.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(collision, 1.3f);
    }
    #endregion
}
