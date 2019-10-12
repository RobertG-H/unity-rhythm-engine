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
    private float ticksperQuarterNote;
    private String fileName = "test-midi.mid";
    private TimeSig timeSig;
    private int finalTick;

    void Start()
    {
        var strictMode = false;
        var mf = new MidiFile(Application.streamingAssetsPath + "/Midi/" + fileName, strictMode);
        ticksperQuarterNote = mf.DeltaTicksPerQuarterNote;
        timeSignature = (TimeSignatureEvent)mf.Events[0][1];
        for (int n = 0; n < mf.Tracks; n++) {
            int counter = 0;
            foreach (var midiEvent in mf.Events [n]) {
                if (!MidiEvent.IsNoteOff (midiEvent)) {
                    //Debug.LogFormat ("{0} {1}\r\n", ToMBT (midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), midiEvent);
                    if (counter == mf.Events[n].Count-1)
                    {
                        //Debug.Log(midiEvent.AbsoluteTime);
                        finalTick = (int)midiEvent.AbsoluteTime;
                    }

                    MidiNote note = ToMBT (midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature);
                    midiNotes.Add (note);
                }
                counter += 1;
            }
        }

        foreach (MidiNote note in midiNotes)
        {
            float notePosition = 0;
            //Debug.Log ("next note");
            //Debug.Log (String.Format ("{0}:{1}:{2}", note.Bar, note.Beat, note.Tick));
            notePosition += (note.Bar) * timeSignature.Numerator  ;
            notePosition += (note.Beat);
            notePosition += note.Tick / ticksperQuarterNote;
        }
        Conductor.Instance.SetMidiNotes (midiNotes);
        Conductor.Instance.SetTicksperQuarterNote (ticksperQuarterNote);
        Conductor.Instance.SetTimeSig(new TimeSig(timeSignature.Numerator, timeSignature.Denominator));
        Conductor.Instance.SetFinalTick(finalTick);
        Debug.Log ("Notes set");
    }

    private MidiNote ToMBT (long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature) {
        int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
        int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : ticksPerQuarterNote * timeSignature.Numerator;
        int ticksPerBeat = ticksPerBar / beatsPerBar;
        long bar = 1 + (eventTime / ticksPerBar);
        long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
        long tick = eventTime % ticksPerBeat;
        return new MidiNote ((int)bar-1, (int)beat-1, (int)tick);//String.Format ("{0}:{1}:{2}", bar, beat, tick);
    }
}
