using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefEncounterSound : MonoBehaviour
{
    AudioSource m_AudioSource;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        AudioClip clip = SoundsManager.instance.ChefBreathSound;

        if ( m_AudioSource != null )
        {
            m_AudioSource.PlayOneShot(clip, 0.4f);
        }
    }

    public void Stop()
    {
        m_AudioSource.Stop();
    }
}
