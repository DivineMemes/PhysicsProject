using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DelayedSoundPlayer : MonoBehaviour {

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private float volume;
    [SerializeField]
    private float timeToWait;



    private void Start()
    {
        Invoke("PlaySound", timeToWait);
    }

    private void PlaySound()
    {
        audioSource.PlayOneShot(audioSource.clip, volume);
    }

}
