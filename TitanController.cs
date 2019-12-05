using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class TitanController : MonoBehaviour
{

    public bool titanMoveEnabled = true;

    [SerializeField] int maxHp = 100;
    [SerializeField] int minPlayerDamage = 5;
    [SerializeField] int midlePlayerDamage = 10;
    [SerializeField] float attackInterval = 3.0f;
    [SerializeField] float damage1Interval = 1.0f;
    [SerializeField] float damage2_1Interval = 1.2f;
    [SerializeField] float damage2_2Interval = 0.9f;
    [SerializeField] float damage3_1Interval = 1.0f;
    [SerializeField] float damage3_2Interval = 1.0f;
    [SerializeField] float damage3_3Inetrval = 1.0f;
    [SerializeField] float rageInterval = 1.5f;
    [SerializeField] float stanInterval = 1.0f;
    [SerializeField] int score = 2500;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float deadTime = 3;
    [SerializeField] GameObject Sphere;

    public AudioClip titanAttack;
    public AudioClip titanDeath;
    public AudioClip titanRage;
    AudioSource audiosource;
    AudioSource bossBGMSource;

    bool attacking = false;
    bool raging = false;
    bool staning = false;
    bool destroyed = false;
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
    CapsuleCollider capsuleCollider;

    public int TitanHp
    {
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp <= maxHp / 2)
                StartCoroutine(Rage());
            if (hp % (maxHp/5) == 0 && hp != maxHp && hp != 0)
                StartCoroutine(Stan());
            if (hp <= 0)
            {
                StartCoroutine(Dead());
                destroyed = true;
            }
        }
        get
        {
            return hp;
        }
    }

    public bool GetDestroyed()
    {
        return destroyed;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        meshCollider = GetComponent<MeshCollider>();
        audiosource = GetComponent<AudioSource>();
        bossBGMSource= GameObject.FindGameObjectWithTag("BossAudioSource").GetComponent<AudioSource>();
        capsuleCollider = GameObject.FindGameObjectWithTag("TitanHead").GetComponent<CapsuleCollider>();
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        InitCharacter();
    }


    // Update is called once per frame
    void Update()
    {
        if (destroyed)
            bossBGMSource.volume -= Time.deltaTime / 7;
        if (titanMoveEnabled)
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
        TitanHp = maxHp;
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

    void Stop()
    {
        agent.speed = 0;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);
    }

    IEnumerator Dead()
    {
        titanMoveEnabled = false;
        Stop();
        gameManager.Score += score;
        rigidbody.isKinematic = true;
        animator.SetTrigger("Dead");
        audiosource.PlayOneShot(titanDeath);
        yield return new WaitForSeconds(deadTime);
        //player.normalBGMSource.enabled = true;
        Instantiate(Sphere, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Quaternion.identity);
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
        if (!raging)
        {
            raging = true;
            titanMoveEnabled = false;
            collisionPlayerTimer = 0f;
            audiosource.PlayOneShot(titanRage);
            animator.SetTrigger("Rage");
            yield return new WaitForSeconds(rageInterval);
            titanMoveEnabled = true;
        }
        yield return null;
    }

    IEnumerator Stan()
    {
        if (!staning)
        {
            staning = true;
            titanMoveEnabled = false;
            collisionPlayerTimer = 0f;
            animator.SetTrigger("GetHit");
            yield return new WaitForSeconds(stanInterval);
            titanMoveEnabled = true;
            staning = false;
        }
        yield return null;
    }

    IEnumerator AttackTimer()
    {
        if (!attacking)
        {

            attacking = true;
            titanMoveEnabled = false;
            if (!raging)
                attackNum = 1;
            else
                attackNum = Random.Range(1, 4);
            audiosource.PlayOneShot(titanAttack);
            if (attackNum == 1)
            {
                animator.SetTrigger("Attack1");
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage1Interval);
                if (collisionPlayerTimer >= damage1Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage1Interval);
            }
            else if (attackNum == 2)
            {
                animator.SetTrigger("Attack2");
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage2_1Interval);
                if (collisionPlayerTimer >= damage2_1Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage2_2Interval - damage2_1Interval);
                if (collisionPlayerTimer >= damage2_2Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage2_2Interval);
            }
            else if (attackNum == 3)
            {
                animator.SetTrigger("Attack3");
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage3_1Interval);
                if (collisionPlayerTimer >= damage3_1Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage3_2Interval - damage3_1Interval);
                if (collisionPlayerTimer >= damage3_2Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage3_3Inetrval - damage3_2Interval);
                if (collisionPlayerTimer >= damage3_3Inetrval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(attackInterval - damage3_3Inetrval);
            }
            attacking = false;
            titanMoveEnabled = true;
        }
        yield return null;
    }
}