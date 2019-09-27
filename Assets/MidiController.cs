using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NAudio;
using NAudio.Midi;

public class MidiController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var strictMode = false;
        var mf = new MidiFile("test.mid", strictMode);
        Debug.Log (mf.Tracks);
        for (int n = 0; n < mf.Tracks; n++) {
            foreach (var midiEvent in mf.Events [n]) {
                if (!MidiEvent.IsNoteOff (midiEvent)) {
                    Debug.Log ("{0} {1}\r\n", ToMBT (midiEvent.AbsoluteTime, mf.DeltaTicksPerQuarterNote, timeSignature), midiEvent);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
