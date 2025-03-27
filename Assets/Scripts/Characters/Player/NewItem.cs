using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class NewItem : MonoBehaviour
{
    [Header("Type Item")]
    public TypeItem typeItem;
    [Space]
    public PlayerItems playerItem;
    public PlayerWeapon weapon;

    [Header("Animation - float")]
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private float floatHeight = 0.2f;
    private Vector3 startPos;
    private float randomOffset;

    [Header("Animation - PickUp")]
    [SerializeField] public SpriteRenderer principalImage;
    [SerializeField] private float pickUpDistance = 2f;
    [SerializeField] private float timeAnimation = 0.5f;

    [Header("Items")]
    public List<SpriteWeaponItem> spritesWeaponItem = new List<SpriteWeaponItem>();
    public List<SpritePlayerItem> spritesPlayerItem = new List<SpritePlayerItem>();

    private Vector3 finalPosition = Vector3.zero;
    private Vector3 initialPosition = Vector3.zero;

    private bool onPickUp = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (spritesWeaponItem.Count <= 0) return;

        foreach (var item in spritesWeaponItem)
        {
            string newName = item.item.ToString().Replace("Unlock_", "");
            item.name = newName;
        }
    }
#endif

    private void OnEnable()
    {
        startPos = transform.position;
        randomOffset = Random.Range(0f, Mathf.PI * 2);
    }
    private void Update()
    {
        if (!onPickUp)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatHeight;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
        else if (GameManager.Instance.GetCurrentState == playerState.Inspection)
        {
            if (Input.anyKeyDown)
            {
                GameManager.Instance.SetPlayerState(playerState.Exploration);
                Destroy(this.gameObject);
            }
        }
    }


    private IEnumerator FloatAnimation()
    {
        yield return new WaitForSeconds(timeAnimation);
    }

    public void UpdateItem(Enum type)
    {
        if (principalImage == null) return;

        switch (type)
        {
            case PlayerItems newPlayerItem:

                typeItem = TypeItem.player;
                playerItem = newPlayerItem;
                principalImage.sprite = GetSpriteForPlayerItem(newPlayerItem);

                break;

            case PlayerWeapon newWeapon:
                typeItem = TypeItem.Weapon;
                weapon = newWeapon;
                principalImage.sprite = GetSpriteForWeapon(newWeapon);
                break;

            default:
                Debug.LogWarning("TYPE ITEM: " + type);
                break;
        }
    }
    private void ApplyItem()
    {
        if (typeItem == TypeItem.None) return;
        ItemManager.Instance.RemoveItem(this);

        if (typeItem == TypeItem.player)
        {
            SO_PlayerAbility newPlayerAbility = new SO_PlayerAbility()
            { 
                type = PlayerBasicStats.hearts,
                upgradeValue = playerItem == PlayerItems.NewHeart ? 2 : 1,
            };

            AbilityBasicData data = new AbilityBasicData(newPlayerAbility, UpgradeType.playerUpgrade);

            EventManager.Instance.TriggerEvent(PlayerEvents.OnReceiveUpgrade, data);
            return;
        }

        EventManager.Instance.TriggerEvent(PlayerEvents.OnChangeWeapon, weapon);
    }

    private IEnumerator PickUpAnimation()
    {
        float elapsedTime = 0;
        onPickUp = true;

        while (elapsedTime < timeAnimation)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / timeAnimation);

            yield return null;
        }

        transform.position = finalPosition;
    }
    private Sprite GetSpriteForPlayerItem(PlayerItems item)
    {
        SpritePlayerItem itemSprite = spritesPlayerItem.Find(i => i.item == item);
        return itemSprite != null ? itemSprite.sprite : null;
    }
    private Sprite GetSpriteForWeapon(PlayerWeapon weapon)
    {
        SpriteWeaponItem weaponSprite = spritesWeaponItem.Find(w => w.item == weapon);
        return weaponSprite != null ? weaponSprite.sprite : null;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.TryGetComponent(out Player player))
        {
            ApplyItem();
            EventManager.Instance.TriggerEvent(GameWorldEvents.OnChangeState, playerState.Inspection);

            initialPosition = GameManager.Instance.GetPlayer.transform.position;
            finalPosition = new Vector3(initialPosition.x, initialPosition.y + pickUpDistance, 0);
            transform.position = initialPosition;
            StartCoroutine(PickUpAnimation());
        }
    }
}

[Serializable]
public class SpriteWeaponItem
{
    [HideInInspector]
    public string name;

    public PlayerWeapon item;
    public Sprite sprite;
}

[Serializable]
public class SpritePlayerItem
{
    [HideInInspector]
    public string name;

    public PlayerItems item;
    public Sprite sprite;
}