using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JUMP_AMOUNT = 90f;

    private static Bird instance;



    public static Bird GetInstance()
    {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private Rigidbody2D birdRigidBody2D;
    private State state;
    public bool autoStart = true;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead,
    }

    private void Awake()
    {
        instance = this;
        birdRigidBody2D = GetComponent<Rigidbody2D>();
        birdRigidBody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (TestInput() || autoStart)
                {
                    state = State.Playing;
                    birdRigidBody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (TestInput())
                {
                    Jump();
                }
                break;
            case State.Dead:
                break;
        }
    }
    private bool TestInput()
    {
        //return false;
        
        return 
            Input.GetKeyDown(KeyCode.Space) || 
            Input.GetMouseButtonDown(0) ||
            Input.touchCount > 0;
    }
    public void Jump()
    {
        birdRigidBody2D.velocity = Vector2.up * JUMP_AMOUNT;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        int checkpointLayer = 7;
        if (collider.gameObject.layer == checkpointLayer)
        {
            // Touched a checkpoint
            Debug.Log("Touched CheckPoint");
        }
        else
        {
            birdRigidBody2D.bodyType = RigidbodyType2D.Static;
            if (OnDied != null) OnDied(this, EventArgs.Empty);
        }
    }

    public void Reset()
    {
        birdRigidBody2D.velocity = Vector2.zero;
        birdRigidBody2D.bodyType = RigidbodyType2D.Static;
        transform.position = Vector2.zero;
        state = State.WaitingToStart;
    }
    public float GetVelocityY()
    {
        return birdRigidBody2D.velocity.y;
    }
}
