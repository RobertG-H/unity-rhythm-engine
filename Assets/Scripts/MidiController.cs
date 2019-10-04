using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Midi;
using System;

public class MidiController : MonoBehaviour
{
    private TimeSignatureEvent timeSignature; 
    void Start()
    {

        var strictMode = false;
        var mf = new MidiFile(Application.streamingAssetsPath + "/Midi/Song Of Storms Treble.mid", strictMode);
        Debug.Log (mf.DeltaTicksPerQuarterNote);
        for (int n = 0; n < mf.Tracks; n++) {
            foreach (var midiEvent in mf.Events [n]) {
                if (!MidiEvent.IsNoteOff (midiEvent)) {
                    Debug.LogFormat ("{0} {1}\r\n", ToMBT (midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), midiEvent);
                }
            }
        }
    }

    void Update()
    {
        
    }

    private string ToMBT (long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature) {
        timeSignature = new TimeSignatureEvent (eventTime, 3, 4, ticksPerQuarterNote, 8);
        int beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
        int ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : ticksPerQuarterNote * timeSignature.Numerator;
        int ticksPerBeat = ticksPerBar / beatsPerBar;
        long bar = 1 + (eventTime / ticksPerBar);
        long beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
        long tick = eventTime % ticksPerBeat;
        return String.Format ("{0}:{1}:{2}", bar, beat, tick);
    }
}
