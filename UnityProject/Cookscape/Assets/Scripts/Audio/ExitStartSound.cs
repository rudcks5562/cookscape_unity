using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitStartSound : MonoBehaviour
{
    AudioSource m_AudioSource;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        AudioClip clip = SoundsManager.instance.ExitStartSound;

        if ( m_AudioSource != null )
        {
            m_AudioSource.clip = SoundsManager.instance.ExitStartSound;;
            m_AudioSource.Play();
            m_AudioSource.volume = 0.1f;
            m_AudioSource.loop = true;
        }
    }

    public void Stop()
    {
        m_AudioSource.Stop();
    }
}
