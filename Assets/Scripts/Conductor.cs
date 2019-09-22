using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary> The Conductor tracks the song position and controls any other synced action.</summary>
public class Conductor : MonoBehaviour
{
    //Conductor instance
    public static Conductor Instance;

    private AudioSource musicSource;

    // Variables that keep track of song.
    private double previousFrameTime;
    private double lastReportedPlayheadPosition = 0;
    private double songTime;
    private double songPositionInBeats;

    // Variables for properties of the song.
    [SerializeField]
    private double songBpm;
    private double secPerBeat;
    // firstBeatOffset accounts for small silences before the first beat of the song in the audio file.
    [SerializeField]
    private double firstBeatOffset;

    private bool hasStarted = false;

    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        secPerBeat = 60f / songBpm;

    }

    void Update()
    {
        if (hasStarted)
        {
            songTime += AudioSettings.dspTime - previousFrameTime - firstBeatOffset;
            previousFrameTime = AudioSettings.dspTime;
            if(musicSource.time != lastReportedPlayheadPosition) {
                songTime = (songTime + musicSource.time)/2;
                lastReportedPlayheadPosition = musicSource.time;
            }
            songPositionInBeats = songTime / secPerBeat;
        }
    }


    public void StartSong()
    {
        musicSource.Play();
        //song started
        previousFrameTime = AudioSettings.dspTime;
        songTime = 0;
        hasStarted = true;
    }

    public double getAudioSourceTime()
    {
        return musicSource.time;
    }

    public double getSongTime()
    {
        return songTime;
    }

    public double getSongBeat()
    {
        return songPositionInBeats;
    }
}
