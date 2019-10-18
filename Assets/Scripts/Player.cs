using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Graphic objects
    private SpriteRenderer sprite;
    [SerializeField]
    private ScreenFlash screenFlash;

    // Private bools
    private bool isHitting;
    private bool killable;


    void Start()
    {
        sprite = GetComponent < SpriteRenderer >();
        isHitting = false;
    }

    void Update()
    {
        if(Input.GetKeyDown("space")  || Input.GetKeyDown("m")) 
        {
            isHitting = true;
            StartCoroutine (Hitting ());
            if (Conductor.Instance.CheckHit () == true)
            {
                screenFlash.Flash ();
                killable = true;
            }
        }
        else
        {
            isHitting = false;
        }
    }

    /// <summary>
    /// Controls color of player on hit, and puts hitting on cooldown, so player can't spam.
    /// </summary>
    IEnumerator Hitting()
    {
        UpdateHitColor();
        yield return new WaitForSeconds(0.05f);
        isHitting = false;
        killable = false;
        UpdateHitColor ();
    }

    public void UpdateHitColor ()
    {
        if (isHitting)
            sprite.color = Color.blue;
        else
            sprite.color = Color.white;
    }

    private void OnTriggerStay2D (Collider2D collision) {
        if (killable)
        {
            collision.GetComponent<Note> ().Death ();
        }
    }
}
