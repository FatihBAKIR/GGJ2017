﻿using System.Collections.Generic;
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

        private List<int>[,] _gridHints;
        List<int> _tris = new List<int>();

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
            for (int j = 0; j < Height + 1; j += Resolution)
            {
                for (int i = 0; i < Width + 1; i += Resolution)
                {
                    _fluctuate[i, j] += 0.01f;
                }
            }
        }

        void UpdateMap()
        {
            for (int j = 0; j < Height + 1; j += 1)
            {
                for (int i = 0; i < Width + 1; i += 1)
                {
                    _map[i, j] = BaseDepth + Mathf.Sin(_fluctuate[i, j]) / 5 + 0.5f;
                }
            }

            for (int j = 0; j < Height; j += Resolution)
            {
                for (int i = 0; i < Width; i += Resolution)
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
                    }

                    // diagonal first
                    for (var l = 1; l < Resolution; ++l)
                    {
                        var t = l * 1.0f / Resolution;
                        _map[i + l, j + l] = a + (d - a) * t;
                    }
                }
            }

            for (int j = 0; j < Height + 1; ++j)
            {
                for (int i = 0; i < Width + 1; i++)
                {
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
        }
    }
}