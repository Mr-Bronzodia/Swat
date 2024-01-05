using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public enum RoomTypes 
{
    Root,
    Kitchen,
    Bedroom,
    Bathroom,
    Connector,
    Livingroom,
    StorageArea,
    Laundry,
    Office
}

public struct PreferedConnections
{
    public static List<RoomTypes> Get(RoomTypes type)
    {
        List<RoomTypes> connections = new List<RoomTypes>();
        switch (type)
        {
            case RoomTypes.Kitchen:
                connections.AddRange(new List<RoomTypes>()
                {
                    RoomTypes.Livingroom,
                    RoomTypes.Bedroom,
                    RoomTypes.Connector,
                });
                break;
            case RoomTypes.Bedroom:
                connections.AddRange(new List<RoomTypes>() 
                { 
                    RoomTypes.Bathroom,
                    RoomTypes.Connector,
                    RoomTypes.Office,
                    RoomTypes.Livingroom,
                    RoomTypes.StorageArea,
                    RoomTypes.Laundry
                });
                break;
            case RoomTypes.Bathroom:
                connections.AddRange(new List<RoomTypes>()
                {
                    RoomTypes.Bedroom,
                    RoomTypes.Office,
                    RoomTypes.Connector,
                    RoomTypes.Laundry,
                });
                break;
            case RoomTypes.Connector:
                connections.AddRange(Enum.GetValues(typeof(RoomTypes)).Cast<RoomTypes>().ToList());
                break;
            case RoomTypes.Livingroom:
                connections.AddRange(new List<RoomTypes>()
                {
                    RoomTypes.StorageArea,
                    RoomTypes.Kitchen,
                    RoomTypes.Connector,
                    RoomTypes.StorageArea,
                    RoomTypes.Office,
                    RoomTypes.Bathroom
                });
                break;
            case RoomTypes.StorageArea:
                connections.AddRange(new List<RoomTypes>()
                {
                    RoomTypes.Connector,
                    RoomTypes.Bathroom,
                    RoomTypes.Laundry
                });
                break;
            case RoomTypes.Laundry:
                connections.AddRange(new List<RoomTypes>()
                {
                    RoomTypes.Connector,
                    RoomTypes.Bathroom,
                });
                break;
            case RoomTypes.Office:
                connections.AddRange(new List<RoomTypes>() 
                {
                    RoomTypes.Bedroom,
                    RoomTypes.Livingroom,
                    RoomTypes.Connector
                });
                break;
            default:
                Debug.LogError("No prefered connections for " + type.ToString());
                break;
        }

        return connections;
    }
}

