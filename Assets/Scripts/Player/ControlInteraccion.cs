using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class ControlInteraccion : MonoBehaviour
{
    private float ultimoChequeo;
    private AI_Dialog dialog;
    public GameObject panelTxtBox;

    [Header("Interactions")]
    public float indiceTiempoChequeo = 0.05f;
    public float maxDistanciaChequeo;
    public LayerMask capaRayMask;

    private GameObject actualGameobjectInteractuable;
    private string actualInteractuable;

    public TextMeshProUGUI mensajeTexto;
    private Camera camara;

    private void Start()
    {
        camara = Camera.main;
    }

    private void Update()
    {
        if (Time.time - ultimoChequeo > indiceTiempoChequeo)
        {
            ultimoChequeo = Time.time;
            Ray rayo = camara.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit lanzaRayo;

            if (Physics.Raycast(rayo, out lanzaRayo, maxDistanciaChequeo, capaRayMask))
            {
                actualGameobjectInteractuable = lanzaRayo.collider.gameObject;
                actualInteractuable = lanzaRayo.collider.tag;
                EstablecerMensaje();
                Interaction();
            }
            else
            {
                actualGameobjectInteractuable = null;
                actualInteractuable = null;
                mensajeTexto.gameObject.SetActive(false);
            }
        }
    }

    private void EstablecerMensaje()
    {
        mensajeTexto.gameObject.SetActive(true);
        mensajeTexto.text = string.Format("PRESS <b>[E]</b> TO TALK TO {0}", actualInteractuable);
    }

    private void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.E) && !panelTxtBox.activeSelf)
        {
            indiceTiempoChequeo = 999;
            dialog = actualGameobjectInteractuable.GetComponent<AI_Dialog>();
            mensajeTexto.gameObject.SetActive(false);
            dialog.StartDialog();
            Debug.Log("Hablaste con " + actualInteractuable);
        }
    }
}