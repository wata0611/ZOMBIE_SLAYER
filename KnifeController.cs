using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnifeController : MonoBehaviour
{
    [SerializeField] float attackInterval = 1.0f;
    [SerializeField] int damage = 5;
    [SerializeField] int hitScore = 100;
    [SerializeField] float motionSpeed = 40f;
    [SerializeField] Text ammoText;


    public AudioClip hitSE;
    AudioSource audioSource;

    bool attacking = false;
    public bool attackEnabled = true;
    Transform start;
    Transform finish;
    Transform weaponOut;
    FirstPersonGunController player;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        start= GameObject.FindGameObjectWithTag("KnifeStart").GetComponent<Transform>();
        finish = GameObject.FindGameObjectWithTag("KnifeFinish").GetComponent<Transform>();
        player= GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        weaponOut= GameObject.FindGameObjectWithTag("WeaponOut").GetComponent<Transform>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.Knife)
        {
            if (Input.GetMouseButtonDown(0))
                StartCoroutine(AttackTimer());
            ammoText.text = "- / -";
        }
        if (attacking)
        {
            transform.position = Vector3.MoveTowards(transform.position, finish.position, Time.deltaTime * 10);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finish.rotation, motionSpeed);
        }
        else if(player.haveWeapon1 == FirstPersonGunController.HaveWeapon.Knife)
        {
            transform.position = Vector3.MoveTowards(transform.position, start.position, Time.deltaTime * 20);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, start.rotation, motionSpeed);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "EnemyHead" && attacking)
        {
            audioSource.PlayOneShot(hitSE);
            gameManager.Score += hitScore;
            EnemyController enemy = collider.transform.parent.gameObject.GetComponent<EnemyController>();
            enemy.Hp -= damage * 5;
            player.hitCount++;
        }
        if (collider.gameObject.tag == "Enemy" && attacking)
        {
            audioSource.PlayOneShot(hitSE);
            gameManager.Score += hitScore;
            EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
            enemy.Hp -= damage;
            player.hitCount++;
        }
    }

    public IEnumerator AttackTimer()
    {
        if (!attacking && attackEnabled)
        {
            attacking = true;
            player.shootCount++;
            yield return new WaitForSeconds(attackInterval);
            transform.position = weaponOut.transform.position;
            transform.rotation = weaponOut.transform.rotation;
            attacking = false;
        }
    }
}
