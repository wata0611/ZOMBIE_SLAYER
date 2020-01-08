using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonGunController : MonoBehaviour
{
    public enum HaveWeapon { M4A1, Knife, HandGun, LMG, ScifiGun }

    public bool shootEnabled = true;
    public bool shootEnabled2 = true;

    [SerializeField] public HaveWeapon haveWeapon1= HaveWeapon.M4A1;
    [SerializeField] public HaveWeapon haveWeapon2 = HaveWeapon.Knife;
    [SerializeField] int maxPlayerHP = 50;
    [SerializeField] int supplyScore = 1000;
    [SerializeField] int repairScore = 50;
    [SerializeField] int heelScore = 50;
    [SerializeField] int unlockedScore = 500;
    [SerializeField] int enhancePlayerHPScore = 2500;
    [SerializeField] int enhancePlayerWalkSpeedScore = 2500;
    [SerializeField] int enhancePlayerAutoHeelScore = 2500;
    [SerializeField] float supplyInterval = 0.1f;
    [SerializeField] float repairInterval = 0.5f;
    [SerializeField] float heelInterval = 0.5f;
    [SerializeField] float unlockedInterval = 0.5f;
    [SerializeField] float enhanceInterval = 0.5f;
    [SerializeField] float beatSpeed = 0.5f;
    [SerializeField] float beatSoundTime = 7.0f;
    [SerializeField] float motionInterval = 1.0f;
    [SerializeField] float switchInterval = 1.0f;
    [SerializeField] Text ammoText;
    [SerializeField] Image playerHPGauge;
    [SerializeField] Text reroadText;
    [SerializeField] Text descriptionText;
    [SerializeField] Image damageEffect;
    [SerializeField] FirstPersonAIO firstPersonAIO;
    [SerializeField] GameManager gameManager;
    [SerializeField] WarehouseController warehouse;
    [SerializeField] public M4A1Controller M4A1;
    [SerializeField] public LMGController LMG;
    [SerializeField] HandGunController HandGun;
    [SerializeField] HandGunController2 HandGun2;
    [SerializeField] public ScifiGunController ScifiGun;
    [SerializeField] public GameObject M4A1Obj;
    [SerializeField] public GameObject LMGObj;
    [SerializeField] public GameObject HandGunObj;
    [SerializeField] public GameObject HandGun2Obj;
    [SerializeField] public GameObject ScifiGunObj;
    [SerializeField] public GameObject Knife;
    [SerializeField] Image[] reticle;

    public AudioClip repairSound;
    public AudioClip supplySound;
    public AudioClip heelSound;
    public AudioClip upgradeSound;
    public AudioClip beatSound;
    public AudioClip playerDamgeVoice;
    public AudioClip getKey;
    public AudioClip getSphere;
    public AudioClip close;
    public AudioSource audioSource;
    public AudioSource normalBGMSource;

    public bool shooting = false;
    public bool shooting2 = false;
    bool supplying = false;
    bool reroading = false;
    bool repairing = false;
    bool heeling = false;
    bool unlocked = false;
    bool enhancedPlayerHP = false;
    bool enhancedPlayerWalkSpeed = false;
    bool enhancedPlayerAutoHeel = false;
    bool beat = false;
    bool motionRight = false;
    bool motionLeft = false;
    bool movingWeapon1 = false;
    bool movingWeapon2 = false;
    bool slowWalkSpeed = false;
    public bool getRedKey = false;
    public bool getYellowKey = false;
    public bool getBlueKey = false;
    public int getKeyCount = 0;
    public int heelLevel = 1;
    public int repairLevel = 1;
    public bool getTitanSphere = false;
    public bool getReptileSphere = false;
    public bool getMagmadarSphere = false;
    public bool reptileKilled = false;
    public bool gotM4A1 = false;
    public bool gotLMG = false;
    public bool gotScifiGun = false;
    public bool end = false;
    int playerHP = 0;
    int tmpPlayerHP = 0;
    int tmpHeelLevel = 0;
    int tmpRepairLevel = 0;
    int motionCount = 1;
    public double shootCount;
    public double hitCount;
    float shootRange = 0f;
    float moveStep = 0f;
    float rotateStep = 0f;
    float beatStep = 0f;
    float playerAutoHeelTimer = 0f;
    float playerAutoHeelTime = 5f;
    float initFirstPersonWalkSpeed;
    float reticleRadius;
    Transform moveRight;
    Transform moveLeft;
    Transform playerCamera;
    Transform cameraST;
    Transform weaponOut;
    Transform knifeStart;

    public int GetSupplyScore()
    {
        return supplyScore;
    }

    public int GetRepairScore()
    {
        return repairScore;
    }

    public int GetHeelScore()
    {
        return heelScore;
    }

    public string GetUnlockedScore()
    {
        switch (haveWeapon1) {
            case HaveWeapon.M4A1:
                if (M4A1.unlockedLevel < 10)
                    return ((M4A1.unlockedLevel + 1) * 500).ToString();
                else
                    return "Max Level";
            case HaveWeapon.LMG:
                if (LMG.unlockedLevel < 10)
                    return ((LMG.unlockedLevel + 1) * 500).ToString();
                else
                    return "Max Level";
            case HaveWeapon.HandGun:
                if (HandGun.unlockedLevel < 10)
                    return ((HandGun.unlockedLevel + 1) * 500).ToString();
                else
                    return "Max Level";
            case HaveWeapon.ScifiGun:
                return "Max Level";
            case HaveWeapon.Knife:
                return "Max Level";
        }
        return "";
    }

    public double GetShootCount()
    {
        return shootCount;
    }

    public double GetHitCount()
    {
        return hitCount;
    }

    public int GetEnhancePlayerHPScore()
    {
        return enhancePlayerHPScore;
    }

    public int GetEnhancePlayerWalkSpeedScore()
    {
        return enhancePlayerWalkSpeedScore;
    }

    public int GetEnhancePlayerAutoHeelScore()
    {
        return enhancePlayerAutoHeelScore;
    }

    public int PlayerHP
    {
        set
        {
            playerHP = Mathf.Clamp(value, 0, maxPlayerHP);
            if (playerHP <= 0)
                Knife.GetComponent<KnifeController>().attackEnabled = false;
            //UIの表示を操作
            //テキスト

            //ゲージ
            float scaleX = (float)playerHP / maxPlayerHP;
            playerHPGauge.rectTransform.localScale = new Vector3(scaleX, 1, 1);

        }

        get
        {
            return playerHP;
        }
    }

    public int GetmaxPlayerHP()
    {
        return maxPlayerHP;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerHP = maxPlayerHP;
        beatStep = beatSpeed * Time.deltaTime;

        shootCount = 0;
        hitCount = 0;
        tmpPlayerHP = maxPlayerHP;
        tmpHeelLevel = heelLevel;
        tmpRepairLevel = repairLevel;
        initFirstPersonWalkSpeed = firstPersonAIO.walkSpeed;
        audioSource = GetComponent<AudioSource>();
        moveRight = GameObject.FindGameObjectWithTag("MoveRight").GetComponent<Transform>();
        moveLeft = GameObject.FindGameObjectWithTag("MoveLeft").GetComponent<Transform>();
        cameraST = GameObject.FindGameObjectWithTag("CameraPosition").GetComponent<Transform>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        normalBGMSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        weaponOut = GameObject.FindGameObjectWithTag("WeaponOut").GetComponent<Transform>();
        knifeStart= GameObject.FindGameObjectWithTag("KnifeStart").GetComponent<Transform>();
        M4A1Obj.SetActive(false);
        LMGObj.SetActive(false);
        HandGunObj.SetActive(false);
        HandGun2Obj.SetActive(false);
        ScifiGunObj.SetActive(false);
        Knife.SetActive(false);
        ObjSetActiveManager();
    }

    void ObjSetActiveManager()
    {
        if (haveWeapon1 == HaveWeapon.M4A1 || haveWeapon2 == HaveWeapon.M4A1)
            M4A1Obj.SetActive(true);
        if(haveWeapon1 == HaveWeapon.HandGun || haveWeapon2 == HaveWeapon.HandGun)
        {
            HandGunObj.SetActive(true);
            HandGun2Obj.SetActive(true);
        }
        if (haveWeapon1 == HaveWeapon.LMG || haveWeapon2 == HaveWeapon.LMG)
            LMGObj.SetActive(true);
        if (haveWeapon1 == HaveWeapon.ScifiGun || haveWeapon2 == HaveWeapon.ScifiGun)
            ScifiGunObj.SetActive(true);
        if(haveWeapon1 == HaveWeapon.Knife || haveWeapon2 == HaveWeapon.Knife)
            Knife.SetActive(true);
    }

    IEnumerator BeatSoundTimer()
    {
        if (!beat)
        {
            beat = true;
            audioSource.PlayOneShot(beatSound);
            yield return new WaitForSeconds(beatSoundTime);
            beat = false;
        }
    }

    void DamageEffect()
    {
        if (tmpPlayerHP < PlayerHP && heeling)
        {
            damageEffect.color = new Color(10f / 255f, 128f / 255f, 44f / 255f, 90f / 255f);
            tmpPlayerHP = PlayerHP;
        }
        if (tmpPlayerHP > PlayerHP)
        {
            if (PlayerHP > maxPlayerHP / 5)
                damageEffect.color = new Color(128f / 255f, 10f / 255f, 10f / 255f, 90f / 255f);
            tmpPlayerHP = PlayerHP;
            audioSource.PlayOneShot(playerDamgeVoice);
        }

        if (tmpHeelLevel < heelLevel)
        {
            damageEffect.color = new Color(97f / 255f, 0f / 255f, 255f / 255f, 90f / 255f);
            tmpHeelLevel = heelLevel;
        }

        if (tmpRepairLevel < repairLevel)
        {
            damageEffect.color = new Color(97f / 255f, 0f / 255f, 255f / 255f, 90f / 255f);
            tmpRepairLevel = repairLevel;
        }
        if (end)
            damageEffect.color = Color.Lerp(damageEffect.color, new Color(0f, 0f, 0f, 1f), Time.deltaTime);

        else if (PlayerHP <= maxPlayerHP / 5)
        {
            //BeatSoundTimer();
            damageEffect.color = new Color(128f / 255f, 10f / 255f, 10f / 255f, Mathf.Sin(2 * Mathf.PI / 3 * Time.time) * (120f/ 255f));
        }

        else
            damageEffect.color = Color.Lerp(damageEffect.color, Color.clear, Time.deltaTime);
    }

    void MovingWeapon1()
    {
        if (haveWeapon2 == HaveWeapon.M4A1)
            M4A1.weaponTransform.position = Vector3.MoveTowards(M4A1.weaponTransform.position, weaponOut.position, Time.deltaTime*2);
        if (haveWeapon2 == HaveWeapon.LMG)
            LMG.weaponTransform.position = Vector3.MoveTowards(LMG.weaponTransform.position, weaponOut.position, Time.deltaTime * 2);
        if(haveWeapon2==HaveWeapon.HandGun)
            HandGun.weaponTransform.position = Vector3.MoveTowards(HandGun.weaponTransform.position, weaponOut.position, Time.deltaTime * 2);
        if (haveWeapon2 == HaveWeapon.ScifiGun)
            ScifiGun.weaponTransform.position = Vector3.MoveTowards(ScifiGun.weaponTransform.position, weaponOut.position, Time.deltaTime * 2);
        else if (haveWeapon2 == HaveWeapon.Knife)
            Knife.transform.position = Vector3.MoveTowards(Knife.transform.position, weaponOut.position, Time.deltaTime);
    }
    void MovingWeapon2()
    {
        if (haveWeapon1 == HaveWeapon.Knife)
        {
            Knife.transform.position = Vector3.MoveTowards(Knife.transform.position, knifeStart.position, Time.deltaTime);
            Knife.transform.rotation = Quaternion.RotateTowards(Knife.transform.rotation, knifeStart.rotation, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DamageEffect();
        WalkSpeedChanger();
        //MoveReticle();
        if (movingWeapon1)
            MovingWeapon1();
        if (movingWeapon2)
            MovingWeapon2();
        Shoot();
        if (enhancedPlayerAutoHeel)
        {
            if (playerAutoHeelTime <= playerAutoHeelTimer && PlayerHP < maxPlayerHP)
            {
                PlayerHP++;
                playerAutoHeelTimer = 0f;
            }
            playerAutoHeelTimer += Time.deltaTime;
        }
        Reroad();
        if (Input.GetKeyDown(KeyCode.LeftShift))
            StartCoroutine(SwitchTimer());
        
    }

    void MoveReticle()
    {
        if (haveWeapon1 == HaveWeapon.M4A1)
            StartCoroutine(ReroadTimer());
        else if (haveWeapon1 == HaveWeapon.LMG)
            StartCoroutine(ReroadTimer());
        else if (haveWeapon1 == HaveWeapon.HandGun)
        {
            if (slowWalkSpeed)
            {
                for (int i = 0; i < reticle.Length; i++)
                {
                    reticle[i].transform.localPosition = Vector3.MoveTowards(reticle[i].transform.localPosition, new Vector3(0, 0, 0), Time.deltaTime * 50);
                }
                if (reticle[0].transform.localPosition.y < 5.0f)
                {
                    for (int i = 0; i < reticle.Length; i++)
                        reticle[i].enabled = false;
                }

            }
            else if (!slowWalkSpeed)
            {
                if (!reticle[0].enabled)
                {
                    for (int i = 0; i < reticle.Length; i++)
                    {
                        reticle[i].enabled = true;
                    }
                }
                reticle[0].transform.localPosition = Vector3.MoveTowards(reticle[0].transform.localPosition, new Vector3(0, HandGun.notAimReticleRadius + 5f, 0), Time.deltaTime*50);
                reticle[1].transform.localPosition = Vector3.MoveTowards(reticle[1].transform.localPosition, new Vector3(0, -HandGun.notAimReticleRadius - 5f, 0), Time.deltaTime*50);
                reticle[2].transform.localPosition = Vector3.MoveTowards(reticle[2].transform.localPosition, new Vector3(HandGun.notAimReticleRadius + 5f, 0, 0), Time.deltaTime*50);
                reticle[3].transform.localPosition = Vector3.MoveTowards(reticle[3].transform.localPosition, new Vector3(-HandGun.notAimReticleRadius - 5f, 0, 0), Time.deltaTime*50);
            }
        }
        else if (haveWeapon1 == HaveWeapon.ScifiGun)
            StartCoroutine(ReroadTimer());
    }

    void WalkSpeedChanger()
    {
        if (Input.GetMouseButtonDown(1) && !slowWalkSpeed) {
            slowWalkSpeed = true;
            firstPersonAIO.walkSpeed = firstPersonAIO.walkSpeed / 2;
        }
        else if(Input.GetMouseButtonUp(1) && slowWalkSpeed)
        {
            slowWalkSpeed = false;
            firstPersonAIO.walkSpeed = initFirstPersonWalkSpeed;
        }
    }

    void Reroad()
    {
        if (haveWeapon1 == HaveWeapon.M4A1 && M4A1.Ammo < M4A1.maxAmmo && M4A1.Magazine > 0 && Input.GetKey(KeyCode.R))
            StartCoroutine(ReroadTimer());
        else if (haveWeapon1 == HaveWeapon.LMG && LMG.Ammo < LMG.maxAmmo && LMG.Magazine > 0 && Input.GetKey(KeyCode.R))
            StartCoroutine(ReroadTimer());
        else if (haveWeapon1 == HaveWeapon.HandGun && HandGun.Ammo < HandGun.maxAmmo && HandGun.Magazine > 0 && Input.GetKey(KeyCode.R))
            StartCoroutine(ReroadTimer());
        else if(haveWeapon1 == HaveWeapon.ScifiGun && ScifiGun.Ammo < ScifiGun.maxAmmo && ScifiGun.Magazine > 0 && Input.GetKey(KeyCode.R))
            StartCoroutine(ReroadTimer());
    }

    void Shoot()
    {
        if (shootEnabled && haveWeapon1 == HaveWeapon.M4A1 && M4A1.Ammo > 0 && M4A1.GetInput())
            StartCoroutine(ShootTimer());
        else if (shootEnabled && haveWeapon1 == HaveWeapon.LMG && LMG.Ammo > 0 && LMG.GetInput())
            StartCoroutine(ShootTimer());
        else if (shootEnabled && haveWeapon1 == HaveWeapon.HandGun && HandGun.Ammo > 0)
        {
            if (HandGun.GetInput())
                StartCoroutine(ShootTimer());
            if (HandGun.dual && HandGun2.Ammo > 0 && HandGun2.GetInput())
                StartCoroutine(ShootTimer2());
        }
        else if(shootEnabled && haveWeapon1 == HaveWeapon.ScifiGun && ScifiGun.Ammo > 0 && ScifiGun.GetInput())
            StartCoroutine(ShootTimer());
    }

    IEnumerator ShootTimer()
    {
        if (!shooting)
        {
            shooting = true;
            if (haveWeapon1 == HaveWeapon.M4A1)
            {
                shoot();
                audioSource.PlayOneShot(M4A1.shootSound);
                yield return new WaitForSeconds(M4A1.shootInterval);
                if (M4A1.muzzleFlash != null)
                    M4A1.muzzleFlash.SetActive(false);
                M4A1.Shake();
            }

            if (haveWeapon1 == HaveWeapon.LMG)
            {
                shoot();
                audioSource.PlayOneShot(LMG.shootSound);
                yield return new WaitForSeconds(LMG.shootInterval);
                if (LMG.muzzleFlash != null)
                    LMG.muzzleFlash.SetActive(false);
                LMG.Shake();
            }

            if (haveWeapon1 == HaveWeapon.HandGun)
            {
                shoot();
                audioSource.PlayOneShot(HandGun.shootSound);
                yield return new WaitForSeconds(HandGun.shootInterval);
                if (HandGun.muzzleFlash != null)
                    HandGun.muzzleFlash.SetActive(false);
                HandGun.Shake();
            }

            if (haveWeapon1 == HaveWeapon.ScifiGun)
            {
                shoot();
                audioSource.PlayOneShot(ScifiGun.shootSound);
                yield return new WaitForSeconds(ScifiGun.shootInterval);
            }
            shooting = false;
        }
        else
            yield return null;
    }

    IEnumerator ShootTimer2()
    {
        if (!shooting2)
        {
            shooting2 = true;
            if (haveWeapon1 == HaveWeapon.HandGun)
            {
                HandGun2.MuzzleFlash();
                HandGun2.Shake();
                audioSource.PlayOneShot(HandGun2.shootSound);
                yield return new WaitForSeconds(HandGun2.shootInterval);
                if (HandGun2.muzzleFlash != null)
                    HandGun2.muzzleFlash.SetActive(false);
                HandGun2.shoot();
            }
            shooting2 = false;
        }
        else
            yield return null;
    }

    IEnumerator SwitchTimer()
    {
        if (!movingWeapon1 && !movingWeapon2)
        {
            shootEnabled = false;
            Knife.gameObject.GetComponent<KnifeController>().attackEnabled = false;
            movingWeapon1 = true;
            HaveWeapon tmp = haveWeapon1;
            haveWeapon1 = haveWeapon2;
            haveWeapon2 = tmp;
            reroadText.enabled = true;
            reroadText.text = "weapon changing";
            yield return new WaitForSeconds(switchInterval);
            movingWeapon1 = false;
            movingWeapon2 = true;
            yield return new WaitForSeconds(switchInterval);
            movingWeapon2 = false;
            reroadText.enabled = false;
            if(haveWeapon1 == HaveWeapon.Knife)
                Knife.gameObject.GetComponent<KnifeController>().attackEnabled = true;
            shootEnabled = true;
        }
    }

    IEnumerator ReroadTimer()
    {
        if (!reroading)
        {
            reroading = true;
            shootEnabled = false;
            reroadText.enabled = true;
            reroadText.text = "now reroading";
            if (haveWeapon1 == HaveWeapon.M4A1)
            {
                M4A1.Reroad();
                yield return new WaitForSeconds(M4A1.reroadInterval);
            }
            else if (haveWeapon1 == HaveWeapon.LMG)
            {
                LMG.Reroad();
                yield return new WaitForSeconds(LMG.reroadInterval);
            }
            else if (haveWeapon1 == HaveWeapon.HandGun)
            {
                HandGun.Reroad();
                yield return new WaitForSeconds(HandGun.reroadInterval);
            }
            else if (haveWeapon1 == HaveWeapon.ScifiGun)
            {
                ScifiGun.Reroad();
                yield return new WaitForSeconds(ScifiGun.reroadInterval);
            }
            reroadText.enabled = false;
            shootEnabled = true;
            reroading = false;
        }
        yield return null;
    }

    public IEnumerator SupplyTimer()
    {
        if (!supplying)
        {
            supplying = true;
            if (haveWeapon1 == HaveWeapon.M4A1 && M4A1.Magazine < M4A1.maxMagazine)
            {
                M4A1.Supply();
                gameManager.Score -= supplyScore;
                audioSource.PlayOneShot(supplySound);
                yield return new WaitForSeconds(supplyInterval);
            }
            else if (haveWeapon1 == HaveWeapon.LMG && LMG.Magazine < LMG.maxMagazine)
            {
                LMG.Supply();
                gameManager.Score -= supplyScore;
                audioSource.PlayOneShot(supplySound);
                yield return new WaitForSeconds(supplyInterval);
            }
            else if (haveWeapon1 == HaveWeapon.HandGun && HandGun.Magazine < HandGun.maxMagazine)
            {
                HandGun.Supply();
                gameManager.Score -= supplyScore;
                audioSource.PlayOneShot(supplySound);
                yield return new WaitForSeconds(supplyInterval);
            }
            else if (haveWeapon1 == HaveWeapon.ScifiGun && ScifiGun.Magazine < ScifiGun.maxMagazine)
            {
                ScifiGun.Supply();
                gameManager.Score -= supplyScore;
                audioSource.PlayOneShot(supplySound);
                yield return new WaitForSeconds(supplyInterval);
            }
            supplying = false;
        }
        yield return null;
    }

    public IEnumerator RepairTimer()
    {
        if (!repairing)
        {
            repairing = true;
            warehouse.WarehouseHP += 5 * repairLevel;
            gameManager.Score -= repairScore;
            audioSource.PlayOneShot(repairSound);
            yield return new WaitForSeconds(repairInterval);
            repairing = false;   
        }
    }

    public IEnumerator HeelTimer()
    {
        if (!heeling)
        {
            heeling = true;
            PlayerHP += 1 * heelLevel;
            gameManager.Score -= heelScore;
            audioSource.PlayOneShot(heelSound);
            yield return new WaitForSeconds(heelInterval);
            heeling = false;   
        }
    }

    public IEnumerator EnhancePlayerHPTimer()
    {
        if (!enhancedPlayerHP)
        {
            enhancedPlayerHP = true;
            maxPlayerHP = maxPlayerHP * 2;
            PlayerHP = maxPlayerHP;
            tmpPlayerHP = PlayerHP;
            gameManager.Score -= enhancePlayerHPScore;
            audioSource.PlayOneShot(heelSound);
            damageEffect.color = new Color(222f / 255f, 244f / 255f, 135f / 255f, 150f / 255f);
            yield return new WaitForSeconds(enhanceInterval);
        }
    }

    public IEnumerator EnhancePlayerWalkSpeedTimer()
    {
        if (!enhancedPlayerWalkSpeed)
        {
            enhancedPlayerWalkSpeed = true;
            firstPersonAIO.walkSpeed += 2f;
            initFirstPersonWalkSpeed = firstPersonAIO.walkSpeed;
            gameManager.Score -= enhancePlayerWalkSpeedScore;
            audioSource.PlayOneShot(heelSound);
            damageEffect.color = new Color(222f / 255f, 244f / 255f, 135f / 255f, 150f / 255f);
            yield return new WaitForSeconds(enhanceInterval);
        }
    }

    public IEnumerator EnhancePlayerAutoHeelTimer()
    {
        if (!enhancedPlayerAutoHeel)
        {
            enhancedPlayerAutoHeel = true;
            gameManager.Score -= enhancePlayerAutoHeelScore;
            audioSource.PlayOneShot(heelSound);
            damageEffect.color = new Color(222f / 255f, 244f / 255f, 135f / 255f, 150f / 255f);
            yield return new WaitForSeconds(enhanceInterval);
        }
    }

    public IEnumerator UnlockedTimer()
    {
        if (!unlocked)
        {
            unlocked = true;
            if (haveWeapon1 == HaveWeapon.M4A1 && M4A1.unlockedLevel < 10 && unlockedScore * (M4A1.unlockedLevel + 1) <= gameManager.Score)
            {
                M4A1.unlockedLevel++;
                gameManager.Score -= unlockedScore * M4A1.unlockedLevel;
                M4A1.WeaponManager();
                audioSource.PlayOneShot(upgradeSound);
            }
            if (haveWeapon1 == HaveWeapon.LMG && LMG.unlockedLevel < 10 && unlockedScore * (LMG.unlockedLevel + 1) <= gameManager.Score)
            {
                LMG.unlockedLevel++;
                gameManager.Score -= unlockedScore * LMG.unlockedLevel;
                LMG.WeaponManager();
                audioSource.PlayOneShot(upgradeSound);
            }
            if (haveWeapon1 == HaveWeapon.HandGun && HandGun.unlockedLevel < 10 && unlockedScore * (HandGun.unlockedLevel + 1) <= gameManager.Score)
            {
                HandGun.unlockedLevel++;
                gameManager.Score -= unlockedScore * HandGun.unlockedLevel;
                HandGun.WeaponManager();
                audioSource.PlayOneShot(upgradeSound);
            }
            yield return new WaitForSeconds(unlockedInterval);
            unlocked = false;
        }
    }

    void shoot()
    {
        if (haveWeapon1 == HaveWeapon.M4A1)
            M4A1.shoot();
        else if (haveWeapon1 == HaveWeapon.LMG)
            LMG.shoot();
        else if (haveWeapon1 == HaveWeapon.HandGun)
            HandGun.shoot();
        else if (haveWeapon1 == HaveWeapon.ScifiGun)
            ScifiGun.shoot();
    }
}