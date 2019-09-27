using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer sprite;

    [SerializeField]
    private ScreenFlash screenFlash;

    private bool isHitting;

    private bool killable;
    

    void Start()
    {
        sprite = GetComponent < SpriteRenderer >();
        isHitting = false;
    }

    void Update()
    {
        if(Input.GetKeyDown("a")) 
        {
            isHitting = true;
            Debug.Log ("press");
            StartCoroutine (Hitting ());
            if (Conductor.Instance.IsQuarterBeat () == true) 
            {
                screenFlash.Flash ();
                Debug.Log ("Hit");
                killable = true;
            }
            
        }
        else 
        {
            
            isHitting = false;
        }
    }

    IEnumerator Hitting() 
    {
        UpdateHitColor();
        yield return new WaitForSeconds(0.1f);
        Debug.Log ("unpress");
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


