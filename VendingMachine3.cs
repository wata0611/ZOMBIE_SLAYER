using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine3 : MonoBehaviour
{
    [SerializeField] GameObject gameObject;

    FirstPersonGunController player;
    GameManager gameManager;
    public AudioClip vendingMachineMusic3;
    public AudioClip unlockedVendingMachine3Sound;
    public AudioClip lunchVendingMachine3Sound;
    AudioSource audioSource;
    float musicInterval = 71f;
    bool playingMusic = false;
    bool unlockedVendingMachine = false;
    public Material unlockedVendingMachine3Display;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator PlayVendingMachineMusic()
    {
        if (!playingMusic)
        {
            playingMusic = true;
            audioSource.PlayOneShot(vendingMachineMusic3);
            yield return new WaitForSeconds(musicInterval);
            playingMusic = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.C) && player.GetEnhancePlayerAutoHeelScore() <= gameManager.Score && unlockedVendingMachine)
            {
                StartCoroutine(player.EnhancePlayerAutoHeelTimer());
            }
            if (Input.GetKeyDown(KeyCode.V) && player.getYellowKey && !unlockedVendingMachine)
            {
                unlockedVendingMachine = true;
                player.getYellowKey = false;
                audioSource.PlayOneShot(unlockedVendingMachine3Sound);
                audioSource.PlayOneShot(lunchVendingMachine3Sound);
                GameObject.FindGameObjectWithTag("Display3").GetComponent<Renderer>().material = unlockedVendingMachine3Display;
                Instantiate(gameObject, new Vector3(120, 0, 95), Quaternion.identity);
                gameManager.bossBattle = true;
                player.normalBGMSource.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (unlockedVendingMachine)
            StartCoroutine(PlayVendingMachineMusic());
    }

}
