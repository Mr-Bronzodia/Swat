using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InteriorGenerator : MonoBehaviour, ISubscriber
{
    public List<Room> Rooms { get; private set; }
    public Action OnRoomsGenerated;

    [Header("Generation Settings")]
    [SerializeField]
    private bool _shouldRandomizeChildren;
    [SerializeField]
    private bool _regenerateOnPlay;

    [Header("Debug Settings")]
    [SerializeField]
    private bool _showRoomConnection;
    [SerializeField]
    private bool _showRoomNames;
    [SerializeField]
    private bool _showRoomBounds;
    [SerializeField]
    private bool _showRoomCenter;
    [SerializeField]
    private bool _showRoomDoors;

    private bool IsSubscribed = false;

    private void OnEnable()
    {
        WorldStateManager.Instance.OnWorldStateChanged += WorldListener;

        Subscribe();
        IsSubscribed = true;
    }

    private void OnDisable()
    {
        WorldStateManager.Instance.OnWorldStateChanged -= WorldListener;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_regenerateOnPlay) Generate();
    }

    public void Generate()
    {
        DestroyHouse();

        if (!IsSubscribed) Subscribe();

        BoxCollider collider = GetComponent<BoxCollider>();

        House house = new House(collider.bounds, this.gameObject);

        HouseTheme[] themes = Resources.LoadAll<HouseTheme>("HouseThemes");

        house.InstantiateHouse(themes[UnityEngine.Random.Range(0, themes.Length)]);

        Rooms = house.Rooms;

        NotifyTaskCompleted();
    }

    public void DestroyHouse()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        if (Rooms != null && Rooms.Count > 0) Rooms.Clear();

        IsSubscribed = false;
    }

    private void WorldListener(WorldState state)
    {
        if (state == WorldState.PlotsGenerated) Generate();
    }

    /// <summary>
    /// Draws bounds.
    /// Thanks to https://gist.github.com/unitycoder/58f4b5d80f423d29e35c814a9556f9d9
    /// </summary>
    void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.blue, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.yellow, delay);
        Debug.DrawLine(p4, p1, Color.magenta, delay);
        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);

        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.blue, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.yellow, delay);
        Debug.DrawLine(p8, p5, Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.white, delay);
        Debug.DrawLine(p2, p6, Color.gray, delay);
        Debug.DrawLine(p3, p7, Color.green, delay);
        Debug.DrawLine(p4, p8, Color.cyan, delay);
    }


    private void OnDrawGizmos()
    {
        if (Rooms == null) return;

        foreach (Room room in Rooms)
        {
            if (room.Bounds.size.x < 0 || room.Bounds.size.z < 0) continue;

            if (_showRoomBounds) DrawBounds(room.Bounds);
            Gizmos.color = Color.red;
            if (_showRoomCenter) Gizmos.DrawSphere(room.Bounds.center, 0.2f);
            Gizmos.color = Color.white;

            Vector3 offset = new Vector3(-0.7f, 0, 0.5f);
            if (_showRoomNames) Handles.Label(room.Bounds.center + offset, room.RoomType.ToString());

            Gizmos.color = Color.blue;

            foreach (Room connected in room.ConnectedRooms)
            {
                if (!_showRoomConnection) continue;
                Gizmos.DrawLine(room.Bounds.center, connected.Bounds.center);
            }
            Gizmos.color = Color.green;

            foreach (KeyValuePair<Sides, Wall> wall in room.Walls)
            {
                if (!_showRoomDoors) continue;
                foreach (Vector3 doorPos in wall.Value._doorPositions)
                {
                    Gizmos.DrawCube(doorPos + new Vector3(0f, 1f, 0), new Vector3(.2f, 2, 1f));
                }
            }

        }


    }

    public void Subscribe()
    {
        WorldStateManager.Instance.AddSubscriber();
    }

    public void NotifyTaskCompleted()
    {
        WorldStateManager.Instance.NotifyComplete();
        OnRoomsGenerated?.Invoke();
    }
}
