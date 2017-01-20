﻿using UnityEngine;

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
        private int[] _tris;

        public void Start()
        {
            _map = new float[Width + 1, Height + 1];
            _fluctuate = new float[Width + 1, Height + 1];
            _mesh = new Mesh();

            _vertices = new Vector3[(Width + 1) * (Height + 1)];
            _tris = new int[Width * Height * 3 * 2];

            for (int j = 0; j < Height; ++j)
            {
                for (int i = 0; i < Width; ++i)
                {
                    var kutuNo = j * Width + i;

                    var sol_ust = kutuNo + j;
                    var sag_ust = sol_ust + 1;
                    var sol_alt = sag_ust + Width;
                    var sag_alt = sol_alt + 1;

                    _tris[kutuNo * 6 + 0] = sol_ust;
                    _tris[kutuNo * 6 + 1] = sag_alt;
                    _tris[kutuNo * 6 + 2] = sol_alt;

                    _tris[kutuNo * 6 + 3] = sol_ust;
                    _tris[kutuNo * 6 + 4] = sag_ust;
                    _tris[kutuNo * 6 + 5] = sag_alt;
                }
            }

            for (int j = 0; j < Height + 1; ++j)
            {
                for (int i = 0; i < Width + 1; ++i)
                {
                    _vertices[j * (Width + 1) + i] = new Vector3(i, BaseDepth, j);
                    _map[i, j] = BaseDepth;
                    _fluctuate[i, j] = Random.Range(0, 2 * Mathf.PI);
                }
            }

            _mesh.vertices = _vertices;
            _mesh.triangles = _tris;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            GetComponent<MeshFilter>().mesh = _mesh;
        }

        void DoFluctuate()
        {
            for (int i = 0; i < Width + 1; ++i)
            {
                for (int j = 0; j < Height + 1; ++j)
                {
                    _fluctuate[i, j] += 0.05f;
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
                    _vertices[j * (Width + 1) + i].y = _map[i, j];
                }
            }
        }

        public void Update()
        {
            DoFluctuate();
            UpdateMap();
            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _tris;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
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