using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkBubblingSound : MonoBehaviour
{
    public GameObject[] m_Sinks;

    void Start()
    {

    }

    public void Play()
    {
        foreach(GameObject sink in m_Sinks) {
            AudioSource audioSource = sink.GetComponent<AudioSource>();
            if ( audioSource != null )
            {
                AudioClip clip = SoundsManager.instance.SinkBubblingSound;
                audioSource.clip = clip;
                audioSource.volume = 0.1f;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    public void Stop()
    {
        foreach(GameObject sink in m_Sinks) {
            AudioSource audioSource = sink.GetComponent<AudioSource>();
            if ( audioSource != null )
            {
                audioSource.Stop();
            }
        }
    }
}
