using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class SquerifiedTreeMap 
{
    public Dictionary<TreeMapNode ,Bounds> GenerateTreeMap(TreeMapNode root, Bounds bounds)
    {
        Dictionary<TreeMapNode,Bounds> rooms = new Dictionary<TreeMapNode, Bounds>();
        RecursiveSquarify(root, bounds, rooms);
        return rooms;
    }

    private void RecursiveSquarify(TreeMapNode node, Bounds bounds, Dictionary<TreeMapNode, Bounds> rooms)
    {
        if (node == null || node.AspectRatio == 0) return;

        rooms.Add(node, bounds);

        if (node.Children.Count == 0) return;

        List<float> normalizedAspect = new List<float>();
        float sum = 0;

        foreach (TreeMapNode child in node.Children)
        {
            normalizedAspect.Add(child.AspectRatio);
            sum += child.AspectRatio;
        }

        for (int i = 0; i < normalizedAspect.Count; i++)
        {
            float ratio = normalizedAspect[i] / sum;
            Bounds childBounds = CalculateChildBounds(bounds, ratio, true);
            RecursiveSquarify(node.Children[i], childBounds, rooms);
        }
    }


    private Bounds CalculateChildBounds(Bounds parentBounds, float ratio, bool horizontal)
    {
        Bounds childBounds;

        if (horizontal)
        {
            float width = parentBounds.size.x * ratio;

            Vector3 childCentre = new Vector3((parentBounds.center.x - parentBounds.extents.x) + (width / 2), parentBounds.center.y, parentBounds.center.z);
            Vector3 childSize = new Vector3(width, parentBounds.size.y, parentBounds.size.z);
            childBounds = new Bounds(childCentre, childSize);

            Vector3 parentCentre = new Vector3(parentBounds.center.x + width, parentBounds.center.y, parentBounds.center.z);
            Vector3 parentSize = new Vector3(parentBounds.size.x - width, parentBounds.size.y, parentBounds.size.z);
            parentBounds = new Bounds(parentCentre, parentSize);
        }
        else
        {
            float height = parentBounds.size.z * ratio;

            Vector3 childCentre = new Vector3(parentBounds.center.x, parentBounds.center.y, (parentBounds.center.z - parentBounds.extents.z) + (height / 2));
            Vector3 childSize = new Vector3(parentBounds.size.x, parentBounds.size.y, height);
            childBounds = new Bounds(childCentre, childSize);

            Vector3 parentCentre = new Vector3(parentBounds.center.x, parentBounds.center.y, parentBounds.center.z + height);
            Vector3 parentSize = new Vector3(parentBounds.size.x, parentBounds.size.y, parentBounds.size.z - height);
            parentBounds = new Bounds(parentCentre, parentSize);
        }


        return childBounds;
    }
}
