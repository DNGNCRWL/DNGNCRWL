using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorAverageLight : MonoBehaviour
{
    static Light[] lights;
    static bool foundLights;
    SpriteRenderer sr;
    public float distanceFromLights;

    void FindLights()
    {
        var lightsFound = Object.FindObjectsOfType<Light>();
        lights = (Light[])lightsFound;
        foundLights = true;
        Debug.Log("Found " + lightsFound.Length + " lights");
    }

    private void Awake()
    {
        if (!foundLights)
        {
            FindLights();
        }

        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        distanceFromLights = 0;

        foreach (Light light in lights)
        {
            if (!light.gameObject.activeInHierarchy)
                continue;
            Vector3 distance = (transform.position - light.transform.position);
            float rangesFromLightInverted = (distance.sqrMagnitude > 0) ?
                light.range / distance.sqrMagnitude :
                float.MaxValue;
            float multiplier = (light.color.r + light.color.g + light.color.b)/3f * light.intensity;
            distanceFromLights += Mathf.Min(1, rangesFromLightInverted * multiplier);
        }

        if (lights.Length > 0)
            sr.color = new Color(1, 1, 1, distanceFromLights * distanceFromLights);
    }

}
