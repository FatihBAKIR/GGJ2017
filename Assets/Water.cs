using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DefaultNamespace
{
    public class Water : MonoBehaviour
    {
        public int Width;
        public int Height;

        public float BaseDepth;

        private float[,] _map;
        private float[,] _fluctuate;

        private Mesh _mesh;

        private Vector3[] _vertices;

        private List<int>[,] _gridHints;
        List<int> _tris = new List<int>();
		private Wave waveSystem=new Wave();
		private float currentTime=0.0f;

        public void Start()
        {
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

            Update();

            GetComponent<MeshFilter>().mesh = _mesh;
        }

        void DoFluctuate()
        {
            for (int i = 0; i < Width + 1; ++i)
            {
                for (int j = 0; j < Height + 1; ++j)
                {
                    _fluctuate[i, j] += 0.01f;
                }
            }
        }

        void UpdateMap()
        {


            for (int j = 0; j < Height + 1; ++j)
            {
                for (int i = 0; i < Width + 1; ++i)
                {
                    _map[i, j] = BaseDepth + Mathf.Sin(_fluctuate[i, j]) / 5 + 0.5f;

					_map[i, j] +=waveSystem.checkWaveEffect(new Vector3(i,0,j),currentTime);

                    foreach (var index in _gridHints[i, j])
                    {
                        _vertices[index].y = _map[i, j];
                    }
                }
            }
        }

        public void Update()
        {
            DoFluctuate();
            UpdateMap();
            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _tris.ToArray();

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
			currentTime += Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            return;
            foreach (var vertex in _vertices)
            {
                Gizmos.DrawLine(new Vector3(vertex.x, 0, vertex.z), vertex);
            }
        }
    }
}