using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundEffectsController : MonoBehaviour
{
    const string battleSoundName = "Effect_BattleSound";

    private AudioSource battleSound;
    public float effectsVolume;

    void Awake()
    {
        List<AudioSource> soundEffects = GetComponents<AudioSource>().ToList();

        battleSound = soundEffects.Find(item => item.clip.name == battleSoundName);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVolume();
    }

    public void PlayBattleSound()
    {
        if (!battleSound.isPlaying)
        {
            battleSound.Play();
        }
    }

    public void UpdateVolume()
    {
        battleSound.volume = effectsVolume;
    }
}
