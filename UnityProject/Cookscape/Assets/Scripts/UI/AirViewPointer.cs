using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirViewPointer : MonoBehaviour
{
    void Start()
    {
        MeshRenderer mapPointer = GetComponent<MeshRenderer>();
        mapPointer.material.color = Color.green;
        StartCoroutine("FadeMyPointerToLowerGreen");
    }

    protected IEnumerator FadeMyPointerToFullGreen()
    {
        MeshRenderer mapPointer = GetComponent<MeshRenderer>();
        Color color = new Color(mapPointer.material.color.r, mapPointer.material.color.g, mapPointer.material.color.b, mapPointer.material.color.a);
        mapPointer.material.color = color;
        while (mapPointer.material.color.g < 1f)
        {
            color = new Color(mapPointer.material.color.r, mapPointer.material.color.g + (Time.deltaTime / 1.0f), mapPointer.material.color.b, mapPointer.material.color.a);
            mapPointer.material.color = color;
            yield return null;
        }
        StartCoroutine(FadeMyPointerToLowerGreen());
    }

    protected IEnumerator FadeMyPointerToLowerGreen()
    {
        MeshRenderer mapPointer = GetComponent<MeshRenderer>();
        Color color = new Color(mapPointer.material.color.r, mapPointer.material.color.g, mapPointer.material.color.b, mapPointer.material.color.a);
        mapPointer.material.color = color;
        while (mapPointer.material.color.g > 0.6f)
        {
            color = new Color(mapPointer.material.color.r, mapPointer.material.color.g - (Time.deltaTime / 1.0f), mapPointer.material.color.b, mapPointer.material.color.a);
            mapPointer.material.color = color;
            yield return null;
        }
        StartCoroutine(FadeMyPointerToFullGreen());
    }
}
