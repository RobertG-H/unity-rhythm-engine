using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Midi;
using System;

public class MidiController : MonoBehaviour
{
    private TimeSignatureEvent timeSignature;
    private List<MidiNote> midiNotes = new List<MidiNote> ();

    void Start()
    {
        var strictMode = false;
        var mf = new MidiFile(Application.streamingAssetsPath + "/Midi/Song Of Storms Treble.mid", strictMode);
        for (int n = 0; n < mf.Tracks; n++) {
            foreach (var midiEvent in mf.Events [n]) {
                if (!MidiEvent.IsNoteOff (midiEvent)) {
                    //Debug.LogFormat ("{0} {1}\r\n", ToMBT (midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), midiEvent);
                    //Debug.Log (midiEvent);
                    MidiNote note = ToMBT (midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature);
                    midiNotes.Add (note);
                }
            }
        }

        foreach (MidiNote note in midiNotes)
        {
            float notePosition = 0;
            Debug.Log ("next note");
            Debug.Log (String.Format ("{0}:{1}:{2}", note.Bar, note.Beat, note.Tick));
            notePosition += (note.Bar) * 3;
            notePosition += (note.Beat);
            notePosition += note.Tick / 15360.0f;// TicksperQuarterNote is 15360
            Debug.Log (notePosition);
        }
        Conductor.Instance.SetMidiNotes (midiNotes);
        Debug.Log ("Notes set");
    }

    void Update()
    {
        
    }

    private MidiNote ToMBT (long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature) {
        timeSignature = new TimeSignatureEvent (eventTime, 3, 4, ticksPerQuarterNote, 8);
        int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
        int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : ticksPerQuarterNote * timeSignature.Numerator;
        int ticksPerBeat = ticksPerBar / beatsPerBar;
        long bar = 1 + (eventTime / ticksPerBar);
        long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
        long tick = eventTime % ticksPerBeat;
        return new MidiNote ((int)bar-1, (int)beat-1, (int)tick);//String.Format ("{0}:{1}:{2}", bar, beat, tick);
    }
}
