using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/** Author: Sebasti�n Jim�nez Fern�ndez.
 * Class for Extra obj.
 * */
public enum ExtraType
{
    Health,
    Bullet,
    Skull
}

public class ControlExtras : MonoBehaviour
{
    public ExtraType tipo;
    public int quantity;
    private AudioManager audioM;

    private void Awake()
    {
        audioM = AudioManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            switch (tipo)
            {
                case ExtraType.Health:
                    // Llamo a m�todo del jugador para incrementar la vida
                    if(!(other.GetComponent<PlayerController>().actualHealth >= other.GetComponent<PlayerController>().maxHealth))
                    {
                        audioM.Play("Heart");
                        other.GetComponent<PlayerController>().IncreasePlayerHealth(quantity);
                        gameObject.SetActive(false);
                    }
                    break;
                case ExtraType.Bullet:
                    // Llamo a m�todo de arma para incrementar el n�mero de balas, siempre que no est� al m�ximo.
                    if(other.GetComponentInChildren<Shooting>().totalAmmo < other.GetComponentInChildren<Shooting>().maxAmmo)
                    {
                        audioM.Play("Ammo");
                        other.GetComponentInChildren<Shooting>().IncreaseAmmo(quantity);
                        gameObject.SetActive(false);
                    }
                    break;
                case ExtraType.Skull:
                    audioM.Play("Skull");
                    other.GetComponent<PlayerController>().IncreasePlayerSkulls();
                    Debug.Log(transform.gameObject.tag); 
                    PlayerPrefs.SetString(transform.gameObject.tag, transform.gameObject.tag);
                    
                    gameObject.SetActive(false);
                    break;
            }
        }
        
    }
}


