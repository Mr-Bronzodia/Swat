using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InteriorGenerator : MonoBehaviour
{
    BoxCollider _collider;

    [SerializeField]
    GameObject _wall;

    Dictionary<TreeMapNode, Bounds> _rooms;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider>();

        Vector3 topRight = new Vector3(gameObject.transform.position.x + (_collider.bounds.size.x / 2), 0, gameObject.transform.position.z + (_collider.bounds.size.z / 2));
        Vector3 bottomRight = new Vector3(gameObject.transform.position.x + (_collider.bounds.size.x / 2), 0, gameObject.transform.position.z - (_collider.bounds.size.z / 2));

        Vector3 topLeft = new Vector3(gameObject.transform.position.x - (_collider.bounds.size.x / 2), 0, gameObject.transform.position.z + (_collider.bounds.size.z / 2));
        Vector3 bottomLeft = new Vector3(gameObject.transform.position.x - (_collider.bounds.size.x / 2), 0, gameObject.transform.position.z - (_collider.bounds.size.z / 2));

        float wallLenght = _wall.GetComponent<MeshRenderer>().bounds.size.z;
        float wallHeight = _wall.GetComponent<MeshRenderer>().bounds.size.y;

        TreeMapNode root = new TreeMapNode(RoomTypes.Root, _collider.bounds.size.x, _collider.size.z);

        TreeMapNode k = new TreeMapNode(RoomTypes.Kitchen, 5f, 5f);
        TreeMapNode bt = new TreeMapNode(RoomTypes.Bathroom, 5f, 5f);
        TreeMapNode b = new TreeMapNode(RoomTypes.Bedroom, 10f, 5f);

        root.Children.Add(bt);
        root.Children.Add(b);
        root.Children.Add(k);

        SquerifiedTreeMap treeMap = new SquerifiedTreeMap(root, _collider.bounds);

        _rooms = treeMap.GenerateTreemap();

        //Debug.Log(_rooms.Count);

        //foreach (KeyValuePair<TreeMapNode, Bounds> room in _rooms)
        //{
        //    Debug.Log(room.Key + " " + room.Value);
        //}

        //InstantiateWalls(bottomLeft, topLeft, wallLenght, wallHeight, Quaternion.Euler(0f, 0f, 0f), _wall);
        //InstantiateWalls(bottomRight, topRight, wallLenght, wallHeight, Quaternion.Euler(0f, 0f, 0f), _wall);

        //InstantiateWalls(topLeft, topRight, wallLenght, wallHeight, Quaternion.Euler(0f, 90f, 0f), _wall);
        //InstantiateWalls(bottomLeft, bottomRight, wallLenght, wallHeight, Quaternion.Euler(0f, 90f, 0f), _wall);


    }

    private void InstantiateWalls(Vector3 start, Vector3 end, float wallLenght, float wallHeight, Quaternion rotation, GameObject wallPrefab)
    {
        float structureLenght = Vector3.Distance(start, end);

        int numWalls = (int)(structureLenght / wallLenght);

        float reminder = (structureLenght % wallLenght) / wallLenght;

        Debug.Log("Rotation:" + rotation.eulerAngles);

        for (int i = 0; i < numWalls; i++)
        {
            if (rotation.eulerAngles.y > 85f)
            {
                Vector3 wallPos = new Vector3((start.x + (wallLenght * i) + wallLenght / 2), start.y + wallHeight / 2, start.z);
                Instantiate(wallPrefab, wallPos, rotation, gameObject.transform);
            }
            else
            {
                Vector3 wallPos = new Vector3(start.x, start.y + wallHeight / 2, (start.z + (wallLenght * i) + wallLenght / 2));
                Instantiate(wallPrefab, wallPos, rotation, gameObject.transform);
            }
            
            
        }

        Debug.Log("Reminder: " + reminder);

        if (reminder == 0) return;

        if (rotation.eulerAngles.y > 85f)
        {
            Vector3 reminderWallPos = new Vector3((start.x + (wallLenght * (numWalls - 1)) + wallLenght / 2), start.y + wallHeight / 2, start.z);
            reminderWallPos.x += (wallLenght / 2) + (wallLenght * reminder) / 2;
            GameObject remiderWallInstance = Instantiate(wallPrefab, reminderWallPos, rotation, gameObject.transform);
            remiderWallInstance.transform.localScale = new Vector3(remiderWallInstance.transform.localScale.x, remiderWallInstance.transform.localScale.y, remiderWallInstance.transform.localScale.z * reminder);
        }
        else
        {
            Vector3 reminderWallPos = new Vector3(start.x , start.y + wallHeight / 2, (start.z + (wallLenght * (numWalls - 1)) + wallLenght / 2));
            reminderWallPos.z += (wallLenght / 2) + (wallLenght * reminder) / 2;
            GameObject remiderWallInstance = Instantiate(wallPrefab, reminderWallPos, rotation, gameObject.transform);
            remiderWallInstance.transform.localScale = new Vector3(remiderWallInstance.transform.localScale.x, remiderWallInstance.transform.localScale.y, remiderWallInstance.transform.localScale.z * reminder);
        }
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
        if (_rooms == null) return;

        foreach (KeyValuePair<TreeMapNode, Bounds> room in _rooms)
        {
            if (room.Value.size.x < 0 || room.Value.size.z < 0) continue;

            DrawBounds(room.Value);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(room.Value.center, 0.2f);
            Gizmos.color = Color.white;

            Vector3 offset = new Vector3(-0.7f, 0, 0.5f);
            Handles.Label(room.Value.center + offset, room.Key.RoomType.ToString());
        }
    }
}
