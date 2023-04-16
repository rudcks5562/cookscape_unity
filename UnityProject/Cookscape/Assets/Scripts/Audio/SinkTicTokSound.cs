using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkTicTokSound : MonoBehaviour
{
    AudioSource m_AudioSource;
    AudioClip tick;
    AudioClip tock;
    bool m_IsPlaying = false;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Play(float interval)
    {
        if ( m_AudioSource != null )
        {
            tick = SoundsManager.instance.ClockTickSound;
            tock = SoundsManager.instance.ClockTockSound;
            StopCoroutine("TickTock");
            StartCoroutine("TickTock", interval);
        }
    }

    IEnumerator TickTock(float interval)
    {
        m_IsPlaying = true;
        while (m_IsPlaying) {
            m_AudioSource.PlayOneShot(tick, 0.2f);
            yield return new WaitForSeconds(interval);
            m_AudioSource.PlayOneShot(tock, 0.2f);
            yield return new WaitForSeconds(interval);
        }
    }

    public void Stop()
    {
        m_IsPlaying = false;
        StopCoroutine("TickTock");
    }
}
