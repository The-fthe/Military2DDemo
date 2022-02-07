using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace InGameAsset.Scripts.AI
{
    public class AIDetector : SerializedMonoBehaviour
    {
        public event Action OnTargetFind;
        [OdinSerialize]public Transform Target { get; set; }
        [OdinSerialize]public bool TargetVisible { get; private set; }

        [SerializeField] [Range(0, 360)] public float _viewAngle = 30f;
        [SerializeField] LayerMask _targetMask;
        [SerializeField] LayerMask _obstacleMask;
        [SerializeField] MeshFilter _viewMeshFilter;
        [SerializeField] float _delayTime = 0.2f;
        [SerializeField] float _meshResolution = 4;
        [SerializeField] int _edgeResolveIteration = 4;
        [SerializeField] float _edgeDstThreshold = 0.5f;
        [SerializeField] Color _fieldOfViewColor = Color.cyan;
        [SerializeField] Color _detectedColor = Color.red;
        MeshRenderer _meshRenderer;
        IEnumerator callFindUnit;

        //  public List<Transform> visibleTargets = new List<Transform>();
       public float ViewRadius = 5f;

        Mesh _viewMesh;
        float _zOffset = -1;
        Renderer _renderer;
        string[] _sortingLayerName;

        void OnEnable() => _renderer = GetComponent<Renderer>();
        bool gameIsStarted;

        void Start()
        {
            _viewMeshFilter = GetComponent<MeshFilter>();
            _zOffset = transform.position.z;
            _viewMesh = new Mesh();
            _viewMesh.name = $"View Mesh {gameObject.name}";
            _viewMeshFilter.mesh = _viewMesh;
            _meshRenderer ??= GetComponent<MeshRenderer>();
            _meshRenderer.material.color = _fieldOfViewColor;
            SetSortedLayer();
            FindObjectOfType<PlayerManager>().OnPlayerActivated.AddListener(Initialize);
            callFindUnit = FindTargetWithDelay(_delayTime);
           StartCoroutine(callFindUnit);
           // Initialize();
        }

        void Initialize()
        {
            gameIsStarted= true;
        }

        void SetSortedLayer()
        {
            _renderer.sortingLayerName = "BottomDetail";
            _renderer.sortingOrder = 1;
        }

        void LateUpdate() => DrawFieldOfView();

        IEnumerator FindTargetWithDelay(float delay)
        {
                gameIsStarted = true;
                FindVisibleTargets();
                yield return new WaitForSeconds(delay);
                StopCoroutine(callFindUnit);
                if (gameIsStarted)
                {
                    callFindUnit = FindTargetWithDelay(delay);
                    StartCoroutine(callFindUnit);
                }
        }
        public void Aim(Vector2 pointPos)
        {
            var turretDirection = (Vector3) pointPos - transform.position;
            var desiredAngle = Mathf.Atan2(turretDirection.y, turretDirection.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.Euler(0, 0, desiredAngle),
                150 * Time.deltaTime); //TODO: find better way to avoid repeat update
        }

        void FindVisibleTargets()
        {
            Collider2D[] targetInViewRadius = Physics2D.OverlapCircleAll(transform.position, ViewRadius, _targetMask);
            for (int i = 0; i < targetInViewRadius.Length; i++)
            {
                 Target = targetInViewRadius[i].transform;
                Vector3 dirToTarget = new Vector3((Target.position - transform.position).x,
                    (Target.position - transform.position).y, 0);
                if (Vector3.Angle(transform.right, dirToTarget) < _viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, Target.position);
                    if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
                    {
                        _meshRenderer.material.color = _detectedColor;
                        TargetVisible = true;
                        OnTargetFind?.Invoke();
                        return;
                    }
                }
            }
            _meshRenderer.material.color = _fieldOfViewColor;
            TargetVisible = false;
        }

        void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
            float stepAngleSize = _viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.z - _viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);
                if (i > 0)
                {
                    bool edgeDstThreholdExceeded = Mathf.Abs(oldViewCast.Dst - newViewCast.Dst) > _edgeDstThreshold;
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
            _viewMesh.triangles = triangles.Reverse().ToArray();
            _viewMesh.RecalculateNormals();
        }

        EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.Angle;
            float maxAngle = maxViewCast.Angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < _edgeResolveIteration; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);
                bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.Dst - newViewCast.Dst) > _edgeDstThreshold;
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
            if (Physics2D.Raycast((Vector2) transform.position, dir, ViewRadius, _obstacleMask))
            {
                hit = Physics2D.Raycast((Vector2) transform.position, dir, ViewRadius, _obstacleMask);
                Vector3 displayDistance = new Vector3(hit.point.x, hit.point.y, _zOffset);
                return new ViewCastInfo(true, displayDistance, hit.distance, globalAngle);
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

        void OnDestroy()
        {
          //  PlayerSpawner.OnPlayerSpawn -= Initialize;
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
}