using Assets.Scripts.Environment;
using Assets.Scripts.Interfaces;
using UnityEngine;
using static Assets.Scripts.Helpers.Helpers;

public class Keypad : MonoBehaviour, IHackable
{
    public bool hacked;
    public Sprite hackedSprite;
    public Sprite normalSprite;

    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        hacked = false;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        NullChecker(_spriteRenderer, "Sprite Renderer not found on child. Please attach to child.");

        _spriteRenderer.sprite = normalSprite;
    }

    public bool IsHacked()
    {
        return hacked;
    }

    public void Hacked()
    {
        _spriteRenderer.sprite = hackedSprite;
        Door.Instance.Unlock();
    }
}
