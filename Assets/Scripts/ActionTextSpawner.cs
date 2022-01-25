using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionTextSpawner : MonoBehaviour
{
    public bool defaultTextSpawner;
    public GameObject actionText;
    public int maxNumberOfLines;
    RectTransform actionTextSpawnPosition;
    public float actionTextSpawnOffset;
    public float actionTextTime;
    public float rotationMin;
    public float rotationMax;
    [SerializeField] List<ActionText> actionTexts;

    public string testString;
    public bool setText;
    public bool addText;

    private void Awake()
    {
        if(defaultTextSpawner)
            GameManager.DEFAULT_TEXT_SPAWNER = this;

        actionTextSpawnPosition = GetComponent<RectTransform>();
        actionTexts = new List<ActionText>();
    }

    private void Update()
    {
        if (setText)
        {
            setText = false;
            SetText(testString);
        }

        if (addText)
        {
            addText = false;
            AddText(testString);
        }
    }

    public void SetText(string message)
    {
        ClearText();
        AddText(message);
    }

    public void AddText(string message)
    {
        //create new text object
        GameObject spawnedText = Instantiate(actionText, actionTextSpawnPosition);
        RectTransform spawnedTextRT = spawnedText.GetComponent<RectTransform>();
        ActionText spawnedTextAT = spawnedText.GetComponent<ActionText>();
        TextMeshProUGUI spawnedTextTMP = spawnedText.GetComponent<TextMeshProUGUI>();
        actionTexts.Add(spawnedTextAT.GetComponent<ActionText>());

        //rotate the object
        spawnedTextRT.Rotate(0, 0, Random.Range(rotationMin, rotationMax));

        //offset object based on how many other actionTexts there are
        spawnedTextRT.anchoredPosition = spawnedTextRT.anchoredPosition - Vector2.up * actionTextSpawnOffset * (actionTexts.Count -1);

        spawnedTextTMP.text = message;

        if (actionTexts.Count > maxNumberOfLines)
        {
            ActionText head = actionTexts[0];
            head.MoveRelative(Random.Range(actionTextSpawnOffset, -actionTextSpawnOffset),
                Random.Range(-actionTextSpawnOffset, -2 * actionTextSpawnOffset), actionTextTime);
            head.FadeAndDestroy(actionTextTime);
            actionTexts.Remove(head);

            foreach (ActionText eachAT in actionTexts)
            {
                eachAT.MoveRelative(0, actionTextSpawnOffset, actionTextTime);
            }
        }
    }

    public void ClearText()
    {
        foreach (ActionText at in actionTexts)
        {
            at.MoveRelative(Random.Range(actionTextSpawnOffset, -actionTextSpawnOffset), Random.Range(-actionTextSpawnOffset, -2 * actionTextSpawnOffset), actionTextTime);
            at.FadeAndDestroy(actionTextTime);
        }
        actionTexts.Clear();
    }
}
