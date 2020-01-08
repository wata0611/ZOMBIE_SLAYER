using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine2 : MonoBehaviour
{

    [SerializeField] GameObject gameObject;

    FirstPersonGunController player;
    GameManager gameManager;
    public AudioClip vendingMachineMusic2;
    public AudioClip unlockedVendingMachine2Sound;
    public AudioClip lunchVendingMachine2Sound;
    AudioSource audioSource;
    float musicInterval = 59f;
    bool playingMusic = false;
    bool unlockedVendingMachine = false;
    public Material unlockedVendingMachine2Display;

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
            audioSource.PlayOneShot(vendingMachineMusic2);
            yield return new WaitForSeconds(musicInterval);
            playingMusic = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.C) && player.GetEnhancePlayerWalkSpeedScore() <= gameManager.Score && unlockedVendingMachine)
            {
                StartCoroutine(player.EnhancePlayerWalkSpeedTimer());
            }
            if (Input.GetKeyDown(KeyCode.V) && player.getBlueKey && !unlockedVendingMachine)
            {

                unlockedVendingMachine = true;
                player.getBlueKey = false;
                audioSource.PlayOneShot(unlockedVendingMachine2Sound);
                audioSource.PlayOneShot(lunchVendingMachine2Sound);
                GameObject.FindGameObjectWithTag("Display2").GetComponent<Renderer>().material = unlockedVendingMachine2Display;
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
