using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private List<AudioSource> songsList;
    private AudioSource currentSong;
    private int currentSongTrack;
    public float musicVolume;

    // Start is called before the first frame update
    void Start()
    {
        songsList = GetComponents<AudioSource>().ToList();
        currentSong = songsList[0];
        currentSongTrack = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentSong.isPlaying)
        {
            StartNewSong();
        }

        UpdateVolume();
    }

    private void StartNewSong()
    {
        int newSongTrack = currentSongTrack;

        currentSong.Stop();

        while (newSongTrack == currentSongTrack)
        {
            newSongTrack = RandomUtils.Next(0, songsList.Count - 1);
        }

        currentSong = songsList[newSongTrack];
        currentSongTrack = newSongTrack;
        currentSong.Play();
    }

    private void UpdateVolume()
    {
        currentSong.volume = musicVolume;
    }
}
