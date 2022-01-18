using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    static readonly float blockSize = 4;

    public float moveTime;
    public float rotateTime;
    public bool calculateSpeeds;
    public float moveSpeed;
    public float rotateSpeed;
    public bool forward;
    public bool backward;
    public bool turnLeft;
    public bool turnRight;
    public State state;

    [System.Serializable]
    public enum State { Idle, Turning, Moving }

    private void Awake()
    {
        CalculateSpeeds();
    }

    private void CalculateSpeeds()
    {
        calculateSpeeds = false;
        moveSpeed = blockSize / moveTime;
        rotateSpeed = 90 / rotateTime;
    }

    void Update()
    {
        if (calculateSpeeds)
            CalculateSpeeds();

        if (state == State.Idle)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                forward = true;
            if (Input.GetKey(KeyCode.DownArrow))
                backward = true;
            if (Input.GetKey(KeyCode.LeftArrow))
                turnLeft = true;
            if (Input.GetKey(KeyCode.RightArrow))
                turnRight = true;

            if (turnLeft && turnRight)
                turnLeft = turnRight = false;
            else if (turnLeft)
                StartCoroutine(Turn(-90));
            else if (turnRight)
                StartCoroutine(Turn(90));
            else if (forward)
                StartCoroutine(Move(blockSize));
            else if (backward)
                StartCoroutine(Move(-blockSize));
        }
    }

    IEnumerator Move(float distance)
    {
        forward = backward = turnLeft = turnRight = false;

        bool blocked = Physics.Raycast(transform.position, transform.forward * (distance < 0 ? -1 : 1), blockSize);
        if (blocked)
            yield break;

        state = State.Moving;
        float toMove = distance;

        while(toMove != 0)
        {
            float toMoveThisFrame = Time.deltaTime * moveSpeed * (distance < 0 ? -1 : 1);

            if (toMove > 0 &&
                toMove > toMoveThisFrame)
            {
                transform.position += transform.forward * toMoveThisFrame;
                toMove -= toMoveThisFrame;
            }
            else if(toMove < 0 &&
                toMove < toMoveThisFrame)
            {
                transform.position += transform.forward * toMoveThisFrame;
                toMove -= toMoveThisFrame;
            }
            else
            {
                transform.position += transform.forward * toMove;
                toMove = 0;
            }

            yield return null;
        }

        state = State.Idle;
    }

    IEnumerator Turn(float angle)
    {
        forward = backward = turnLeft = turnRight = false;
        state = State.Turning;
        float toRotate = angle;

        while(toRotate != 0)
        {
            float toRotateThisFrame = Time.deltaTime * rotateSpeed * (angle < 0 ? -1 : 1);

            Debug.Log(Time.deltaTime + " * " + rotateSpeed + " = " + toRotateThisFrame);
            Debug.Log(toRotate < toRotateThisFrame);

            if (toRotateThisFrame > 0 &&
                toRotate > toRotateThisFrame)
            {
                transform.Rotate(Vector3.up, toRotateThisFrame);
                toRotate -= toRotateThisFrame;
            }
            else if(toRotateThisFrame < 0 &&
                toRotate < toRotateThisFrame)
            {
                transform.Rotate(Vector3.up, toRotateThisFrame);
                toRotate -= toRotateThisFrame;
            }
            else
            {
                transform.Rotate(Vector3.up, toRotate);
                toRotate = 0;
            }

            yield return null;
        }

        state = State.Idle;
    }
}
