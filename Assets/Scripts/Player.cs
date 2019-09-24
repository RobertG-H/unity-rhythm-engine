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
    

    void Start()
    {
        sprite = GetComponent < SpriteRenderer >();
        isHitting = false;
    }

    void Update()
    {
        if(Input.GetKeyDown("a") && isHitting == false) 
        {
            isHitting = true;
            Debug.Log ("press");
            StartCoroutine (Hitting());
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
        if (isHitting)
        {
            screenFlash.Flash ();
            Debug.Log ("Hit");
            GameObject.Destroy (collision.gameObject);

        }
    }
}


