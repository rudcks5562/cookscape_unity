using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstepSound()
    {
        AudioClip[] clips = SoundsManager.instance.WalkingSounds;
        int randomIndex = Random.Range(0, clips.Length);
        AudioClip now = clips[randomIndex];

        if ( m_AudioSource != null )
        {
            m_AudioSource.PlayOneShot(now);
        }
    }
}
