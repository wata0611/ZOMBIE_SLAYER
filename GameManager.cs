using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int maxScore = 99999999;
    [SerializeField] int maxKill = 100;
    [SerializeField] int initSensitivityLevel = 10;
    [SerializeField] float maxSensitivity = 8.0f;
    [SerializeField] int sensitivityLevel = 10;
    [SerializeField] Canvas mainCanvas;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] Text scoreText;
    [SerializeField] Text killText;
    [SerializeField] Text totalKillText;
    [SerializeField] FirstPersonAIO firstPerson;
    [SerializeField] FirstPersonGunController player;
    [SerializeField] Text centerText;
    [SerializeField] EnemySpawner spawner;
    [SerializeField] float waitTime = 2;
    [SerializeField] float waitDeath = 5;
    [SerializeField] Text weaponLevelText;
    [SerializeField] Text roundText;
    [SerializeField] public int bodyScore = 10;
    [SerializeField] public int headScore = 50;
    [SerializeField] M4A1Controller M4A1;
    [SerializeField] LMGController LMG;
    [SerializeField] HandGunController HandGun;
    [SerializeField] ScifiGunController ScifiGun;
    [SerializeField] Text sensitivityText;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] bool Debug = false;

    public AudioClip nextRound;
    public AudioClip bonus;
    public AudioClip playerDeath;
    public AudioClip buttonSe;
    public AudioClip menuSe;
    public AudioSource audioSource;

    int score = 0;
    int kill = 0;
    int minSensitivityLevel = 1;
    int maxSensitivityLevel = 20;
    public int totalKill = 0;
    int bonusScore;
    double tmpBonusScore;
    public int round = 1;
    bool gameStart = false;
    public bool gameOver = false;
    bool roundClear = false;
    public bool gameClear = false;
    public bool bossBattle = false;
    public bool lastBossBattle = false;
    bool once = false;
    bool wait = true;

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
        if(!Debug)
            SceneManager.UnloadScene("Title");
        InitGame();
        bonusScore = 0;
        tmpBonusScore = 0;
        audioSource = GetComponent<AudioSource>();
        pauseCanvas.SetActive(false);
        StartCoroutine(GameStart());
        m = new Material(RenderSettings.skybox);
        RenderSettings.skybox = m;
        if (PlayerPrefs.HasKey("SensitivityLevel"))
            sensitivityLevel = PlayerPrefs.GetInt("SensitivityLevel");
        else
            sensitivityLevel = initSensitivityLevel;
        sensitivitySlider.value = sensitivityLevel;
        firstPerson.mouseSensitivity = ((float)sensitivityLevel / maxSensitivityLevel) * maxSensitivity;
        sensitivityText.text = sensitivityLevel.ToString();
    }

    void Update()
    {
        if(!gameClear && !gameOver && !roundClear && !gameStart)
            PauseManager();
        if (player.end && wait)
        {
            wait = false;
            StartCoroutine(SceneChanger());
        }
        if(bossBattle)
            m.SetFloat("_Rotation", Mathf.Repeat(m.GetFloat("_Rotation") + Time.deltaTime * 4, 360f));
        else if(lastBossBattle)
            m.SetFloat("_Rotation", Mathf.Repeat(m.GetFloat("_Rotation") + Time.deltaTime * 8, 360f));
        else
            m.SetFloat("_Rotation", Mathf.Repeat(m.GetFloat("_Rotation") + Time.deltaTime, 360f));
        SkyColorChanger();
        if (player.PlayerHP <= 0 && once == false)
        {
            once = true;
            StartCoroutine(GameOver());
        }
        if (kill >= maxKill)
            StartCoroutine(Rounder());
        killText.text = Kill.ToString("D3") + "/" + maxKill.ToString();
        totalKillText.text = totalKill.ToString("D3");
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
        if (gameClear && once == false)
        {
            once = true;
            StartCoroutine(GameClear());
        }
    }

    void PauseManager()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            print("pause");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseCanvas.SetActive(true);
            if(!roundClear)
                spawner.spawnEnabled = false;
            if (GameObject.FindGameObjectWithTag("Titan") != null)
            {
                GameObject titan = GameObject.FindGameObjectWithTag("Titan");
                titan.GetComponent<NavMeshAgent>().enabled = false;
                titan.GetComponent<Animator>().enabled = false;
                titan.GetComponent<TitanController>().bossBGMSource.Pause();
                titan.GetComponent<TitanController>().enabled = false;
            }
            if (GameObject.FindGameObjectWithTag("Reptile") != null)
            {
                GameObject[] reptiles = GameObject.FindGameObjectsWithTag("Reptile");
                foreach (GameObject reptile in reptiles)
                {
                    if (reptile.transform.root.GetComponent<ReptileController>().bossBGMSource != null)
                        reptile.transform.root.GetComponent<ReptileController>().bossBGMSource.Pause();
                    reptile.transform.root.GetComponent<ReptileController>().enabled = false;
                    reptile.transform.root.GetComponent<NavMeshAgent>().enabled = false;
                    reptile.transform.root.GetComponent<Animator>().enabled = false;
                }
            }
            if (GameObject.FindGameObjectWithTag("Magmadar") != null)
            {
                GameObject magmadar = GameObject.FindGameObjectWithTag("Magmadar").transform.root.gameObject;
                magmadar.GetComponent<NavMeshAgent>().enabled = false;
                magmadar.GetComponent<Animator>().enabled = false;
                magmadar.GetComponent<MagmadarController>().bossBGMSource.Pause();
                magmadar.GetComponent<MagmadarController>().enabled = false;
            }
            if (GameObject.FindGameObjectWithTag("OrkBerserker") != null)
            {
                GameObject ork = GameObject.FindGameObjectWithTag("OrkBerserker").transform.root.gameObject;
                ork.GetComponent<NavMeshAgent>().enabled = false;
                ork.GetComponent<Animator>().enabled = false;
                ork.GetComponent<OrkberserkerController>().bossBGMSource.Pause();
                ork.GetComponent<OrkberserkerController>().enabled = false;
            }
            foreach (GameObject enemy1 in spawner.EnemiesTargetOfPlayer)
            {
                enemy1.GetComponent<EnemyController>().enabled = false;
                enemy1.GetComponent<Animator>().enabled = false;
                enemy1.GetComponent<NavMeshAgent>().enabled = false;
            }
            foreach (GameObject enemy2 in spawner.EnemiesTargetOfWarehouse)
            {
                enemy2.GetComponent<EnemyController>().enabled = false;
                enemy2.GetComponent<Animator>().enabled = false;
                enemy2.GetComponent<NavMeshAgent>().enabled = false;
            }
            foreach (GameObject mutant in spawner.Mutants)
            {
                mutant.GetComponent<MutantController>().enabled = false;
                mutant.GetComponent<Animator>().enabled = false;
                mutant.GetComponent<NavMeshAgent>().enabled = false;
            }
            firstPerson.playerCanMove = false;
            firstPerson.enableCameraMovement = false;
            player.normalBGMSource.Pause();
            player.enabled = false;
            audioSource.PlayOneShot(menuSe);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("unpause");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (GameObject.FindGameObjectWithTag("Titan") != null)
            {
                GameObject titan = GameObject.FindGameObjectWithTag("Titan");
                titan.GetComponent<NavMeshAgent>().enabled = true;
                titan.GetComponent<Animator>().enabled = true;
                titan.GetComponent<TitanController>().enabled = true;
                titan.GetComponent<TitanController>().bossBGMSource.Play();
            }
            if (GameObject.FindGameObjectWithTag("Reptile") != null)
            {
                GameObject[] reptiles = GameObject.FindGameObjectsWithTag("Reptile");
                foreach (GameObject reptile in reptiles)
                {
                    reptile.transform.root.GetComponent<ReptileController>().enabled = true;
                    if (reptile.transform.root.GetComponent<ReptileController>().bossBGMSource != null)
                        reptile.transform.root.GetComponent<ReptileController>().bossBGMSource.Play();
                    reptile.transform.root.GetComponent<Animator>().enabled = true;
                    reptile.transform.root.GetComponent<NavMeshAgent>().enabled = true;
                }
            }
            if (GameObject.FindGameObjectWithTag("Magmadar") != null)
            {
                GameObject magmadar = GameObject.FindGameObjectWithTag("Magmadar").transform.root.gameObject;
                magmadar.GetComponent<NavMeshAgent>().enabled = true;
                magmadar.GetComponent<Animator>().enabled = true;
                magmadar.GetComponent<MagmadarController>().enabled = true;
                magmadar.GetComponent<MagmadarController>().bossBGMSource.Play();
            }
            if (GameObject.FindGameObjectWithTag("OrkBerserker") != null)
            {
                GameObject ork = GameObject.FindGameObjectWithTag("OrkBerserker");
                ork.GetComponent<NavMeshAgent>().enabled = true;
                ork.GetComponent<Animator>().enabled = true;
                ork.GetComponent<OrkberserkerController>().enabled = true;
                ork.GetComponent<OrkberserkerController>().bossBGMSource.Play();
            }
            foreach (GameObject enemy1 in spawner.EnemiesTargetOfPlayer)
            {
                enemy1.GetComponent<NavMeshAgent>().enabled = true;
                enemy1.GetComponent<Animator>().enabled = true;
                enemy1.GetComponent<EnemyController>().enabled = true;
            }
            foreach (GameObject enemy2 in spawner.EnemiesTargetOfWarehouse)
            {
                enemy2.GetComponent<NavMeshAgent>().enabled = true;
                enemy2.GetComponent<Animator>().enabled = true;
                enemy2.GetComponent<EnemyController>().enabled = true;
            }
            foreach (GameObject mutant in spawner.Mutants)
            {
                mutant.GetComponent<NavMeshAgent>().enabled = true;
                mutant.GetComponent<Animator>().enabled = true;
                mutant.GetComponent<MutantController>().enabled = true;
            }
            if (!roundClear)
                spawner.spawnEnabled = true;
            pauseCanvas.SetActive(false);
            firstPerson.playerCanMove = true;
            firstPerson.enableCameraMovement = true;
            player.enabled = true;
            player.normalBGMSource.Play();
        }
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
        if (lastBossBattle)
        {
            var c = m.GetColor("_Tint");
            if (c.r <= 1f)
                c.r += Time.deltaTime * 3 / 255f;
            if (c.g >= 1f / 255f)
                c.g -= Time.deltaTime * 3 / 255f;
            if (c.b >= 1f / 255f)
                c.b -= Time.deltaTime * 3 / 255f; ;
            m.SetColor("_Tint", c);
        }
        if(!bossBattle && !lastBossBattle)
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
        gameStart = true;
        firstPerson.playerCanMove = false;
        firstPerson.enableCameraMovement = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.shootEnabled = false;
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
        player.shootEnabled = true;
        SetSpawners(true);
        yield return new WaitForSeconds(1);
        centerText.text = "";
        centerText.enabled = false;
        spawner.spawnEnabled = true;
        gameStart = false;
        yield return null;
    }

    public IEnumerator GameOver()
    {
        gameOver = true;
        PlayerPrefs.Save();
        firstPerson.playerCanMove = false;
        firstPerson.enableCameraMovement = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.shootEnabled = false;
        SetSpawners(false);
        centerText.enabled = true;
        centerText.text = "Game Over\n" + "Score " + Score.ToString();
        StopEnemies();
        audioSource.PlayOneShot(playerDeath);
        yield return new WaitForSeconds(waitDeath);
        centerText.text = "";
        centerText.enabled = false;
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(Score);
        //yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator GameClear()
    {
        gameClear = true;
        PlayerPrefs.Save();
        firstPerson.playerCanMove = false;
        firstPerson.enableCameraMovement = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.shootEnabled = false;
        SetSpawners(false);
        centerText.enabled = true;
        centerText.text = "Game Clear!!";
        StopEnemies();
        DestroyEnemies();
        yield return new WaitForSeconds(waitTime);
        centerText.text = "";
        centerText.enabled = false;
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(Score);
        //yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public IEnumerator Rounder()
    {
        if (!roundClear)
        {
            roundClear = true;
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
            player.shootEnabled = true;
            audioSource.PlayOneShot(nextRound);
            yield return new WaitForSeconds(1);
            centerText.text = "";
            centerText.enabled = false;
            SetSpawners(true);
            player.hitCount = 0;
            player.shootCount = 0;
            roundClear = false;
        }
        yield return null;
    }

    IEnumerator SceneChanger()
    {
        yield return new WaitForSeconds(4.0f);
        if (gameClear)
        {
            SceneManager.LoadScene("Ending");
        }
        else if (gameOver)
        {
            SceneManager.LoadScene("Title");
        }
    }

    void StopEnemies()
    {
        foreach(GameObject EnemyTargetOfPlayer in spawner.EnemiesTargetOfPlayer)
        {
            EnemyController controller = EnemyTargetOfPlayer.GetComponent<EnemyController>();
            controller.moveEnabled = false;
        }
        foreach (GameObject EnemyTargetOfWarehouse in spawner.EnemiesTargetOfWarehouse)
        {
            EnemyController controller = EnemyTargetOfWarehouse.GetComponent<EnemyController>();
            controller.moveEnabled = false;
        }
        foreach (GameObject Mutant in spawner.Mutants)
        {
            MutantController controller = Mutant.GetComponent<MutantController>();
            controller.mutantMoveEnabled = false;
        }
    }

    void DestroyEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] mutants = GameObject.FindGameObjectsWithTag("Mutant");
        foreach (GameObject enemy in enemies)
            Destroy(enemy);
        foreach (GameObject mutant in mutants)
            Destroy(mutant);
    }

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

    public void MouseSensitivitySliderChanger()
    {
        sensitivityLevel = (int)sensitivitySlider.value;
        firstPerson.mouseSensitivity = ((float)sensitivityLevel / maxSensitivityLevel) * maxSensitivity;
        PlayerPrefs.SetInt("SensitivityLevel", sensitivityLevel);
        sensitivityText.text = sensitivityLevel.ToString();
        audioSource.PlayOneShot(buttonSe);
    }

    public void MouseSensitivityUpButtonChanger()
    {
        if (sensitivityLevel < maxSensitivityLevel)
        {
            sensitivityLevel++;
            sensitivitySlider.value = sensitivityLevel;
            firstPerson.mouseSensitivity = ((float)sensitivityLevel / maxSensitivityLevel) * maxSensitivity;
            PlayerPrefs.SetInt("SensitivityLevel", sensitivityLevel);
            sensitivityText.text = sensitivityLevel.ToString();
            audioSource.PlayOneShot(buttonSe);
        }
    }

    public void MouseSensitivityDownButtonChanger()
    {
        if(sensitivityLevel > minSensitivityLevel)
        {
            sensitivityLevel--;
            sensitivitySlider.value = sensitivityLevel;
            firstPerson.mouseSensitivity = ((float)sensitivityLevel / maxSensitivityLevel) * maxSensitivity;
            PlayerPrefs.SetInt("SensitivityLevel", sensitivityLevel);
            sensitivityText.text = sensitivityLevel.ToString();
            audioSource.PlayOneShot(buttonSe);
        }
    }

}

