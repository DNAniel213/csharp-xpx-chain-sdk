using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnce : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip miningSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void PlayMiningSound()
    {
        audioSource.PlayOneShot(miningSound);
    }
}
