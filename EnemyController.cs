using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{

    public bool moveEnabled = true;

    [SerializeField] int initMaxHp = 3;
    [SerializeField] int playerDamage = 5;
    [SerializeField] int warehouseDamage = 5;
    [SerializeField] float attackInterval1 = 1.0f;
    [SerializeField] float attackInterval2 = 0.8f;
    [SerializeField] int score1 = 100;
    [SerializeField] int score2 = 500;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float deadTime = 3;

    public AudioClip umeki;
    public AudioClip zombieAttack;
    public AudioClip zombieDeath;
    AudioSource audiosource;

    bool attacking = false;
    bool dead = true;
    int hp;
    int maxHp;
    int score;
    float moveSpeed;
    float moveSpeed2;
    float attackInterval;
    Animator animator;
    public BoxCollider boxCollider;
    public CapsuleCollider capsuleCollider;
    Rigidbody rigidbody;
    NavMeshAgent agent;
    Transform target;
    Transform targetPlayer;
    GameManager gameManager;
    FirstPersonGunController player;
    WarehouseController warehouse;

    public int Hp
    {
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp <= 0)
            {
                StartCoroutine(Dead());
            }
        }
        get
        {
            return hp;
        }
    }

    public bool GetDead()
    {
        return dead;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = transform.Find("Head").gameObject.GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        audiosource = GetComponent<AudioSource>();

        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        warehouse = GameObject.FindGameObjectWithTag("Warehouse").GetComponentInChildren<WarehouseController>();
        InitCharacter();
    }

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = transform.Find("Head").gameObject.GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        audiosource = GetComponent<AudioSource>();

        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        warehouse = GameObject.FindGameObjectWithTag("Warehouse").GetComponentInChildren<WarehouseController>();
        InitCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveEnabled)
        {
            Move();
        }
        else
        {
            Stop();
        }
    }

    public void InitCharacter()
    {
        agent.speed = 1.0f;
        moveEnabled = true;
        dead = false;
        moveSpeed = agent.speed;
        maxHp =initMaxHp + gameManager.round - 1;
        attackInterval = attackInterval1;
        score = score1;
        if (Random.Range(1, 21) <= gameManager.round - 2)
        {
            agent.speed = 1.5f;
            moveSpeed = agent.speed;
            attackInterval = attackInterval2;
            score = score2;
        }
        Hp = maxHp;
    }

    void Move()
    {
        agent.speed = moveSpeed;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);

        if (warehouse.WarehouseHP > 0)
        {
            if (target.tag == "Warehouse")
                targetTag = "Warehouse";
            agent.SetDestination(target.position);
        }
        else
        {
            targetTag = "Player";
            agent.SetDestination(targetPlayer.position);
        }
        rigidbody.velocity = agent.desiredVelocity;
    }

    void Stop()
    {
        agent.speed = 0;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);
    }

    //使いまわす。
    IEnumerator Dead()
    {
        if (!dead)
        {
            dead = true;
            moveEnabled = false;
            Stop();
            gameManager.Score += score;
            gameManager.Kill++;
            gameManager.totalKill++;
            animator.SetTrigger("Dead");
            audiosource.PlayOneShot(zombieDeath);
            yield return new WaitForSeconds(deadTime);
            if (transform.childCount > 18) 
            {
                for (int i = 18; i < transform.childCount; i++)
                    Destroy(transform.GetChild(i).gameObject);
            }
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && targetTag == "Player" && moveEnabled && Hp > 0)
            StartCoroutine(AttackTimer());
        else if (collision.gameObject.tag == "Warehouse" && warehouse.WarehouseHP > 0 && targetTag == "Warehouse" && moveEnabled && Hp > 0)
            StartCoroutine(AttackTimer());
    }

    IEnumerator AttackTimer()
    {
        if (!attacking)
        {
            attacking = true;
            moveEnabled = false;
            animator.SetTrigger("Attack");
            if (targetTag == "Player")
            {
                player.PlayerHP -= playerDamage;
            }
            if (targetTag == "Warehouse")
                warehouse.WarehouseHP -= warehouseDamage;
            audiosource.PlayOneShot(zombieAttack);
            yield return new WaitForSeconds(attackInterval);

            attacking = false;
            moveEnabled = true;
        }
        yield return null;
    }
}
