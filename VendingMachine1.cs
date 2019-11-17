using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine1 : MonoBehaviour
{
    [SerializeField] GameObject gameObject;

    FirstPersonGunController player;
    GameManager gameManager;
    public AudioClip vendingMachineMusic1;
    public AudioClip unlockedVendingMachine1Sound;
    public AudioClip lunchVendingMachine1Sound;
    AudioSource audioSource;
    float musicInterval = 100f;
    bool playingMusic = false;
    bool unlockedVendingMachine = false;
    public Material unlockedVendingMachine1Display;

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
            audioSource.PlayOneShot(vendingMachineMusic1);
            yield return new WaitForSeconds(musicInterval);
            playingMusic = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Z) && player.GetEnhancePlayerHPScore() <= gameManager.Score && unlockedVendingMachine)
            {
                StartCoroutine(player.EnhancePlayerHPTimer());
            }
            if(Input.GetKeyDown(KeyCode.U) && player.getRedKey && !unlockedVendingMachine)
            {
                unlockedVendingMachine = true;
                player.getRedKey = false;
                audioSource.PlayOneShot(unlockedVendingMachine1Sound);
                audioSource.PlayOneShot(lunchVendingMachine1Sound);
                GameObject.FindGameObjectWithTag("Display1").GetComponent<Renderer>().material = unlockedVendingMachine1Display;
                Instantiate(gameObject, new Vector3(120, 0, 95), Quaternion.identity);
                gameManager.bossBattle = true;
                player.normalBGMSource.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(unlockedVendingMachine)
            StartCoroutine(PlayVendingMachineMusic());
    }
}
