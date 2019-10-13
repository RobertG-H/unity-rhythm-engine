using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{

    private int songPositionInBeatsInt;
    private double rangeCalc;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        songPositionInBeatsInt = (int)Conductor.Instance.GetSongBeat();
        rangeCalc = songPositionInBeatsInt;
        // Debug.Log(songPositionInBeatsInt);
        if (Input.GetKeyDown("p"))
        {
            if(Conductor.Instance.GetSongBeat() < rangeCalc + 0.4 && Conductor.Instance.GetSongBeat() > rangeCalc - 0.4)
            {
                Debug.Log("Hit");
            }
            else
            {
                Debug.Log("Miss");
            }

        }
    }
}
