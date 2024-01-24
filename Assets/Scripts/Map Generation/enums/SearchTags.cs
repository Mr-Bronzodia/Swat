using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectTag
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

public enum DescriptorTags
{
    Big,
    Small,
    Medium,
    Standing,
    Hanging,
    Corner,
    WallAdjusted
}

public enum SearchMode
{
    RequireAll,
    RequireOne,
    BlackList
}