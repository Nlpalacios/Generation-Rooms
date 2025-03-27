using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject itemPrefab; 

    private PlayerWeapon currentWeapon;

    private List<NewItem> activeItems = new List<NewItem>();
    private static PlayerWeapon[] validItems;
    public static ItemManager Instance;
    public bool hasBoomerang = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        EventManager.Instance.Subscribe(GameWorldEvents.OnFinishLoop, OnReset);
        ResetTotalWeapons();
    }
    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameWorldEvents.OnFinishLoop, OnReset);
    }
    private void OnReset(object call)
    {
        activeItems.ForEach(item => Destroy(item.gameObject));
        activeItems.Clear();
        currentWeapon = PlayerWeapon.None;
    }
    private void ResetTotalWeapons()
    {
        PlayerWeapon[] playerItems = (PlayerWeapon[])Enum.GetValues(typeof(PlayerWeapon));
        validItems = playerItems.Where(i => i != PlayerWeapon.None &&
                                            i != PlayerWeapon.AnimationDistance &&
                                            i != PlayerWeapon.Weapon_Boomerang).ToArray();
        if (validItems.Length == 0)
        {
            Debug.LogWarning("NOT FIND WEAPON");
            return;
        }
    }

    int RandomInt(int max) => Random.Range(0, max);
    GameObject InstantiateItem(Vector3 pos)
    {
        return Instantiate(itemPrefab, pos, Quaternion.identity, this.transform);
    }
    public void ResetCurrentWeapon(PlayerWeapon weapon)
    {
        ResetTotalWeapons();
        currentWeapon = weapon;
        validItems = validItems.Where(i => i != weapon).ToArray();
    }

    public void SpawnNewMeleeWeapon(Vector3 pos, PlayerWeapon type = PlayerWeapon.None)
    {  
        GameObject newItem = InstantiateItem(pos);
        NewItem item = newItem.GetComponent<NewItem>();
        if (item == null) { Debug.LogWarning("NO SCRIPT ITEM"); return; }

        PlayerWeapon randomItem = validItems[RandomInt(validItems.Length)];
        PlayerWeapon finalItem = type == PlayerWeapon.None ? randomItem : type;

        item.UpdateItem(finalItem);
        activeItems.Add(item);
    }
    public void RemoveItem(NewItem item)
    {
        if (item == null) return;

        if (activeItems.Contains(item))
        {
            activeItems.Remove(item);
        }
    }
}