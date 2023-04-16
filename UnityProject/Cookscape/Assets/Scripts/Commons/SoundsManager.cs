using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;

    [Header("References")]
    public AudioSource m_AudioSource;
    
    #region BGM
    public AudioClip LoginSceneBGM;
    public AudioClip ChannelSceneBGM;
    public AudioClip[] MetabusSceneBGMs;
    public AudioClip InGameBGM;
    #endregion

    public AudioClip[] WalkingSounds;
    public AudioClip[] AttackedSounds;
    public AudioClip ChefBreathSound;
    public AudioClip ChefChaseSound;
    public AudioClip RunnerSinkSound;
    public AudioClip[] ChefLaughSounds;
    public AudioClip[] RunnerInciteSounds;
    public AudioClip[] RunnerFartSounds;
    public AudioClip BellRingingSound;
    public AudioClip ExitStartSound;
    public AudioClip ClockTickSound;
    public AudioClip ClockTockSound;
    public AudioClip SinkBubblingSound;
    public AudioClip[] ChickHitSounds;

    private void Awake()
    {
        if ( instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
