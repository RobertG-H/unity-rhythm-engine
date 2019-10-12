using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject note;
    [SerializeField]
    private GameObject measure;
    private float noteStartOffset = -6.3f;
    private float measureStartOffset;
    private bool runOnce = true;
    // Start is called before the first frame update
    void Start()
    {
        measureStartOffset = noteStartOffset - 1;
        /*for(int i = 0; i < 128; i++) {
            Instantiate (note, new Vector3 (startOffset + i*4, 0, 0), Quaternion.identity);
            Instantiate (measure, new Vector3 (3.5f+i*16, 0, 0), Quaternion.identity);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (runOnce)
        {
            runOnce = false;
            List<MidiNote> midiNotes = Conductor.Instance.GetMidiNotes ();
            List<float> notePositionList = new List<float>();
            Debug.Log ("Starting note position logging");
            float prevNotePosition = -1;
            int maxIndex = -1;
            foreach (MidiNote midiNote in midiNotes)
            {
                float notePosition = 0;
                notePosition += (midiNote.Bar) * Conductor.Instance.GetTimeSig().Num;
                notePosition += (midiNote.Beat);
                notePosition += (float)midiNote.Tick / (float)Conductor.Instance.GetTicksPerQuarterNote();
                Debug.Log(notePosition);
                if (notePosition != prevNotePosition)
                {
                    notePositionList.Add(notePosition);
                    prevNotePosition = notePosition;
                    maxIndex++;
                }
            }
            int index = 0;

            // 1 on score position = 1 quarter note
            for (float scorePosition = 0.0f; scorePosition < Conductor.Instance.GetFinalTick(); scorePosition+=0.25f)
            {
                //Debug.Log(notePositionList[index]);
                if (scorePosition == notePositionList[index] && index < maxIndex)
                {
                    Instantiate (note, new Vector3 (noteStartOffset + ((float) notePositionList[index])*(4.0f), 0, 0), Quaternion.identity);
                    index++;
                }
                if (scorePosition % (Conductor.Instance.GetTimeSig().Num) == 0)
                {
                    Instantiate (measure, new Vector3(measureStartOffset + (scorePosition *4.0f), 0, 0), Quaternion.identity);
                }

            }

        }
    }


}
