using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBoxController : MonoBehaviour
{
    [SerializeField] int score = 1000;
    [SerializeField] float openTimer = 60f;
    [SerializeField] GameObject[] weaponPrefabs;

    public int choosedIndex = 0;
    bool opening = false;
    float weaponRotate = 0f;
    public bool weaponDestroyed = false;
    FirstPersonGunController player;
    GameManager gameManager;
    WeaponBoxSpawner spawner;
    GameObject boxTop;

    public AudioClip open;
    AudioSource audioSource;

    public bool GetOpening()
    {
        return opening;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        boxTop = GameObject.FindGameObjectWithTag("BoxTop");
        spawner = GameObject.FindGameObjectWithTag("WeaponBoxSpawner").GetComponent<WeaponBoxSpawner>();
        audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame

    void OnTriggerStay(Collider collider) 
    {
        if(collider.gameObject.tag == "Player" && !opening && Input.GetKey(KeyCode.O) && score <= gameManager.Score)
        {
            gameManager.Score -= score;
            StartCoroutine(OpenBox());
        }
    }

    IEnumerator OpenBox()
    {
        if (!opening)
        {
            opening = true;
            gameManager.Score -= score;
            weaponDestroyed = false;
            audioSource.PlayOneShot(open);
            WeaponInstantiate();
            yield return new WaitForSeconds(openTimer);
            spawner.spawnEnabled = true;
            opening = false;
            Destroy(this.gameObject.transform.parent.gameObject);
        }
    }

    void WeaponInstantiate()
    {
        int random = Random.Range(1, 101);
        if (random <= 40)
            choosedIndex = 0;
        else if (41 <= random && random <= 70)
            choosedIndex = 1;
        else
            choosedIndex = 2;
        Vector3 weaponPos = this.gameObject.transform.position;
        weaponPos.y += 0.5f;
        if (choosedIndex == 0)
            weaponRotate = -90f;
        else if (choosedIndex == 1)
            weaponRotate = 180;
        else if (choosedIndex == 2)
            weaponRotate = 90f;
        Instantiate(weaponPrefabs[choosedIndex], weaponPos, Quaternion.Euler(0f,transform.rotation.eulerAngles.y + weaponRotate,0f));
    }
    
    void Update()
    {
        if (opening)
        {
            if (boxTop.transform.rotation.eulerAngles.x  <= 268 || boxTop.transform.rotation.eulerAngles.x >= 272)
            {
                Quaternion rot = Quaternion.AngleAxis(-2, Vector3.right);
                boxTop.transform.rotation = boxTop.transform.rotation * rot;
            }
        }
    }
}
