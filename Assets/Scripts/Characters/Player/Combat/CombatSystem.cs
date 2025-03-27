using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(BoxCollider2D))]
public class CombatSystem : MonoBehaviour
{
    [Header("Current Weapon")]
    [SerializeField] private PlayerWeapon actualDistanceWeapon;
    [SerializeField] private PlayerWeapon actualMeleeWeapon;

    [Header("Detection")]
    [SerializeField] private LayerMask layerEnemies;

    [Header("Rectangle Player Zone")]
    [SerializeField] private float distanceX_Left;
    [SerializeField] private float distanceX_Right;
    [SerializeField] private float distanceY_Top;
    [SerializeField] private float distanceY_Down;

    [Header("Weapons")]
    [SerializeField] private List<SO_WeaponProperties> weaponsComponents = new List<SO_WeaponProperties>();
     
    [Header("Pooling")]
    [SerializeField] private Transform parentObjects;

    private Dictionary<PlayerWeapon, SO_WeaponProperties> weaponDictionary = new Dictionary<PlayerWeapon, SO_WeaponProperties>();
    private Dictionary<PlayerWeapon, List<GameObject>> dicBullets = new Dictionary<PlayerWeapon, List<GameObject>>();

    //RECTANGLE
    private Vector2 TopRightcorner;
    private Vector2 BottomLefttcorner;
    private Vector2 directionAttack = Vector2.down;

    //private bool isAttack = false;
    //Animations
    private Animator animator;

    //Attack
    private float meleeDelay = 0f;
    private float rangedDelay = 0f;
    private Player playerReference;

    public PlayerWeapon GetActualDisWeapon { get => actualDistanceWeapon; set => actualDistanceWeapon = value; }
    public PlayerWeapon GetActualMeleeWeapon { get => actualMeleeWeapon; set => actualMeleeWeapon = value; }

