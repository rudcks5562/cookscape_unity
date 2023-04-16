using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerSinkSound : MonoBehaviour
{
    AudioSource m_AudioSource;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        AudioClip clip = SoundsManager.instance.RunnerSinkSound;

        if ( m_AudioSource != null )
        {
            m_AudioSource.PlayOneShot(clip, 0.2f);
        }
    }
}
