using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//裏処理でロードできるようにする。
public class TitleButton : MonoBehaviour
{
    AudioSource audioSource;
    float SEInterval = 4f;
    public AudioClip startSE;
    bool start = false;
    Image image;
    //private AsyncOperation async;

    void Start()
    {
        image= GameObject.FindGameObjectWithTag("TitlePanel").GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        //SceneManager.LoadScene(LoadSceneName, LoadSceneMode.Additive);
        //async = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        //async.allowSceneActivation = false;
    }

    /**public void GameStart()
    {
        StartCoroutine(TitleSETimer());
    }**/

    public void OnClick()
    {
        //async = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        //async.allowSceneActivation = false;
        StartCoroutine(TitleSETimer());
    }

    void Update()
    {
        if (start)
            image.color = Color.Lerp(image.color, Color.black, Time.deltaTime / 1.5f);
    }

    IEnumerator TitleSETimer()
    {
        if (!start)
        {
            start = true;
            transform.parent.gameObject.GetComponent<AudioSource>().enabled = false;
            audioSource.PlayOneShot(startSE);
            yield return new WaitForSeconds(SEInterval);
            SceneManager.LoadSceneAsync("GameScene");
            //async.allowSceneActivation = true;
            //SceneManager.UnloadSceneAsync("Title");
        }
    }

    /**IEnumerator SceneLoad()
    {
        async.allowSceneActivation = true;
        yield return null;
        SceneManager.UnloadScene(LoadSceneName);
        yield return null;
    }**/
}
