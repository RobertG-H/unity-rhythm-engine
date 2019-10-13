using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    [SerializeField]
    private Text audioSourceTime;
    [SerializeField]
    private Text calcSongTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        audioSourceTime.text = "Song Beat Time: " + Conductor.Instance.GetSongBeat();
        calcSongTime.text = "Calc Song Time: " + Conductor.Instance.GetSongTime();
    }
}
