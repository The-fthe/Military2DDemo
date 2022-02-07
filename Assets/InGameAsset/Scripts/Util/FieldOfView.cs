using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FieldOfView : MonoBehaviour
{
    public float ViewRadius = 5f;
    [SerializeField] [Range(0, 360)] public float viewAngle;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] MeshFilter viewMeshFilter;
    [SerializeField] float _delayTime = 0.2f;
    [SerializeField] float _meshResolution;
    [SerializeField] int edgeResolveIteration;
    [SerializeField] float edgeDstThreshold;
    public List<Transform> visibleTargets = new List<Transform>();

    Mesh _viewMesh;
    float z_offset = -1;
    public Renderer _renderer;
    string[] sortingLayerName;

    void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        z_offset = transform.position.z;
        _viewMesh = new Mesh();
        _viewMesh.name = $"View Mesh {gameObject.name}";
        viewMeshFilter.mesh = _viewMesh;
        SetSortedLayer();
        StartCoroutine(FindTargetWithDelay(_delayTime));
    }

    void SetSortedLayer()
    {
        _renderer.sortingLayerName = "UpperDetail";
        _renderer.sortingOrder = 1;
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetInViewRadius = Physics2D.OverlapCircleAll(transform.position, ViewRadius, targetMask);
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = new Vector3((target.position - transform.position).x,(target.position - transform.position).y,0);
            if (Vector3.Angle(transform.right, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.z- viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            if (i > 0)
            {
                bool edgeDstThreholdExceeded = Mathf.Abs(oldViewCast.Dst - newViewCast.Dst) > edgeDstThreshold;
                if (oldViewCast.Hit != newViewCast.Hit ||
                    (oldViewCast.Hit && newViewCast.Hit && edgeDstThreholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    }

                    if (edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.Point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
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
         _viewMesh.triangles = triangles.Reverse().ToArray();
       _viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.Angle;
        float maxAngle = maxViewCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.Dst - newViewCast.Dst) > edgeDstThreshold;
            if (newViewCast.Hit == minViewCast.Hit && !edgeDstThresholdExceeded ||
                newViewCast.Hit != minViewCast.Hit && edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.Point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.Point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit;
        if (Physics2D.Raycast((Vector2) transform.position, dir, ViewRadius, obstacleMask))
        {
            hit = Physics2D.Raycast((Vector2) transform.position, dir, ViewRadius, obstacleMask);
            Vector3 DisplayDistance = new Vector3(hit.point.x, hit.point.y, z_offset);
            return new ViewCastInfo(true, DisplayDistance, hit.distance, globalAngle);
        }

        return new ViewCastInfo(false, transform.position + dir * ViewRadius, ViewRadius, globalAngle);
    }

    public Vector3 DirFromAngle(float angleInDegree, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegree += transform.eulerAngles.z;
        }

        return new Vector3(Mathf.Cos(angleInDegree * Mathf.Deg2Rad), Mathf.Sin(angleInDegree * Mathf.Deg2Rad), 0);
    }

    public struct EdgeInfo
    {
        public Vector3 PointA;
        public Vector3 PointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }

    public struct ViewCastInfo
    {
        public bool Hit;
        public Vector3 Point;
        public float Dst;
        public float Angle;

        public ViewCastInfo(bool hit, Vector3 point, float dst, float angle)
        {
            Hit = hit;
            Point = point;
            Dst = dst;
            Angle = angle;
        }
    }
}