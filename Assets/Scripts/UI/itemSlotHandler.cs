using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class itemSlotHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //UI_Tooltip tooltip = GameObject.Find("UI_Tooltip").GetComponent<UI_Tooltip>();

    [SerializeField]
    private UI_Tooltip tooltip;
    [SerializeField]
    private UI_ContextMenu contextMenu;

    public string text;

    public float delay = 1f;
    public Item item;
    public Inventory inventory;

    private bool mouseIsHovering;
    private float mouseHoverTime;
    private RectTransform rectTransform;

    private bool toolTipShown = false;

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

        if (contextMenu == null)
        {
            var canvases = Resources.FindObjectsOfTypeAll<UI_ContextMenu>();
            if (canvases.Length > 0)
                contextMenu = canvases[0];
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("The cursor entered the selectable UI element.");
        mouseIsHovering = true;
        mouseHoverTime = 0;
        //tt.ShowTooltip_Static("Test Text");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("The cursor exited the selectable UI element.");
        mouseIsHovering = false;
        HideTooltip();
    }

    public void OnPointerClick (PointerEventData eventData) 
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Debug.Log("Left click");
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Debug.Log("Middle click");
            //ShowContextMenu();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click");
            ShowContextMenu();
        }
    }

    private void Update() {
        // Cancel on any mouse down
        // not via event because we might not receive it
        if (Input.GetMouseButtonDown(0)) // Left Click
        {
            mouseIsHovering = false;
            HideTooltip();
        }

        if (mouseIsHovering && !toolTipShown)
        {
            mouseHoverTime += Time.unscaledDeltaTime;
            if (mouseHoverTime >= delay )
                ShowTooltip();
        }

        
    }

    private void ShowTooltip()
    {
        if (tooltip != null) {
            tooltip.ShowTooltip(item.description);
            toolTipShown = true;
        }

    }

    private void HideTooltip()
    {
        if (tooltip != null) {
            tooltip.HideTooltip();
            toolTipShown = false;
        }
    }

    private void ShowContextMenu() 
    {
        if (contextMenu!=null && item != null)
            contextMenu.ShowContextMenu(item, inventory);
    }

    private void HideContextMenu()
    {
        if (contextMenu != null)
            contextMenu.HideContextMenu();
    }
}
