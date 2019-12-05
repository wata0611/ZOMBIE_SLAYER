using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{

    public bool moveEnabled = true;

    [SerializeField] int maxHp = 3;
    [SerializeField] int playerDamage = 5;
    [SerializeField] int warehouseDamage = 5;
    [SerializeField] float attackInterval = 1.0f;
    [SerializeField] int score = 100;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float deadTime = 3;

    public AudioClip umeki;
    public AudioClip zombieAttack;
    public AudioClip zombieDeath;
    AudioSource audiosource;

    bool attacking = false;
    bool Destroyed = false;
    int hp;
    float moveSpeed;
    Animator animator;
    BoxCollider boxCollider;
    CapsuleCollider capsuleCollider;
    Rigidbody rigidbody;
    NavMeshAgent agent;
    Transform target;
    Transform targetPlayer;
    GameManager gameManager;
    FirstPersonGunController player;
    WarehouseController warehouse;
    EnemySpawner spawner;

    public int Hp
    {
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp <= 0)
                StartCoroutine(Dead());
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
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemySpawner>();
        InitCharacter();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (moveEnabled)
        {
            //audiosource.Play(umeki);
            Move();
        }
        else
        {
            //audiosource.Stop(umeki);
            Stop();
        }
    }

    void InitCharacter()
    {
        moveSpeed = agent.speed;
        maxHp += gameManager.round - 1;
        if(Random.Range(1,21) <= gameManager.round - 2)
        {
            moveSpeed += 1f;
            attackInterval -= 0.2f;
            score = score * 5;
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

    IEnumerator Dead()
    {
        moveEnabled = false;
        Stop();
        gameManager.Score += score;
        gameManager.Kill++;
        gameManager.totalKill++;
        animator.SetTrigger("Dead");
        boxCollider.enabled = false;
        capsuleCollider.enabled = false;
        rigidbody.isKinematic = true;
        audiosource.PlayOneShot(zombieDeath);
        yield return new WaitForSeconds(deadTime);
        Destroy(gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && targetTag == "Player" && moveEnabled)
            StartCoroutine(AttackTimer());
        else if (collision.gameObject.tag == "Warehouse" && warehouse.WarehouseHP > 0 && targetTag == "Warehouse" && moveEnabled)
            StartCoroutine(AttackTimer());
    }

    /**private void OnCollisionEnter(Collision colliision)
    {
        if(colliision.gameObject.tag == "Obstacle")
        {
            if (!Destroyed)
            {
                Destroy(this.gameObject);
                spawner.enemySpawnCount--;
                Destroyed = true;
            }
        }
    }**/

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
