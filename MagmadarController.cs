using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class MagmadarController : MonoBehaviour
{

    public bool magmadarMoveEnabled = true;

    [SerializeField] int maxHp = 500;
    [SerializeField] int minPlayerDamage = 5;
    [SerializeField] int midlePlayerDamage = 10;
    [SerializeField] float attackInterval = 3.0f;
    [SerializeField] float damage1Interval = 1.0f;
    [SerializeField] float damage3_1Interval = 0.3f;
    [SerializeField] float damage3_2Interval = 0.6f;
    [SerializeField] float damage3_3Interval = 0.9f;
    [SerializeField] float damage3_4Interval = 1.2f;
    [SerializeField] float fireFinishInterval = 2.5f;
    [SerializeField] float rageInterval = 1.5f;
    [SerializeField] float stanInterval = 1.0f;
    [SerializeField] int score = 2500;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float deadTime = 3;
    [SerializeField] GameObject Sphere;

    public AudioClip magmadarAttack;
    public AudioClip magmadarDeath;
    public AudioClip magmadarRage;
    public AudioClip fireSE;
    public AudioSource audiosource;
    public AudioSource bossBGMSource;

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
    CapsuleCollider[] capsuleColliders;
    BoxCollider[] boxColliders;
    GameObject[] fire;
    

    public int MagmadarHp
    {
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp <= maxHp / 2)
                StartCoroutine(Rage());
            if (hp % (maxHp / 5) == 0 && hp != maxHp && hp != 0)
                StartCoroutine(Stan());
            if (hp <= 0)
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
        audiosource = GetComponent<AudioSource>();
        bossBGMSource = GameObject.FindGameObjectWithTag("BossAudioSource2").GetComponent<AudioSource>();
        capsuleColliders = GetComponentsInChildren<CapsuleCollider>();
        boxColliders= GetComponentsInChildren<BoxCollider>();
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        fire = GameObject.FindGameObjectsWithTag("Fire");
        fire[0].SetActive(false);
        fire[1].SetActive(false);
        InitCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed)
            bossBGMSource.volume -= Time.deltaTime / 7;
        if (magmadarMoveEnabled)
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
        MagmadarHp = maxHp;
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
        magmadarMoveEnabled = false;
        Stop();
        gameManager.Score += score;
        rigidbody.isKinematic = true;
        animator.SetTrigger("Dead");
        audiosource.PlayOneShot(magmadarDeath);
        yield return new WaitForSeconds(deadTime);
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
            magmadarMoveEnabled = false;
            Stop();
            audiosource.PlayOneShot(magmadarRage);
            animator.SetTrigger("Rage");
            yield return new WaitForSeconds(rageInterval);
            magmadarMoveEnabled = true;
        }
        yield return null;
    }

    IEnumerator Stan()
    {
        if (!staning)
        {
            staning = true;
            magmadarMoveEnabled = false;
            animator.SetTrigger("Stan");
            yield return new WaitForSeconds(stanInterval);
            magmadarMoveEnabled = true;
            staning = false;
        }
        yield return null;
    }

    IEnumerator AttackTimer()
    {
        if (!attacking)
        {

            attacking = true;
            magmadarMoveEnabled = false;
            if (!raging)
                attackNum = 1;
            else
                attackNum = Random.Range(1, 3);
            if(attackNum == 1)
                audiosource.PlayOneShot(magmadarAttack);
            if(attackNum==2)
                audiosource.PlayOneShot(fireSE);
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
                animator.SetTrigger("Attack3");
                if (!destroyed)
                {
                    fire[0].SetActive(true);
                    fire[1].SetActive(true);
                }
                collisionPlayerTimer = 0f;
                yield return new WaitForSeconds(damage3_1Interval);
                if (collisionPlayerTimer >= damage3_1Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage3_2Interval - damage3_1Interval);
                if (collisionPlayerTimer >= damage3_2Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage3_3Interval - damage3_2Interval);
                if (collisionPlayerTimer >= damage3_3Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(damage3_4Interval - damage3_3Interval);
                if (collisionPlayerTimer >= damage3_4Interval)
                    player.PlayerHP -= minPlayerDamage;
                yield return new WaitForSeconds(fireFinishInterval - damage3_4Interval);
                if (!destroyed)
                {
                    fire[0].SetActive(false);
                    fire[1].SetActive(false);
                }
                yield return new WaitForSeconds(attackInterval - fireFinishInterval);
            }
            attacking = false;
            magmadarMoveEnabled = true;
        }
        yield return null;
    }
}