using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public float Strength;
    public float StartTime;

    public Vector3 Center;

	public float Speed = 0.2f;
	public List<Wave> currentWaves=new List<Wave>();
	public float maxDistance=10;
	public float checkWaveEffect(Vector3 point,float time){
		float totalChange = 0.0f;
		for(int i=0;i>currentWaves.Count;i++){
		
			float waveCurrLength = Speed * (time - StartTime);
			float distance = Vector3.Distance (Center, point);
			float waveWidth = Strength;
			if (  distance<waveCurrLength+waveWidth&&distance>waveCurrLength-waveWidth) {
				distance= (waveCurrLength + waveWidth) - distance;
				totalChange += (distance - waveWidth) / waveWidth;
			}
			if (waveCurrLength > maxDistance) {
				currentWaves.RemoveAt(i);
				i--;
			}
		}
		return totalChange;
	
	}
}
