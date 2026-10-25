using UnityEngine;
using System;

/* 
 * A target that slides across its lane
 * It is the product of the builder and the subject of the observer pattern: it raises OnTargetHit when shot
*/

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Target : MonoBehaviour
{
    public static event Action<int> OnTargetHit;

    [SerializeField] private float despawnMargin = 1f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private float speed;
    private int pointValue;
    private float direction = 1f;
    private bool isHit;

    // Caches the renderer and collider the builder configures
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Applies the values chosen by the builder and fits the collider to the new sprite
    public void Initialize(Sprite sprite, Color color, float speed, int pointValue, float direction)
    {
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
            boxCollider.size = sprite.bounds.size;
            boxCollider.offset = sprite.bounds.center;
        }

        spriteRenderer.color = color;
        spriteRenderer.flipX = direction < 0f;
        this.speed = speed;
        this.pointValue = pointValue;
        this.direction = direction;
    }

    // Moves the target along its lane and removes it after it passes the far edge of the screen
    private void Update()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);

        if (transform.position.x * direction > ScreenBounds.HalfWidth + despawnMargin)
        {
            Destroy(gameObject);
        }
    }

    // Notifies every observer of the points earned and removes the target. Ignores repeat hits in the same frame
    public void Hit()
    {
        if (isHit)
        {
            return;
        }

        isHit = true;
        OnTargetHit?.Invoke(pointValue);
        Destroy(gameObject);
    }
}