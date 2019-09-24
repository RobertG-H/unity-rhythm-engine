using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField]
    private float speed =-4;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate () 
    {
        rb.velocity = new Vector2 (speed, 0.0f);    
    }
}
