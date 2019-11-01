using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField]
    private float speed = -4;
    [SerializeField]
    private GameObject deathParticles;

    private NoteType noteType;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(speed, 0.0f);
    }

    public void Death()
    {
        Instantiate(deathParticles, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetNoteType(NoteType type)
    {
        noteType = type;
    }

    public NoteType GetNoteType()
    {
        return noteType;
    }
}
