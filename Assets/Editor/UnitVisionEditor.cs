using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitVision))]
public class UnitVisionEditor : Editor
{
    private void OnSceneGUI()
    {
        UnitVision unitVision = (UnitVision)target;
        Handles.color = Color.white;

        Vector3 viewAngleA = unitVision.DirectionFromAngle(-unitVision.ViewAngle / 2, false);
        Vector3 viewAngleB = unitVision.DirectionFromAngle(unitVision.ViewAngle / 2, false);

        Handles.DrawWireArc(unitVision.HeadTransform.position, Vector3.up, viewAngleA, unitVision.ViewAngle, unitVision.ViewRadius);

        Handles.DrawLine(unitVision.HeadTransform.position, unitVision.HeadTransform.position + viewAngleA * unitVision.ViewRadius);
        Handles.DrawLine(unitVision.HeadTransform.position, unitVision.HeadTransform.position + viewAngleB * unitVision.ViewRadius);

        Handles.color = Color.red;
        foreach (Transform visible in unitVision._visibleTargetsList)
        {
            Handles.DrawWireCube(visible.position, Vector3.one);
        }
    }
}
