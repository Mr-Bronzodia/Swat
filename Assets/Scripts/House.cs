using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class House 
{

    public List<Room> Rooms { get; protected set; }

    public Bounds Bounds { get; protected set; }

    protected House(Bounds bounds)
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

    public abstract void Instantiate();
}

public class SmallHouse : House
{
    public SmallHouse(Bounds bounds) : base(bounds)
    {
    }

    public override void Instantiate()
    {
        TreeMapNode root = new TreeMapNode(RoomTypes.Root, Bounds.size.x, Bounds.size.z);

        float totalArea = Bounds.size.x * Bounds.size.z;
        float bedRoomArea = totalArea * Random.Range(.25f, .35f);
        float bathroomArea = totalArea * Random.Range(.1f, .15f);
        float kitchenArea = totalArea * Random.Range(.2f, .25f);
        float livingRoomArea = totalArea - (kitchenArea + bedRoomArea + bathroomArea);

        float livingRoomHeightRatio = Random.Range(1.2f, 1.5f);
        float bedroomHeightRatio = Random.Range(1.2f, 1.5f);
        float kitchenHeightRatio = Random.Range(1f, 1.5f);
        float bathroomHeightRatio = 1;

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
