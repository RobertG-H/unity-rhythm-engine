using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject note;
    [SerializeField]
    private GameObject measure;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 128; i++) {
            Instantiate (note, new Vector3 (10.2f+i*4, 0, 0), Quaternion.identity);
            Instantiate (measure, new Vector3 (3.5f+i*16, 0, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
