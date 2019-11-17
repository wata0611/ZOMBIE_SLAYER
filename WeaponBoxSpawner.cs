using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBoxSpawner : MonoBehaviour
{
    public bool spawnEnabled = true;
    int spawnPosNum = 0;
    [SerializeField] GameObject weaponBox;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnEnabled)
        {
            spawnPosNum = Random.Range(1, 3);
            if (spawnPosNum == 1)
                Instantiate(weaponBox, new Vector3(165f, 20f, 107.5f), Quaternion.Euler(0f, 180f, 0f));
            else if (spawnPosNum == 2)
                Instantiate(weaponBox, new Vector3(87f, 20f, 122f), Quaternion.Euler(0f, 90f, 0f));
            spawnEnabled = false;
        }
    }
}
