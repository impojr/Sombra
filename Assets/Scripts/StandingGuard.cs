using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Constants;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class StandingGuard : MonoBehaviour
{
    //public bool playerDetected;
    public bool hacked;
    public SpriteRenderer visor;

    public float timeDisabledWhileHacked = 5f;
    public float timeBeforeDetected = 0.3f;

    private Coroutine _detectPlayerCoroutine;

    private void Start()
    {
        if (visor == null)
            throw new NullReferenceException("Visor is missing. Please add the visor as a child to the object and reference it.");
        
        //playerDetected = false;
        visor.color = Color.white;
        hacked = false;
    }

    private void OnTriggerEnter2D([NotNull] Collider2D other)
    {
        if (!other.tag.Equals(Tags.Player)) return;

        //playerDetected = true;
        Debug.Log("AH");
        _detectPlayerCoroutine = StartCoroutine(DetectPlayer());
    }

    private void OnTriggerExit2D([NotNull] Collider2D other)
    {
        if (!other.tag.Equals(Tags.Player)) return;

        //playerDetected = false;
        visor.color = Color.white;
        Debug.Log("OH");
        StopCoroutine(_detectPlayerCoroutine);
    }

    private IEnumerator DetectPlayer()
    {
        visor.color = Color.yellow;
        yield return new WaitForSeconds(timeBeforeDetected);
        visor.color = Color.red;
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
