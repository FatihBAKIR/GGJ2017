using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DefaultNamespace
{
    public class Water : MonoBehaviour
    {
        public int Width;
        public int Height;

        public int Resolution;

        public float BaseDepth;

        private float[,] _map;
        private float[,] _fluctuate;

        private Mesh _mesh;

        private Vector3[] _vertices;
        private Vector3[,] _forces;
        private int[] _trisCache;

        private List<int>[,] _gridHints;
        List<int> _tris = new List<int>();

        public WaveSystem waveSystem = new WaveSystem();

        public void Start()
        {
            _forces = new Vector3[Width + 1, Height + 1];
            _map = new float[Width + 1, Height + 1];
            _fluctuate = new float[Width + 1, Height + 1];
            _mesh = new Mesh();

            _vertices = new Vector3[Width * Height * 6];
            _gridHints = new List<int>[Width + 1, Height + 1];

            for (int i = 0; i < Width + 1; i++)
            {
                for (int j = 0; j < Height + 1; j++)
                {
                    _gridHints[i, j] = new List<int>();
                }
            }

            var nextVert = 0;

            for (int j = 0; j < Height; ++j)
            {
                for (int i = 0; i < Width; ++i)
                {
                    var kutuNo = j * Width + i;

                    var tri1_0 = nextVert++;
                    var tri1_1 = nextVert++;
                    var tri1_2 = nextVert++;

                    var tri2_0 = nextVert++;
                    var tri2_1 = nextVert++;
                    var tri2_2 = nextVert++;

                    _gridHints[i, j].Add(tri1_0);
                    _gridHints[i, j].Add(tri2_0);

                    _gridHints[i + 1, j].Add(tri1_1);

                    _gridHints[i, j + 1].Add(tri2_2);

                    _gridHints[i + 1, j + 1].Add(tri1_2);
                    _gridHints[i + 1, j + 1].Add(tri2_1);

                    _tris.Add(tri1_0);
                    _tris.Add(tri1_1);
                    _tris.Add(tri1_2);
                    _tris.Add(tri2_0);
                    _tris.Add(tri2_1);
                    _tris.Add(tri2_2);
                }
            }


            for (int j = 0; j < Height + 1; ++j)
            {
                for (int i = 0; i < Width + 1; ++i)
                {
                    _fluctuate[i, j] = Random.Range(0, 2 * Mathf.PI);

                    foreach (var index in _gridHints[i, j])
                    {
                        _vertices[index] = new Vector3(i - Width / 2, 0, j - Height / 2);
                    }
                }
            }

            _trisCache = _tris.ToArray();

            Update();

            GetComponent<MeshFilter>().mesh = _mesh;
        }

        void UpdateMap()
        {
            for (int i = 0; i < Width + 1; i += Resolution)
            {
                for (int j = 0; j < Height + 1; j += Resolution)
                {
                    _fluctuate[i, j] += Random.Range(0.005f, 0.015f);
                    _map[i, j] = BaseDepth + Mathf.Sin(_fluctuate[i, j]) / 2.5f + 0.5f;
                    _forces[i, j] = Vector3.zero;
                }
            }

            for (int i = 0; i < Width; i += Resolution)
            {
                for (int j = 0; j < Height; j += Resolution)
                {
                    var a = _map[i, j];
                    var b = _map[i + Resolution, j];
                    var c = _map[i, j + Resolution];
                    var d = _map[i + Resolution, j + Resolution];

                    for (var l = 1; l < Resolution; ++l)
                    {
                        var t = l * 1.0f / Resolution;

                        _map[i + l, j] = a + (b - a) * t;
                        _map[i, j + l] = a + (c - a) * t;
                        _map[i + l, j + Resolution] = c + (d - c) * t;
                        _map[i + Resolution, j + l] = b + (d - b) * t;
                        _map[i + l, j + l] = a + (d - a) * t;

                        _forces[i + l, j] = Vector3.zero;
                        _forces[i, j + l] = Vector3.zero;
                        _forces[i + l, j + Resolution] = Vector3.zero;
                        _forces[i + Resolution, j + l] = Vector3.zero;
                        _forces[i + l, j + l] = Vector3.zero;
                    }
                }
            }

            for (int i = 0; i < waveSystem.currentWaves.Count; i++)
            {
                var wave = waveSystem.currentWaves[i];
                if (wave.Parameter(Time.timeSinceLevelLoad) >= 1)
                {
                    waveSystem.currentWaves.RemoveAt(i);
                    i--;
                    continue;
                }
                foreach (var res in wave.GetAffectedPoints(Time.timeSinceLevelLoad, Width, Height))
                {
                    _map[(int)res.Point.x, (int)res.Point.z] += res.Height;
                    _forces[(int)res.Point.x, (int)res.Point.z] += res.Force;
                }
            }

            for (int i = 0; i < Width + 1; i++)
            {
                for (int j = 0; j < Height + 1; ++j)
                {
                    foreach (var index in _gridHints[i, j])
                    {
                        _vertices[index].y = _map[i, j];
                    }
                }
            }
        }

        public float GetHeight(Vector3 pos)
        {
            return _map[(int) pos.x + Width / 2, (int) pos.z + Height / 2];
        }

        public Vector3 GetForce(Vector3 pos)
        {
            var v = _forces[(int) pos.x + Width / 2, (int) pos.z + Height / 2];
            v.x *= -1;
            return v;
        }

        private bool _animate;

        private float _hold;
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _hold = 0;
            }
            if (Input.GetMouseButton(0))
            {
                _hold += Time.deltaTime;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit rh;
                if (Physics.Raycast(r, out rh))
                {
                    var x = transform.InverseTransformPoint(rh.point);
                    waveSystem.currentWaves.Add(new Wave
                    {
                        Center = new Vector3((int) x.x + Width / 2, 0, (int) x.z + Height / 2),
                        Strength = 8f + _hold,
                        StartTime = Time.timeSinceLevelLoad
                    });
                }
            }

            UpdateMap();

            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _trisCache;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < Width + 1; i++)
            {
                for (int j = 0; j < Height + 1; j++)
                {
                    Gizmos.DrawLine(new Vector3(i, 0, j), new Vector3(i, _forces[i, j].magnitude, j));
                }
            }
        }
    }
}