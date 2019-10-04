using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject note;
    [SerializeField]
    private GameObject measure;
    private float startOffset = -6.3f;
    private bool runOnce = true;
    // Start is called before the first frame update
    void Start()
    {

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
            Debug.Log ("Starting note position logging");
            foreach (MidiNote midiNote in midiNotes)
            {
                double notePosition = 0;
                // TODO make sure this works with different time signatures MAYBE JUST ADD THESE CALCS AS A FUNCTION
                notePosition += (midiNote.Bar) * 3;
                notePosition += (midiNote.Beat);
                notePosition += midiNote.Tick / 15360.0;// TicksperQuarterNote is 15360
                //Debug.Log (notePosition);
                Instantiate (note, new Vector3 (startOffset + ((float) notePosition)*(4.0f/1.5f), 0, 0), Quaternion.identity);
            }
        }
    }


}
