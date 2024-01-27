using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;


public class House 
{
    public List<Room> Rooms { get; protected set; }

    public Bounds Bounds { get; protected set; }

    private GameObject _parentInstance;

    private readonly int ROOMTHRESHOLD = 150;
    private readonly int HOUSEINCREMENT = 20;

    public House(Bounds bounds, GameObject parentInstance)
    {
        Bounds = bounds;
        _parentInstance = parentInstance;
    }

    protected void BuildRoomConnections()
    {
        for (int i = 0; i < Rooms.Count; i++)
        {
            List<Room> adjustedRooms = new List<Room>();

            for (int j = 0; j < Rooms.Count; j++)
            {
                if (Rooms[i] == Rooms[j]) continue;

                if (Rooms[i].IsAdjusted(Rooms[j]))
                {
                    adjustedRooms.Add(Rooms[j]);
                    Rooms[i].AddAdjustedRoom(Rooms[j]);
                    Rooms[j].AddAdjustedRoom(Rooms[i]);
                }
            }

            if (adjustedRooms.Count == 0) Debug.LogError("No connections found for " + Rooms[i].RoomType.ToString());

            foreach (Room other in adjustedRooms)
            {
                if (PreferedConnections.Get(Rooms[i].RoomType).Contains(other.RoomType))
                {
                    Rooms[i].AddRoomConnection(other);
                    other.AddRoomConnection(Rooms[i]);
                }
            }

            if (Rooms[i].ConnectedRooms.Count == 0) Rooms[i].AddRoomConnection(adjustedRooms[UnityEngine.Random.Range(0, adjustedRooms.Count)]);
        }
    }

    private GameObject GetRandomObject(List<GameObject> listObj)
    {
        return listObj[Random.Range(0, listObj.Count)];
    }

