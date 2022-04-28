using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Navigation : MonoBehaviour
{
    static readonly int blockSize = 4;
    public float sightRange;
    public bool isInRange;
    public LayerMask whatIsChest;
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

    public int steps_min, steps_max, steps;

    public Light torchLight;
    public int lightLevel;
    public int minLight = 6;
    public bool increaseLightLevel;
    public bool decreaseLightLevel;
    public bool useFog;

    static Vector3Int SAVE_POSITION = new Vector3Int(0,1,0);
    Vector3 lastPos;
    static int SAVE_ROTATION_Y;

    private Vector3 testPos;

    public static GameObject INSTANCE;

    public EnemyEncounter[] enemy_encounters;

    public GameObject chestHud;

    public Color fog;

    [System.Serializable]
    public enum State { Idle, Turning, Moving }

    [System.Serializable]
    public enum NavAction { Nothing, Forward, Backward, Left, Right}

    private void Awake()
    {
        state = State.Idle;
        // DontDestroyOnLoad(this.gameObject);
        if(INSTANCE == null)
        {
            INSTANCE = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
        //INSTANCE.SetActive(true);
        // if (EndTrigger.UPSTAIRCOLLISION)
        // {
        //     Debug.Log("going back to 010");
        //     transform.position = new Vector3Int(0, 1, 0);
        //     //SAVE_POSITION = new Vector3Int(0,1,0);
        //     Debug.Log(transform.position);
        //     //respawn();
        //     transform.eulerAngles = new Vector3Int(0, 180, 0);
        // }
        if(!EndTrigger.UPSTAIRCOLLISION || !EndTrigger.DOWNSTAIRCOLLISION)
        {
            transform.position = SAVE_POSITION;
            transform.eulerAngles = new Vector3Int(0, SAVE_ROTATION_Y, 0);
        }
        //EndTrigger.STAIRCOLLISION = false;
        transform.position = new Vector3Int(0, 1, 0);

        SetRandomSteps();
        CalculateSpeeds();
        actionQueue = new List<NavAction>();
        ChangeLightLevel(0);
    }

    private void CalculateSpeeds()
    {
        calculateSpeeds = false;

        if(moveTime != 0)
            moveSpeed = blockSize / moveTime;
        else
            moveSpeed = float.MaxValue;
        
        if(rotateTime != 0)
            rotateSpeed = 90 / rotateTime;
        else
            rotateSpeed = float.MaxValue;
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
        lastPos = transform.position;
        if(steps == 0)
        {
            StartEncounter(enemy_encounters[Random.Range(0,enemy_encounters.Length)]);
            SetRandomSteps();
            INSTANCE.SetActive(false);
            //set inactive
        }
        isInRange = Physics.CheckSphere(transform.position, sightRange, whatIsChest);
        if(isInRange){
            UI_Chest.UI_CHEST.OpenChestUI();
            if (Input.GetKeyDown(KeyCode.F))
                {
                    UI_Chest.UI_CHEST.CloseChestUI();
                    Debug.Log("OPEN DA CHEST!~");
                    FindObjectOfType<Chest>().fillChest();

                    if (UI_LootMenu.UI_LOOTMENU == null) {
                        UI_LootMenu lootMenu = null;
                        var canvases = Resources.FindObjectsOfTypeAll<UI_LootMenu>();
                        if (canvases.Length > 0)
                        lootMenu = canvases[0];

                        if (lootMenu != null)
                        lootMenu.OpenLootUI();
                    }
                    Inventory tmp = FindObjectOfType<Chest>().getChestInventory(Navigation.INSTANCE.transform.position);
                    UI_LootMenu.UI_LOOTMENU.SetInventory(tmp);
                    UI_LootMenu.UI_LOOTMENU.OpenLootUI();

                }
        }else{
            UI_Chest.UI_CHEST.CloseChestUI();
        }

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
                    StartCoroutine(Move(Vector3Int.RoundToInt(transform.forward * blockSize)));//blockSize));
                    break;
                case NavAction.Backward:
                    StartCoroutine(Move(Vector3Int.RoundToInt(-transform.forward * blockSize)));// -blockSize));
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

    IEnumerator Move(Vector3Int move)//float distance)
    {
         
        actionQueue.RemoveAt(0);

        bool blocked = Physics.Raycast(transform.position, move, blockSize);
        
        if (blocked)
            yield break;

        state = State.Moving;
        steps--;

        //transform.DOMove(transform.position + move, moveTime);

        Vector3 finalPos = transform.position + move;
        SAVE_POSITION = Vector3Int.FloorToInt(finalPos);
        //Vector3Int finalPos = new Vector3Int(Mathf.RoundToInt(transform.position.x) + move.x, Mathf.RoundToInt(transform.position.y) + move.y, Mathf.RoundToInt(transform.position.z) + move.z);
        Vector3 toMove = move;

        while (toMove != Vector3Int.zero)
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
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);
        int z = Mathf.RoundToInt(transform.position.z);
        int yRotate = Mathf.RoundToInt(transform.eulerAngles.y / 90) * 90;
        
        transform.position = new Vector3(x, y, z);
        if (x % blockSize == 0 && z % blockSize == 0)
        {
            
            SAVE_POSITION = new Vector3Int(x, y, z);
            SAVE_ROTATION_Y = yRotate;
        }
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
                transform.Rotate(Vector3Int.up, toRotateThisFrame);
                toRotate -= toRotateThisFrame;
            }
            else
            {
                transform.Rotate(Vector3Int.up, toRotate);
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
            RenderSettings.fogMode = FogMode.Linear;
            Camera.main.backgroundColor = fog;
        }
    }

    void StartEncounter(EnemyEncounter enemyencounter)
    {
        BattleManager.SetENEMY_ENCOUNTER(enemyencounter);
        //DungeonGenerator.SAVED_DUNGEON.SetActive(false);
        //set inactive
        SceneManager.LoadScene("Battle");
    }

    void SetRandomSteps()
    {
        steps = Random.Range(steps_min, steps_max + 1);
    }

    public void respawn()
    {
        transform.position = new Vector3(0, 1, 0);
    }

    public static void Clear()
    {
        // Navigation.INSTANCE.SetActive(true);
        Navigation.INSTANCE.transform.position = new Vector3(0,1,0);
        // Navigation.INSTANCE.SetActive(false);
    }
    public void SetState()
    {
        state = State.Idle;
    }
}