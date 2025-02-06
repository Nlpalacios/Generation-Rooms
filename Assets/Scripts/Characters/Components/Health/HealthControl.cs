using System;
using System.Collections;
using UnityEngine;

public class HealthControl : MonoBehaviour, IHealthCharacterControl
{
    [Header("Animation")]
    [SerializeField] private float timeReceiveAttack;
    [SerializeField] private float forceMagnitude;

    [Header("Health")]
    [SerializeField] private int actual_hearts = 0;
    [SerializeField, Range(0, 10)] private int max_hearts = 5;

    //Private variables
    private Rigidbody2D rb2D;

    //Event delegate
    public Action OnDeathCharacter;
    public Action<int> OnHealthChanged;

    public int GetCurrentHealth { get { return actual_hearts; } }
    public int GetMaxHearts {  get { return max_hearts; } }
    public int SetMaxHeart { set => max_hearts = value; }

    private void Start()
    {
        actual_hearts = GetMaxHearts;
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void ResetVariables()
    {
        actual_hearts = max_hearts;
    }

    public void AddHeart(int hearts)
    {
        actual_hearts = Mathf.Clamp(actual_hearts + hearts, 0, GetMaxHearts);
    }

    public void RemoveHearts(int damage)
    {
        actual_hearts = Mathf.Max(actual_hearts - damage, 0);
        TakeDamage();

        OnHealthChanged?.Invoke(damage);
    }

    public void TakeDamage()
    {
        StartCoroutine(takeDamageAnimation());
    }

    IEnumerator takeDamageAnimation()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || !rb2D) yield break;

        //Vector2 backwardForce = -rb2D.velocity.normalized * forceMagnitude;
        //rb2D.AddForce(backwardForce, ForceMode2D.Impulse);

        Color initColor = spriteRenderer.color;
        Color blinkColor = new Color(initColor.r, initColor.g, initColor.b, 0);

        float elapsedTime = 0f;
        float blinkInterval = 0.1f;

        while (elapsedTime < timeReceiveAttack)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkInterval / 2);

            spriteRenderer.color = initColor; // Restore sprite color
            yield return new WaitForSeconds(blinkInterval / 2);

            elapsedTime += blinkInterval;
        }

        spriteRenderer.color = initColor;

        // Trigger Event
        if (actual_hearts <= 0)
        {
            OnDeathCharacter?.Invoke();
        }
    }
}