    private void GenerateFloorPlan()
    {
        TreeMapNode root = new TreeMapNode(RoomTypes.Root, Bounds.size.x, Bounds.size.z);

        float totalArea = Bounds.size.x * Bounds.size.z;
        float bedRoomArea = totalArea * Random.Range(.20f, .30f);
        float bathroomArea = totalArea * Random.Range(.1f, .15f);
        float kitchenArea = totalArea * Random.Range(.2f, .25f);
        float livingRoomArea = totalArea - (kitchenArea + bedRoomArea + bathroomArea);

        float livingRoomHeightRatio = Random.Range(1.2f, 1.5f);
        float bedroomHeightRatio = Random.Range(1.2f, 1.5f);
        float kitchenHeightRatio = Random.Range(1f, 1.5f);
        float bathroomHeightRatio = Random.Range(1f, 1.5f); ;

        if (root.Size >= ROOMTHRESHOLD)
        {
            float officeArea = totalArea * Random.Range(.1f, .12f);
            bedRoomArea -= officeArea;
            float officeHeightRatio = Random.Range(1f, 1.5f);
            float officeHeight = Mathf.Sqrt(officeArea / officeHeightRatio);
            TreeMapNode office = new TreeMapNode(RoomTypes.Office, officeHeight * officeHeightRatio, officeHeight);
            root.Children.Add(office);
        }

        if (root.Size >= ROOMTHRESHOLD + HOUSEINCREMENT * 1)
        {
            float connectorArea = totalArea * Random.Range(.08f, .12f);
            livingRoomArea -= connectorArea;
            float connectorHeightRatio = 1;
            float connectorHeight = Mathf.Sqrt(connectorArea / connectorHeightRatio);
            TreeMapNode connector = new TreeMapNode(RoomTypes.Connector, connectorHeight * connectorHeightRatio, connectorHeight);
            root.Children.Add(connector);
        }

        if (root.Size >= ROOMTHRESHOLD + HOUSEINCREMENT * 2)
        {
            float laundryArea = totalArea * Random.Range(.07f, .1f);
            livingRoomArea -= laundryArea;
            float laundryHeightRatio = 1;
            float laundyHeight = Mathf.Sqrt(laundryArea / laundryHeightRatio);
            TreeMapNode laundry = new TreeMapNode(RoomTypes.Laundry, laundyHeight * laundryHeightRatio, laundyHeight);
            root.Children.Add(laundry);
        }

        if (root.Size >= ROOMTHRESHOLD + HOUSEINCREMENT * 3)
        {
            float storageArea = totalArea * Random.Range(.05f, .07f);
            kitchenArea -= storageArea;
            float storageHeightRatio = Random.Range(1.1f, 1.3f);
            float storageHeight = Mathf.Sqrt(storageArea / storageHeightRatio);
            TreeMapNode storage = new TreeMapNode(RoomTypes.StorageArea, storageHeight * storageHeightRatio, storageHeight);
            root.Children.Add(storage);
        }

        if (root.Size >= ROOMTHRESHOLD + HOUSEINCREMENT * 4)
        {
            float additionalBedroomArea = totalArea * Random.Range(.08f, .1f);
            livingRoomArea -= additionalBedroomArea / 2;
            bedRoomArea -= additionalBedroomArea / 2;
            float newBedroomHeightRatio = Random.Range(1.1f, 1.2f);
            float newBedroomHeight = Mathf.Sqrt(additionalBedroomArea / newBedroomHeightRatio);
            TreeMapNode newBedroom = new TreeMapNode(RoomTypes.Bedroom, newBedroomHeight * newBedroomHeightRatio, newBedroomHeight);
            root.Children.Add(newBedroom);
        }

        if (root.Size >= ROOMTHRESHOLD + HOUSEINCREMENT * 5)
        {
            float newBathroomArea = totalArea * Random.Range(.03f, .05f);
            bathroomArea -= newBathroomArea;
            float newBathroomHeightRatio = Random.Range(1.1f, 1.2f);
            float newBathroomHeight = Mathf.Sqrt(newBathroomArea / newBathroomHeightRatio);
            TreeMapNode newBathroom = new TreeMapNode(RoomTypes.Bathroom, newBathroomHeight * newBathroomHeightRatio, newBathroomHeight);
            root.Children.Add(newBathroom);
        }

        float livingRoomHeight = Mathf.Sqrt(livingRoomArea / livingRoomHeightRatio);
        float kitchenHeight = Mathf.Sqrt(kitchenArea / kitchenHeightRatio);
        float bedroomHeight = Mathf.Sqrt(bedRoomArea / bedroomHeightRatio);
        float bathRoomHeight = Mathf.Sqrt(bathroomArea / bathroomHeightRatio);


        TreeMapNode livingRoom = new TreeMapNode(RoomTypes.Livingroom, livingRoomHeight * livingRoomHeightRatio, livingRoomHeight);
        TreeMapNode kitchen = new TreeMapNode(RoomTypes.Kitchen, kitchenHeight * kitchenHeightRatio, kitchenHeight);
        TreeMapNode bedroom = new TreeMapNode(RoomTypes.Bedroom, bedroomHeight * bedroomHeightRatio, bedroomHeight);
        TreeMapNode bathroom = new TreeMapNode(RoomTypes.Bathroom, bathRoomHeight * bathroomHeightRatio, bathRoomHeight);

        root.Children.Add(livingRoom);
        root.Children.Add(kitchen);
        root.Children.Add(bedroom);
        root.Children.Add(bathroom);

        SquerifiedTreeMap treeMap = new SquerifiedTreeMap(root, Bounds);

        Rooms = treeMap.GenerateTreemap(true, .7f);
    }

    private bool IsLayoutConnected()
    {
        List<Room> connected = new List<Room>();
        TraverseLayout(Rooms[Random.Range(0, Rooms.Count)], connected);

        return connected.Count == Rooms.Count;
    }

    public void TraverseLayout(Room room, List<Room> visited)
    {
        if (visited.Contains(room)) return;

        visited.Add(room);

        foreach (Room connected in room.ConnectedRooms) 
        {
            TraverseLayout(connected, visited);
        }
    }


    public void InstantiateHouse(HouseTheme houseTheme)
    {

        bool isLayoutValid = false;

        while (!isLayoutValid)
        {
            GenerateFloorPlan();

            foreach (Room room in Rooms)
            {
                room.FinalizeLayout();
            }

            BuildRoomConnections();

            isLayoutValid = IsLayoutConnected();

        }

        foreach (Room room in Rooms)
        {
            room.BuildFloor(GetRandomObject(houseTheme.Floor), _parentInstance);
            room.BuildFacade(GetRandomObject(houseTheme.InteriorWall),
                        GetRandomObject(houseTheme.ExtiriorWall),
                        houseTheme.ExteriorWindows,
                        GetRandomObject(houseTheme.Doors),
                        _parentInstance);
           room.BuildRoof(houseTheme.Roof, 3f, _parentInstance);
        }
    }
}
