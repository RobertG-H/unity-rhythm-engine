using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject note;
    [SerializeField]
    private GameObject measure;
    [SerializeField]
    private Sprite wholeNoteSprite;
    [SerializeField]
    private Sprite halfNoteSprite;
    [SerializeField]
    private Sprite quarterNoteSprite;
    [SerializeField]
    private Sprite eighthNoteSprite;
    [SerializeField]
    private Sprite doubleEighthNoteSprite;
    [SerializeField]
    private Sprite wholeRestSprite;
    [SerializeField]
    private Sprite halfRestSprite;
    [SerializeField]
    private Sprite quarterRestSprite;
    [SerializeField]
    private Sprite eighthRestSprite;
    [SerializeField]
    private float noteStartOffset = -6.3f;
    [SerializeField]
    private float measureStartOffset;
    private bool runOnce = true;
    private float noteHeightOffset = 0.1f;
    // What size step to move when generating notes. 1 = 1 beat
    private float scoreStep = 0.5f;
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
            List<float> noteLengthsList = new List<float> ();
            Debug.Log ("Starting note position logging");
            float prevNotePosition = -1;
            int maxIndex = -1;
            foreach (MidiNote midiNote in midiNotes)
            {
                float notePosition = 0;
                notePosition += (midiNote.Bar) * Conductor.Instance.GetTimeSig().Num;
                notePosition += (midiNote.Beat);
                notePosition += midiNote.Tick / Conductor.Instance.GetTicksPerQuarterNote ();
                if (notePosition != prevNotePosition)
                {
                    notePositionList.Add(notePosition);
                    noteLengthsList.Add (midiNote.Length);
                    prevNotePosition = notePosition;
                    maxIndex++;
                }
            }
            int index = 0;
            float currentNotePosition = notePositionList [index];
            float currentNoteLength = noteLengthsList [index];
            float nextNotePosition = notePositionList [index + 1];
            float nextNoteLength = noteLengthsList [index + 1];

            // Iterate through the score one step at a time (8th or 16th step) 
            for (float scorePosition = 0.0f; scorePosition < Conductor.Instance.GetFinalTick(); scorePosition += scoreStep)
            {
                if (scorePosition == currentNotePosition && index < maxIndex)
                {

                    // Check for rest spawning
                    bool spawnedRest = CreateRest (currentNotePosition + currentNoteLength, nextNotePosition);

                    // Spawn rest first because need to check for connected eighth notes.
                    if (spawnedRest)
                    {
                        // Spawn note normally
                        CreateNote (scorePosition, currentNoteLength);
                        // Update index 
                        index++;
                    }
                    else
                    {
                        // If the current note and the next note are eighth notes then they should be connected
                        if (nextNoteLength == 0.5f && currentNoteLength == 0.5f)
                        {
                            // TODO fix connected note spawning
                            /*TEMP*/
                            CreateNote (scorePosition, currentNoteLength);
                            index++;
                            /*ENDTEMP*/
                            //CreateNote (scorePosition, 0.55f);
                            // Update index + 2 because spawned two note 
                            //index += 2;
                        }
                        else
                        {
                            // Spawn note normally
                            CreateNote (scorePosition, currentNoteLength);
                            // Update index 
                            index++;
                        }
                    }





                    // Update note positions
                    if (index <+ maxIndex)
                    {
                        currentNotePosition = notePositionList [index];
                        currentNoteLength = noteLengthsList [index];
                        if (index < maxIndex)
                        {
                            nextNotePosition = notePositionList [index + 1];
                            nextNoteLength = noteLengthsList [index + 1];
                        }
                    }

                        
                }
                if (scorePosition % (Conductor.Instance.GetTimeSig().Num) == 0)
                {
                    Instantiate (measure, new Vector3(measureStartOffset + (scorePosition *4.0f), 0, 0), Quaternion.identity);
                }
                //return;

            }

        }
    }

    private void CreateNote (float scorePosition, float currentNoteLength)
    {
        // Default state
        Sprite sprite = wholeNoteSprite;
        Debug.Log ("Position: " + scorePosition + " current note length: " + currentNoteLength);
        // Whole sprite
        if (currentNoteLength == Conductor.Instance.GetTimeSig ().Num)
            sprite = wholeNoteSprite;
        // Half sprite
        else if (currentNoteLength == (int) (Conductor.Instance.GetTimeSig ().Num / 2.0f))
            sprite = halfNoteSprite;
        // Quarter sprite
        else if (currentNoteLength == 1)
            sprite = quarterNoteSprite;
        // eighth sprite
        else if (currentNoteLength == 0.5f)
            sprite = eighthNoteSprite;
        else if (currentNoteLength == 0.55f)
            sprite = doubleEighthNoteSprite;
        GameObject newNote = Instantiate (note, new Vector3 (noteStartOffset + (scorePosition * (4.0f)), noteHeightOffset, 0), Quaternion.identity);
        newNote.GetComponent<SpriteRenderer> ().sprite = sprite;
    }

    private bool CreateRest (float endOfCurrentNote, float startOfNextNote)
    {
        float restDur = startOfNextNote - endOfCurrentNote;
        Sprite sprite = wholeRestSprite;
        // No space for rest
        if (restDur <= 0) return false;
        // Cases for different sized rests

        // Whole rest
        if (restDur == Conductor.Instance.GetTimeSig ().Num)
            sprite = wholeRestSprite;
        // Half rest
        else if (restDur == Conductor.Instance.GetTimeSig ().Num/2.0f)
            sprite = halfRestSprite;
        // Quarter rest
        else if (restDur == 1.0f)
            sprite = quarterRestSprite;
        // eighth rest
        else if (restDur == 0.5f)
            sprite = eighthRestSprite;
        GameObject newSprite = Instantiate (note, new Vector3 (noteStartOffset + (endOfCurrentNote * (4.0f)), noteHeightOffset, 0), Quaternion.identity);
        newSprite.GetComponent<SpriteRenderer> ().sprite = sprite;

        return true;
    }


}
