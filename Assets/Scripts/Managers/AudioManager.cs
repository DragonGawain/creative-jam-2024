using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip normalMode;

    [SerializeField]
    AudioClip ghostMode;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayNormalMusic()
    {
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.PlayOneShot(normalMode);
    }

    public void PlayGhostMusic()
    {
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.PlayOneShot(ghostMode);
    }
}
