using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoteSpawner : MonoBehaviour
{
    // GAME OBJECT PREFABS
    [SerializeField]
    private GameObject note;
    [SerializeField]
    private float noteSpeed;
    [SerializeField]
    private GameObject measure;
    [SerializeField]
    private GameObject trebleScore;
    [SerializeField]
    private GameObject bassScore;


    // SPRITES
    [SerializeField]
    private Sprite wholeNoteSprite;
    [SerializeField]
    private Sprite halfNoteSprite;
    [SerializeField]
    private Sprite quarterNoteSprite;
    [SerializeField]
    private Sprite eighthNoteSprite;
    [SerializeField]
    private Sprite wholeRestSprite;
    [SerializeField]
    private Sprite halfRestSprite;
    [SerializeField]
    private Sprite quarterRestSprite;
    [SerializeField]
    private Sprite eighthRestSprite;

    // SPAWN CALCULATIONS
    [SerializeField]
    private float noteStartOffset = -6.3f;
    [SerializeField]
    private float measureStartOffset;
    private float noteHeightOffset = 0.1f;
    private float trebleScoreHeight;
    private float bassScoreHeight;
    // What size step to move when generating notes.
    // 1 scoreStep = check for a note every beat (quarter note).
    // 0.5 scoreStep = check for a note every half beat (eighth note).
    private float scoreStep = 0.5f;

    private bool runOnce = true;
    private float spawnDistanceMultiplier;

    void Start()
    {
        measureStartOffset = noteStartOffset - 1;
        float bps = Conductor.Instance.GetBpm() / 60.0f;
        spawnDistanceMultiplier = Math.Abs(noteSpeed) / bps;
        trebleScoreHeight = trebleScore.transform.position.y;
        bassScoreHeight = bassScore.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (runOnce)
        {
            runOnce = false;
            SpawnAllNotes(NoteType.TREBLE, trebleScoreHeight);
            SpawnAllNotes(NoteType.BASS, bassScoreHeight);
        }
    }

    private void SpawnAllNotes(NoteType noteType, float scoreHeight)
    {
        List<MidiNote> midiNotes = Conductor.Instance.GetMidiNotes(noteType);
        Debug.Log("Starting note position logging");
        int index = 0;
        // Iterate through the score one step at a time
        float test = Conductor.Instance.GetFinalBeat();
        for (float scorePosition = 0.0f; scorePosition < Conductor.Instance.GetFinalBeat(); scorePosition += scoreStep)
        {
            if (scorePosition == midiNotes[index].Position && index < midiNotes.Count - 1)
            {
                // Check for rest spawning
                bool spawnedRest = CreateRest(midiNotes[index].Position + midiNotes[index].Length, midiNotes[index + 1].Position, scoreHeight);
                // Spawn note
                CreateNote(scorePosition, midiNotes[index].Length, scoreHeight, noteType);
                // Update index 
                index++;
            }

            // Spawn measure line
            if (scorePosition % (Conductor.Instance.GetTimeSig().Num) == 0)
            {
                GameObject newSprite = Instantiate(measure, new Vector3(measureStartOffset + (scorePosition * spawnDistanceMultiplier), scoreHeight, 0), Quaternion.identity);
                newSprite.GetComponent<Note>().SetSpeed(noteSpeed);
            }
        }
    }

    private void CreateNote(float scorePosition, float currentNoteLength, float scoreHeight, NoteType noteType)
    {
        float roundedLength = RoundLength(currentNoteLength);
        // Default state
        Sprite sprite = wholeNoteSprite;
        // Whole sprite
        if (roundedLength == Conductor.Instance.GetTimeSig().WHOLE)
            sprite = wholeNoteSprite;
        // Half sprite
        else if (roundedLength == Conductor.Instance.GetTimeSig().HALF)
            sprite = halfNoteSprite;
        // Quarter sprite
        else if (roundedLength == Conductor.Instance.GetTimeSig().QUARTER)
            sprite = quarterNoteSprite;
        // eighth sprite
        else if (roundedLength == Conductor.Instance.GetTimeSig().EIGHTH)
            sprite = eighthNoteSprite;
        GameObject newNote = Instantiate(note, new Vector3(noteStartOffset + (scorePosition * spawnDistanceMultiplier), noteHeightOffset + scoreHeight, 0), Quaternion.identity);
        newNote.GetComponent<SpriteRenderer>().sprite = sprite;
        newNote.GetComponent<Note>().SetSpeed(noteSpeed);
        newNote.GetComponent<Note>().SetNoteType(noteType);
    }

    private bool CreateRest(float endOfCurrentNote, float startOfNextNote, float scoreHeight)
    {
        float restDur = startOfNextNote - endOfCurrentNote;
        float roundedLength = RoundLength(restDur);
        Sprite sprite = wholeRestSprite;
        // No space for rest
        if (roundedLength <= 0) return false;
        // Cases for different sized rests

        // Whole rest
        if (roundedLength == Conductor.Instance.GetTimeSig().WHOLE)
            sprite = wholeRestSprite;
        // Half rest
        else if (roundedLength == Conductor.Instance.GetTimeSig().HALF)
            sprite = halfRestSprite;
        // Quarter rest
        else if (roundedLength == Conductor.Instance.GetTimeSig().QUARTER)
            sprite = quarterRestSprite;
        // eighth rest
        else if (roundedLength == Conductor.Instance.GetTimeSig().EIGHTH)
            sprite = eighthRestSprite;
        GameObject newSprite = Instantiate(note, new Vector3(noteStartOffset + (endOfCurrentNote * spawnDistanceMultiplier), noteHeightOffset + scoreHeight, 0), Quaternion.identity);
        newSprite.GetComponent<SpriteRenderer>().sprite = sprite;
        newSprite.GetComponent<Note>().SetSpeed(noteSpeed);
        return true;
    }

    private float RoundLength(float num)
    {
        num *= 4;
        num = (float)Math.Round((double)num);
        num /= 4;
        return num;
    }
}
