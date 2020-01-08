using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class OrkberserkerController : MonoBehaviour
{

    public bool orkBerserkerMoveEnabled = true;

    [SerializeField] int maxHp = 500;
    [SerializeField] int maxWeaponHP = 100;
    [SerializeField] int minPlayerDamage = 5;
    [SerializeField] int midlePlayerDamage = 10;
    [SerializeField] int maxPlayerDamage = 20;
    [SerializeField] float attackInterval = 3.0f;
    [SerializeField] float damage1Interval = 1.0f;
    [SerializeField] float damage2_1Interval = 0.5f;
    [SerializeField] float damage2_2Interval = 0.7f;
    [SerializeField] float damage3Interval = 0.9f;
    [SerializeField] float damage4_1Interval = 1.2f;
    [SerializeField] float damage4_2Interval = 1.5f;
    [SerializeField] float damage4_3Interval = 1.8f;
    [SerializeField] float damage4_4Interval = 2.1f;
    [SerializeField] float damage4_5Interval = 2.5f;
    [SerializeField] float fireFinishInterval = 2.7f;
    [SerializeField] float rageInterval = 1.5f;
    [SerializeField] float stanInterval = 1.0f;
    [SerializeField] int score = 2500;
    [SerializeField] int weaponScore = 10000;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float deadTime = 3;
    [SerializeField] GameObject firePiller;

    public AudioClip orkBerserkerAttack;
    public AudioClip orkBerserkerFire;
    public AudioClip orkBerserkerDeath;
    public AudioClip orkBerserkerRage;
    AudioSource audiosource;
    public AudioSource bossBGMSource;

    bool attacking = false;
    bool raging = false;
    bool wounding = false;
    bool staning = false;
    bool destroyed = false;
    bool destroyedWeapon = false;
    int hp;
    int weaponHP;
    int attackNum = 0;
    float moveSpeed;
    float collisionTimer = 0f;
    float collisionPlayerTimer = 0f;
    Animator animator;
    Rigidbody rigidbody;
    NavMeshAgent agent;
    Transform target;
    GameManager gameManager;
    FirstPersonGunController player;
    MeshCollider meshCollider;
    CapsuleCollider capsuleCollider;
    GameObject fire;


    public int OrkBerserkerHp
    {
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp <= maxHp / 2)
                StartCoroutine(Rage());
            if (hp <= maxHp / 10)
            {
                StartCoroutine(Rage2());
            }
            if (hp % (maxHp / 5) == 0 && hp != maxHp && hp != 0)
                StartCoroutine(Stan());
            if (hp <= 0)
                StartCoroutine(Dead());
        }
        get
        {
            return hp;
        }
    }

    public int OrkBerserkerWeaponHP
    {
        set
        {
            weaponHP = maxWeaponHP;
            if (weaponHP <= 0)
                StartCoroutine(WeaponBreak());
        }
        get
        {
            return weaponHP;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        meshCollider = GetComponentInChildren<MeshCollider>();
        audiosource = GetComponent<AudioSource>();
        bossBGMSource = GameObject.FindGameObjectWithTag("OrkBerserkerHead").GetComponent<AudioSource>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        fire = GameObject.FindGameObjectWithTag("Fire");
        fire.SetActive(false);
        InitCharacter();
    }

    public bool GetDestroyed()
    {
        return destroyed;
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed)
            bossBGMSource.volume -= Time.deltaTime / 7;
        if (orkBerserkerMoveEnabled)
        {
            if (raging && !wounding)
                Run();
            if (wounding && !raging)
                Wound();
            if(!raging&&!wounding)
                Move();
        }
        else
            Stop();
    }

    void InitCharacter()
    {
        OrkBerserkerHp = maxHp;
        moveSpeed = agent.speed;
    }

    void Move()
    {
        agent.speed = moveSpeed;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);
        agent.SetDestination(target.position);
        rigidbody.velocity = agent.desiredVelocity;
    }
    void Run()
    {
        agent.speed = moveSpeed * 2;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);
        agent.SetDestination(target.position);
        rigidbody.velocity = agent.desiredVelocity;
    }

    void Wound()
    {
        agent.speed = moveSpeed / 2;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);
        animator.SetTrigger("Wound");
        agent.SetDestination(target.position);
        rigidbody.velocity = agent.desiredVelocity;
    }

    void Stop()
    {
        agent.speed = 0;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);
    }

    IEnumerator Dead()
    {
        destroyed = true;
        orkBerserkerMoveEnabled = false;
        Stop();
        gameManager.Score += score;
        rigidbody.isKinematic = true;
        animator.SetTrigger("Dead");
        audiosource.PlayOneShot(orkBerserkerDeath);
        yield return new WaitForSeconds(deadTime);
        gameManager.lastBossBattle = false;
        gameManager.gameClear = true;
        Destroy(gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            StartCoroutine(AttackTimer());
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
            collisionPlayerTimer += Time.deltaTime;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
            collisionPlayerTimer = 0f;
    }

    IEnumerator Rage()
    {
        if (!raging && !wounding)
        {
            raging = true;
            orkBerserkerMoveEnabled = false;
            Stop();
            audiosource.PlayOneShot(orkBerserkerRage);
            animator.SetTrigger("Rage");
            yield return new WaitForSeconds(rageInterval);
            orkBerserkerMoveEnabled = true;
        }
        yield return null;
    }

    IEnumerator Rage2()
    {
        if (!wounding)
        {
            raging = false;
            wounding = true;
            orkBerserkerMoveEnabled = false;
            audiosource.PlayOneShot(orkBerserkerRage);
            animator.SetTrigger("Rage");
            yield return new WaitForSeconds(rageInterval);
            orkBerserkerMoveEnabled = true;
        }
        yield return null;
    }

    IEnumerator WeaponBreak()
    {
        if (!staning)
        {
            staning = true;
            destroyedWeapon = true;
            orkBerserkerMoveEnabled = false;
            animator.SetTrigger("Stan");
            gameManager.Score += weaponScore;
            Destroy(GameObject.FindGameObjectWithTag("OrkBerserkerWeapon"));
            yield return new WaitForSeconds(stanInterval);
            orkBerserkerMoveEnabled = true;
            staning = false;
        }
    }

    IEnumerator Stan()
    {
        if (!staning)
        {
            staning = true;
            orkBerserkerMoveEnabled = false;
            animator.SetTrigger("Stan");
            yield return new WaitForSeconds(stanInterval);
            orkBerserkerMoveEnabled = true;
            staning = false;
        }
        yield return null;
    }

    IEnumerator AttackTimer()
    {
        if (!attacking)
        {
            attacking = true;
            orkBerserkerMoveEnabled = false;
            Stop();
            if (!raging&&!wounding)
                attackNum = Random.Range(1, 4);
            else
                attackNum = Random.Range(1, 5);
            audiosource.PlayOneShot(orkBerserkerAttack);
            if (attackNum == 1)
            {
                animator.SetTrigger("Attack3");
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage3Interval);
                if (collisionPlayerTimer >= damage3Interval)
                    player.PlayerHP -= midlePlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage3Interval);
            }
            else if (attackNum == 2)
            {
                animator.SetTrigger("Attack2");
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage2_1Interval);
                if (collisionPlayerTimer >= damage2_1Interval)
                {
                    if (!destroyedWeapon)
                        player.PlayerHP -= maxPlayerDamage;
                    else
                        player.PlayerHP -= midlePlayerDamage;
                }
                yield return new WaitForSeconds(damage2_2Interval);
                if (collisionPlayerTimer >= damage2_2Interval)
                    player.PlayerHP -= midlePlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage2_2Interval);
            }
            else if (attackNum == 3)
            {
                animator.SetTrigger("Attack4");
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage4_1Interval);
                fire.SetActive(true);
                audiosource.PlayOneShot(orkBerserkerFire);
                if (collisionPlayerTimer >= damage4_1Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage4_2Interval - damage4_1Interval);
                if (collisionPlayerTimer >= damage4_2Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage4_3Interval - damage4_2Interval);
                if (collisionPlayerTimer >= damage4_3Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage4_4Interval - damage4_3Interval);
                if (collisionPlayerTimer >= damage4_4Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage4_5Interval - damage4_4Interval);
                if (collisionPlayerTimer >= damage4_5Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(fireFinishInterval - damage4_5Interval);
                fire.SetActive(false);
                yield return new WaitForSeconds(attackInterval - fireFinishInterval);
            }
            else if (attackNum == 4)
            {
                animator.SetTrigger("Attack1");
                collisionPlayerTimer = 0f;
                
                yield return new WaitForSeconds(damage1Interval);
                if (collisionPlayerTimer >= damage1Interval)
                    player.PlayerHP -= maxPlayerDamage;
                Instantiate(firePiller, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f), Quaternion.Euler(-90f, 0f, 0f)); ;
                yield return new WaitForSeconds(attackInterval - damage1Interval);
            }
            attacking = false;
            orkBerserkerMoveEnabled = true;
        }
        yield return null;
    }
}
