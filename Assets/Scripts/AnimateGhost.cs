using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateGhost : MonoBehaviour
{
    public Color ghostColor;
    public SpriteRenderer bodySprite;
    public SpriteRenderer eyesSprite;
    private Ghost ghost;
    public Sprite[] normalSprites;
    public Sprite[] scaredSprites;
    public Sprite[] waningScaredSprites;
    public Sprite[] eyeSprites;
    public float animationTime = 0.25f;
    public int animationFrame { get; private set; }
    public bool loop = true;

    private void Awake()
    {
        ghost = GetComponent<Ghost>();
    }

    // Update is called once per frame
    void Start()
    {
        InvokeRepeating(nameof(NextFrame), animationTime, animationTime);    
    }
    void NextFrame()
    {
        if (!this.bodySprite.enabled)
        {
            return;
        }
        if (!ghost.scared)
        {
            eyesSprite.enabled = true;
            bodySprite.color = ghostColor;
            if (ghost.direction == Vector2.up)
            {
                eyesSprite.sprite = eyeSprites[0];
            }
            else if (ghost.direction == Vector2.down)
            {
                eyesSprite.sprite = eyeSprites[1];
            }
            else if (ghost.direction == Vector2.left)
            {
                eyesSprite.sprite = eyeSprites[2];
            }
            else if (ghost.direction == Vector2.right)
            {
                eyesSprite.sprite = eyeSprites[3];
            }
        }
        else
        {
            eyesSprite.enabled = false;
            bodySprite.color = Color.white;
        }
        this.animationFrame++;

        if (this.animationFrame >= this.normalSprites.Length && this.loop&&!ghost.scared)
        {
            this.animationFrame = 0;
        }
        else if (this.animationFrame >= this.scaredSprites.Length && this.loop && ghost.scared)
        {
            this.animationFrame = 0;
        }
        else if (this.animationFrame >= this.waningScaredSprites.Length && this.loop && ghost.remainingTime<=1)
        {
            this.animationFrame = 0;
        }
        if (this.animationFrame >= 0 && this.animationFrame < this.normalSprites.Length&&!ghost.scared)
        {
            this.bodySprite.sprite = this.normalSprites[this.animationFrame];
        }
        else if (this.animationFrame >= 0 && this.animationFrame < this.scaredSprites.Length&&ghost.remainingTime>1)
        {
            this.bodySprite.sprite = this.scaredSprites[this.animationFrame];
        }
        else if (this.animationFrame >= 0 && this.animationFrame < this.waningScaredSprites.Length && ghost.remainingTime <= 1)
        {
            this.bodySprite.sprite = this.waningScaredSprites[this.animationFrame];
        }
    }
}
