using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedAnimation : MonoBehaviour
{

    //The animator controller attached to this GameObject
    private Animator animator;

    //Records the animation state or animation that the Animator is currently in
    private AnimatorStateInfo animatorStateInfo;

    //Used to address the current state within the Animator using the Play() function
    private int currentState;

    // Start is called before the first frame update
    void Start()
    {
        //Load the animator attached to this object
        animator = GetComponent<Animator>();

        //Get the info about the current animator state
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //Convert the current state name to an integer hash for identification
        currentState = animatorStateInfo.fullPathHash;
    }

    // Update is called once per frame
    void Update()
    {
        //Start playing the current animation from wherever the current conductor loop is
        animator.Play(currentState, -1, ((float)Conductor.Instance.GetSongTime()));
        //Set the speed to 0 so it will only change frames when you next update it
        animator.speed = 0;
    }
}
