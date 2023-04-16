using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBGM : MonoBehaviour
{
    AudioSource m_AudioSource;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if ( m_AudioSource != null )
        {
            StartCoroutine("PlayGameSound");
        }
    }

    private IEnumerator PlayGameSound()
    {
        m_AudioSource.loop = true;
        m_AudioSource.volume = 0.05f;
        AudioClip inGameBGM = SoundsManager.instance.InGameBGM;
        m_AudioSource.clip = inGameBGM;
        m_AudioSource.Play();
        
        AudioClip bellRinging = SoundsManager.instance.BellRingingSound;
        for (int i=0; i<3; i++) {
            m_AudioSource.PlayOneShot(bellRinging, 1.0f);
            yield return new WaitForSeconds(3.0f);
        }

    }

    public void Stop()
    {
        m_AudioSource.Stop();
    }
}
