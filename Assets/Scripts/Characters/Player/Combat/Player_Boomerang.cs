 using System;
using UnityEngine;

public class Player_Boomerang : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float returnSpeed = 7f;
    [SerializeField] private float speedMultiplier = .3f;
    private float maxSpeed = 20;

    [SerializeField] private LayerMask layerCollision;
    [SerializeField] private SO_WeaponProperties weaponProperties;

    private bool isActive = false;
    private bool isReturning = false;

    private Vector2 targetPoint; 
    private Vector2 direction;

    private GameObject playerPos;

    public void InitBoomerang()
    {
        if (GameManager.Instance == null) return;

        playerPos = GameManager.Instance.GetPlayer.gameObject;
        transform.position = playerPos.transform.position;

        direction = GameManager.Instance.GetPlayer.GetDirection.normalized;
        targetPoint = (Vector2)this.transform.position + direction * weaponProperties.maxScope;

        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return; 

        if (!isReturning)
        {
            MoveTowards(targetPoint);

            if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
            {
                Back();
            }
        }
        else
        {
            MoveTowards(playerPos.transform.position);

            if (Vector2.Distance(transform.position, playerPos.transform.position) < 0.3f)
            {
                ResetBoomerang(); 
            }
        }
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 currentPosition = transform.position;
        Vector2 moveDirection = (target - currentPosition).normalized;

        transform.position += (Vector3)(moveDirection * (isReturning ? returnSpeed : speed) * Time.deltaTime);
    }

    private void Back()
    {
        isReturning = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.TryGetComponent(out IHealthCharacterControl enemyHealth))
        {
            speed = Math.Min(speed + speedMultiplier, maxSpeed);
            enemyHealth.RemoveHearts(weaponProperties.damage);
        }

        if (((1 << collision.gameObject.layer) & layerCollision) != 0)
        {
            Back();
        }
    }

    private void ResetBoomerang()
    {
        speed = 13;
        isActive = false; 
        isReturning = false;

        this.gameObject.SetActive(false);
    }
}