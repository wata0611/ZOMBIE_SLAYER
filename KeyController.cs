using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
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
        gameObject.transform.Rotate(new Vector3(0, Mathf.Sin(2 * Mathf.PI * 5 * Time.deltaTime), 0));
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && !player.getRedKey && gameObject.tag == "RedKey")
        {
            player.getRedKey = true;
            player.audioSource.PlayOneShot(player.getKey);
            Destroy(gameObject);
        }
        else if (collider.gameObject.tag == "Player" && !player.getYellowKey && gameObject.tag == "YellowKey")
        {
            player.getYellowKey = true;
            player.audioSource.PlayOneShot(player.getKey);
            Destroy(gameObject);
        }
        else if (collider.gameObject.tag == "Player" && !player.getBlueKey && gameObject.tag == "BlueKey")
        {
            player.getBlueKey = true;
            player.audioSource.PlayOneShot(player.getKey);
            Destroy(gameObject);
        }
    }
}
