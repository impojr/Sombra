using System;
using System.Collections;
using Assets.Scripts.Environment;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Constants;
using Assets.Scripts.Player;
using UnityEngine.Experimental.Rendering.Universal;
using static Assets.Scripts.Helpers.Helpers;

public class Keypad : MonoBehaviour, IHackable
{
    public bool hacked;
    public Transform hackLineEndPos;
    public Sprite hackedSprite;
    public Sprite normalSprite;

    [Header("Reaction Images")]
    public Sprite hackedReactionSprite;

    private Image _reactionImage;
    private Light2D _pointLight;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        hacked = false;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _reactionImage = GetComponentInChildren<Image>();
        _pointLight = GetComponentInChildren<Light2D>();

        NullChecker(_reactionImage, "Image is missing on Guard canvas. Please add to child.");
        NullChecker(_spriteRenderer, "Sprite Renderer not found on child. Please attach to child.");
        NullChecker(_pointLight, "Point Light is missing on Guard. Please add to child.");
        NullChecker(hackLineEndPos, "hackLineEndPos is missing. Please add as child.");

        _spriteRenderer.sprite = normalSprite;
        _reactionImage.enabled = false;
        _pointLight.color = Color.red;
    }

    private void OnEnable()
    {
        PlayerCaught.OnCaughtAnimEnded += ResetKeypad;
    }

    private void OnDisable()
    {
        PlayerCaught.OnCaughtAnimEnded -= ResetKeypad;
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
        _pointLight.color = Color.green;
        hacked = true;
        Door.Instance.Unlock();
    }

    public Transform GetHackPos()
    {
        return hackLineEndPos;
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
        _pointLight.color = Color.red;
        hacked = false;
    } 
}
