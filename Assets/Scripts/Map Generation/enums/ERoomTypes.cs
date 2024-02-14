using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public enum ERoomTypes 
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
    /// <summary>
    /// Returns preferd connections for given room type
    /// </summary>
    public static List<ERoomTypes> Get(ERoomTypes type)
    {
        List<ERoomTypes> connections = new List<ERoomTypes>();
        switch (type)
        {
            case ERoomTypes.Kitchen:
                connections.AddRange(new List<ERoomTypes>()
                {
                    ERoomTypes.Livingroom,
                    ERoomTypes.Connector,
                });
                break;
            case ERoomTypes.Bedroom:
                connections.AddRange(new List<ERoomTypes>() 
                { 
                    ERoomTypes.Bathroom,
                    ERoomTypes.Connector,
                    ERoomTypes.Office,
                    ERoomTypes.Livingroom,
                    ERoomTypes.StorageArea,
                    ERoomTypes.Laundry
                });
                break;
            case ERoomTypes.Bathroom:
                connections.AddRange(new List<ERoomTypes>()
                {
                    ERoomTypes.Bedroom,
                    ERoomTypes.Office,
                    ERoomTypes.Connector,
                    ERoomTypes.Laundry,
                });
                break;
            case ERoomTypes.Connector:
                connections.AddRange(Enum.GetValues(typeof(ERoomTypes)).Cast<ERoomTypes>().ToList());
                break;
            case ERoomTypes.Livingroom:
                connections.AddRange(new List<ERoomTypes>()
                {
                    ERoomTypes.StorageArea,
                    ERoomTypes.Kitchen,
                    ERoomTypes.Connector,
                    ERoomTypes.StorageArea,
                    ERoomTypes.Office,
                    ERoomTypes.Bathroom,
                    ERoomTypes.Bedroom
                });
                break;
            case ERoomTypes.StorageArea:
                connections.AddRange(new List<ERoomTypes>()
                {
                    ERoomTypes.Connector,
                    ERoomTypes.Bathroom,
                    ERoomTypes.Laundry
                });
                break;
            case ERoomTypes.Laundry:
                connections.AddRange(new List<ERoomTypes>()
                {
                    ERoomTypes.Connector,
                    ERoomTypes.Bathroom,
                });
                break;
            case ERoomTypes.Office:
                connections.AddRange(new List<ERoomTypes>() 
                {
                    ERoomTypes.Bedroom,
                    ERoomTypes.Livingroom,
                    ERoomTypes.Connector
                });
                break;
            default:
                Debug.LogError("No prefered connections for " + type.ToString());
                break;
        }

        return connections;
    }
}

