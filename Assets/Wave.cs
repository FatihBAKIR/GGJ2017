using System;
using System.Collections.Generic;
using UnityEngine;

public struct WaveResult
{
    public float Height;
    public Vector3 Force;
}

[Serializable]
public class WaveSystem
{
    public List<Wave> currentWaves = new List<Wave>();
    public float maxDistance = 30;

    public WaveResult checkWaveEffect(Vector3 point, float time)
    {
        float totalChange = 0.0f;
        Vector3 totalForce = Vector3.zero;

        for (int i = 0; i < currentWaves.Count; i++)
        {
            var w = currentWaves[i];
            var life = time - w.StartTime;
            float waveCurrLength = w.Speed * life;
            var t = waveCurrLength / maxDistance;
            float distance = Vector3.Distance(w.Center, point);
            float waveWidth = w.Strength - t * 8;

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
}
