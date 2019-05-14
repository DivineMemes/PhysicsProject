using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

    [System.Serializable]
	public struct MusicCue
    {
        public AudioClip song;
        //if lengthOfTimeToPlay is <= 0 it will loop forever
        public float lengthOfTimeToplay;
        public float volume;
    }



    [SerializeField]
    private MusicCue[] cues;
    [SerializeField]
    private float fadeoutLength;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioMixer musicMixer;
    [SerializeField]
    private PlayerHealth coreHealth;
    [SerializeField]
    private PlayerHealth headHealth;
    [SerializeField, Range(0f, 100f)]
    private float healthDeathMusicTrigger;
    [SerializeField]
    private int deathMusicCue;

    private float currentPlayTime;
    private int currentCue;
    private bool fadingOut;



    private void OnValidate()
    {
        cues[cues.Length - 1].lengthOfTimeToplay = 0;
    }

    private void Start()
    {
        PlayCurrentCue();
    }

    private void Update()
    {
        if(fadingOut)
        {
            audioSource.volume -= Time.deltaTime * (cues[currentCue-1].volume / fadeoutLength);
            
            if(audioSource.volume <= 0)
            {
                fadingOut = false;
                PlayCurrentCue();
            }
        }
        else if(((coreHealth.health / coreHealth.maxHealth) * 100f <= healthDeathMusicTrigger
              || (headHealth.health / headHealth.maxHealth) * 100f <= healthDeathMusicTrigger)
                && currentCue != deathMusicCue)
        {
            currentCue = deathMusicCue;
            currentPlayTime = 0f;
            fadingOut = true;
        }
        
        if(cues[currentCue].lengthOfTimeToplay > 0)
        {
            currentPlayTime += Time.deltaTime;

            if(currentPlayTime >= cues[currentCue].lengthOfTimeToplay)
            {
                currentPlayTime = 0f;
                ++currentCue;
                fadingOut = true;
            }
        }
    }

    private void PlayCurrentCue()
    {
        audioSource.clip = cues[currentCue].song;
        audioSource.volume = cues[currentCue].volume;
        musicMixer.SetFloat("MasterVolume", cues[currentCue].volume);
        audioSource.Play();
    }

}
