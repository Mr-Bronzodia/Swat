using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InteriorGenerator : MonoBehaviour
{
    BoxCollider _collider;

    [SerializeField]
    GameObject _wall;

    Vector3 _test;
    Vector3 _test1;
    Vector3 _test2;
    Vector3 _test3;


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

        _test = topLeft;
        _test1 = bottomLeft;
        _test2 = topRight;
        _test3 = bottomRight;

        InstantiateWalls(bottomLeft, topLeft, wallLenght, wallHeight,Quaternion.Euler(0f, 0f, 0f), _wall);
        InstantiateWalls(bottomRight, topRight, wallLenght, wallHeight, Quaternion.Euler(0f, 0f, 0f), _wall);

        InstantiateWalls(topLeft, topRight, wallLenght, wallHeight, Quaternion.Euler(0f, 90f, 0f), _wall);
        InstantiateWalls(bottomLeft, bottomRight, wallLenght, wallHeight, Quaternion.Euler(0f, 90f, 0f), _wall);
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (_test != null) Gizmos.DrawSphere(_test, 0.2f);
        Gizmos.color = Color.red;
        if (_test1 != null) Gizmos.DrawSphere(_test1, 0.2f);
        Gizmos.color = Color.green;
        if (_test2 != null) Gizmos.DrawSphere(_test2, 0.2f);
        Gizmos.color = Color.blue;
        if (_test3 != null) Gizmos.DrawSphere(_test3, 0.2f);
        Gizmos.color = Color.white;
    }
}
