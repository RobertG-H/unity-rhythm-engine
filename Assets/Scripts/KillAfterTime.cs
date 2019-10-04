using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAfterTime : MonoBehaviour
{
    [SerializeField]
    private float timeToKill;
    // Start is called before the first frame update
    void Start()
    {
        Destroy (gameObject, timeToKill);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
