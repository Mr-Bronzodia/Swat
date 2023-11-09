using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot 
{
    public Bounds bounds;
    public Sides side;

    public Plot()
    {

    }

    public bool isValid()
    {
        if (this.bounds == null) return false;

        if (this.bounds.extents.magnitude < 1) return false;

        return true;
    }
}
