using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Graphic objects
    private SpriteRenderer sprite;

    [SerializeField]
    private NoteSpawner noteSpawner;
    private Animator trebleAnimator;
    private Animator bassAnimator;
    [SerializeField]
    private ScreenFlash screenFlash;

    // Private bools
    private bool isHittingTreble;
    private bool isHittingBass;
    private bool killable;

    private NoteType currentKillingType;


    void Start()
    {
        //sprite = GetComponent<SpriteRenderer>();
        trebleAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        bassAnimator = gameObject.transform.GetChild(1).GetComponent<Animator>();
        isHittingTreble = false;
        isHittingBass = false;
    }

    void Update()
    {
        // Bass
        if (Input.GetKeyDown("s") || Input.GetKeyDown("d") || Input.GetKeyDown("f"))
        {
            isHittingBass = true;
            bassAnimator.SetBool("isHitting", isHittingBass);
            StartCoroutine(HittingBass());
            MidiNote? _bassNote = Conductor.Instance.CheckHit(NoteType.BASS);
            if (_bassNote != null)
            {
                MidiNote bassNote = (MidiNote)_bassNote;
                if (screenFlash)
                {
                    //screenFlash.Flash();
                }
                killable = true;
                currentKillingType = NoteType.BASS;
                noteSpawner.DeleteNote(bassNote.Position, currentKillingType);
            }
        }
        // Treble
        if (Input.GetKeyDown("j") || Input.GetKeyDown("k") || Input.GetKeyDown("l"))
        {
            isHittingTreble = true;
            trebleAnimator.SetBool("isHitting", isHittingTreble);
            StartCoroutine(Hitting());
            MidiNote? _trebleNote = Conductor.Instance.CheckHit(NoteType.TREBLE);
            if (_trebleNote != null)
            {
                MidiNote trebleNote = (MidiNote)_trebleNote;
                if (screenFlash)
                {
                    //screenFlash.Flash();
                }
                killable = true;
                currentKillingType = NoteType.TREBLE;
                noteSpawner.DeleteNote(trebleNote.Position, currentKillingType);
            }
        }
        else
        {
            isHittingTreble = false;
            isHittingBass = false;
        }
    }

    /// <summary>
    /// Controls color of player on hit, and puts hitting on cooldown, so player can't spam.
    /// </summary>
    IEnumerator Hitting()
    {
        UpdateHitColor();
        yield return new WaitForSeconds(0.05f);
        isHittingTreble = false;
        trebleAnimator.SetBool("isHitting", isHittingTreble);
        killable = false;
        UpdateHitColor();
    }

    IEnumerator HittingBass()
    {
        yield return new WaitForSeconds(0.05f);
        isHittingBass = false;
        bassAnimator.SetBool("isHitting", isHittingBass);
    }

    public void UpdateHitColor()
    {
        return;
        if (isHittingTreble)
            sprite.color = Color.blue;
        else
            sprite.color = Color.white;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        return;
        if (killable)
        {
            try
            {
                if (currentKillingType == collision.GetComponent<Note>().GetNoteType())
                    collision.GetComponent<Note>().Death();
            }
            catch (Exception e)
            {

            }


        }
    }
}
