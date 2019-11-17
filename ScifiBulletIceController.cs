using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScifiBulletIceController : MonoBehaviour
{
    [SerializeField] int damage = 5;
    float lifeTime = 5.0f;
    float hitTimer = 0f;
    bool once = false;
    AudioSource audioSource;
    public AudioClip iceSE;

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
                audioSource.PlayOneShot(iceSE);
                once = true;
            }
            EnemyMoveStop();
        }
    }

    void EnemyMoveStop()
    {
        hitTimer += Time.deltaTime;
        if (transform.parent.tag == "EnemyHead" || transform.parent.tag == "Enemy")
        {
            EnemyController enemy = transform.root.gameObject.GetComponent<EnemyController>();
            enemy.moveEnabled = false;
            if (hitTimer >= lifeTime && enemy.Hp > 0)
            {
                enemy.moveEnabled = true;
                enemy.Hp -= damage;
            }
        }
        if (transform.parent.tag == "Mutant" || transform.parent.tag == "MutantHead")
        {
            MutantController mutant = transform.root.gameObject.GetComponent<MutantController>();
            mutant.mutantMoveEnabled = false;
            if (!mutant.GetDestroyed() && hitTimer >= lifeTime)
            {
                mutant.mutantMoveEnabled = true;
                mutant.MutantHp -= damage;
            }
        }
        if (transform.parent.tag == "Titan" || transform.parent.tag == "TitanHead")
        {
            TitanController titan = transform.root.gameObject.GetComponent<TitanController>();
            if(!titan.GetDestroyed())
                titan.TitanHp -= damage;
            /**titan.titanMoveEnabled = false;
            if (!titan.GetDestroyed() && hitTimer >= lifeTime)
                titan.titanMoveEnabled = true;
        **/
        }
        if (transform.parent.tag == "Reptile" || transform.parent.tag == "ReptileHead")
        {
            ReptileController reptile = transform.root.gameObject.GetComponent<ReptileController>();
            reptile.reptileMoveEnabled = false;
            if (!reptile.GetDestroyed() && hitTimer >= lifeTime)
            {
                reptile.reptileMoveEnabled = true;
                reptile.ReptileHp -= damage;
            }
        }
        if (transform.parent.tag == "Magmadar" || transform.parent.tag == "MagMadarHead")
        {
            MagmadarController magmadar = transform.root.gameObject.GetComponent<MagmadarController>();
            if(!magmadar.GetDestroyed())
                magmadar.MagmadarHp -= damage;
            /**magmadar.magmadarMoveEnabled = false;
            if (!magmadar.GetDestroyed() && hitTimer >= lifeTime)
                magmadar.magmadarMoveEnabled = true;
            **/
        }
        if(transform.parent.tag == "OrkBerserker" || transform.parent.tag == "OrkBerserkerWeapon" || transform.parent.tag == "OrkBerserkerHead")
        {
            OrkberserkerController orkBerserker = transform.root.gameObject.GetComponent<OrkberserkerController>();
            if (!orkBerserker.GetDestroyed())
                orkBerserker.OrkBerserkerHp -= damage;
        }
        ScaleManager();
        if (hitTimer >= lifeTime)
            Destroy(this.gameObject);
    }

    void ScaleManager()
    {
        if (hitTimer >= (lifeTime / 5) * 4 || transform.parent == null)
        {
            transform.localScale = new Vector3(
            transform.localScale.x - Time.deltaTime,
            transform.localScale.y - Time.deltaTime,
            transform.localScale.z - Time.deltaTime);
        }
    }
}
