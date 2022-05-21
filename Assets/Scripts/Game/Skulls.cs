using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/** Author: Sebastián Jiménez Fernández.
 * Class Skulls.
 * */
public class Skulls : MonoBehaviour
{
    public GameObject[] skulls;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            if (PlayerPrefs.GetString("Skull" + (i + 1)) != "" && skulls[i].CompareTag(PlayerPrefs.GetString("Skull" + (i + 1))))
                skulls[i].SetActive(false);
        }
    }
}
