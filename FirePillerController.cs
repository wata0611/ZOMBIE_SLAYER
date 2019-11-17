using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePillerController : MonoBehaviour
{
    [SerializeField] float firePillerTime = 61f;
    [SerializeField] int damage = 5;
    [SerializeField] int limit = 120;
    [SerializeField] float damageInterval = 0.5f;
    CapsuleCollider capsuleCollider;
    bool fire = false;
    bool damaging = false;
    FirstPersonGunController player;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!fire)
        {
            fire = true;
            StartCoroutine(FirePillerTimer());
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
            StartCoroutine(Damage());
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
            damaging = false;
    }

    IEnumerator FirePillerTimer()
    {
        yield return new WaitForSeconds(firePillerTime);
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator Damage()
    {
        if (!damaging)
        {
            damaging = true;
            player.PlayerHP -= damage;
            yield return new WaitForSeconds(damageInterval);
            damaging = false;
            yield return null;
        }
    }
}
