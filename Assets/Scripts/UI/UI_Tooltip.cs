using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Tooltip : MonoBehaviour
{

    [SerializeField]
    private Camera uiCamera;
    private TextMeshProUGUI tooltipText;
    RectTransform backgroundTransform;
    private void Awake() {
        backgroundTransform = transform.Find("background").GetComponent<RectTransform>();
        tooltipText = transform.Find("text").GetComponent<TextMeshProUGUI>();

        //ShowTooltip("test string");

        
    }

    private void Update() {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);
        transform.localPosition = localPoint;

    }
    public void ShowTooltip(string tooltipString) {
        if (tooltipString == "") return;
        gameObject.SetActive(true);
        tooltipText.text = tooltipString;
        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
        backgroundTransform.sizeDelta = backgroundSize;
    }

    public void HideTooltip() {
        gameObject.SetActive(false);
    }

}
