using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AI_Dialog : MonoBehaviour
{
    [Header("TextBoxs")]
    public string[] lines;
    public float txtSpeed;
    public GameObject panelTxtBox;
    public GameObject panelResponseBox;
    public TextMeshProUGUI personTxt;
    public TextMeshProUGUI textBox;


    public float speedRotation = 1f;

    private Transform target;
    private Coroutine LookCoroutine;
    private int index;
    private ControlInteraccion tiempoCheck;
    private MouseLook look;
    private Shooting shoot;
    private bool start;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("GroundCheck").transform;
        textBox.text = string.Empty;
        tiempoCheck = FindObjectOfType<ControlInteraccion>();
        look = FindObjectOfType<MouseLook>();
        shoot = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shooting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && start)
        {
            if(textBox.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textBox.text = lines[index];
            }
        }
    }

    #region Dialog
    public void StartDialog()
    {
        StartRotating();
        Cursor.lockState = CursorLockMode.None;

        start = true;
        look.enabled = false;
        shoot.enabled = false;

        index = 0;
        panelTxtBox.SetActive(true);
        personTxt.text = tag;

        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        //Escribe cada carácter letra por letra.
        foreach (char c in lines[index].ToCharArray())
        {
            textBox.text += c;
            yield return new WaitForSeconds(txtSpeed);
        }
    }
    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textBox.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            if(gameObject.CompareTag("Boss"))
            {
                panelResponseBox.SetActive(true);
            }
            else
            {
                Reset();
            }
        }
    }

    public void YesOption()
    {
        Reset();
        SceneManager.LoadScene("Game");
    }

    public void NoOption()
    {
        Reset();
    }

    private void Reset()
    {
        panelTxtBox.SetActive(false);
        if(panelResponseBox.activeSelf)
            panelResponseBox.SetActive(false);
        tiempoCheck.indiceTiempoChequeo = 0;
        look.enabled = true;
        shoot.enabled = true;
        start = false;
        textBox.text = string.Empty;
        Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion

    #region Rotation
    public void StartRotating()
    {
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt());
    }

    private IEnumerator LookAt()
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);

        float time = 0;

        Quaternion initialRotation = transform.rotation;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, lookRotation, time);

            time += Time.deltaTime * speedRotation;

            yield return null;
        }
    }
    #endregion
}
