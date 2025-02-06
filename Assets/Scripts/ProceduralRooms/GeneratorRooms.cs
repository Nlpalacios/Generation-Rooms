using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratorRooms : MonoBehaviour
{
    [Space, Header("Prefab")]
    [SerializeField] SO_RoomsManager SO_DataRoom;
    [SerializeField] private Transform roomsPatern;

    [Space, Header("Num Rooms")]
    [SerializeField] private int maxRooms = 10;

    [Header("Grid Configuration")]
    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;
    [SerializeField] int RoomWidth = 20;
    [SerializeField] int RoomHeight = 12;

    [Space, Header("Special attributes for generation")]
    [SerializeField] private int maxDistanceBetweenRewards;

    public bool completeInstantiate = false;

    #region Debug

    [Header("Debuggin")]
    [SerializeField] private GameObject pointReference;
    private List<GameObject> points = new List<GameObject>();

    #endregion

    //Grid
    private int[,] roomGrid;

    //Positions
    private List<Vector2Int> roomPositions = new List<Vector2Int>();

    //Private variables
    private Dictionary<Vector2Int, GameObject> dicRoomsAndPositions = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, TypeRoom> typeAndPosition = new Dictionary<Vector2Int, TypeRoom>();
    private Queue<Vector2Int> queueRooms = new Queue<Vector2Int>();

    //Counters
    private int positionCount = 0;

    //Private variables
    private bool generationPositionComplete = false;

    //InitialRoom
    Vector2Int positionCenterGrid;

    //Singleton
    public static GeneratorRooms Instance { get; private set; }

    #region StarGame

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        GenerateRooms();
    }

    private void GenerateRooms()
    {
        ClearPreviousRooms();

        roomGrid = new int[gridSizeY, gridSizeX];
        queueRooms = new Queue<Vector2Int>();

        //Generate positions and locations
        positionCenterGrid = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        typeAndPosition.Add(positionCenterGrid, TypeRoom.Initial);
        SelectPoints();
    }

    private void ClearPreviousRooms()
    {
        points.ForEach(Destroy);

        points.Clear();
        queueRooms.Clear();
        roomPositions.Clear();
        typeAndPosition.Clear();
        dicRoomsAndPositions.Clear();
    }

    //Clear variables and try again 
    void RegenerateRooms()
    {
        Debug.LogWarning("RE-GENERATE ROOMS");

        ClearPreviousRooms();
        roomGrid = new int[gridSizeX, gridSizeY];

        positionCount = 0;
        generationPositionComplete = false;

        //Save the initial room
        positionCenterGrid = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        typeAndPosition.Add(positionCenterGrid, TypeRoom.Initial);
        SelectPoints();
    }

    #endregion

    #region Generate rooms 
    ///-----------------------------------------------------------------
    private void SelectPoints()
    {
        int attempts = 0;
        pointGeneration(positionCenterGrid);

        while (!generationPositionComplete)
        {
            if (queueRooms.Count <= 0)
            {
                attempts++;

                if (attempts >= 50)
                {
                    RegenerateRooms();
                    return;
                }

                //InitialRoom
                pointGeneration(positionCenterGrid);
                continue;
            }

            Vector2Int roomIndex = queueRooms.Dequeue();

            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            pointGeneration(new Vector2Int(gridX - 1, gridY));
            pointGeneration(new Vector2Int(gridX + 1, gridY));
            pointGeneration(new Vector2Int(gridX, gridY - 1));
            pointGeneration(new Vector2Int(gridX, gridY + 1));

            if (positionCount == maxRooms)
            {
                generationPositionComplete = true;
                break;
            }
        }

        if (!generationPositionComplete) return;
        SpawnRooms();
    }

    private bool pointGeneration(Vector2Int roomIndex)
    {
        //Return -----------------------------------------------
        if (positionCount >= maxRooms) return false;
        if (UnityEngine.Random.value < .5 && roomIndex != Vector2Int.zero) return false;
        if (roomPositions.Contains(roomIndex)) return false;
        if (CountAdjacentRooms(roomIndex) > 1) return false;

        roomGrid[roomIndex.x, roomIndex.y] = 1;
        queueRooms.Enqueue(roomIndex);

        //Add position at list
        roomPositions.Add(roomIndex);

        //Display point for references
        var position = Instantiate(pointReference, GetPositionGrid(roomIndex), Quaternion.identity);
        position.name = $"Position: {positionCount}";
        position.transform.SetParent(roomsPatern, false);

        points.Add(position);

        //Counter
        positionCount++;

        return true;
    }

    private void SpawnRooms()
    {
        if (positionCount > maxRooms)
        {
            RegenerateRooms();
            return;
        }

        FindFinalRoom();
        FindRewardsRooms();

        int roomCount = 0;

        for (int i = 0; i < roomPositions.Count; i++)
        {
            if (typeAndPosition.Keys.Contains(roomPositions[i])) continue;

            int random = UnityEngine.Random.Range(0, 10);

            if (random < 5)
            {
                typeAndPosition.Add(roomPositions[i], TypeRoom.Basic);
            }
            else if (random >= 5)
            {
                typeAndPosition.Add(roomPositions[i], TypeRoom.Enemy);
            }
        }

        foreach (Vector2Int position in typeAndPosition.Keys)
        {
            GameObject roomPrefab = SO_DataRoom.GetDataRoom(typeAndPosition[position]);
            if (!roomPrefab) return;

            var roomObject = Instantiate(roomPrefab, GetPositionGrid(position), Quaternion.identity);
            roomObject.name = $"Room: {roomCount}";
            roomObject.transform.SetParent(roomsPatern, false);

            //-----------------------------------------------------------
            //Add object and position in list
            dicRoomsAndPositions.Add(position, roomObject);
            roomCount++;

            OpenDoors(roomObject, position);
        }

        completeInstantiate = true;
    }

    #endregion

    #region FindRooms

    private Vector2Int FindFinalRoom()
    {
        Vector2Int finalPosition = roomPositions[0];
        float maxDistance = Vector2.Distance(GetPositionGrid(finalPosition), GetPositionGrid(positionCenterGrid));

        for (int i = 1; i < roomPositions.Count; i++)
        {
            Vector2Int roomIndex = roomPositions[i];
            float distance = Vector2.Distance(GetPositionGrid(roomIndex), GetPositionGrid(positionCenterGrid));

            if (distance > maxDistance)
            {
                maxDistance = distance;
                finalPosition = roomIndex;
            }
        }

        typeAndPosition.Add(finalPosition, TypeRoom.Final);
        return finalPosition;
    }

    private Vector2Int[] FindRewardsRooms()
    {
        List<Vector2Int> specialRooms = new List<Vector2Int>();

        //First pass: Find potencial rewards rooms
        for (int i = roomPositions.Count - 1; i >= 0; i--)
        {
            Vector2Int roomIndex = roomPositions[i];

            if (CountAdjacentRooms(roomIndex) > 1 ||
                roomIndex == positionCenterGrid ||
                typeAndPosition.ContainsKey(roomIndex))
            {
                continue;
            }

            specialRooms.Add(roomPositions[i]);
        }

        //Add first room
        if (specialRooms.Count > 0 && !typeAndPosition.ContainsKey(specialRooms[0]))
        {
            typeAndPosition.Add(specialRooms[0], TypeRoom.Treasure);
        }

        //Second pass: Rooms rewards
        for (int i = 1; i < specialRooms.Count; i++)
        {
            Vector2Int actualRoom = specialRooms[i];
            bool isFarEnough = true;

            for (int j = 0; j < specialRooms.Count; j++)
            {
                if (i == j) continue;

                Vector2Int otherRoom = specialRooms[j];
                float distanceBetweenRewards = Vector2.Distance(GetPositionGrid(actualRoom), GetPositionGrid(otherRoom)); //23.32

                if (distanceBetweenRewards < maxDistanceBetweenRewards)
                {
                    isFarEnough = false;
                    break;
                }
            }

            if (isFarEnough && !typeAndPosition.ContainsKey(actualRoom))
            {
                typeAndPosition.Add(actualRoom, TypeRoom.Treasure);
            }
        }

        return specialRooms.ToArray();
    }

    #endregion

    #region Getters
    //GETTERS ------------------------------------------------------------
    //Position in grid --------------------------------------------------
    public Vector3 GetPositionGrid(Vector2Int vector)
    {
        Vector3 position =
            new Vector3(RoomWidth * (vector.x - gridSizeX / 2),
                        RoomHeight * (vector.y - gridSizeY / 2));

        return position;
    }

    RoomSettings GetRoomSettings(Vector2Int index)
    {
        if (!dicRoomsAndPositions.ContainsKey(index)) return null;

        if (dicRoomsAndPositions.TryGetValue(index, out GameObject room))
        {
            if (room != null)
            {
                RoomSettings roomSettings = room.GetComponent<RoomSettings>();
                return roomSettings;
            }
        }

        return null;
    }

    #endregion

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++;
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++;
        if (y > 0 && roomGrid[x, y - 1] != 0) count++;
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++;

        return count;
    }

    //Open doors in rooms
    private void OpenDoors(GameObject room, Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        RoomSettings actualRoom = room.GetComponent<RoomSettings>();
        if (!actualRoom)
        {
            Debug.Log("Error - Principal");
            return;
        }

        Vector2Int leftPosition = new Vector2Int(x - 1, y);
        Vector2Int rightPosition = new Vector2Int(x + 1, y);
        Vector2Int upPosition = new Vector2Int(x, y + 1);
        Vector2Int downPosition = new Vector2Int(x, y - 1);

        //Left room
        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            RoomSettings leftRoom = GetRoomSettings(leftPosition);
            if (leftRoom != null)
            {
                actualRoom.RemoveDoor(directionDoor.Left);
                leftRoom.RemoveDoor(directionDoor.Right);
            }
        }

        //Right room
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            RoomSettings rightRoom = GetRoomSettings(rightPosition);
            if (rightRoom != null)
            {
                actualRoom.RemoveDoor(directionDoor.Right);
                rightRoom.RemoveDoor(directionDoor.Left);
            }
        }

        //Down Room
        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            RoomSettings downRoom = GetRoomSettings(downPosition);
            if (downRoom != null)
            {
                actualRoom.RemoveDoor(directionDoor.Down);
                downRoom.RemoveDoor(directionDoor.Up);
            }
        }

        //Up room
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            RoomSettings upRoom = GetRoomSettings(upPosition);
            if (upRoom != null)
            {
                actualRoom.RemoveDoor(directionDoor.Up);
                upRoom.RemoveDoor(directionDoor.Down);
            }
        }

    }

    //GIZMOS --------------------------------------------------------------
    private void OnDrawGizmos()
    {
        for (int i = 0; i < gridSizeX; i++)
        {

            for (int j = 0; j < gridSizeY; j++)
            {
                // Calcular la posición de cada cubo en la matriz
                Vector3 position = GetPositionGrid(new Vector2Int(i, j));

                //Tamaño del cubo 
                Vector3 CubeSize = new Vector3(RoomWidth, RoomHeight);

                //Dibujar el cubo en la posicion
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(position, CubeSize);
            }
        }
    }

}