using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource SfxSound;
    public List<AudioClip> endAudioClip,startAudioClips;
    // Start is called before the first frame update
    void Awake()
    {
        SfxSound = GetComponent<AudioSource>();
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
  

    public void PlaySFXSound(AudioClip PlayAudio)
    {
        SfxSound.clip = PlayAudio;
        SfxSound.Play();
    }

    public void EndSFXSound()
    {
        int rand = Random.Range(0, endAudioClip.Count);
        SfxSound.clip = endAudioClip[rand];
        SfxSound.Play();
    }

    public void startSFXSound()
    {
        int rand = Random.Range(0, startAudioClips.Count);
        SfxSound.clip = startAudioClips[rand];
        SfxSound.Play();
    }
}
