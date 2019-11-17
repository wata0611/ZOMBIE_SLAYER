using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int maxScore = 99999999;
    [SerializeField] int maxKill = 100;
    [SerializeField] Canvas mainCanvas;
    [SerializeField] Canvas titleCanvas;
    [SerializeField] Text scoreText;
    [SerializeField] Text killText;
    [SerializeField] FirstPersonAIO firstPerson;
    [SerializeField] FirstPersonGunController gunController;
    [SerializeField] Text centerText;
    [SerializeField] EnemySpawner spawner;
    [SerializeField] float waitTime = 2;
    [SerializeField] float waitDeath = 5;
    [SerializeField] Text weaponLevelText;
    [SerializeField] Text roundText;
    [SerializeField] public int bodyScore = 10;
    [SerializeField] public int headScore = 50;

    public AudioClip nextRound;
    public AudioClip bonus;
    public AudioClip playerDeath;
    AudioSource audioSource;

    int score = 0;
    int kill = 0;
    int bonusScore;
    double tmpBonusScore;
    public int round = 1;
    bool gameOver = false;
    bool gameClear = false;
    public bool bossBattle = false;
    FirstPersonGunController player;
    M4A1Controller M4A1;
    LMGController LMG;
    HandGunController HandGun;
    ScifiGunController ScifiGun;
    KnifeController Knife;

    Material m;
    public Material BossSky;
    public Material NormalSky;

    public int Score
    {
        set
        {
            score = Mathf.Clamp(value, 0, maxScore);
            scoreText.text = score.ToString("D6");
        }
        get
        {
            return score;
        }
    }

    public int Kill
    {
        set
        {
            kill = value;
        }

        get
        {
            return kill;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
        bonusScore = 0;
        tmpBonusScore = 0;
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FirstPersonGunController>();
        M4A1 = GameObject.FindGameObjectWithTag("M4A1").GetComponent<M4A1Controller>();
        LMG = GameObject.FindGameObjectWithTag("LMG").GetComponent<LMGController>();
        HandGun = GameObject.FindGameObjectWithTag("HandGun").GetComponent<HandGunController>();
        ScifiGun= GameObject.FindGameObjectWithTag("ScifiGun").GetComponent<ScifiGunController>();
        Knife = GameObject.FindGameObjectWithTag("Knife").GetComponent<KnifeController>();
        StartCoroutine(GameStart());
        m = new Material(RenderSettings.skybox);
        RenderSettings.skybox = m;
    }
    void Update()
    {
        m.SetFloat("_Rotation", Mathf.Repeat(m.GetFloat("_Rotation") + Time.deltaTime, 360f));
        SkyColorChanger();
        if (player.PlayerHP <= 0)
            StartCoroutine(GameOver());
        if (kill >= maxKill)
            StartCoroutine(Rounder());
        killText.text = Kill.ToString("D3") + "/" + maxKill.ToString();
        if(player.haveWeapon1 == FirstPersonGunController.HaveWeapon.M4A1)
            weaponLevelText.text = M4A1.GetUnlockedLevel().ToString();
        else if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.LMG)
            weaponLevelText.text = LMG.GetUnlockedLevel().ToString();
        else if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.HandGun)
            weaponLevelText.text = HandGun.GetUnlockedLevel().ToString();
        else if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.ScifiGun)
            weaponLevelText.text = ScifiGun.GetUnlockedLevel().ToString();
        else if (player.haveWeapon1 == FirstPersonGunController.HaveWeapon.Knife)
            weaponLevelText.text = "-";
        roundText.text = round.ToString();   
    }
    void SkyColorChanger()
    {
        if (bossBattle)
        {
            var c = m.GetColor("_Tint");
            if(c.r <= 1f)
                c.r += Time.deltaTime * 3 / 255f;
            if(c.g >= 84f / 255f)
                c.g -= Time.deltaTime * 3 / 255f;
            if(c.b >= 84f / 255f)
                c.b -= Time.deltaTime * 3 / 255f; ;
            m.SetColor("_Tint", c);
        }
        else
        {
            var c = m.GetColor("_Tint");
            if (c.r >= 128f / 255f)
                c.r -= Time.deltaTime * 3 / 255f;
            if (c.g <= 128f / 255f)
                c.g += Time.deltaTime * 3 / 255f;
            if (c.b <= 128f / 255f)
                c.b += Time.deltaTime * 3/ 255f; ;
            m.SetColor("_Tint", c);
        }
    }

    void InitGame()
    {
        Score = 0;
        Kill = 0;
        firstPerson.playerCanMove = false;
        firstPerson.enableCameraMovement = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gunController.shootEnabled = false;
    }

    public void StartGamebyButton()
    {
        StartCoroutine(GameStart());
    }

    public IEnumerator GameStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        firstPerson.enableCameraMovement = true;
        yield return new WaitForSeconds(waitTime);
        centerText.enabled = true;
        for(int i = 3; i >= 1; i--)
        {
            centerText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        audioSource.PlayOneShot(nextRound);
        centerText.text = "Round" + round.ToString();
        firstPerson.playerCanMove = true;
        firstPerson.enableCameraMovement = true;
        gunController.shootEnabled = true;
        SetSpawners(true);
        yield return new WaitForSeconds(1);
        centerText.text = "";
        centerText.enabled = false;
        yield return null;
    }

    public IEnumerator GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            firstPerson.playerCanMove = false;
            firstPerson.enableCameraMovement = false;
            gunController.shootEnabled = false;
            SetSpawners(false);
            centerText.enabled = true;
            centerText.text = "Game Over\n"+"Score "+ Score.ToString();
            StopEnemies();
            audioSource.PlayOneShot(playerDeath);
            yield return new WaitForSeconds(waitDeath);
            //DestroyEnemies();
            centerText.text = "";
            centerText.enabled = false;
            gameOver = false;
            yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        else
        {
            yield return null;
        }
    }

    public IEnumerator Rounder()
    {
        if (!gameClear)
        {
            gameClear = true;
            round++;
            Kill = 0;
            maxKill = round * 5;
            tmpBonusScore = player.GetHitCount() / player.GetShootCount() * 100;
            bonusScore = (int)(tmpBonusScore) * 10　* round;
            SetSpawners(false);
            yield return new WaitForSeconds(waitTime);
            centerText.enabled = true;
            centerText.text = "Round Bounus "+ bonusScore.ToString() ;
            audioSource.PlayOneShot(bonus);
            Score += bonusScore;
            yield return new WaitForSeconds(1);
            for (int i = 10; i >= 1; i--)
            {
                centerText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
            centerText.text = "Round" + round.ToString();
            firstPerson.playerCanMove = true;
            firstPerson.enableCameraMovement = true;
            gunController.shootEnabled = true;
            audioSource.PlayOneShot(nextRound);
            yield return new WaitForSeconds(1);
            centerText.text = "";
            centerText.enabled = false;
            SetSpawners(true);
            player.hitCount = 0;
            player.shootCount = 0;
            gameClear = false;
        }
        yield return null;
    }

   /** public IEnumerator GameClear()
    {
        if (!gameClear)
        {
            gameClear = true;
            firstPerson.playerCanMove = false;
            firstPerson.enableCameraMovement = true;
            gunController.shootEnabled = false;
            SetSpawners(false);
            centerText.enabled = true;
            centerText.text = "Game Clear!!";
            StopEnemies();
            yield return new WaitForSeconds(waitTime);
            centerText.text = "";
            centerText.enabled = false;
            gameClear = false;
            yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        else
        {
            yield return null;
        }
    }**/

    void StopEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject enemy in enemies)
        {
            EnemyController controller = enemy.GetComponent<EnemyController>();
            controller.moveEnabled = false;
        }
    }

    /*** void DestroyEnemies()
     {
         GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
         foreach (GameObject enemy in enemies)
         {
             Destroy(enemy);
         }
     }
     ***/

    void SetSpawners(bool isEnable)
    {
        spawner.spawnEnabled = isEnable;
        spawner.enemySpawnCount = 0;
        spawner.mutantSpawnCount = 0;
    }

    public int GetmaxKill()
    {
        return maxKill;
    }

}

