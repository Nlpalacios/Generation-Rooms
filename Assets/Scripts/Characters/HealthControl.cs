using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HealthControl : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float timeReceiveAttack;
    [SerializeField] private float forceMagnitude;

    [Header("Health")]
    [SerializeField, Range(0, 10)] private int max_hearts = 5;

    //Private variables
    private int actual_hearts = 0;
    private Rigidbody2D rb2D;

    //Event delegate
    public delegate void HealthControlDelegate();
    public event HealthControlDelegate OnDeathCharacter;

    public delegate void HealthChangedDelegate(int currentHealth);
    public event HealthChangedDelegate OnHealthChanged;

    public int GetCurrentHealth { get { return actual_hearts; } }

    public int SetMaxHeart { get => max_hearts; set => max_hearts = value; }

    private void Start()
    {
        actual_hearts = SetMaxHeart;
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void AddHearts(int hearts)
    {
        actual_hearts = Mathf.Clamp(actual_hearts + hearts, 0, SetMaxHeart);
    }

    public void RemoveHearts(int damage)
    {
        actual_hearts = Mathf.Max(actual_hearts - damage, 0);
        StartCoroutine(takeDamageAnimation());

        OnHealthChanged?.Invoke(damage);

        // Disparar evento
        if (actual_hearts <= 0)
        {
            OnDeathCharacter?.Invoke();
        }
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
    }
}
