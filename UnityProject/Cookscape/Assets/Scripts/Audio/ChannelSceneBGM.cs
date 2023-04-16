using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelSceneBGM : MonoBehaviour
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
        AudioClip channelSceneBGM = SoundsManager.instance.ChannelSceneBGM;
        m_AudioSource.clip = channelSceneBGM;
        yield return new WaitForSeconds(1.0f);
        m_AudioSource.Play();
    }

    public void Stop()
    {
        m_AudioSource.Stop();
    }
}
