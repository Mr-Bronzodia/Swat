using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

public class UnitVision : MonoBehaviour
{
    private LayerMask _characterMask;
    private LayerMask _obstacleMask;
    private Unit _thisUnit;

    private Mesh _viewMesh;

    [SerializeField]
    private float _viewRadius;
    [SerializeField, Range(0, 360)]
    private float _viewAngle;
    [SerializeField]
    private float _meshResolution;
    [SerializeField]
    private MeshFilter _viewMeshFilter;
    [SerializeField]
    private bool _shouldDrawViewMesh = true;

    public float ViewRadius { get => _viewRadius; }
    public float ViewAngle { get => _viewAngle; }
    public List<Unit> _visibleTargetsList;


    private struct ViewCastInfo
    {
        public bool Hit;
        public float Distance;
        public Vector3 Point;
        public float Angle;

        public ViewCastInfo(bool hit, float distance, Vector3 point, float angle)
        {
            Hit = hit;
            Distance = distance;
            Point = point;
            Angle = angle;
        }
    }


    private void Awake()
    {
        Assert.AreNotEqual(0, _viewRadius, "ViewRadius is 0 in UnitVision on unit " + gameObject.name);
        Assert.AreNotApproximatelyEqual(0, _viewAngle, "ViewAngle is 0 in UnitVision on unit " + gameObject.name);
        Assert.IsNotNull(_viewMeshFilter, "View mesh filter is empty in " + gameObject.name);
        Assert.AreEqual(LayerMask.NameToLayer("CharacterMask"), _viewMeshFilter.gameObject.layer, "View mesh not assigned character mask layer in " + gameObject.name);

        _viewMesh = new Mesh();
        _viewMesh.name = "ViewMesh";
        _viewMeshFilter.mesh = _viewMesh;

        _characterMask = LayerMask.GetMask("Character");
        _obstacleMask = LayerMask.GetMask("Obstacle");

        _visibleTargetsList = new List<Unit>();
        _thisUnit = gameObject.GetComponent<Unit>();
    }

    private void Start()
    {
        StartCoroutine("FindTargets", 1f);
    }

    private void LateUpdate()
    {
        if (!_shouldDrawViewMesh) return;
        DrawViewMesh();
    }

    IEnumerator FindTargets(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindTargetsInView();
        }
    }

    private void FindTargetsInView()
    {
        _visibleTargetsList.Clear();

        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, _viewRadius, _characterMask);

        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            Transform target = targetsInRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) > _viewAngle / 2f) continue;

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask)) continue;

            Unit t = targetsInRadius[i].GetComponent<Unit>();

            if (t == _thisUnit) continue;

            if (t.IsHostage) continue;

            if (_thisUnit.BlackBoard.Team == t.BlackBoard.Team) continue;

            _visibleTargetsList.Add(targetsInRadius[i].GetComponent<Unit>());
        }

    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, _viewRadius, _obstacleMask))
        {
            return new ViewCastInfo(true, hit.distance, hit.point, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, _viewRadius, transform.position + dir * _viewRadius, globalAngle);
        }
    }

    private void DrawViewMesh()
    {
        if (Vector3.Distance(Camera.main.transform.position, transform.position) >= 25f) return;

        int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
        float stepAngleSize = _viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - _viewAngle / 2f + stepAngleSize * i;

            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.Point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = transform.InverseTransformPoint(transform.position);

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();

    }


    public Vector3 DirectionFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal) angleInDegrees += transform.eulerAngles.y;

        Vector3 direction = new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));

        return direction;
    }
}
