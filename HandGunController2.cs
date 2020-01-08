using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandGunController2 : MonoBehaviour
{

    public enum ShootMode { AUTO, SEMIAUTO }
    int count = 0;
    [SerializeField] ShootMode shootMode = ShootMode.SEMIAUTO;
    [SerializeField] public int maxAmmo = 30;
    [SerializeField] public int damage = 1;
    [SerializeField] public float shootInterval = 0.1f;
    [SerializeField] public float shootRange = 50;
    [SerializeField] public float reroadInterval = 0.1f;
    [SerializeField] float minWave = 0.001f;
    [SerializeField] GameObject muzzleFlashPrefab;
    [SerializeField] Vector3 muzzleFlashScale;
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] GameObject hitEffectPrefab2;
    [SerializeField] FirstPersonGunController player;
    [SerializeField] GameManager gameManager;

    public AudioClip shootSound;
    public AudioClip reroadSound;
    public AudioClip shootmodechangeSound;

    AudioSource audioSource;

    public int unlockedLevel = 0;
    public bool got = false;
    public bool dual = false;
    int ammo = 0;
    int magazine = 0;
    float waveH = 0f;
    float waveV = 0f;
    public GameObject muzzleFlash;
    public Transform weaponTransform;
    Transform aimTarget;
    public Transform notAimTarget;
    GameObject hitEffect;
    HandGunController HandGun1;
    Transform weaponOut;


    // Start is called before the first frame update
    void Start()
    {
        InitGun();
        weaponTransform = transform.parent.gameObject.GetComponent<Transform>();
        notAimTarget = GameObject.FindGameObjectWithTag("NotAimTarget3_2").GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
        weaponOut = GameObject.FindGameObjectWithTag("WeaponOut").GetComponent<Transform>();
        HandGun1 = GameObject.FindGameObjectWithTag("HandGun").GetComponent<HandGunController>();
    }

    public int Ammo
    {
        set
        {
            ammo = Mathf.Clamp(value, 0, maxAmmo);
        }

        get
        {
            return ammo;
        }
    }

    public void InitGun()
    {
        Ammo = maxAmmo;
    }

    public bool GetInput()
    {
        switch (shootMode)
        {
            case ShootMode.AUTO:
                return Input.GetMouseButton(0);
            case ShootMode.SEMIAUTO:
                return Input.GetMouseButtonDown(0);
        }
        return false;
    }

    public void Shake()
    {
        Vector3 gunPos = weaponTransform.position;
        waveH = Random.Range(-minWave, minWave);
        waveV = Random.Range(minWave, minWave);
        gunPos.y += waveV;
        gunPos.z += minWave;
        weaponTransform.position = Vector3.MoveTowards(weaponTransform.position, gunPos, Time.deltaTime);
    }

    public void MuzzleFlash()
    {
        if (muzzleFlashPrefab != null)
        {
            if (muzzleFlash != null)
                muzzleFlash.SetActive(true);
            else
            {
                muzzleFlash = Instantiate(muzzleFlashPrefab, transform.position, transform.rotation);
                muzzleFlash.transform.SetParent(gameObject.transform);
                muzzleFlash.transform.localScale = muzzleFlashScale;
            }
        }
    }

    public void shoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        player.shootCount++;
        MuzzleFlash();
        //レイを飛して、ヒットしたオブジェクトの情報を得る。
        if (Physics.Raycast(ray, out hit, shootRange))
        {
            //ヒットエフェクトON
            if (hitEffectPrefab != null)
            {
                if (!HandGun1.explosion)
                {
                    if (hitEffect != null)
                    {
                        hitEffect.transform.position = hit.point;
                        hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                        hitEffect.SetActive(true);
                    }
                    else
                    {
                        hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    }
                }
                else
                    Instantiate(hitEffectPrefab2, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                //Instantiate(hitEffectPrefab, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }
            //敵へのダメージ処理
            string tagName = hit.collider.gameObject.tag;
            if (tagName == "EnemyHead")
            {
                EnemyController enemy = hit.collider.transform.parent.gameObject.GetComponent<EnemyController>();
                if (!enemy.GetDead())
                {
                    gameManager.Score += gameManager.headScore;
                    enemy.Hp -= damage * 5;
                    player.hitCount++;
                }
            }
            if (tagName == "Enemy")
            {
                EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
                if (!enemy.GetDead())
                {
                    gameManager.Score += gameManager.bodyScore;
                    enemy.Hp -= damage;
                    player.hitCount++;
                }
            }
            if (tagName == "Mutant")
            {
                MutantController mutant = hit.collider.transform.root.gameObject.GetComponent<MutantController>();
                if (!mutant.GetDestroyed())
                {
                    gameManager.Score += gameManager.bodyScore;
                    mutant.MutantHp -= damage;
                    player.hitCount++;
                }
            }
            if (tagName == "MutantHead")
            {
                MutantController mutant = hit.collider.transform.root.gameObject.GetComponent<MutantController>();
                if (!mutant.GetDestroyed())
                {
                    gameManager.Score += gameManager.headScore;
                    mutant.MutantHp -= damage * 5;
                    player.hitCount++;
                }
            }
            if (tagName == "Titan")
            {
                TitanController titan = hit.collider.transform.root.gameObject.GetComponent<TitanController>();
                if (!titan.GetDestroyed())
                {
                    gameManager.Score += gameManager.bodyScore;
                    titan.TitanHp -= damage;
                    player.hitCount++;
                }
            }
            if (tagName == "TitanHead")
            {
                TitanController titan = hit.collider.transform.root.gameObject.GetComponent<TitanController>();
                if (!titan.GetDestroyed())
                {
                    gameManager.Score += gameManager.headScore;
                    titan.TitanHp -= damage * 5;
                    player.hitCount++;
                }
            }
            if (tagName == "Reptile")
            {
                ReptileController reptile = hit.collider.transform.root.gameObject.GetComponent<ReptileController>();
                if (!reptile.GetDestroyed())
                {
                    gameManager.Score += gameManager.bodyScore;
                    reptile.ReptileHp -= damage;
                    player.hitCount++;
                }
            }
            if (tagName == "ReptileHead")
            {
                ReptileController reptile = hit.collider.transform.gameObject.GetComponent<ReptileController>();
                if (!reptile.GetDestroyed())
                {
                    gameManager.Score += gameManager.headScore;
                    reptile.ReptileHp -= damage * 5;
                    player.hitCount++;
                }
            }
            if (tagName == "Magmadar")
            {
                MagmadarController magmadar = hit.collider.transform.root.gameObject.GetComponent<MagmadarController>();
                if (!magmadar.GetDestroyed())
                {
                    gameManager.Score += gameManager.bodyScore;
                    magmadar.MagmadarHp -= damage;
                    player.hitCount++;
                }
            }
            if (tagName == "MagmadarHead")
            {
                MagmadarController magmadar = hit.collider.transform.root.gameObject.GetComponent<MagmadarController>();
                if (!magmadar.GetDestroyed())
                {
                    gameManager.Score += gameManager.headScore;
                    magmadar.MagmadarHp -= damage * 5;
                    player.hitCount++;
                }
            }
            if (tagName == "OrkBerserker")
            {
                OrkberserkerController orkBerserker = hit.collider.transform.root.gameObject.GetComponent<OrkberserkerController>();
                if (!orkBerserker.GetDestroyed())
                {
                    gameManager.Score += gameManager.bodyScore;
                    orkBerserker.OrkBerserkerHp -= damage;
                    player.hitCount++;
                }
            }
            if (tagName == "OrkBerserkerWeapon")
            {
                OrkberserkerController orkBerserker = hit.collider.transform.root.gameObject.GetComponent<OrkberserkerController>();
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
                OrkberserkerController orkBerserker = hit.collider.transform.root.gameObject.GetComponent<OrkberserkerController>();
                if (!orkBerserker.GetDestroyed())
                {
                    gameManager.Score += gameManager.headScore;
                    orkBerserker.OrkBerserkerHp -= damage * 5;
                    player.hitCount++;
                }
            }
        }
        Ammo--;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.HandGun && HandGun1.dual)
        {
            weaponTransform.position = Vector3.MoveTowards(weaponTransform.position, notAimTarget.position, Time.deltaTime);
            weaponTransform.rotation = Quaternion.RotateTowards(weaponTransform.rotation, notAimTarget.rotation, 1.0f);
        }
        else
            weaponTransform.position = Vector3.MoveTowards(weaponTransform.position, weaponOut.position, 100f);
    }
}
