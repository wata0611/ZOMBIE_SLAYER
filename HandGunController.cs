using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandGunController : MonoBehaviour
{

    public enum ShootMode { AUTO, SEMIAUTO }
    int count = 0;
    [SerializeField] ShootMode shootMode = ShootMode.SEMIAUTO;
    [SerializeField] public int maxAmmo = 30;
    [SerializeField] public int maxMagazine = 180;
    [SerializeField] public int damage = 1;
    [SerializeField] int unlockedScore = 500;
    [SerializeField] public float shootInterval = 0.1f;
    [SerializeField] public float shootRange = 50;
    [SerializeField] public float reroadInterval = 0.1f;
    [SerializeField] float maxWave = 0.01f;
    [SerializeField] float minWave = 0.001f;
    [SerializeField] GameObject muzzleFlashPrefab;
    [SerializeField] Vector3 muzzleFlashScale;
    [SerializeField] Text ammoText;
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] GameObject hitEffectPrefab2;

    public AudioClip shootSound;
    public AudioClip reroadSound;
    public AudioClip shootmodechangeSound;

    AudioSource audioSource;

    public int unlockedLevel = 0;
    public bool got = false;
    public bool dual = false;
    public bool explosion = false;
    int ammo = 0;
    int magazine = 0;
    float waveH = 0f;
    float waveV = 0f;
    public GameObject muzzleFlash;
    FirstPersonGunController player;
    GameManager gameManager;
    public Transform weaponTransform;
    Transform aimTarget;
    public Transform notAimTarget;
    GameObject hitEffect;
    HandGunController2 HandGun2;
    GameObject HandGun2Obj;


    // Start is called before the first frame update
    void Start()
    {
        InitGun();
        weaponTransform = transform.parent.gameObject.GetComponent<Transform>();
        aimTarget = GameObject.FindGameObjectWithTag("AimTarget3").GetComponent<Transform>();
        notAimTarget = GameObject.FindGameObjectWithTag("NotAimTarget3").GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        HandGun2= GameObject.FindGameObjectWithTag("HandGun2").GetComponent<HandGunController2>();
        HandGun2Obj = GameObject.FindGameObjectWithTag("HandGun2").transform.parent.gameObject;
    }

    public int GetMaxMagazine()
    {
        return maxMagazine;
    }

    public int GetUnlockedLevel()
    {
        return unlockedLevel;
    }

    public int Magazine
    {
        set
        {
            magazine = Mathf.Clamp(value, 0, maxMagazine);
        }

        get
        {
            return magazine;
        }
    }

    public int Ammo
    {
        set
        {
            ammo = Mathf.Clamp(value, 0, maxAmmo);

            //UIの表示を操作
            //テキスト
            if(!dual)
                ammoText.text = ammo.ToString("D3") + "/" + Magazine.ToString("D3");
            else
                ammoText.text = HandGun2.Ammo.ToString("D3") + "/" + ammo.ToString("D3") + "/" + Magazine.ToString("D3");
        }

        get
        {
            return ammo;
        }
    }

    public void InitGun()
    {
        Ammo = maxAmmo;
        Magazine = maxMagazine;
    }

    public bool GetInput()
    {
        if (!dual)
        {
            switch (shootMode)
            {
                case ShootMode.AUTO:
                    return Input.GetMouseButton(0);
                case ShootMode.SEMIAUTO:
                    return Input.GetMouseButtonDown(0);
            }
        }
        else
        {
            switch (shootMode)
            {
                case ShootMode.AUTO:
                    return Input.GetMouseButton(1);
                case ShootMode.SEMIAUTO:
                    return Input.GetMouseButtonDown(1);
            }
        }
        return false;
    }

    public void WeaponManager()
    {
        //将来的にshootRangeのところは武器のブレ補正にする
        if (unlockedLevel == 1)
            maxAmmo += 4;
        if (unlockedLevel == 2)
            shootRange += 10;
        if (unlockedLevel == 3)
            damage++;
        if (unlockedLevel == 4)
            shootRange += 10;
        if (unlockedLevel == 5)
            maxAmmo += 4;
        if (unlockedLevel == 6)
            shootRange += 10;
        if (unlockedLevel == 7)
            damage++;
        if (unlockedLevel == 8)
            maxAmmo += 4;
        if (unlockedLevel == 9)
        {
            dual = true;
            maxMagazine = maxMagazine * 2;
        }
        if (unlockedLevel == 10)
        {
            damage = damage * 2;
            explosion = true;
        }
    }

    void Aim()
    {
        //右クリック長押しでエイムできるようにする
        if (Input.GetMouseButton(1))
        {
            weaponTransform.position = Vector3.MoveTowards(weaponTransform.position, aimTarget.position, Time.deltaTime*3);
            weaponTransform.rotation = Quaternion.RotateTowards(weaponTransform.rotation, aimTarget.rotation, 1.0f);
            //playerCamera.position = Vector3.MoveTowards(playerCamera.position, cameraST.position, Time.deltaTime/4);
        }
        else
        {
            weaponTransform.position = Vector3.MoveTowards(weaponTransform.position, notAimTarget.position, Time.deltaTime);
            weaponTransform.rotation = Quaternion.RotateTowards(weaponTransform.rotation, notAimTarget.rotation, 1.0f);
            //playerCamera.position = Vector3.MoveTowards(playerCamera.position, cameraST.position, Time.deltaTime/4);
        }
        //playerCamera.rotation = Quaternion.RotateTowards(playerCamera.rotation, cameraST.rotation, motionInterval);
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

    void MuzzleFlash()
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
                if (!explosion){
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
                gameManager.Score += gameManager.headScore;
                EnemyController enemy = hit.collider.transform.parent.gameObject.GetComponent<EnemyController>();
                enemy.Hp -= damage * 5;
                player.hitCount++;
            }
            if (tagName == "Enemy")
            {
                gameManager.Score += gameManager.bodyScore;
                EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
                enemy.Hp -= damage;
                player.hitCount++;
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
                TitanController titan = hit.collider.transform.gameObject.GetComponent<TitanController>();
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

    public void Reroad()
    {
        if (Magazine >= maxAmmo)
        {
            Magazine -= maxAmmo - Ammo;
            Ammo = maxAmmo;
            audioSource.PlayOneShot(reroadSound);
        }
        else if (Magazine > 0)
        {
            Ammo += Magazine;
            Magazine = 0;
            audioSource.PlayOneShot(reroadSound);
        }
        if (dual && Magazine >= maxAmmo)
        {
            Magazine -= maxAmmo - HandGun2.Ammo;
            HandGun2.Ammo = maxAmmo;
            audioSource.PlayOneShot(reroadSound);
        }
        else if (dual && Magazine > 0)
        {
            HandGun2.Ammo += Magazine;
            Magazine = 0;
            audioSource.PlayOneShot(reroadSound);
        }
    }

    public void Supply()
    {
        if (Magazine < maxMagazine)
        {
            Magazine = maxMagazine;
            Ammo = maxAmmo;
            if (dual)
                HandGun2.Ammo = maxAmmo;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.HandGun)
        {
            if (!dual)
                Aim();
            else
            {
                weaponTransform.position = Vector3.MoveTowards(weaponTransform.position, notAimTarget.position, Time.deltaTime);
                weaponTransform.rotation = Quaternion.RotateTowards(weaponTransform.rotation, notAimTarget.rotation, 1.0f);
            }
            if (!dual)
                ammoText.text = ammo.ToString("D3") + "/" + Magazine.ToString("D3");
            else
                ammoText.text = HandGun2.Ammo.ToString("D3") + "/" + ammo.ToString("D3") + "/" + Magazine.ToString("D3");
        }
    }
}
