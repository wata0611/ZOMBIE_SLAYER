using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class ReptileController : MonoBehaviour
{

    public bool reptileMoveEnabled = true;

    [SerializeField] int maxHp = 10;
    [SerializeField] int minPlayerDamage = 5;
    [SerializeField] int midlePlayerDamage = 10;
    [SerializeField] float attackInterval = 3.0f;
    [SerializeField] float damage1_1Interval = 1.0f;
    [SerializeField] float damage1_2Interval = 1.2f;
    [SerializeField] float damage2Interval = 0.9f;
    [SerializeField] float stanInterval = 1.0f;
    [SerializeField] int score = 2500;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float deadTime = 3;
    [SerializeField] GameObject miniReptile;
    [SerializeField] GameObject Sphere;

    public AudioClip reptileAttack;
    public AudioClip reptileDeath;
    public AudioSource audiosource;
    public AudioSource bossBGMSource;

    bool attacking = false;
    bool raging = false;
    bool staning = false;
    bool destroyed = false;
    bool Audistop = false;
    int hp;
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

    public int ReptileHp
    {
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp <= maxHp / 2)
                raging = true;
            if (hp % (maxHp / 5) == 0 && hp != maxHp && hp != 0)
                StartCoroutine(Stan());
            if (hp <= 0 && !destroyed)
            {
                destroyed = true;
                StartCoroutine(Dead());
            }
        }
        get
        {
            return hp;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        meshCollider = GameObject.FindGameObjectWithTag("Reptile").GetComponent<MeshCollider>();
        audiosource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        //if (!player.reptileKilled)
            bossBGMSource= GameObject.FindGameObjectWithTag("BossAudioSource3").GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        InitCharacter();
    }

    public bool GetDestroyed()
    {
        return destroyed;
    }

    // Update is called once per frame
    void Update()
    { 
        if (!player.reptileKilled && destroyed)
            bossBGMSource.volume -= Time.deltaTime / 7;
        if (reptileMoveEnabled)
        {
            if (!raging)
                Move();
            else
                Run();
        }
        else
            Stop();
    }

    void InitCharacter()
    {
        ReptileHp = maxHp;
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
        agent.speed = moveSpeed * 3;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);
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
        reptileMoveEnabled = false;
        Stop();
        gameManager.Score += score;
        rigidbody.isKinematic = true;
        animator.SetTrigger("Dead");
        audiosource.PlayOneShot(reptileDeath);
        if (!player.reptileKilled)
        {
            player.reptileKilled = true;
            yield return new WaitForSeconds(deadTime / 2);
            for (int i = 0; i < 3; i++)
            {
                float diffPositionX = Random.Range(-1, 1);
                float diffPositionZ = Random.Range(-1, 1);
                Vector3 position = new Vector3(transform.position.x + diffPositionX, transform.position.y, transform.position.z + diffPositionZ);
                Instantiate(miniReptile, position, Quaternion.identity);
            }
            Instantiate(Sphere, new Vector3(transform.position.x, transform.position.y+1.0f,transform.position.z), Quaternion.identity);
        }
        yield return new WaitForSeconds(deadTime);
        Destroy(gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !destroyed)
        {
            collisionPlayerTimer += Time.deltaTime;
            StartCoroutine(AttackTimer());
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            collisionPlayerTimer = 0f;
    }

    IEnumerator Stan()
    {
        if (!staning)
        {
            staning = true;
            reptileMoveEnabled = false;
            collisionPlayerTimer = 0f;
            animator.SetTrigger("Stan");
            yield return new WaitForSeconds(stanInterval);
            reptileMoveEnabled = true;
            staning = false;
        }
        yield return null;
    }

    IEnumerator AttackTimer()
    {
        if (!attacking)
        {
            attacking = true;
            reptileMoveEnabled = false;
            if (!raging)
                attackNum = 2;
            else
                attackNum = Random.Range(1, 3);
            audiosource.PlayOneShot(reptileAttack);
            if (attackNum == 1)
            {
                animator.SetTrigger("Attack1");
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage1_1Interval);
                if (collisionPlayerTimer >= damage1_1Interval)
                {
                    if (!player.reptileKilled)
                        player.PlayerHP -= midlePlayerDamage;
                    else
                        player.PlayerHP -= minPlayerDamage;
                }
                yield return new WaitForSeconds(damage1_2Interval - damage1_1Interval);
                if (collisionPlayerTimer >= damage1_2Interval)
                {
                    if (!player.reptileKilled)
                        player.PlayerHP -= midlePlayerDamage;
                    else
                        player.PlayerHP -= minPlayerDamage;
                }
                yield return new WaitForSeconds(attackInterval - damage1_2Interval);
            }
            else if (attackNum == 2)
            {
                animator.SetTrigger("Attack2");
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage2Interval);
                if (collisionPlayerTimer >= damage2Interval)
                {
                    if (!player.reptileKilled)
                        player.PlayerHP -= midlePlayerDamage;
                    else
                        player.PlayerHP -= minPlayerDamage;
                }
                yield return new WaitForSeconds(attackInterval - damage2Interval);
            }
            attacking = false;
            reptileMoveEnabled = true;
        }
        yield return null;
    }
}