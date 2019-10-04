using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MidiNote
{
    public int Bar;
    public int Beat;
    public int Tick;

    public MidiNote (int bar, int beat, int tick)
    {
        Bar = bar;
        Beat = beat;
        Tick = tick;
    }
}

/// <summary> The Conductor tracks the song position and controls any other synced action.</summary>
public class Conductor : MonoBehaviour
{
    //Conductor instance
    public static Conductor Instance;

    private AudioSource musicSource;

    private List<MidiNote> midiNotes;

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

    private float correctThreshold = 0.3f;

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
            songTime += AudioSettings.dspTime - previousFrameTime - firstBeatOffset; // TODO fix firstbeatoffset
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

    public double GetAudioSourceTime()
    {
        return musicSource.time;
    }

    public double GetSongTime()
    {
        return songTime;
    }

    public double GetSongBeat()
    {
        return songPositionInBeats;
    }

    public bool IsQuarterBeat() 
    {
        float intSongPositionInBeats = (int) Math.Round (songPositionInBeats, 0) + 0.5f;
        if (songPositionInBeats < intSongPositionInBeats + correctThreshold && songPositionInBeats > intSongPositionInBeats - correctThreshold) 
        {
            Debug.Log (songPositionInBeats);
            return true;
        }
        return false;
    }

    public void SetMidiNotes(List<MidiNote> newNoteList)
    {
        midiNotes = newNoteList;
    }

    public List<MidiNote> GetMidiNotes()
    {
        return midiNotes;
    }

    public bool CheckHit()
    {
        double currentBeat = songPositionInBeats -0.5f;
        //Debug.Log ("currentBeat: " + currentBeat);
        foreach (MidiNote midiNote in midiNotes)
        {
            double notePosition = 0;
            // TODO make sure this works with different time signatures
            notePosition += (midiNote.Bar) * 3;
            notePosition += (midiNote.Beat);
            notePosition += midiNote.Tick / 15360.0;// TicksperQuarterNote is 15360
            if (currentBeat > notePosition + correctThreshold)
            {
                //midiNotes.Remove (midiNote);
            }
            //Debug.Log ("Noteposition: " +notePosition);
            if (currentBeat < notePosition + correctThreshold && currentBeat > notePosition - correctThreshold)
            {
                   
                return true;
            }
        }

        return false;
    }
}
