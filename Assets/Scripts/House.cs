using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class House 
{

    public List<Room> Rooms { get; protected set; }

    public Bounds Bounds { get; protected set; }

    public House(Bounds bounds)
    {
        Bounds = bounds;
    }

    protected void BuildRoomConnections()
    {
        for (int i = 0; i < Rooms.Count; i++)
        {
            List<Room> adjustedRooms = new List<Room>();

            for (int j = 0; j < Rooms.Count; j++)
            {
                if (Rooms[i] == Rooms[j]) continue;

                if (Rooms[i].IsAdjusted(Rooms[j])) adjustedRooms.Add(Rooms[j]);
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

    public void Instantiate()
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
        float bathroomHeightRatio = 1;

        if (root.Size >= 150)
        {
            float officeArea = totalArea * Random.Range(.1f, .12f);
            bedRoomArea -= officeArea;
            float officeHeightRatio = Random.Range(1f, 1.5f);
            float officeHeight = Mathf.Sqrt(officeArea / officeHeightRatio);
            TreeMapNode office = new TreeMapNode(RoomTypes.Office, officeHeight * officeHeightRatio, officeHeight);
            root.Children.Add(office);
        }

        if (root.Size >= 169)
        {
            float connectorArea = totalArea * Random.Range(.08f, .12f);
            livingRoomArea -= connectorArea;
            float connectorHeightRatio = Random.Range(2f, 2.5f);
            float connectorHeight = Mathf.Sqrt(connectorArea / connectorHeightRatio);
            TreeMapNode connector = new TreeMapNode(RoomTypes.Connector, connectorHeight * connectorHeightRatio, connectorHeight);
            root.Children.Add(connector);
        }

        float livingRoomHeight = Mathf.Sqrt(livingRoomArea / livingRoomHeightRatio);
        float kitchenHeight = Mathf.Sqrt(kitchenArea / kitchenHeightRatio);
        float bedroomeHeight = Mathf.Sqrt(bedRoomArea / bedroomHeightRatio);
        float bathRoomHeight = Mathf.Sqrt(bathroomArea / bathroomHeightRatio);


        TreeMapNode livingRoom = new TreeMapNode(RoomTypes.Livingroom, livingRoomHeight * livingRoomHeightRatio, livingRoomHeight);
        TreeMapNode kitchen = new TreeMapNode(RoomTypes.Kitchen, kitchenHeight * kitchenHeightRatio, kitchenHeight);
        TreeMapNode bedroom = new TreeMapNode(RoomTypes.Bedroom, bedroomeHeight * bedroomHeightRatio, bedroomeHeight);
        TreeMapNode bathroom = new TreeMapNode(RoomTypes.Bathroom, bathRoomHeight * bathroomHeightRatio, bathRoomHeight);



        root.Children.Add(livingRoom);
        root.Children.Add(kitchen);
        root.Children.Add(bedroom);
        root.Children.Add(bathroom);

        SquerifiedTreeMap treeMap = new SquerifiedTreeMap(root, Bounds);

        Rooms = treeMap.GenerateTreemap(true);

        BuildRoomConnections();
    }
}

//public class SmallHouse : House
//{
//    public SmallHouse(Bounds bounds) : base(bounds)
//    {
//    }

//    public override void Instantiate()
//    {

//    }
//}

//public class MediumHouse : House
//{
//    public MediumHouse(Bounds bounds) : base(bounds)
//    {
//    }

//    public override void Instantiate()
//    {
//        TreeMapNode root = new TreeMapNode(RoomTypes.Root, Bounds.size.x, Bounds.size.z);

//        float totalArea = Bounds.size.x * Bounds.size.z;
//        float bedRoomArea = totalArea * Random.Range(.25f, .35f);
//        float bathroomArea = totalArea * Random.Range(.1f, .15f);
//        float kitchenArea = totalArea * Random.Range(.15f, .12f);
//        float connectorArea = totalArea * Random.Range(.05f, .1f);
//        float laundryArea = totalArea * Random.Range(.05f, .07f);
//        float officeArea = totalArea * Random.Range(.05f, .1f);
//        float livingRoomArea = totalArea - (kitchenArea + bedRoomArea + bathroomArea + connectorArea + laundryArea + officeArea);

//        float livingRoomHeightRatio = Random.Range(1.2f, 1.5f);
//        float bedroomHeightRatio = Random.Range(1.2f, 1.5f);
//        float kitchenHeightRatio = Random.Range(1f, 1.5f);
//        float connectorHeightRatio = Random.Range(2f, 2.5f);
//        float laundryHeightRatio = Random.Range(1f, 1.1f);
//        float officeHeightRatio = Random.Range(1f, 1.5f);
//        float bathroomHeightRatio = 1;

//        int bedroomNo = Random.Range(2, 4);

//        float livingRoomHeight = Mathf.Sqrt(livingRoomArea / livingRoomHeightRatio);
//        float kitchenHeight = Mathf.Sqrt(kitchenArea / kitchenHeightRatio);
//        float bedroomeHeight = Mathf.Sqrt((bedRoomArea / bedroomNo) / bedroomHeightRatio);
//        float bathRoomHeight = Mathf.Sqrt(bathroomArea / bathroomHeightRatio);
//        float connectorHeight = Mathf.Sqrt(connectorArea / connectorHeightRatio);
//        float laundryHeight = Mathf.Sqrt(laundryArea / laundryHeightRatio);
//        float officeHeight = Mathf.Sqrt(officeArea / officeHeightRatio);

//        for (int i = 0; i < bedroomNo; i++)
//        {
//            TreeMapNode bedroom = new TreeMapNode(RoomTypes.Bedroom, bedroomeHeight * bedroomHeightRatio, bedroomeHeight);
//            root.Children.Add(bedroom);
//        }


//        TreeMapNode livingRoom = new TreeMapNode(RoomTypes.Livingroom, livingRoomHeight * livingRoomHeightRatio, livingRoomHeight);
//        TreeMapNode kitchen = new TreeMapNode(RoomTypes.Kitchen, kitchenHeight * kitchenHeightRatio, kitchenHeight);
//        TreeMapNode bathroom = new TreeMapNode(RoomTypes.Bathroom, bathRoomHeight * bathroomHeightRatio, bathRoomHeight);
//        TreeMapNode connector = new TreeMapNode(RoomTypes.Connector, connectorHeight * connectorHeightRatio, connectorHeight);
//        TreeMapNode laundry = new TreeMapNode(RoomTypes.Laundry, laundryHeight * laundryHeightRatio, laundryHeight);
//        TreeMapNode office = new TreeMapNode(RoomTypes.Office, officeHeight * officeHeightRatio, officeHeight);

//        root.Children.Add(livingRoom);
//        root.Children.Add(kitchen);
//        root.Children.Add(bathroom);
//        root.Children.Add(connector);
//        root.Children.Add(laundry);
//        root.Children.Add(office);

//        SquerifiedTreeMap treeMap = new SquerifiedTreeMap(root, Bounds);

//        Rooms = treeMap.GenerateTreemap(true);

//        BuildRoomConnections();
//    }
//}
