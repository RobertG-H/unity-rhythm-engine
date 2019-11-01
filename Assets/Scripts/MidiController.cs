using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Midi;
using System;

/// <summary>
/// Reads midi file information and gives it to Conductor instance.
/// </summary>
public class MidiController : MonoBehaviour
{
    [SerializeField]
    private String trebleFileName;
    [SerializeField]
    private String bassFileName;
    private TimeSignatureEvent timeSignature;
    private List<MidiNote> trebleMidiNotes = new List<MidiNote>();
    private List<MidiNote> bassMidiNotes = new List<MidiNote>();
    private float ticksperQuarterNote;
    private float bpm;
    private TimeSig timeSig;
    private int finalTick = 0;

    /// <summary>
    /// Loads supplied midi file from streaming assets.
    /// Creates and adds to the conductor these variables:
    /// MidiNotes, TicksPerQuarter, TimeSig, FinalTick
    /// </summary>
    void Start()
    {
        var strictMode = false;
        var trebleFile = new MidiFile(Application.streamingAssetsPath + "/Midi/" + trebleFileName, strictMode);
        var bassFile = new MidiFile(Application.streamingAssetsPath + "/Midi/" + bassFileName, strictMode);
        ticksperQuarterNote = trebleFile.DeltaTicksPerQuarterNote;
        if (bassFile.DeltaTicksPerQuarterNote != ticksperQuarterNote)
        {
            Debug.LogWarning("Warning treble midi and bass midi formats do not match! This will cause them to be out of sync.");
        }

        ReadMidiFile(trebleFile, trebleMidiNotes);
        ReadMidiFile(bassFile, bassMidiNotes);

        // Set all Conductor variables
        Conductor.Instance.SetMidiNotes(trebleMidiNotes, bassMidiNotes);
        Conductor.Instance.SetTicksperQuarterNote(ticksperQuarterNote);
        Conductor.Instance.SetTimeSig(new TimeSig(timeSignature.Numerator, timeSignature.Denominator));
        //Conductor.Instance.SetBpm (bpm); // TODO set bpm auto
        Conductor.Instance.SetFinalBeat(finalTick / ticksperQuarterNote);
    }

    private void ReadMidiFile(MidiFile midiFile, List<MidiNote> midiNotes)
    {
        if (midiFile.Tracks > 1)
        {
            Debug.LogWarning("Warning! Midi file has more than one track. Taking first track");
        }
        int totalMidiEvents = midiFile.Events[0].Count;
        for (int i = 0; i < totalMidiEvents; i++)
        {
            MidiEvent midiEvent = midiFile.Events[0][i];
            if (timeSignature is null)
            {
                try
                {
                    timeSignature = (TimeSignatureEvent)midiEvent;
                }
                catch (InvalidCastException e) when (e.Data != null) { }
            }
            if (!MidiEvent.IsNoteOff(midiEvent))
            {
                // Get the final tick of the song
                if (i == totalMidiEvents - 1)
                {
                    int eventTime = (int)midiEvent.AbsoluteTime;
                    if (eventTime > finalTick)
                        finalTick = eventTime;
                }
                // Ensure that the midievent is a note, and not metadata
                if (midiEvent.CommandCode.ToString() == "NoteOn")
                {
                    // Note length is retrieved from the next midiEvent's deltaTime
                    float noteLength = 0;

                    // Not at the end yet
                    if (i < totalMidiEvents)
                    {
                        MidiEvent nextMidievent = midiFile.Events[0][i + 1];
                        noteLength = ((float)nextMidievent.DeltaTime / ticksperQuarterNote);
                    }
                    Debug.Log(noteLength);
                    MidiNote note = GenerateMidiNote(midiEvent.AbsoluteTime, noteLength);
                    midiNotes.Add(note);
                }
            }
        }
    }

    /// <summary>
    /// Creates a new MidiNote
    /// </summary>
    /// <param name="eventTime"> The time in ticks of the midi event</param>
    /// <param name="noteLength"> The length of the note in ticks</param>
    /// <returns></returns>
    private MidiNote GenerateMidiNote(long eventTime, float noteLength)
    {
        int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
        int ticksPerBar = timeSignature == null ? (int)ticksperQuarterNote * 4 : (int)ticksperQuarterNote * timeSignature.Numerator;
        int ticksPerBeat = ticksPerBar / beatsPerBar;
        long bar = (eventTime / ticksPerBar);
        long beat = ((eventTime % ticksPerBar) / ticksPerBeat);
        long tick = eventTime % ticksPerBeat;

        float notePosition = 0;
        notePosition += (bar) * timeSignature.Numerator;
        notePosition += (beat);
        notePosition += tick / ticksperQuarterNote;
        return new MidiNote((int)bar, (int)beat, (int)tick, (float)notePosition, (float)noteLength);
    }
}
