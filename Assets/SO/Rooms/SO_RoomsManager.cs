using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_RoomsManager", menuName = "Scriptable Objects/SO_RoomsManager")]
public class SO_RoomsManager : ScriptableObject
{
    [Space, Header("Prefab")]
    public List<TypeOfRooms> roomsPrefab = new List<TypeOfRooms>();

    public GameObject GetDataRoom(TypeRoom type)
    {
        TypeOfRooms classtype = roomsPrefab.Find(n => n.typeRoom == type);
        return classtype.roomPrefab;
    }
}

[Serializable]
public class TypeOfRooms
{
    [Header("Type room")]
    [SerializeField] public TypeRoom typeRoom;

    [Space, Header("Prefab")]
    [SerializeField] public GameObject roomPrefab;

}
