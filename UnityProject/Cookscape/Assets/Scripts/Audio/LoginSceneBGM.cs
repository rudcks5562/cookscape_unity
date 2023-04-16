using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneBGM : MonoBehaviour
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
            StartCoroutine("PlayLoginBGM");
        }
    }

    private IEnumerator PlayLoginBGM()
    {
        m_AudioSource.loop = true;
        m_AudioSource.volume = 0.3f;
        AudioClip LoginSceneBGM = SoundsManager.instance.LoginSceneBGM;
        m_AudioSource.clip = LoginSceneBGM;
        yield return new WaitForSeconds(1.0f);
        m_AudioSource.Play();
    }

    public void Stop()
    {
        m_AudioSource.Stop();
    }
}
