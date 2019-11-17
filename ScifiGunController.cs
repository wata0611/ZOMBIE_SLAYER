using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScifiGunController : MonoBehaviour
{

    public enum ShootMode { AUTO, SEMIAUTO }
    public enum BulletMode { EXPLOSION, FIRE, ICE }
    int count = 0;
    [SerializeField] ShootMode shootMode = ShootMode.SEMIAUTO;
    [SerializeField] public BulletMode bulletMode = BulletMode.EXPLOSION;
    [SerializeField] public int maxAmmo = 30;
    [SerializeField] public int maxMagazine = 180;
    [SerializeField] public float shootInterval = 0.1f;
    [SerializeField] public float reroadInterval = 0.1f;
    [SerializeField] Text ammoText;
    [SerializeField] GameObject[] bullet;

    public AudioClip shootSound;
    public AudioClip reroadSound;
    public AudioClip bulletmodechangeSound;
    AudioSource audioSource;

    public int unlockedLevel = 0;
    public bool got = false;
    int ammo = 0;
    int magazine = 0;
    FirstPersonGunController player;
    GameManager gameManager;
    public Transform weaponTransform;
    Transform aimTarget;
    public Transform notAimTarget;


    // Start is called before the first frame update
    void Start()
    {
        InitGun();
        weaponTransform = transform.parent.gameObject.GetComponent<Transform>();
        aimTarget = GameObject.FindGameObjectWithTag("AimTarget4").GetComponent<Transform>();
        notAimTarget = GameObject.FindGameObjectWithTag("NotAimTarget4").GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
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
            ammoText.text = ammo.ToString("D3") + "/" + Magazine.ToString("D3");
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

        switch (shootMode)
        {
            case ShootMode.AUTO:
                return Input.GetMouseButton(0);
            case ShootMode.SEMIAUTO:
                return Input.GetMouseButtonDown(0);
        }
        return false;
    }

    void BulletModeChanger()
    {
        if (Input.GetKeyDown(KeyCode.C) )
        {
            audioSource.PlayOneShot(bulletmodechangeSound);
            if (bulletMode == BulletMode.EXPLOSION)
                bulletMode = BulletMode.FIRE;
            else if(bulletMode == BulletMode.FIRE)
                bulletMode = BulletMode.ICE;
            else if (bulletMode == BulletMode.ICE)
                bulletMode = BulletMode.EXPLOSION;
        }
    }

    public void WeaponManager()
    {
        if (unlockedLevel == 1)
            bulletMode = BulletMode.FIRE;
        if (unlockedLevel == 2)
            bulletMode = BulletMode.ICE;
    }

    void Aim()
    {
        //右クリック長押しでエイムできるようにする
        if (Input.GetMouseButton(1))
        {
            weaponTransform.position = Vector3.MoveTowards(weaponTransform.position, aimTarget.position, Time.deltaTime);
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

    public void shoot()
    {
        if (bulletMode == BulletMode.EXPLOSION)
            Instantiate(bullet[0], transform.position, transform.rotation);
        else if (bulletMode == BulletMode.FIRE)
            Instantiate(bullet[1], transform.position, transform.rotation);
        else if (bulletMode == BulletMode.ICE)
            Instantiate(bullet[2], transform.position, transform.rotation);
        player.shootCount++;
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
    }

    public void Supply()
    {
        if (Magazine < maxMagazine)
        {
            Magazine = maxMagazine;
            Ammo = maxAmmo;
        }
    }

    // Update is called once per frame
    void Update()
    {
        BulletModeChanger();
        if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.ScifiGun)
        {
            Aim();
            ammoText.text = ammo.ToString("D3") + "/" + Magazine.ToString("D3");
        }
    }
}
