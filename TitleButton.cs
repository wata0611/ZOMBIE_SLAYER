using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour
{
    [SerializeField] float loadStartTimer = 1f;
    AudioSource audioSource;
    float SEInterval = 4f;
    public AudioClip startSE;
    bool start = false;
    Image image;
    private AsyncOperation async;

    void Start()
    {
        image= GameObject.FindGameObjectWithTag("TitlePanel").GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(LoadStart());
    }

    IEnumerator LoadStart()
    {
        yield return new WaitForSeconds(loadStartTimer);
        async = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        async.allowSceneActivation = false;
    }

    public void OnClick()
    {
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
            async.allowSceneActivation = true;
        }
    }
}
