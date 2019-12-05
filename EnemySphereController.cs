using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySphereController : MonoBehaviour
{
    FirstPersonGunController player;
    GameManager gameManager;
    AudioSource audioSource;
    GargoyleController gargoyle;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        gargoyle=GameObject.FindGameObjectWithTag("Gargoyle").GetComponent<GargoyleController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, Mathf.Sin(2 * Mathf.PI * 5 * Time.deltaTime), 0));
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && !player.getTitanSphere && gameObject.tag == "TitanSphere" && gargoyle.GetCount() == 0)
        {
            player.getTitanSphere = true;
            player.audioSource.PlayOneShot(player.getSphere);
            gameManager.bossBattle = false;
            player.normalBGMSource.enabled = true;
            Destroy(gameObject);
        }

        else if (collider.gameObject.tag == "Player" && !player.getReptileSphere && gameObject.tag == "ReptileSphere")
        {
            player.getReptileSphere = true;
            player.audioSource.PlayOneShot(player.getSphere);
            gameManager.bossBattle = false;
            player.normalBGMSource.enabled = true;
            Destroy(gameObject);
        }

        else if (collider.gameObject.tag == "Player" && !player.getMagmadarSphere && gameObject.tag == "MagmadarSphere")
        {
            player.getMagmadarSphere = true;
            player.audioSource.PlayOneShot(player.getSphere);
            gameManager.bossBattle = false;
            player.normalBGMSource.enabled = true;
            Destroy(gameObject);
        }
    }
}
