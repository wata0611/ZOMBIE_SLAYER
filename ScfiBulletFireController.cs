using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScfiBulletFireController : MonoBehaviour
{
    [SerializeField] int damage = 10;
    float lifeTime = 3.50f;
    float hitTimer= 0f;
    bool once = false;
    AudioSource audioSource;
    public AudioClip fireSE;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != null)
        {
            if (!once)
            {
                audioSource.PlayOneShot(fireSE);
                once = true;
            }
            Damage();
        }
    }

    void Damage()
    {
        if (transform.parent.tag == "EnemyHead")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= lifeTime / 10)
            {
                EnemyController enemy = transform.parent.transform.parent.gameObject.GetComponent<EnemyController>();
                if(enemy.Hp > 0)
                    enemy.Hp -= damage * 5;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "Enemy")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= lifeTime / 10)
            {
                EnemyController enemy = transform.parent.gameObject.GetComponent<EnemyController>();
                if(enemy.Hp > 0)
                    enemy.Hp -= damage;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "Mutant")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= lifeTime / 10)
            {
                MutantController mutant = transform.root.gameObject.GetComponent<MutantController>();
                if (!mutant.GetDestroyed())
                        mutant.MutantHp -= damage;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "MutantHead")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= lifeTime / 10)
            {
                MutantController mutant = transform.root.gameObject.GetComponent<MutantController>();
                if (!mutant.GetDestroyed())
                    mutant.MutantHp -= damage * 5;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "Titan")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= lifeTime / 10)
            {
                TitanController titan = transform.root.transform.gameObject.GetComponent<TitanController>();
                if (!titan.GetDestroyed())
                    titan.TitanHp -= damage;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "TitanHead")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= lifeTime / 10)
            {
                TitanController titan = transform.root.gameObject.GetComponent<TitanController>();
                if (!titan.GetDestroyed())
                    titan.TitanHp -= damage * 5;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "Reptile")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= lifeTime / 10)
            {
                ReptileController reptile = transform.root.gameObject.GetComponent<ReptileController>();
                if (!reptile.GetDestroyed())
                    reptile.ReptileHp -= damage;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "ReptileHead")
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= lifeTime / 10)
            {
                ReptileController reptile = transform.parent.transform.gameObject.GetComponent<ReptileController>();
                if (!reptile.GetDestroyed())
                    reptile.ReptileHp -= damage * 5;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "Magmadar")
        {
            hitTimer += Time.deltaTime;
            MagmadarController magmadar = transform.root.gameObject.GetComponent<MagmadarController>();
            if (hitTimer >= lifeTime / 10)
            {
                if (!magmadar.GetDestroyed())
                    magmadar.MagmadarHp -= damage;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "MagmadarHead")
        {
            hitTimer += Time.deltaTime;
            MagmadarController magmadar = transform.root.gameObject.GetComponent<MagmadarController>();
            if (hitTimer >= lifeTime / 10)
            {
                if (!magmadar.GetDestroyed())
                    magmadar.MagmadarHp -= damage * 5;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "OrkBerserker")
        {
            hitTimer += Time.deltaTime;
            OrkberserkerController orkBerserker = transform.root.gameObject.GetComponent<OrkberserkerController>();
            if (hitTimer >= lifeTime / 10)
            {
                if(!orkBerserker.GetDestroyed())
                    orkBerserker.OrkBerserkerHp -= damage;
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "OrkBerserkerWeapon")
        {
            hitTimer += Time.deltaTime;
            OrkberserkerController orkBerserker = transform.root.gameObject.GetComponent<OrkberserkerController>();
            if (hitTimer >= lifeTime / 10)
            {
                if (!orkBerserker.GetDestroyed())
                {
                    orkBerserker.OrkBerserkerHp -= damage;
                    orkBerserker.OrkBerserkerWeaponHP -= damage;
                }
                hitTimer = 0f;
            }
        }
        if (transform.parent.tag == "OrkBerserkerHead")
        {
            hitTimer += Time.deltaTime;
            OrkberserkerController orkBerserker = transform.root.gameObject.GetComponent<OrkberserkerController>();
            if (hitTimer >= lifeTime / 10)
            {
                if (!orkBerserker.GetDestroyed())
                    orkBerserker.OrkBerserkerHp -= damage * 5;
                hitTimer = 0f;
            }
        }
    }
}
