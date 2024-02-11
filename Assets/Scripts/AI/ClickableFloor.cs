using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClickableFloor : MonoBehaviour, IClickable
{

    public List<Command> GetAvailableCommands(Unit unit)
    {
        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPosition = Vector3.zero;

        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }

        return new List<Command>() { new MoveCommand(unit, worldPosition) };
    }


    public List<Command> GetAvailableCommands(List<Unit> units)
    {
        List<Command> results = new List<Command>();

        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPosition = Vector3.zero;

        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }

        //int i = 0;
        //foreach(Unit unit in u)
        //{
        //    Vector3 dirToTarget = new Vector3(1,0,0);
        //    Vector3 finalPos = worldPosition + i * dirToTarget;
        //    Command move = new MoveCommand(unit, finalPos);
        //    results.Add(move);

        //    i += 1.5f;
        //}

        List<Command> sequence = new List<Command>();

        Unit lead = units[0];
        Vector3 leadForward = lead.gameObject.transform.forward;

        for (int i = 1; i < units.Count; i++)
        {
            Unit unit = units[i];
            Vector3 target = lead.BlackBoard.Position - i * leadForward;

            MoveCommand moveBehind = new MoveCommand(unit, target);
            sequence.Add(moveBehind);

            WaitUntillCommand leadWait = new WaitUntillCommand(lead, unit, typeof(FollowCommand));
            sequence.Add(leadWait);

            FollowCommand unitFollowLead = new FollowCommand(unit, lead, i);
            sequence.Add(unitFollowLead);
        }

        MoveCommand leadMove = new MoveCommand(lead, worldPosition);
        sequence.Add(leadMove);

        SequencerCommand sequenceCommand = new SequencerCommand(lead, "Team Move", sequence);

        results.Add(sequenceCommand);


        if (units.Count == 4)
        {
            List<Command> breachSeq = new List<Command>();

            Vector3 roomCentre = new Vector3(gameObject.transform.position.x, 1.5f, gameObject.transform.position.z);


            //if (Physics.Raycast(roomCentre, gameObject.transform.forward,out hit, Mathf.Infinity))
            //{
            //    roomHeight = hit.distance * 1.5f;
            //}
            //if (Physics.Raycast(roomCentre, gameObject.transform.right, out hit, Mathf.Infinity))
            //{
            //    roomWidth = hit.distance * 1.5f;
            //}

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            float roomHeight = meshRenderer.bounds.size.z * .7f;
            float roomWidth = meshRenderer.bounds.size.x * .7f;

            Bounds roomBounds = new Bounds(roomCentre, new Vector3(roomWidth, 0, roomHeight));

            //DrawBounds(_roomBounds);
            //Debug.Break();

            Vector3 corner1 = new Vector3(roomCentre.x - roomWidth / 2, 0, roomCentre.z - roomHeight / 2);
            Vector3 corner2 = new Vector3(roomCentre.x + roomWidth / 2, 0, roomCentre.z - roomHeight / 2);
            Vector3 corner3 = new Vector3(roomCentre.x - roomWidth / 2, 0, roomCentre.z + roomHeight / 2);
            Vector3 corner4 = new Vector3(roomCentre.x + roomWidth / 2, 0, roomCentre.z + roomHeight / 2);
            List<Vector3> corners = new List<Vector3>() { corner1, corner2, corner3, corner4 };
            List<Vector3> sortedCorners = corners.OrderBy(o => Vector3.Distance(o, lead.BlackBoard.Position)).ToList();

            foreach (Vector3 pos in corners)
            {
                Debug.DrawLine(roomCentre, pos);
            }
            Debug.Break();

            MoveCommand unit1ToFirstCorner = new MoveCommand(lead, sortedCorners[0]);
            breachSeq.Add(unit1ToFirstCorner);
            MoveCommand unit1ToSecondCorner = new MoveCommand(lead, sortedCorners[2]);
            breachSeq.Add(unit1ToSecondCorner);

            MoveCommand unit2ToOppositeCorner = new MoveCommand(units[1], sortedCorners[1]);
            breachSeq.Add(unit2ToOppositeCorner);
            MoveCommand unit2ToSecondCorner = new MoveCommand(units[1], sortedCorners[3]);
            breachSeq.Add(unit2ToSecondCorner);
            WaitToFinishCommand unit1Wait = new WaitToFinishCommand(units[1], lead);
            breachSeq.Add(unit1Wait);

            MoveCommand unit3ToFirstCorner = new MoveCommand(units[2], sortedCorners[0]);
            breachSeq.Add(unit3ToFirstCorner);
            MoveCommand unit3HalfCorner = new MoveCommand(units[2], Vector3.Lerp(sortedCorners[0], sortedCorners[2], .5f));
            breachSeq.Add(unit3HalfCorner);
            WaitToFinishCommand unit2Wait = new WaitToFinishCommand(units[2], units[1]);
            breachSeq.Add(unit2Wait);

            MoveCommand unit4ToOpposite = new MoveCommand(units[3], sortedCorners[1]);
            breachSeq.Add(unit4ToOpposite);
            MoveCommand unit4ToSecond = new MoveCommand(units[3], Vector3.Lerp(sortedCorners[1], sortedCorners[3], .5f));
            breachSeq.Add(unit4ToSecond);
            WaitToFinishCommand unit3Wait = new WaitToFinishCommand(units[3], units[2]);
            breachSeq.Add(unit3Wait);

            SequencerCommand breachSequence = new SequencerCommand(lead, "Breach Room", breachSeq);
            results.Add(breachSequence);
        }

        return results;
    }

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
}