    #region Start
    private void Awake()
    {
        EventManager.Instance.Subscribe(PlayerEvents.OnChangeWeapon, UnlockItem);
    }
    private void Start()
    {
        InitPooling();

        playerReference = GetComponent<Player>();
        animator = GetComponent<Animator>();

        foreach (var weapon in weaponsComponents)
        {
            weaponDictionary[weapon.typeWeapon] = weapon;
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        EventManager.Instance.Unsubscribe(PlayerEvents.OnChangeWeapon, UnlockItem);
    }

    #endregion

    #region Pooling
    private void InitPooling()
    {
        foreach (SO_WeaponProperties weapon in weaponsComponents)
        {
            if (weapon is SO_MeleeWeapon || weapon.typeCombat != TypeCombat.Ranged) continue;

            SO_RangedWeapon dataWeapon = weapon as SO_RangedWeapon;
            dicBullets[weapon.typeWeapon] = new List<GameObject>();

            for (int i = 0; i < dataWeapon.totalBullets; i++)
            {
                GameObject newWeapon = Instantiate(dataWeapon.bulletPrefab, parentObjects);
                dicBullets[weapon.typeWeapon].Add(newWeapon); 
                newWeapon.SetActive(false);
            }
        }
    }
    private GameObject GetBullet(PlayerWeapon type)
    {
        if (!dicBullets.ContainsKey(type) || dicBullets[type] == null) { Debug.Log("NOT FIND DIC: " + type); return null; }

        foreach(GameObject newBullet in dicBullets[type])
        {
            if (!newBullet.activeInHierarchy)
            {
                newBullet.SetActive(true);
                return newBullet;
            }
        }

        SO_RangedWeapon weaponData = weaponsComponents.Find(w => w.typeWeapon == type) as SO_RangedWeapon;
        if (weaponData != null)
        {
            GameObject newWeapon = Instantiate(weaponData.bulletPrefab, parentObjects);
            dicBullets[type].Add(newWeapon);
            return newWeapon;
        }

        return null;
    }
    private bool isActiveBullet(PlayerWeapon type)
    {
        if (!dicBullets.ContainsKey(type) || dicBullets[type] == null) { Debug.Log("NOT FIND DIC: " + type); return false; }

        foreach (GameObject newBullet in dicBullets[type])
        {
            if (newBullet.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Attack
    public void CharacterMeleeAttack()
    {
        //Delay
        if (meleeDelay > Time.time) return;
        if (!TryGetWeapon(actualMeleeWeapon, out var weapon)) return;
        SO_WeaponProperties weaponProperties = weapon;

        meleeDelay = Time.time + weaponProperties.basicValues.delay;
        directionAttack = playerReference.GetDirection;

        AnimationClip clip = GetAnimationClipFromDirection(GetActualMeleeWeapon);

        if (!clip)
        { 
            Debug.Log($"ANIMATION NULL: {GetActualMeleeWeapon}"); 
            return;
        }
        
        StartCoroutine(PlayAttackAnimation(clip));
    }
    public void CharacterRangedAttack()
    {
        //Delay
        if (rangedDelay > Time.time) return;
        if (!TryGetWeapon(actualDistanceWeapon, out var weapon)) return;

        SO_RangedWeapon rangedWeapon = weapon as SO_RangedWeapon;
        if (rangedWeapon.isOnlyOneTrigger && isActiveBullet(actualDistanceWeapon)) return;

        rangedDelay = Time.time + weapon.basicValues.delay;
        directionAttack = playerReference.GetDirection;

        AnimationClip clip = GetAnimationClipFromDirection(PlayerWeapon.AnimationDistance);

        if (!clip)
        {
            Debug.Log($"ANIMATION NULL: {GetActualDisWeapon}");
            return;
        }

        //if (!isAttack)
            StartCoroutine(PlayAttackAnimation(clip));
    }


    //Method for animation frame
    public void DetectionAttack()
    {
        UpdateRectangle();
        Collider2D[] colliders = Physics2D.OverlapAreaAll(TopRightcorner, BottomLefttcorner, layerEnemies);

        foreach (var collider in colliders)
        {
            if (!collider) continue;

            if (collider.gameObject.TryGetComponent(out IHealthCharacterControl enemyHealth))
            {
                TryGetWeapon(actualMeleeWeapon, out var weapon);
                enemyHealth.RemoveHearts(weapon.basicValues.damage);
            }
        }
    }
    public void DistanceAttack()
    {
        GameObject boomerang = GetBullet(actualDistanceWeapon);
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
        animator.Play(attackAnimation.name);
        yield return new WaitForSeconds(attackAnimation.length);
        animator.Play("BT_Idle");
    }
    AnimationClip GetAnimationClipFromDirection(PlayerWeapon typeWeapon)
    {
        TryGetWeapon(typeWeapon, out var Attributes);

        if (directionAttack.x != 0)
            return directionAttack.x > 0 ? Attributes.totalAnimations.attack_Right : Attributes.totalAnimations.attack_Left;
        
        else
            return directionAttack.y > 0 ? Attributes.totalAnimations.attack_Up : Attributes.totalAnimations.attack_Down;
        
    }

    #endregion

    #region Getters
    private bool TryGetWeapon(PlayerWeapon weaponType, out SO_WeaponProperties weapon)
    {
        return weaponDictionary.TryGetValue(weaponType, out weapon);
    }

    #endregion

    #region Detector

    private void UpdateRectangle()
    {
    //    if (!TryGetWeapon(actualMeleeWeapon, out var weapon)) return;
    //    float distance = weapon.basicValues.maxScope * (directionAttack.x >= 1 ? 1 : -1);

    //    float xOffset = directionAttack.x * weapon.basicValues.maxScope;
    //    float yOffset = directionAttack.y * weapon.basicValues.maxScope;

    //    BottomLefttcorner = new Vector2(
    //    transform.position.x - distanceX_Left + (directionAttack.x < 0 ? xOffset : 0),
    //    transform.position.y - distanceY_Down + (directionAttack.y < 0 ? yOffset : 0)
    //);

    //    TopRightcorner = new Vector2(
    //        transform.position.x + distanceX_Right + (directionAttack.x > 0 ? xOffset : 0),
    //        transform.position.y + distanceY_Top + (directionAttack.y > 0 ? yOffset : 0)
    //    );

        if (!TryGetWeapon(actualMeleeWeapon, out var weapon)) return;

        float xOffset = directionAttack.x * weapon.basicValues.maxScope;
        float yOffset = directionAttack.y * weapon.basicValues.maxScope;

        BottomLefttcorner = (Vector2)transform.position + new Vector2(-distanceX_Left + Mathf.Min(xOffset, 0), -distanceY_Down + Mathf.Min(yOffset, 0));
        TopRightcorner = (Vector2)transform.position + new Vector2(distanceX_Right + Mathf.Max(xOffset, 0), distanceY_Top + Mathf.Max(yOffset, 0));
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

    private void SetWeapon(PlayerWeapon weapon)
    {
        if (!weaponDictionary.TryGetValue(weapon, out SO_WeaponProperties newWeapon))
        {
            Debug.LogError($"Weapon {weapon} not found in dictionary!");
            return;
        }

        if (newWeapon.typeCombat == TypeCombat.Melee)
        {
            actualMeleeWeapon = weapon;
            ItemManager.Instance.ResetCurrentWeapon(weapon);
        }
        else
        {
            actualDistanceWeapon = weapon;
            ItemManager.Instance.hasBoomerang = true;
        }
    }
    private void UnlockItem(object item)
    {
        PlayerWeapon newWeapon = (PlayerWeapon)item;
        if (newWeapon == PlayerWeapon.None) return;

        SetWeapon(newWeapon);
    }
}
