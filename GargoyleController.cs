using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoyleController : MonoBehaviour
{
    [SerializeField] float waitTimer = 3f;
    [SerializeField] GameObject[] Sphere;
    [SerializeField] Material[] materials;
    [SerializeField] GameObject OrkBerserker;
    GameObject titanSphere;
    GameObject reptileSphere;
    GameObject magmadarSphere;
    FirstPersonGunController player;
    GameManager gameManager;
    AudioSource audioSource;
    public AudioClip SE;
    public AudioClip evolution1;
    public AudioClip evolution2;
    public AudioClip evolution3;
    int count = 0;
    bool once = false;
    bool absorbing = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    public int GetCount()
    {
        return count;
    }

    // Update is called once per frame
    void Update()
    {
        SphereMove();
        MaterialChanger();
    }

    void MaterialChanger()
    {
        if (count == 1 && once) {
            once = false;
            gameObject.GetComponentInChildren<Renderer>().material = materials[0];
            audioSource.PlayOneShot(evolution1);
            player.heelLevel++;
            player.repairLevel++;
        }
        if(count==2 && once)
        {
            once = false;
            gameObject.GetComponentInChildren<Renderer>().material = materials[1];
            audioSource.PlayOneShot(evolution2);
            player.heelLevel++;
            player.repairLevel++;
        }
        if (count == 3 && once)
        {
            once = false;
            gameObject.GetComponentInChildren<Renderer>().material = materials[2];
            audioSource.PlayOneShot(evolution3);
            player.heelLevel++;
            player.repairLevel++;
            Instantiate(OrkBerserker, new Vector3(88f, 1f, 96f), Quaternion.identity);
            gameManager.lastBossBattle = true;
            player.normalBGMSource.enabled = false;
        }
    }

    void SphereMove()
    {
        if (titanSphere != null)
        {
            titanSphere.transform.position = Vector3.MoveTowards(titanSphere.transform.position, transform.position, Time.deltaTime);
            if (titanSphere.transform.position == transform.position)
            {
                once = true;
                Destroy(titanSphere);
            }
        }
        if (reptileSphere != null)
        {
            reptileSphere.transform.position = Vector3.MoveTowards(reptileSphere.transform.position, transform.position, Time.deltaTime);
            if (reptileSphere.transform.position == transform.position)
            {
                once = true;
                Destroy(reptileSphere);
            }
        }
        if (magmadarSphere != null)
        {
            magmadarSphere.transform.position = Vector3.MoveTowards(magmadarSphere.transform.position, transform.position, Time.deltaTime);
            if (magmadarSphere.transform.position == transform.position)
            {
                once = true;
                Destroy(magmadarSphere);
            }
        }
    }

    IEnumerator Absorb()
    {
        if (!absorbing)
        {
            absorbing = true;
            if (player.getTitanSphere && count == 0)
            {
                count++;
                Vector3 pos = player.transform.root.gameObject.transform.position;
                titanSphere = Instantiate(Sphere[0], new Vector3(pos.x, pos.y + 1.0f, pos.z), Quaternion.identity);
                player.getTitanSphere = false;
                audioSource.PlayOneShot(SE);
            }
            else if (player.getReptileSphere && count == 1)
            {
                count++;
                Vector3 pos = player.transform.root.gameObject.transform.position;
                reptileSphere = Instantiate(Sphere[0], new Vector3(pos.x, pos.y + 1.0f, pos.z), Quaternion.identity);
                player.getReptileSphere = false;
                audioSource.PlayOneShot(SE);
            }
            else if (player.getMagmadarSphere && count == 2)
            {
                count++;
                Vector3 pos = player.transform.root.gameObject.transform.position;
                magmadarSphere = Instantiate(Sphere[0], new Vector3(pos.x, pos.y + 1.0f, pos.z), Quaternion.identity);
                player.getMagmadarSphere = false;
                audioSource.PlayOneShot(SE);
            }
            yield return new WaitForSeconds(waitTimer);
            absorbing = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.U))
            StartCoroutine(Absorb());
    }
}
