using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
/** Author: Sebastián Jiménez Fernández.
 * Class for Shooting and ammo control.
 * */
public class Shooting : MonoBehaviour
{
    [Header("Bullet Vars")]
    public Transform firePoint;
    public Transform target;
    public GameObject bulletPrefab;
    public float shootTime;
    public float bulletForce = 20f;
    public GameObject shootParticle;
    public Transform fire;


    [Header("Player Bullet Vars")]
    public int actualAmmo; //Munición actual.
    public int ammoClip; //Munición max del cargador.
    public int totalAmmo; //Munición en la reserva.
    public int maxAmmo; //Munición max que puede la reserva.

    private Transform cam;
    private UITexts uiTxt;
    private ObjectPooler objPooler;
    private bool isPlayer;
    private bool isReloading;
    private Vector3 collision;
    private AudioManager audioM;

    public bool canShoot;
    public bool infinite;
    public Animator animPistol;

    private void Awake()
    {
        cam = Camera.main.transform;
        uiTxt = FindObjectOfType<UITexts>();
        audioM = AudioManager.Instance;
        animPistol = GetComponentInChildren<Animator>();
        isPlayer = gameObject.CompareTag("PlayerWeapon");

        canShoot = true;
        if (shootTime <= 0) shootTime = 1F;
        if (ammoClip <= 0) ammoClip = 6;
        if (maxAmmo <= 0) maxAmmo = 12;
        totalAmmo = maxAmmo;
        actualAmmo = ammoClip;
    }

    private void Start()
    {
        objPooler = FindObjectOfType<ObjectPooler>();
        isReloading = false;
        if (!isPlayer)
            target = FindObjectOfType<PlayerController>().transform;
        else
        {
            actualAmmo = PlayerPrefs.GetInt("ammo");
            totalAmmo = PlayerPrefs.GetInt("totalAmmo");
            uiTxt.ammoTxt.text = "Ammo: " + actualAmmo + "/" + totalAmmo;
        }
    }

    void Update()
    {   /* Usado para comprobar donde apunta exactamente el jugador.
        if (isPlayer)
        {
            RaycastHit hitPoint;
            
            if (Physics.Raycast(cam.position, cam.forward, out hitPoint))
            {
                collision = hitPoint.point;
            }

        }*/
        if (isReloading || !isPlayer || Time.timeScale == 0)
            return;

        //Disparo, sonido y animación del mismo.
        if (Input.GetButtonDown("Fire1") && canShoot && actualAmmo > 0)
        {
            animPistol.SetTrigger("Shoot");
            audioM.PlayOneShot("ShootPistol");
            StartCoroutine(ShootTime(shootTime));
        }
        if (CanReload())
        {
            animPistol.SetTrigger("Reload");
            audioM.PlayOneAtTime("ReloadPistol");
            StartCoroutine(Reload());
        }
    }

    #region Coroutines
    //Recarga de la pistola.
    IEnumerator Reload()
    {
        isReloading = true;
        animPistol.SetTrigger("Reload");

        int ammo = actualAmmo;
        actualAmmo = Mathf.Clamp(actualAmmo + totalAmmo, 0, ammoClip);
        if (!infinite)
        {
            totalAmmo -= (actualAmmo - ammo);
            PlayerPrefs.SetInt("totalAmmo", totalAmmo);
            PlayerPrefs.SetInt("ammo", actualAmmo);
        }

        yield return new WaitForSeconds(3.75f);
        animPistol.ResetTrigger("Reload"); //No es necesario aquí, pero si tuviesemos otras animaciones habría que hacerlo para que funcionen correctamente.

        uiTxt.ammoTxt.text = "Ammo: " + actualAmmo + "/" + totalAmmo;
        isReloading = false;
    }

    //Tiempo de disparo.
    public IEnumerator ShootTime(float shootTime)
    {
        canShoot = false;
        Shoot();
        yield return new WaitForSeconds(shootTime);
        if(isPlayer)
            animPistol.ResetTrigger("Shoot");
        canShoot = true;
    }

    IEnumerator ShootEffect()
    {
        //Activamos las partículas del disparo
        GameObject particles = objPooler.GetPooledObject(shootParticle.tag);
        particles.transform.SetPositionAndRotation(fire.position, Quaternion.LookRotation(firePoint.transform.forward));
        particles.SetActive(true);
        yield return new WaitForSeconds(particles.GetComponent<ParticleSystem>().main.duration); 
        particles.SetActive(false);
    }

    #endregion

    #region Others
    //Coge la bala del Pooler y la instancia en la posición de la mira (siempre que no sea null).
    public void Shoot()
    {
        if (isPlayer)
        {
            actualAmmo--;
            PlayerPrefs.SetInt("ammo", actualAmmo);
            uiTxt.ammoTxt.text = "Ammo: " + actualAmmo + "/" + totalAmmo;
        }

        GameObject bullet = objPooler.GetPooledObject(bulletPrefab.tag);
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(firePoint.transform.forward);
            bullet.SetActive(true);
        }
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        StartCoroutine(ShootEffect());

        rb.velocity = firePoint.forward * bulletForce;
    }
    public void IncreaseAmmo(int ammo)
    {
        totalAmmo = Mathf.Clamp(totalAmmo + ammo, 0, maxAmmo);
        uiTxt.ammoTxt.text = "Ammo: " + actualAmmo + "/" + totalAmmo;
    }

    //Para recargar y tener más limpio el Update().
    bool CanReload()
    {
        if(Input.GetKeyDown(KeyCode.R) && isPlayer && totalAmmo > 0 && actualAmmo < ammoClip)// || actualAmmo <= 0 && totalAmmo > 0)
            return true;
        return false;
    }

    //Simplemente usado de guía para ver si chocaba el Raycast.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(collision, 1.3f);
    }
    #endregion
}
