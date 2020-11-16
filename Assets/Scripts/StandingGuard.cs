using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class StandingGuard : MonoBehaviour
{
    //public bool playerDetected;
    public bool hacked;

    public float timeDisabledWhileHacked = 5f;
    public float timeBeforeDetected = 0.3f;

    private Coroutine detectPlayerCoroutine;

    private void Start()
    {
        //playerDetected = false;
        hacked = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals(Constants.PlayerTag))
        {
            //playerDetected = true;
            Debug.Log("AH");
            detectPlayerCoroutine = StartCoroutine(DetectPlayer());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals(Constants.PlayerTag))
        {
            //playerDetected = false;
            Debug.Log("OH");
            StopCoroutine(detectPlayerCoroutine);
        }
    }

    private IEnumerator DetectPlayer()
    {
        yield return new WaitForSeconds(timeBeforeDetected);
        Debug.Log("CAUGHT");
    }

    public void Hack()
    {
        hacked = true;
        StartCoroutine(Restore());
    }

    private IEnumerator Restore()
    {
        yield return new WaitForSeconds(timeDisabledWhileHacked);
        hacked = false;
    }
}
