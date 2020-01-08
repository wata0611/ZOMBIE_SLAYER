using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScifiBulletExplosionController : MonoBehaviour
{
    [SerializeField] int damage = 5;
    SphereCollider sphereCollider;
    bool once = false;
    bool destroyed = false;
    AudioSource audioSource;
    public AudioClip explosionSE;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != null)
        {
            if (!once)
            {
                audioSource.PlayOneShot(explosionSE);
                once = true;
            }
            if (sphereCollider.radius <= 3.0f)
                sphereCollider.radius += 0.5f;
            else
                sphereCollider.enabled = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
            if (enemy.Hp > 0)
                enemy.Hp -= damage;
        }
        if (collider.gameObject.tag == "Mutant")
        {
            MutantController mutant = collider.transform.root.gameObject.GetComponent<MutantController>();
            if (!mutant.GetDestroyed())
                mutant.MutantHp -= damage;
        }
        if (collider.gameObject.tag == "Titan")
        {
            TitanController titan = collider.transform.root.gameObject.GetComponent<TitanController>();
            if (!titan.GetDestroyed())
                titan.TitanHp -= damage;
        }
        if (collider.gameObject.tag == "Reptile")
        {
            ReptileController reptile = collider.transform.root.gameObject.GetComponent<ReptileController>();
            if (!reptile.GetDestroyed())
                reptile.ReptileHp -= damage;
        }
        if (collider.gameObject.tag == "Magmadar")
        {
            MagmadarController magmadar = collider.transform.root.gameObject.GetComponent<MagmadarController>();
            if (!magmadar.GetDestroyed())
                magmadar.MagmadarHp -= damage;
        }
        if (collider.gameObject.tag == "OrkBerserker")
        {
            OrkberserkerController orkBerserker = collider.transform.root.gameObject.GetComponent<OrkberserkerController>();
            if (!orkBerserker.GetDestroyed())
                orkBerserker.OrkBerserkerHp -= damage;
        }
    }
}
