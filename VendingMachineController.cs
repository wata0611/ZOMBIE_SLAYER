using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachineController : MonoBehaviour
{
    [SerializeField] GameObject bossEnemy;
    [SerializeField] FirstPersonGunController player;
    [SerializeField] GameManager gameManager;
    [SerializeField] float musicInterval=100f;
    [SerializeField] Text discription;

    public AudioClip vendingMachineMusic;
    public AudioClip unlockedVendingMachineSound;
    public AudioClip lunchVendingMachineSound;
    AudioSource audioSource;
    bool playingMusic = false;
    bool unlockedVendingMachine = false;
    bool textChangeEnabled = true;
    public Material unlockedVendingMachineDisplay;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator PlayVendingMachineMusic()
    {
        if (!playingMusic)
        {
            playingMusic = true;
            audioSource.PlayOneShot(vendingMachineMusic);
            yield return new WaitForSeconds(musicInterval);
            playingMusic = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            discription.enabled = true;
            if (Input.GetKeyDown(KeyCode.C) && player.GetEnhancePlayerHPScore() <= gameManager.Score && unlockedVendingMachine && gameObject.tag == "VendingMachine1")
            {
                StartCoroutine(player.EnhancePlayerHPTimer());
            }
            if (Input.GetKeyDown(KeyCode.C) && player.GetEnhancePlayerWalkSpeedScore() <= gameManager.Score && unlockedVendingMachine && gameObject.tag == "VendingMachine2")
            {
                StartCoroutine(player.EnhancePlayerWalkSpeedTimer());
            }
            if (Input.GetKeyDown(KeyCode.C) && player.GetEnhancePlayerAutoHeelScore() <= gameManager.Score && unlockedVendingMachine && gameObject.tag == "VendingMachine3")
            {
                StartCoroutine(player.EnhancePlayerAutoHeelTimer());
            }
            if (!unlockedVendingMachine)
            {
                if (textChangeEnabled)
                {
                    textChangeEnabled = false;
                    discription.text = "V:Use Key";
                }
                if (Input.GetKeyDown(KeyCode.V))
                {
                    if (player.getRedKey && gameObject.tag == "VendingMachine1")
                    {
                        unlockedVendingMachine = true;
                        player.getRedKey = false;
                        audioSource.PlayOneShot(unlockedVendingMachineSound);
                        audioSource.PlayOneShot(lunchVendingMachineSound);
                        transform.GetChild(2).GetComponent<Renderer>().material = unlockedVendingMachineDisplay;
                        Instantiate(bossEnemy, new Vector3(120, 0, 95), Quaternion.identity);//.transform.parent=GameObject.FindGameObjectWithTag("EnemyPool").transform;
                        gameManager.bossBattle = true;
                        player.normalBGMSource.enabled = false;
                    }
                    else if (player.getBlueKey && gameObject.tag == "VendingMachine2")
                    {
                        unlockedVendingMachine = true;
                        player.getBlueKey = false;
                        audioSource.PlayOneShot(unlockedVendingMachineSound);
                        audioSource.PlayOneShot(lunchVendingMachineSound);
                        transform.GetChild(2).GetComponent<Renderer>().material = unlockedVendingMachineDisplay;
                        Instantiate(bossEnemy, new Vector3(120, 0, 95), Quaternion.identity);//.transform.parent = GameObject.FindGameObjectWithTag("EnemyPool").transform;
                        gameManager.bossBattle = true;
                        player.normalBGMSource.enabled = false;
                    }
                    else if (player.getYellowKey && gameObject.tag == "VendingMachine3")
                    {
                        unlockedVendingMachine = true;
                        player.getYellowKey = false;
                        audioSource.PlayOneShot(unlockedVendingMachineSound);
                        audioSource.PlayOneShot(lunchVendingMachineSound);
                        transform.GetChild(2).GetComponent<Renderer>().material = unlockedVendingMachineDisplay;
                        Instantiate(bossEnemy, new Vector3(120, 0, 95), Quaternion.identity);//.transform.parent = GameObject.FindGameObjectWithTag("EnemyPool").transform;
                        gameManager.bossBattle = true;
                        player.normalBGMSource.enabled = false;
                    }
                    else
                    {
                        discription.text = "You Don't Have Key or The Key Dose Not Match";
                        Invoke("DeleteText", 2f);
                    }
                }
            }
            else
            {
                if (gameObject.tag == "VendingMachine1")
                    discription.text = "C:Enhance Player Max HP(" + player.GetEnhancePlayerHPScore().ToString() + ")";
                else if(gameObject.tag == "VendingMachine2")
                    discription.text = "C:Enhance Player Max HP(" + player.GetEnhancePlayerHPScore().ToString() + ")";
                else if(gameObject.tag == "VendingMachine3")
                    discription.text = "C:Enhance Player Max HP(" + player.GetEnhancePlayerHPScore().ToString() + ")";
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            textChangeEnabled = true;
            discription.text = "";
            discription.enabled = false;
        }
    }

    void DeleteText()
    {
        textChangeEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (unlockedVendingMachine)
            StartCoroutine(PlayVendingMachineMusic());
    }
}
