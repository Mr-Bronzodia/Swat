using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Furniture")]
public class Furniture : ScriptableObject
{
    public GameObject Prefab;
    public List<RoomTypes> RoomType;
    public ObjectTag ObjectTag;
    public List<DescriptorTags> DescriptorTags;
}