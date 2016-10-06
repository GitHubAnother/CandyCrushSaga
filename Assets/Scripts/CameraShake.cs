/* Copyright (c) Vander Amaral
 * This code holds the camera shake
 * I tried to make it the best and easy to change.
 * You can polish it even more, and add functions to it if you bought.
 */

using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static bool startShake = false;
    public static float seconds = 0.0f;
    public static bool started = false;
    public static float quake = 0.2f;
    private Vector3 camPOS;
    public bool is2D;

    void Start()
    {
        camPOS = transform.position;
    }
    void LateUpdate()
    {
        if (startShake)
        {
            transform.position = Random.insideUnitSphere * quake;
            if (is2D) transform.position = new Vector3(transform.position.x, transform.position.y, camPOS.z);
        }

        if (started)
        {
            StartCoroutine(WaitForSecond(seconds));
            started = false;
        }
    }

    public static void shakeFor(float a, float b)
    {
        seconds = a;
        started = true;
        quake = b;
    }

    IEnumerator WaitForSecond(float a)
    {
        camPOS = transform.position;
        startShake = true;
        yield return new WaitForSeconds(a);
        startShake = false;
        transform.position = camPOS;
    }
}
