using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 30f;
    [SerializeField] float limitPosition = 730f;
    [SerializeField] float appearSpeed = 0.5f;
    [SerializeField] float fadeSpeed = 0.25f;
    [SerializeField] float waitTime1 = 7.0f;
    [SerializeField] float waitTime2 = 7.0f;
    [SerializeField] float waitTime3 = 4.0f;
    [SerializeField] Text clearText;

    bool appearClearText = false;
    bool fadeClearText = false;
    bool inputAnyKey = false;
    bool endScroll = false;

    AudioSource audioSource;
    public AudioClip pushBottun;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= limitPosition)
            transform.position = new Vector2(transform.position.x, transform.position.y + scrollSpeed * Time.deltaTime);
        else
            StartCoroutine(FontChange());
        TextManager();
    }

    void TextManager()
    {
        if (appearClearText)
            clearText.color = Color.Lerp(clearText.color, new Color(1f, 1f, 1f, 1f), appearSpeed * Time.deltaTime);
        if (fadeClearText)
        {
            appearClearText = false;
            clearText.color = Color.Lerp(clearText.color, Color.clear, fadeSpeed * Time.deltaTime);
        }
        if (inputAnyKey && Input.anyKeyDown)
        {
            fadeClearText = true;
            inputAnyKey = false;
            audioSource.PlayOneShot(pushBottun);
            StartCoroutine(LoadTitle());
        }
    }

    IEnumerator FontChange()
    {
        if (!endScroll)
        {
            endScroll = true;
            yield return new WaitForSeconds(waitTime1);
            appearClearText = true;
            yield return new WaitForSeconds(waitTime2);
            appearClearText = false;
            inputAnyKey = true;
        }
    }

    IEnumerator LoadTitle()
    {
        yield return new WaitForSeconds(waitTime3);
        SceneManager.LoadSceneAsync("Title");
    }
}
