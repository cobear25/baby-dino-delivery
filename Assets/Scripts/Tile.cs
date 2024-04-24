using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DinoType
{
    tRex, triceratops, stegosaurus, brachiosaurus, paralophosaurus, ankylosaurus
}
public class Tile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer shadowSpriteRenderer;
    public GameObject whiteCircle;
    public Sprite tRexSprite;
    public Sprite triSprite;
    public Sprite ankSprite;
    public Sprite brachSprite;
    public Sprite paraSprite;
    public Sprite stegSprite;
    public DinoType dinoType;
    public GameController gameController;
    public int x = 0;
    public int y = 0;
    public bool lockedIn = false;

    void Start()
    {
    }

    public void SetType(DinoType dinoType)
    {
        this.dinoType = dinoType;
        if (dinoType == DinoType.tRex)
        {
            spriteRenderer.sprite = tRexSprite;
            shadowSpriteRenderer.sprite = tRexSprite;
        }
        if (dinoType == DinoType.triceratops)
        {
            spriteRenderer.sprite = triSprite;
            shadowSpriteRenderer.sprite = triSprite;
        }
        if (dinoType == DinoType.stegosaurus)
        {
            spriteRenderer.sprite = stegSprite;
            shadowSpriteRenderer.sprite = stegSprite;
        }
        if (dinoType == DinoType.ankylosaurus)
        {
            spriteRenderer.sprite = ankSprite;
            shadowSpriteRenderer.sprite = ankSprite;
        }
        if (dinoType == DinoType.brachiosaurus)
        {
            spriteRenderer.sprite = brachSprite;
            shadowSpriteRenderer.sprite = brachSprite;
        }
        if (dinoType == DinoType.paralophosaurus)
        {
            spriteRenderer.sprite = paraSprite;
            shadowSpriteRenderer.sprite = paraSprite;
        }
    }

    public void SetXY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void LockIn()
    {
        whiteCircle.SetActive(true);
        Invoke("_LockIn", 0.1f);
    }

    void _LockIn()
    {
        lockedIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color ColorForDinoType(DinoType dinoType)
    {
        switch (dinoType)
        {
            case DinoType.tRex:
                return new Color(105f/255f, 195f/255f, 105f/255f);
            case DinoType.triceratops:
                return new Color(206f/255f, 82f/255f, 82f/255f);
            case DinoType.stegosaurus:
                return new Color(251f/255f, 200f/255f, 72f/255f);
            case DinoType.brachiosaurus:
                return new Color(28f/255f, 155f/255f, 174f/255f);
            case DinoType.paralophosaurus:
                return new Color(169f/255f, 94f/255f, 219f/255f);
            case DinoType.ankylosaurus:
                return new Color(242f/255f, 124f/255f, 34f/255f);
            default:
                return Color.blue;
        }
    }

    private void OnMouseDown() {
        transform.localScale = new Vector2(1.2f, 1.2f);
        gameController.TileSelected(this);
    }

    private void OnMouseUp() {
        transform.localScale = new Vector2(1.0f, 1.0f);
    }

    private void OnMouseEnter() {
        if (Input.GetMouseButton(0)) {
            gameController.DraggedOverTile(this);
        }
    }

    public IEnumerator MoveToPos(Vector2 pos, float speed) {
        Vector2 startPos = transform.position;
        var step = (speed / (startPos - pos).magnitude) * Time.fixedDeltaTime;  
        float t = 0;
        while (t <= 1.0f) {
            t += step;
            transform.position = Vector2.Lerp(startPos, pos, t);
            yield return new WaitForFixedUpdate();
        }
        transform.position = pos;
    }
}
