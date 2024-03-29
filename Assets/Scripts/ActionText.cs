using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ActionText : MonoBehaviour
{
    static Canvas canvas;
    RectTransform rectTransform;
    RectTransform parentRectTransform;
    TextMeshProUGUI tmpUGUI;

    public bool activate;
    public bool activate2;
    public bool activate3;
    public float param1;
    public float param2;
    public float param3;

    bool destroyThis;
    bool tweening;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = GetComponentInParent<RectTransform>();
        tmpUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (activate)
            Move(param1, param2, param3);
        if (activate2)
            MoveRelative(param1, param2, param3);
        if (activate3)
            FadeAndDestroy(param3);

        activate3 = activate2 = activate = false;

        if (destroyThis && !tweening)
            Destroy(gameObject, Time.deltaTime);
    }

    public void Move(float x, float y, float time)
    {
        StartCoroutine(MoveCR(x, y, time));
    }
    public void MoveRelative(float x, float y, float time)
    {
        StartCoroutine(MoveCR(rectTransform.anchoredPosition.x + x, rectTransform.anchoredPosition.y + y, time));
    }

    public IEnumerator MoveCR(float x, float y, float time)
    {
        tweening = true;

        yield return rectTransform.DOAnchorPos(new Vector2(x, y), time, true);

        tweening = false;
    }

    public void FadeAndDestroy(float time)
    {
        StartCoroutine(FadeAndDestroyCR(time));
    }

    public IEnumerator FadeAndDestroyCR(float time)
    {
        Color initialColor = tmpUGUI.color;
        float timer = time;

        while(timer > 0)
        {
            tmpUGUI.color = initialColor * timer / time;

            timer -= Time.deltaTime;
            yield return null;
        }

        destroyThis = true;
    }
}
