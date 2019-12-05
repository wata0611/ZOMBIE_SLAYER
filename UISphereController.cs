using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISphereController : MonoBehaviour
{
    FirstPersonGunController player;
    GameObject TitanSphere;
    GameObject ReptileSphere;
    GameObject MagmadarSphere;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        TitanSphere = transform.GetChild(1).gameObject;
        ReptileSphere = transform.GetChild(0).gameObject;
        MagmadarSphere = transform.GetChild(2).gameObject;
        TitanSphere.SetActive(false);
        ReptileSphere.SetActive(false);
        MagmadarSphere.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        TitanSphere.SetActive(player.getTitanSphere);
        ReptileSphere.SetActive(player.getReptileSphere);
        MagmadarSphere.SetActive(player.getMagmadarSphere);
    }
}
