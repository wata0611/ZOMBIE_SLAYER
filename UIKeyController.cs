using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeyController : MonoBehaviour
{
    FirstPersonGunController player;
    GameObject redKey;
    GameObject yellowKey;
    GameObject blueKey;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        redKey = transform.GetChild(0).gameObject;
        yellowKey = transform.GetChild(1).gameObject;
        blueKey = transform.GetChild(2).gameObject;
        redKey.SetActive(false);
        yellowKey.SetActive(false);
        blueKey.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        redKey.SetActive(player.getRedKey);
        yellowKey.SetActive(player.getYellowKey);
        blueKey.SetActive(player.getBlueKey);
    }
}
