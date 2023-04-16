using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerInciteSound : MonoBehaviour
{
    AudioSource m_AudioSource;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Play(int idx)
    {
        if ( m_AudioSource != null )
        {
            AudioClip[] clips = SoundsManager.instance.RunnerInciteSounds;
            AudioClip clip = clips[idx];
            m_AudioSource.PlayOneShot(clip, 0.2f);
        }
    }
}
