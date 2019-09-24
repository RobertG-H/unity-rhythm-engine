using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject note;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 128; i++) {
            Instantiate (note, new Vector3 (1.75f+i*2, 0, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
