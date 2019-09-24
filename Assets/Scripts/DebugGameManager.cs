﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Conductor.Instance.StartSong();
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown("r")) 
        {
            SceneManager.LoadScene ("MusicScore");
        }
    }
}
