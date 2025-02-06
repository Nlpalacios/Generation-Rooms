using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(BoxCollider2D))]
public class CombatSystem : MonoBehaviour
{
    [Header("Current Weapon")]
    [SerializeField] private PlayerWeapon actualWeapon;

    [Header("Detection")]
    [SerializeField] private LayerMask layerEnemies;

    [Header("Rectangle Player Zone")]
    [SerializeField] private float distanceX_Left;
    [SerializeField] private float distanceX_Right;
    [SerializeField] private float distanceY_Top;
    [SerializeField] private float distanceY_Down;

    [Header("Weapons")]
    [SerializeField] private List<SO_WeaponProperties> weaponsComponents = new List<SO_WeaponProperties>();
    [SerializeField] private List<WeaponDistanceID> prefabDistanceWeapons = new List<WeaponDistanceID>();
     
    [Header("Pooling")]
    [SerializeField] private Transform parentObjects;
    [SerializeField] private int totalPool;
    //Add for more objects in pooling

    private Dictionary<TypeBullet, List<GameObject>> dicBullets = new Dictionary<TypeBullet, List<GameObject>>();

    //RECTANGLE
    private Vector2 TopRightcorner;
    private Vector2 BottomLefttcorner;
    private Vector2 directionAttack = Vector2.down;

    //[Header("Attack")]
    private bool isAttack = false;

    //Animations
    private Animator animator;

    //Attack
    private float delay = 0f;

    private Player playerReference;

    public PlayerWeapon ActualWeapon { get => actualWeapon; set => actualWeapon = value; }

    #region Start

    private void Start()
    {
        InitPooling();

        playerReference = GetComponent<Player>();
        animator = GetComponent<Animator>();

        EventManager.Instance.Subscribe(CombatEvents.OnChangeWeapon, SetWeapon);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    #endregion

    #region Pooling
    private void InitPooling()
    {
        foreach (WeaponDistanceID weapon in prefabDistanceWeapons)
        {
            if (weapon == null || weapon.type == TypeBullet.None) continue;
            dicBullets[weapon.type] = new List<GameObject>();

            for (int i = 0; i < totalPool; i++)
            {
                GameObject newWeapon = Instantiate(weapon.gameObject, parentObjects);
                dicBullets[weapon.type].Add(newWeapon); 
                newWeapon.SetActive(false);
            }
        }
    }

    private GameObject GetBullet(TypeBullet type)
    {
        if (!dicBullets.ContainsKey(type) || dicBullets[type] == null) return null;

        foreach(GameObject newBullet in dicBullets[type])
        {
            if (newBullet.activeInHierarchy) continue;

            newBullet.SetActive(true);
            return newBullet;
        }

        return null;
    }

    #endregion

    #region Attack

    public void CharacterAttack(InputAction.CallbackContext context)
    {
        if (this == null) return;

        //Delay
        if (delay > Time.time) return;
        
        delay = Time.time + GetWeapon().delay;
        directionAttack = playerReference.GetDirection;

        AnimationClip clip = null;

        if (GetWeapon().typeWeapon == TypeCombat.Melee)
             clip = GetAnimationClipFromDirection(ActualWeapon);

        else if (GetWeapon().typeWeapon == TypeCombat.Ranged)
            clip = GetAnimationClipFromDirection(PlayerWeapon.AnimationDistance);

        if (!clip)
        { 
            Debug.Log($"ANIMATION NULL: {ActualWeapon}"); 
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

            if (collider.gameObject.TryGetComponent(out IHealthCharacterControl enemyHealth))
            {
                enemyHealth.RemoveHearts(GetWeapon().damage);
            }
        }
    }

    public void DistanceAttack()
    {
        GameObject boomerang = GetBullet(TypeBullet.Boomerang);
        if (boomerang == null) return;

        if (boomerang.TryGetComponent(out Player_Boomerang player_Boomerang))
        {
            player_Boomerang.InitBoomerang();
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

    AnimationClip GetAnimationClipFromDirection(PlayerWeapon typeWeapon)
    {
        SO_WeaponProperties Attributes = GetWeapon(typeWeapon);

        if (directionAttack.x != 0)
            return directionAttack.x > 0 ? Attributes.attack_Right : Attributes.attack_Left;
        
        else
            return directionAttack.y > 0 ? Attributes.attack_Up : Attributes.attack_Down;
        
    }

    #endregion

    #region Getters

    SO_WeaponProperties GetWeapon(PlayerWeapon typeWeapon = PlayerWeapon.None)
    {
        PlayerWeapon weapon = typeWeapon == PlayerWeapon.None ? actualWeapon : typeWeapon;
        return weaponsComponents.Find(n => n.type == weapon);
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

    private void SetWeapon(object call)
    {
        actualWeapon = (PlayerWeapon)call;
    }
}
