﻿using System;
using System.Collections;
using Assets.Scripts.Environment;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Constants;
using Assets.Scripts.Player;
using static Assets.Scripts.Helpers.Helpers;

public class Keypad : MonoBehaviour, IHackable
{
    public bool hacked;
    public Sprite hackedSprite;
    public Sprite normalSprite;

    [Header("Reaction Images")]
    public Sprite hackedReactionSprite;

    private Image _reactionImage;

    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        hacked = false;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _reactionImage = GetComponentInChildren<Image>();

        NullChecker(_reactionImage, "Image is missing on Guard canvas. Please add to child.");
        NullChecker(_spriteRenderer, "Sprite Renderer not found on child. Please attach to child.");

        _spriteRenderer.sprite = normalSprite;
        _reactionImage.enabled = false;
    }

    private void OnEnable()
    {
        PlayerCaught.OnCaught += ResetKeypad;
    }

    private void OnDisable()
    {
        PlayerCaught.OnCaught -= ResetKeypad;
    }

    public bool IsHacked()
    {
        return hacked;
    }

    public void Hacked()
    {
        _spriteRenderer.sprite = hackedSprite;
        _reactionImage.enabled = true;
        _reactionImage.sprite = hackedReactionSprite;
        hacked = true;
        Door.Instance.Unlock();
    }

    private void ResetKeypad()
    {
        if (!IsHacked()) return;
        StartCoroutine(UnhackKeypad());
    }

    private IEnumerator UnhackKeypad()
    {
        yield return new WaitForSeconds(Delays.CaughtDelay);
        _spriteRenderer.sprite = normalSprite;
        _reactionImage.enabled = false;
        hacked = false;
    } 
}
