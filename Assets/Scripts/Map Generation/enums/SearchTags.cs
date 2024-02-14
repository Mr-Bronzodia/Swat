using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EObjectTag
{
    Chair,
    NightStand,
    Desk,
    Bed,
    Sofa,
    Table,
    Dresser,
    Shelve,
    Screen,
    Storage,
    Fridge,
    Toilet,
    Sink,
    Shower,
    Bathtub,
    Carpet,
    Light,
    Mirror,
    Prop
}

public enum EDescriptorTags
{
    Big,
    Small,
    Medium,
    Standing,
    Hanging,
    Corner,
    WallAdjusted
}

public enum ESearchMode
{
    RequireAll,
    RequireOne,
    BlackList,
}