using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetabusSceneBGM : MonoBehaviour
{
    AudioSource m_AudioSource;

    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if ( m_AudioSource != null )
        {
            StartCoroutine("PlayChannelBGM");
        }
    }

    private IEnumerator PlayChannelBGM()
    {
        m_AudioSource.loop = true;
        m_AudioSource.volume = 0.3f;

        AudioClip[] clips = SoundsManager.instance.MetabusSceneBGMs;
        int randomIndex = UnityEngine.Random.Range(0, clips.Length);

        m_AudioSource.clip = clips[randomIndex];
        yield return new WaitForSeconds(1.0f);
        m_AudioSource.Play();
    }

    public void Stop()
    {
        m_AudioSource.Stop();
    }
}
