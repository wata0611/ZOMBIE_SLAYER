using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedKeyManager : MonoBehaviour
{
    FirstPersonGunController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, Mathf.Sin(2*Mathf.PI*5*Time.deltaTime), 0));
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && !player.getRedKey)
        {
            player.getRedKey = true;
            player.audioSource.PlayOneShot(player.getKey);
            Destroy(gameObject);
        }
    }
}
