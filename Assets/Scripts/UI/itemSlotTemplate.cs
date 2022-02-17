using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class itemSlotTemplate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //UI_Tooltip tooltip = GameObject.Find("UI_Tooltip").GetComponent<UI_Tooltip>();

    public UI_Tooltip tooltip;

    public string text;

    public float delay = 1f;

    private bool mouseIsHovering;
    private float mouseHoverTime;
    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        if (tooltip == null)
        {
            var canvases = Resources.FindObjectsOfTypeAll<UI_Tooltip>();
            if (canvases.Length > 0)
                tooltip = canvases[0];
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("The cursor entered the selectable UI element.");
        mouseIsHovering = true;
        mouseHoverTime = 0;
        //tt.ShowTooltip_Static("Test Text");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("The cursor exited the selectable UI element.");
        mouseIsHovering = false;
        HideTooltip();
    }

    private void Update() {
        // Cancel on any mouse down
        // not via event because we might not receive it
        if (Input.GetMouseButtonDown(0))
        {
            mouseIsHovering = false;
            HideTooltip();
        }

        if (mouseIsHovering)
        {
            mouseHoverTime += Time.unscaledDeltaTime;
            if (mouseHoverTime >= delay)
                ShowTooltip();
        }
    }

    private void ShowTooltip()
    {
        if (tooltip != null)
            tooltip.ShowTooltip(text);
    }

    private void HideTooltip()
    {
        if (tooltip != null)
            tooltip.HideTooltip();
    }
}
