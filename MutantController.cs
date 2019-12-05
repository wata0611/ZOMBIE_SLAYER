using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class MutantController : MonoBehaviour
{

    public bool mutantMoveEnabled = true;

    [SerializeField] int maxHp = 30;
    [SerializeField] int minPlayerDamage = 5;
    [SerializeField] int midlePlayerDamage = 10;
    [SerializeField] int maxPlayerDamage = 15;
    [SerializeField] float attackInterval = 3.0f;
    [SerializeField] float damage2_1Interval = 1.0f;
    [SerializeField] float damage2_2Interval = 1.2f;
    [SerializeField] float damage3Interval = 0.9f;
    [SerializeField] float damage7Interval = 1.0f;
    [SerializeField] float damage8Interval = 1.0f;
    [SerializeField] float rageInterval = 1.5f;
    [SerializeField] int score = 1000;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float deadTime = 3;
    [SerializeField] GameObject[] key;

    public AudioClip mutantUmeki;
    public AudioClip mutantAttack;
    public AudioClip mutantDeath;
    public AudioClip mutantRage;
    AudioSource audiosource;

    bool attacking = false;
    bool raging = false;
    bool Destroyed = false;
    int hp;
    int attackNum = 0;
    float moveSpeed;
    //float collisionTimer = 0f;
    float collisionPlayerTimer = 0f;
    Animator animator;
    Rigidbody rigidbody;
    NavMeshAgent agent;
    Transform target;
    GameManager gameManager;
    FirstPersonGunController player;
    EnemySpawner spawner;
    MeshCollider[] meshColliders;
    CapsuleCollider capsuleCollider;

    public bool GetAttacking()
    {
        return attacking;
    }

    public bool GetDestroyed()
    {
        return Destroyed;
    }

    public int MutantHp
    {
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp <= maxHp / 2)
                StartCoroutine(Rage());
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
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        meshColliders = GetComponentsInChildren<MeshCollider>();
        
        capsuleCollider = GameObject.FindGameObjectWithTag("MutantHead").GetComponent<CapsuleCollider>(); 
        audiosource = GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        //spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemySpawner>();
        InitCharacter();
    }


    // Update is called once per frame
    void Update()
    {
        if (mutantMoveEnabled)
            Move();
        else
            Stop();
    }

    void InitCharacter()
    {
        MutantHp = maxHp;
        moveSpeed = agent.speed;
    }

    void Move()
    {
        agent.speed = moveSpeed;
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
        if (!Destroyed)
        {
            Destroyed = true;
            mutantMoveEnabled = false;
            Stop();
            gameManager.Score += score;
            gameManager.Kill++;
            gameManager.totalKill++;
            animator.SetTrigger("Dead");
            rigidbody.isKinematic = true;
            audiosource.PlayOneShot(mutantDeath);
            if (player.getKeyCount < 3)
                Instantiate(key[player.getKeyCount++], this.gameObject.transform.position, Quaternion.identity);
            Debug.Log(player.getKeyCount);
            yield return new WaitForSeconds(deadTime);
            Destroy(gameObject);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(AttackTimer());
        }
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
        if (!raging)
        {
            raging = true;
            mutantMoveEnabled = false;
            collisionPlayerTimer = 0f;
            audiosource.PlayOneShot(mutantRage);
            animator.SetTrigger("Rage");
            yield return new WaitForSeconds(rageInterval);
            mutantMoveEnabled = true;
        }
        yield return null;
    }

    IEnumerator AttackTimer()
    {
        if (!attacking)
        {
            attacking = true;
            mutantMoveEnabled = false;
            if (!raging)
                attackNum = Random.Range(2, 4);
            else
                attackNum = Random.Range(2, 6);
            audiosource.PlayOneShot(mutantAttack);
            if (attackNum == 2)
            {
                animator.SetTrigger("Attack7");
                yield return new WaitForSeconds(damage7Interval);
                if (collisionPlayerTimer >= damage7Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage7Interval);
            }
            else if(attackNum == 3)
            {
                animator.SetTrigger("Attack3");
                yield return new WaitForSeconds(damage3Interval);
                if (collisionPlayerTimer >= damage3Interval)
                    player.PlayerHP -= midlePlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage3Interval);
            }
            else if (attackNum == 4)
            {
                animator.SetTrigger("Attack2");
                yield return new WaitForSeconds(damage2_1Interval);
                if (collisionPlayerTimer >= damage2_1Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage2_2Interval - damage2_1Interval);
                if (collisionPlayerTimer >= damage2_2Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage2_2Interval);
            }
            else if (attackNum == 5)
            {
                animator.SetTrigger("Attack8");
                yield return new WaitForSeconds(damage8Interval);
                if (collisionPlayerTimer >= damage8Interval)
                    player.PlayerHP -= maxPlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage8Interval);
            }
            collisionPlayerTimer = 0f;
            attacking = false;
            mutantMoveEnabled = true;
        }
        yield return null;
    }
}
