using System.Collections;
using UnityEngine;

public class HitEffectController : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(ParticleWorking());
    }

    IEnumerator ParticleWorking()
    {
        yield return new WaitWhile(() => GetComponent<ParticleSystem>().IsAlive(true));
        gameObject.SetActive(false);
    }
}
