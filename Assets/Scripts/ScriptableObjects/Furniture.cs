using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Furniture")]
public class Furniture : ScriptableObject
{
    public GameObject Prefab;
    public List<ERoomTypes> RoomTags;
    public EObjectTag ObjectTag;
    public List<EDescriptorTags> DescriptorTags;
}
