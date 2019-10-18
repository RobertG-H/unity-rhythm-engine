﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public struct MidiNote
{
    public int Bar;
    public int Beat;
    public int Tick;
    public float Position;
    public float Length;

    public MidiNote (int bar, int beat, int tick, float notePosition, float length)
    {
        Bar = bar;
        Beat = beat;
        Tick = tick;
        Position = notePosition;
        Length = length;
    }
}

public struct TimeSig
{
    public int Num;
    public int Denom;
    public float WHOLE;
    public float HALF;
    public float QUARTER;
    public float EIGHTH;
    public float SIXTEENTH;

    // Assumes all time signatures use a denom of 4
    public TimeSig (int num, int denom)
    {
        Num = num;
        // TODO fix denom being 2 sometimes
        denom = 4;
        Assert.AreEqual (denom, 4);
        Denom = denom;
        WHOLE = num;
        HALF = 2.0f;
        QUARTER = 1.0f;
        EIGHTH = QUARTER / 2.0f;
        SIXTEENTH = EIGHTH / 2.0f;
    }
}

/// <summary> 
/// The Conductor tracks the song position and controls any other synced action.
/// </summary>
public class Conductor : MonoBehaviour
{
    //Conductor instance
    public static Conductor Instance;

    private AudioSource musicSource;

    // Main vars with getters and setters
    private List<MidiNote> midiNotes;
    private float ticksperQuarterNote;
    private TimeSig timeSig;
    private float finalBeat;

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
            if(musicSource.time != lastReportedPlayheadPosition)
            {
                songTime = (songTime + musicSource.time)/2;
                lastReportedPlayheadPosition = musicSource.time;
            }
            songPositionInBeats = songTime / secPerBeat;
        }
    }

    /// <summary>
    /// Start the soing
    /// </summary>
    public void StartSong()
    {
        musicSource.Play();
        //song started
        previousFrameTime = AudioSettings.dspTime;
        songTime = 0;
        hasStarted = true;
    }

    /* DEPRECATED
    public bool IsQuarterBeat()
    {
        float intSongPositionInBeats = (int) Math.Round (songPositionInBeats, 0) + 0.5f;
        if (songPositionInBeats < intSongPositionInBeats + correctThreshold && songPositionInBeats > intSongPositionInBeats - correctThreshold)
        {
            Debug.Log (songPositionInBeats);
            return true;
        }
        return false;
    }*/

    public bool CheckHit ()
    {
        double currentBeat = songPositionInBeats - 0.5f; // TODO figure out why this needs a 0.5f offset.
        foreach (MidiNote midiNote in midiNotes)
        {
            if (currentBeat > midiNote.Position + correctThreshold)
            {
                //midiNotes.Remove (midiNote); // TODO figure out a way to remove notes after they have been passed
            }
            if (currentBeat < midiNote.Position + correctThreshold && currentBeat > midiNote.Position - correctThreshold)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// GETTERS AND SETTERS
    /// </summary>

    public float GetBpm ()
    {
        return (float) songBpm;
    }

    public void SetBpm (float newBpm)
    {
        songBpm = (double) newBpm;
    }

    public double GetAudioSourceTime ()
    {
        return musicSource.time;
    }

    public double GetSongTime ()
    {
        return songTime;
    }

    public double GetSongBeat ()
    {
        return songPositionInBeats;
    }

    public List<MidiNote> GetMidiNotes ()
    {
        return midiNotes;
    }

    public void SetMidiNotes(List<MidiNote> newNoteList)
    {
        midiNotes = newNoteList;
    }

    public float GetTicksPerQuarterNote ()
    {
        return ticksperQuarterNote;
    }

    public void SetTicksperQuarterNote (float newTicksperQuarterNote)
    {
        ticksperQuarterNote = newTicksperQuarterNote;
    }

    public TimeSig GetTimeSig ()
    {
        return timeSig;
    }

    public void SetTimeSig (TimeSig newTimeSig)
    {
        timeSig = newTimeSig;
    }

    public float GetFinalBeat ()
    {
        return finalBeat;
    }

    public void SetFinalBeat(float newfinalTick)
    {
        finalBeat = newfinalTick;
    }

}
