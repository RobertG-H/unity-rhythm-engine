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
    private String fileName = "test.mid";
    private TimeSignatureEvent timeSignature;
    private List<MidiNote> midiNotes = new List<MidiNote> ();
    private float ticksperQuarterNote;
    private float bpm;
    private TimeSig timeSig;
    private int finalTick;

    /// <summary>
    /// Loads supplied midi file from streaming assets.
    /// Creates and adds to the conductor these variables:
    /// MidiNotes, TicksPerQuarter, TimeSig, FinalTick
    /// </summary>
    void Start()
    {
        var strictMode = false;
        var midiFile = new MidiFile(Application.streamingAssetsPath + "/Midi/" + fileName, strictMode);
        ticksperQuarterNote = midiFile.DeltaTicksPerQuarterNote;
        
        //bpm = 
        for (int n = 0; n < midiFile.Tracks; n++)
        {
            int counter = 0;
            foreach (var midiEvent in midiFile.Events [n])
            {
                if (timeSignature is null)
                {
                    try
                    {
                        timeSignature = (TimeSignatureEvent) midiEvent;
                    }
                    catch (InvalidCastException e) when (e.Data != null)
                    {
                        Debug.Log ("Not TimeSig");
                    }
                }
                if (!MidiEvent.IsNoteOff (midiEvent))
                {
                    // Get the final tick of the song
                    if (counter == midiFile.Events [n].Count - 1)
                    {
                        finalTick = (int) midiEvent.AbsoluteTime;
                    }
                    // Ensure that the midievent is a note, and not metadata
                    if (midiEvent.CommandCode.ToString() == "NoteOn")
                    {
                        // Note length is retrieved from the next midiEvent's deltaTime
                        float noteLength = 0;

                        // Not at the end yet
                        if (counter != midiFile.Events [n].Count - 1)
                        {
                            MidiEvent nextMidievent = midiFile.Events [n] [counter + 1];
                            noteLength = ((float) nextMidievent.DeltaTime / (float) midiFile.DeltaTicksPerQuarterNote);
                        }
                        
                        MidiNote note = GenerateMidiNote (midiEvent.AbsoluteTime, noteLength);
                        midiNotes.Add (note);
                    }
                }
                counter += 1;
            }
        }

        // Set all Conductor variables
        Conductor.Instance.SetMidiNotes (midiNotes);
        Conductor.Instance.SetTicksperQuarterNote (ticksperQuarterNote);
        Conductor.Instance.SetTimeSig(new TimeSig(timeSignature.Numerator, timeSignature.Denominator));
        //Conductor.Instance.SetBpm (bpm); // TODO set bpm auto
        Conductor.Instance.SetFinalBeat(finalTick/ ticksperQuarterNote);
    }

    /// <summary>
    /// Creates a new MidiNote
    /// </summary>
    /// <param name="eventTime"> The time in ticks of the midi event</param>
    /// <param name="noteLength"> The length of the note in ticks</param>
    /// <returns></returns>
    private MidiNote GenerateMidiNote (long eventTime, float noteLength)
    {
        int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
        int ticksPerBar = timeSignature == null ? (int) ticksperQuarterNote * 4 : (int) ticksperQuarterNote * timeSignature.Numerator;
        int ticksPerBeat = ticksPerBar / beatsPerBar;
        long bar = (eventTime / ticksPerBar);
        long beat = ((eventTime % ticksPerBar) / ticksPerBeat);
        long tick = eventTime % ticksPerBeat;

        float notePosition = 0;
        notePosition += (bar) * timeSignature.Numerator;
        notePosition += (beat);
        notePosition += tick / ticksperQuarterNote;
        return new MidiNote ((int)bar, (int)beat, (int)tick, (float)notePosition, (float)noteLength);
    }
}
