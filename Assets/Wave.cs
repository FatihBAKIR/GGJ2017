using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Water;

public struct WaveResult
{
    public Vector3 Point;
    public float Height;
    public Vector3 Force;
}

[Serializable]
public class WaveSystem
{
    public List<Wave> currentWaves = new List<Wave>();
    public static float maxDistance = 30;

    public WaveResult checkWaveEffect(Vector3 point, float time)
    {
        float totalChange = 0.0f;
        Vector3 totalForce = Vector3.zero;

        for (int i = 0; i < currentWaves.Count; i++)
        {
            var w = currentWaves[i];
            var life = time - w.StartTime;
            var waveCurrLength = w.Speed * life;
            var t = waveCurrLength / maxDistance;
            var distance = Vector3.Distance(w.Center, point);
            var waveWidth = w.Strength - t * 8;

            if (distance < waveCurrLength + waveWidth / 2 && distance > waveCurrLength - waveWidth / 2)
            {
                var diff = (waveCurrLength + waveWidth / 2) - distance;
                totalChange += Mathf.Sin((diff - waveWidth) / waveWidth * Mathf.PI) / 3f;
                totalForce += (point - w.Center).normalized * totalChange * (1 - t);
            }

            if (waveCurrLength > maxDistance)
            {
                currentWaves.RemoveAt(i);
                i--;
            }
        }

        return new WaveResult{Height = totalChange, Force = totalForce};
    }
}

[Serializable]
public class Wave
{
    public float Strength;
    public float StartTime;
    public float Speed = 10;
    public Vector3 Center;

    static Wave()
    {
        GenerateLookup();
    }

    public static List<Vector3>[] Lookup;
    static void GenerateLookup()
    {
        Lookup = new List<Vector3>[30];
        for (int i = 1; i < 30; i++)
        {
            Lookup[i] = new List<Vector3>();
            for (int x = -i; x <= i; ++x)
            {
                for (int y = -i; y <= i; y++)
                {
                    float dist = new Vector3(x, 0, y).magnitude;
                    if (dist <= i)
                    {
                        if (Lookup.FirstOrDefault(a => a != null && a.Contains(new Vector3(x, 0, y))) == null)
                        {
                            Lookup[i].Add(new Vector3(x ,0 , y));
                           
                        }
                    }
                }
            }
        }
    }

    public float Parameter(float time)
    {
        var waveCurrLength = Speed * (time - StartTime);
        var t = waveCurrLength / WaveSystem.maxDistance;
        return t;
    }

    public IEnumerable<WaveResult> GetAffectedPoints(float time, int w, int h)
    {
        var waveCurrLength = Speed * (time - StartTime);
        var t = waveCurrLength / WaveSystem.maxDistance;
        var waveWidth = Strength - t * 8;

        var big_radius = Math.Min(29, waveCurrLength + waveWidth / 2);
        var small_radius = Math.Max(1, big_radius - waveWidth);

        for (int i = (int)Math.Floor(small_radius); i <= (int)Math.Ceiling(big_radius); i++)
        {
            foreach (var offset in Lookup[i])
            {
                var point = Center + offset;

                if (point.x < 0 || point.z < 0 || point.x >= w || point.z >= h)
                {
                    continue;
                }

                var distance = Vector3.Distance(Center, point);

                if (distance < waveCurrLength + waveWidth / 2 && distance > waveCurrLength - waveWidth / 2)
                {
                    var diff = (waveCurrLength + waveWidth / 2) - distance;
                    var totalChange = Mathf.Sin((diff - waveWidth) / waveWidth * Mathf.PI) / 3f * ((Strength + 2) / 10);
                    var totalForce = (point - Center).normalized * totalChange * (1 - t);

                    yield return new WaveResult{Point = point, Force = totalForce, Height = totalChange};
                }
            }
        }
    }
}
