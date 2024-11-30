using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.VisualScripting;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(BoxCollider2D))]
public class CombatSystem : MonoBehaviour
{
    [Header("Current Weapon")]
    [SerializeField] private PlayerWeapon actualWeapon;

    [Header("Attack")]
    [SerializeField] private bool isAttack = false; 
    
    [Header("Detection")]
    [SerializeField] private LayerMask layerEnemies;

    [Header("Rectangle Player Zone")]
    [SerializeField] private float distanceX_Left;
    [SerializeField] private float distanceX_Right;
    [SerializeField] private float distanceY_Top;
    [SerializeField] private float distanceY_Down;

    [Header("Weapons")]
    [SerializeField] private List<SO_WeaponManager> weaponsComponents = new List<SO_WeaponManager>();

    //RECTANGLE
    private Vector2 TopRightcorner;
    private Vector2 BottomLefttcorner;
    private Vector2 directionAttack = Vector2.down;

    //Animations
    private Animator animator;

    //Attack
    private float delay = 0f;

    private Player playerReference;

    public PlayerWeapon ActualWeapon { get => actualWeapon; set => actualWeapon = value; }

    #region Start

    private void Start()
    {
        playerReference = GetComponent<Player>();
    }

    private void OnEnable()
    {
        animator = GetComponent<Animator>(); 
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    #endregion

    #region Attack

    public void CharacterAttack(InputAction.CallbackContext context)
    {
        if (this == null) return;

        //Delay
        if (delay > Time.time) { 
            return;
        }

        delay = Time.time + GetWeapon().delay;
        directionAttack = playerReference.GetDirection;

        AnimationClip clip = GetAnimationFromDirection(ActualWeapon);
        if (!clip) { 
            Debug.Log($"ANIMATION NULL IN {transform.name}"); 
            return;
        }

        if (!isAttack)
            StartCoroutine(PlayAttackAnimation(clip));
    }

    //Method for frame in animation
    public void DetectionAttack()
    {
        UpdateRectangle();
        Collider2D[] colliders = Physics2D.OverlapAreaAll(TopRightcorner, BottomLefttcorner, layerEnemies);

        foreach (var collider in colliders)
        {
            if (!collider || !isAttack) continue;

            HealthControl healthEnemy = collider.GetComponent<HealthControl>();
            if (healthEnemy == null) return;

            healthEnemy.RemoveHearts(GetWeapon().damage);
        }
    }

    #endregion

    #region Animations

    IEnumerator PlayAttackAnimation(AnimationClip attackAnimation)
    {
        isAttack = true;
        animator.Play(attackAnimation.name);
        //DetectionAttack();

        yield return new WaitForSeconds(attackAnimation.length);
        animator.Play("BT_Idle");
        isAttack = false;
    }

    AnimationClip GetAnimationFromDirection(PlayerWeapon typeWeapon)
    {
        SO_WeaponManager Attributes = GetWeapon();

        if (directionAttack.x != 0)
        {
            return directionAttack.x > 0 ? Attributes.attack_Right : Attributes.attack_Left;
        }
        else
        {
            return directionAttack.y > 0 ? Attributes.attack_Up : Attributes.attack_Down;
        }
    }

    #endregion

    #region Getters

    SO_WeaponManager GetWeapon()
    {
        return weaponsComponents.Find(n => n.type == actualWeapon);
    }

    #endregion

    #region Detector

    private void UpdateRectangle()
    {
        float distance = GetWeapon().maxScope * (directionAttack.x >= 1 ? 1 : -1);

        /*
        switch (directionAttack)
        {
            case Vector2 v when v == Vector2.down:
                BottomLefttcorner.x = transform.position.x - distanceX_Left;
                TopRightcorner.x = transform.position.x + distanceX_Right;
                BottomLefttcorner.y = transform.position.y - distanceY_Down + distance;
                TopRightcorner.y = transform.position.y + distanceY_Top;
                break;

            case Vector2 v when v == Vector2.up:
                BottomLefttcorner.x = transform.position.x - distanceX_Left;
                TopRightcorner.x = transform.position.x + distanceX_Right;
                BottomLefttcorner.y = transform.position.y - distanceY_Down;
                TopRightcorner.y = transform.position.y + distanceY_Top - distance;
                break;

            case Vector2 v when v == Vector2.right:
                BottomLefttcorner.x = transform.position.x - distanceX_Left;
                TopRightcorner.x = transform.position.x + distanceX_Right + distance;
                BottomLefttcorner.y = transform.position.y - distanceY_Down;
                TopRightcorner.y = transform.position.y + distanceY_Top;
                break;

            case Vector2 v when v == Vector2.left:
                BottomLefttcorner.x = transform.position.x - distanceX_Left + distance;
                TopRightcorner.x = transform.position.x + distanceX_Right;
                BottomLefttcorner.y = transform.position.y - distanceY_Down;
                TopRightcorner.y = transform.position.y + distanceY_Top;
                break;

            default:
                BottomLefttcorner.x = transform.position.x - distanceX_Left;
                TopRightcorner.x    = transform.position.x + distanceX_Right;
                BottomLefttcorner.y = transform.position.y - distanceY_Down;
                TopRightcorner.y    = transform.position.y + distanceY_Top;
                break;
        }
        */

        float xOffset = directionAttack.x * GetWeapon().maxScope;
        float yOffset = directionAttack.y * GetWeapon().maxScope;

        BottomLefttcorner = new Vector2(
        transform.position.x - distanceX_Left + (directionAttack.x < 0 ? xOffset : 0),
        transform.position.y - distanceY_Down + (directionAttack.y < 0 ? yOffset : 0)
    );

        TopRightcorner = new Vector2(
            transform.position.x + distanceX_Right + (directionAttack.x > 0 ? xOffset : 0),
            transform.position.y + distanceY_Top + (directionAttack.y > 0 ? yOffset : 0)
        );
    }

    private void OnDrawGizmos()
    {
        UpdateRectangle();
        Vector2 centerOffset = (TopRightcorner + BottomLefttcorner) * .5f;
        Vector2 displacementVector = TopRightcorner - BottomLefttcorner;
        float Xprojection = Vector2.Dot(displacementVector, Vector2.right);
        float Yprojection = Vector2.Dot(displacementVector, Vector2.up);

        Vector2 topLeftCorner = new Vector2(-Xprojection * .5f, Yprojection * .5f) + centerOffset;
        Vector2 bottomRightCorner = new Vector2(Xprojection * .5f, -Yprojection * .5f) + centerOffset;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(TopRightcorner, topLeftCorner);
        Gizmos.DrawLine(topLeftCorner, BottomLefttcorner);
        Gizmos.DrawLine(BottomLefttcorner, bottomRightCorner);
        Gizmos.DrawLine(bottomRightCorner, TopRightcorner);
    }

    #endregion
}
