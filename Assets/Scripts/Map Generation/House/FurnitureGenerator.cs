using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FurnitureGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject empty;

    private List<GameObject> _roomParents;
    private Furniture[] _furniture;

    private static string FURNITUREDIR = "Furniture";

    private void OnEnable()
    {
        gameObject.GetComponent<InteriorGenerator>().OnRoomsGenerated += GenerateFurniture;
        _furniture = Resources.LoadAll<Furniture>(FURNITUREDIR);
    }

    private void OnDisable()
    {
        gameObject.GetComponent<InteriorGenerator>().OnRoomsGenerated -= GenerateFurniture;
    }

    private void GenerateFurniture()
    {
        Debug.Log(_furniture.Length);
        List<Room> rooms = gameObject.GetComponent<InteriorGenerator>().Rooms;
        _roomParents = new List<GameObject>();

        foreach (Room room in rooms)
        {
            switch (room.RoomType)
            {
                case RoomTypes.Root:
                    break;
                case RoomTypes.Kitchen:
                    break;
                case RoomTypes.Bedroom:
                    break;
                case RoomTypes.Bathroom:
                    GenerateBathroom();
                    break;
                case RoomTypes.Connector:
                    break;
                case RoomTypes.Livingroom:
                    break;
                case RoomTypes.StorageArea:
                    break;
                case RoomTypes.Laundry:
                    break;
                case RoomTypes.Office:
                    break;
            }
        }
    }

    private void GenerateBathroom()
    {
        Debug.Log("soy");
    }
}