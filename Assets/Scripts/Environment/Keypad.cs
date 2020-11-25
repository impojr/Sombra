using Assets.Scripts.Environment;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;
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

    public bool IsHacked()
    {
        return hacked;
    }

    public void Hacked()
    {
        _spriteRenderer.sprite = hackedSprite;
        _reactionImage.enabled = true;
        _reactionImage.sprite = hackedReactionSprite;
        Door.Instance.Unlock();
    }
}
