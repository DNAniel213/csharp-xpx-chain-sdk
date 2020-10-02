using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnce : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip miningSound;
    public Animator logo;
    // Start is called before the first frame update
    void Start()
    {
        logo = GameObject.Find("ClickLogo").GetComponent<Animator>();
    }

    // Update is called once per frame
    public void PlayMiningSound()
    {
        audioSource.PlayOneShot(miningSound);
    }

    public void Upgrade()
    {
        logo.SetTrigger("Upgrade");
    }

}

