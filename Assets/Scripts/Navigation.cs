using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public List<NavAction> actionQueue;
    static readonly int actionQueueMaxLength = 2;

    public Light torchLight;
    public int lightLevel;
    static readonly int minLight = 5;
    public bool increaseLightLevel;
    public bool decreaseLightLevel;
    public bool useFog;
    public Color fog;

    [System.Serializable]
    public enum State { Idle, Turning, Moving }

    [System.Serializable]
    public enum NavAction { Nothing, Forward, Backward, Left, Right}

    private void Awake()
    {
        CalculateSpeeds();
        actionQueue = new List<NavAction>();
        ChangeLightLevel(0);
    }

    private void CalculateSpeeds()
    {
        calculateSpeeds = false;
        moveSpeed = blockSize / moveTime;
        rotateSpeed = 90 / rotateTime;
    }

    void GetInputTaps()
    {
        if (actionQueue.Count >= actionQueueMaxLength)
            return;
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            actionQueue.Add(NavAction.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            actionQueue.Add(NavAction.Right);
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //    actionQueue.Add(NavAction.Forward);
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //    actionQueue.Add(NavAction.Backward);
    }

    void GetInputHolds()
    {
        if (state != State.Idle ||
           actionQueue.Count > 0)
            return;

        int pressedCount = 0;

        if (Input.GetKey(KeyCode.UpArrow))
            pressedCount++;
        if (Input.GetKey(KeyCode.DownArrow))
            pressedCount++;
        if (Input.GetKey(KeyCode.LeftArrow))
            pressedCount++;
        if (Input.GetKey(KeyCode.RightArrow))
            pressedCount++;

        if (pressedCount != 1)
            return;

        if (Input.GetKey(KeyCode.LeftArrow))
            actionQueue.Add(NavAction.Left);
        if (Input.GetKey(KeyCode.RightArrow))
            actionQueue.Add(NavAction.Right);
        if (Input.GetKey(KeyCode.UpArrow))
            actionQueue.Add(NavAction.Forward);
        if (Input.GetKey(KeyCode.DownArrow))
            actionQueue.Add(NavAction.Backward);
    }

    void GetInputBooleans()
    {

        if (calculateSpeeds)
            CalculateSpeeds();
        if (actionQueue.Count < actionQueueMaxLength && forward)
            actionQueue.Insert(0, NavAction.Forward);
        if (actionQueue.Count < actionQueueMaxLength && backward)
            actionQueue.Insert(0, NavAction.Backward);
        if (actionQueue.Count < actionQueueMaxLength && turnLeft)
            actionQueue.Insert(0, NavAction.Left);
        if (actionQueue.Count < actionQueueMaxLength && turnRight)
            actionQueue.Insert(0, NavAction.Right);
        if (increaseLightLevel)
            ChangeLightLevel(1);
        else if (decreaseLightLevel)
            ChangeLightLevel(-1);

        forward = backward = turnLeft = turnRight = false;
        increaseLightLevel = decreaseLightLevel = false;
    }

    void Update()
    {
        GetInputTaps();
        GetInputHolds();
        GetInputBooleans();
        
        if (Input.GetKeyDown(KeyCode.Z))
            increaseLightLevel = true;
        if (Input.GetKeyDown(KeyCode.X))
            decreaseLightLevel = true;

        if (state == State.Idle &&
            actionQueue.Count > 0)
        {
            switch (actionQueue[0])
            {
                case NavAction.Forward:
                    StartCoroutine(Move(transform.forward * blockSize));//blockSize));
                    break;
                case NavAction.Backward:
                    StartCoroutine(Move(-transform.forward * blockSize));// -blockSize));
                    break;
                case NavAction.Left:
                    StartCoroutine(Turn(-90));
                    break;
                case NavAction.Right:
                    StartCoroutine(Turn(90));
                    break;
            }
        }
    }

    IEnumerator Move(Vector3 move)//float distance)
    {
        actionQueue.RemoveAt(0);

        bool blocked = Physics.Raycast(transform.position, move, blockSize);
        if (blocked)
            yield break;

        state = State.Moving;

        //transform.DOMove(transform.position + move, moveTime);

        Vector3 finalPos = transform.position + move;
        Vector3 toMove = move;

        while (toMove != Vector3.zero)
        {
            Vector3 toMoveThisFrame = Time.deltaTime * moveSpeed * toMove.normalized;

            if (toMove.sqrMagnitude > toMoveThisFrame.sqrMagnitude)
            {
                transform.position += toMoveThisFrame;
                toMove -= toMoveThisFrame;
            }
            else
            {
                transform.position = finalPos;
                //transform.position += toMove;
                toMove = Vector3.zero;
            }

            yield return null;
        }

        state = State.Idle;

        Snap();
    }

    void Snap()
    {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
            );
    }

    IEnumerator Turn(float angle)
    {
        actionQueue.RemoveAt(0);
        state = State.Turning;

        float toRotate = angle;
        int sign = (angle < 0 ? -1 : 1);

        while (toRotate != 0)
        {
            float toRotateThisFrame = Time.deltaTime * rotateSpeed * sign;

            if ((toRotateThisFrame > 0 && toRotate > toRotateThisFrame) ||
                (toRotateThisFrame < 0 &&
                toRotate < toRotateThisFrame))
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

    void ChangeLightLevel(int i)
    {
        increaseLightLevel = decreaseLightLevel = false;

        lightLevel += i;
        lightLevel = lightLevel < 0 ? 0 : lightLevel;

        torchLight.range = blockSize * lightLevel + minLight;

        RenderSettings.fog = useFog;
        if (useFog)
        {
            RenderSettings.fogEndDistance = torchLight.range;
            RenderSettings.fogColor = fog;
        }
    }
}
