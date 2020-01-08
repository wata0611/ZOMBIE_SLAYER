using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScifiBulletController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int speed = 10;
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] int damage = 10;
    Rigidbody rb;
    GameManager gameManager;
    FirstPersonGunController player;
    ScifiGunController ScifiGun;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        ScifiGun = player.ScifiGun;
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward.normalized * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay(Collision collision)
    {
        string tagName = collision.gameObject.tag;
        if (tagName == "EnemyHead")
        {
            EnemyController enemy = collision.transform.parent.gameObject.GetComponent<EnemyController>();
            if (!enemy.GetDead())
            {
                gameManager.Score += gameManager.headScore;
                enemy.Hp -= damage * 5;
                player.hitCount++;
            }
        }
        if (tagName == "Enemy")
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (!enemy.GetDead())
            {
                gameManager.Score += gameManager.bodyScore;
                enemy.Hp -= damage;
                player.hitCount++;
            }
        }
        if (tagName == "Mutant")
        {
            MutantController mutant = collision.transform.root.gameObject.GetComponent<MutantController>();
            if (!mutant.GetDestroyed())
            {
                gameManager.Score += gameManager.bodyScore;
                mutant.MutantHp -= damage;
                player.hitCount++;
            }
        }
        if (tagName == "MutantHead")
        {
            MutantController mutant = collision.transform.root.gameObject.GetComponent<MutantController>();
            if (!mutant.GetDestroyed())
            {
                gameManager.Score += gameManager.headScore;
                mutant.MutantHp -= damage * 5;
                player.hitCount++;
            }
        }
        if (tagName == "Titan")
        {
            TitanController titan = collision.transform.root.gameObject.GetComponent<TitanController>();
            if (!titan.GetDestroyed())
            {
                gameManager.Score += gameManager.bodyScore;
                titan.TitanHp -= damage;
                player.hitCount++;
            }
        }
        if (tagName == "TitanHead")
        {
            TitanController titan = collision.transform.root.gameObject.GetComponent<TitanController>();
            if (!titan.GetDestroyed())
            {
                gameManager.Score += gameManager.headScore;
                titan.TitanHp -= damage * 5;
                player.hitCount++;
            }
        }
        if (tagName == "Reptile")
        {
            ReptileController reptile = collision.transform.root.gameObject.GetComponent<ReptileController>();
            if (!reptile.GetDestroyed())
            {
                gameManager.Score += gameManager.bodyScore;
                reptile.ReptileHp -= damage;
                player.hitCount++;
            }
        }
        if (tagName == "ReptileHead")
        {
            ReptileController reptile = collision.transform.gameObject.GetComponent<ReptileController>();
            if (!reptile.GetDestroyed())
            {
                gameManager.Score += gameManager.headScore;
                reptile.ReptileHp -= damage * 5;
                player.hitCount++;
            }
        }
        if (tagName == "Magmadar")
        {
            MagmadarController magmadar = collision.transform.root.gameObject.GetComponent<MagmadarController>();
            if (!magmadar.GetDestroyed())
            {
                gameManager.Score += gameManager.bodyScore;
                magmadar.MagmadarHp -= damage;
                player.hitCount++;
            }
        }
        if (tagName == "MagmadarHead")
        {
            MagmadarController magmadar = collision.transform.root.gameObject.GetComponent<MagmadarController>();
            if (!magmadar.GetDestroyed())
            {
                gameManager.Score += gameManager.headScore;
                magmadar.MagmadarHp -= damage * 5;
                player.hitCount++;
            }
        }
        if (tagName == "OrkBerserker")
        {
            OrkberserkerController orkBerserker = collision.transform.root.gameObject.GetComponent<OrkberserkerController>();
            if (!orkBerserker.GetDestroyed())
            {
                gameManager.Score += gameManager.bodyScore;
                orkBerserker.OrkBerserkerHp -= damage;
                player.hitCount++;
            }
        }
        if (tagName == "OrkBerserkerWeapon")
        {
            OrkberserkerController orkBerserker = collision.transform.root.gameObject.GetComponent<OrkberserkerController>();
            if (!orkBerserker.GetDestroyed())
            {
                gameManager.Score += gameManager.bodyScore;
                orkBerserker.OrkBerserkerHp -= damage;
                orkBerserker.OrkBerserkerWeaponHP -= damage;
                player.hitCount++;
            }
        }
        if (tagName == "OrkBerserkerHead")
        {
            OrkberserkerController orkBerserker = collision.transform.root.gameObject.GetComponent<OrkberserkerController>();
            if (!orkBerserker.GetDestroyed())
            {
                gameManager.Score += gameManager.headScore;
                orkBerserker.OrkBerserkerHp -= damage * 5;
                player.hitCount++;
            }
        }
        if (tagName != "ScifiGunObj" && tagName != "Player")
        {
            if (ScifiGun.bulletMode == ScifiGunController.BulletMode.FIRE)
            {
                GameObject childObject = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity) as GameObject;
                childObject.transform.parent = collision.transform;
            }
            else if (ScifiGun.bulletMode == ScifiGunController.BulletMode.ICE)
            {
                GameObject childObject = Instantiate(hitEffectPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f)) as GameObject;
                childObject.transform.parent = collision.transform;
            }
            else if (ScifiGun.bulletMode == ScifiGunController.BulletMode.EXPLOSION)
            {
                GameObject childObject = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity) as GameObject;
                childObject.transform.parent = collision.transform;
            }
            Destroy(this.gameObject);
        }
    }
}
