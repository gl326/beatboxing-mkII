using UnityEngine;
using System.Collections;


public class Waveform : MonoBehaviour {

	public bool isMono;
	public bool isStereo;
	
	public float[] left = new float[0];
	public float[] right = new float[0];
	
	public int sampleWindowSize = 0;
	
	private float[] lastData = new float[0];
	private int read;
	private int write;
	
	public void Start()
	{
		if( audio != null )
		{
			float t = audio.time;
			audio.Stop();
			audio.Play();
			audio.time = t;
		}
	}
	
	
	public void OnAudioFilterRead( float[] data, int channels )
	{
		
		SetupChannels( data.Length, channels );
		
		if( lastData.Length == 0 ) lastData = data;
		
		int wrote = 0;
		bool copied = false;
		while( read < lastData.Length )
		{
			
			for( int r = read; r < lastData.Length; r += channels )
			{
				if( isStereo )
				{
					left[write] = data[ r ];
					right[write] = data[ r + 1 ];
				} else {
					left[write] = data[ r ];
				}
			
				write++;
				wrote++;
			
				if( write >= left.Length )
				{
					write = 0;
					read = r;
					
					return;
				}
			}
			
			
		
			if( !copied )
			{
				lastData = data;
				read = 0;
			} else {
				return;
			}
		
		}
		
		
	}
	
	
	
	void SetupChannels( int numSamples, int i )
	{
	
		numSamples = i == 1 ? numSamples : numSamples / 2;
		sampleWindowSize = Mathf.Max( sampleWindowSize, numSamples );
	
		switch( i )
		{
			case 1:
				isMono = true;
				isStereo = false;
				if( left.Length != sampleWindowSize )
				{
					left = new float[sampleWindowSize];
				}
			break;
			
			case 2:
				isMono = false;
				isStereo = true;
			
				if( left.Length != sampleWindowSize )
				{
					left = new float[sampleWindowSize];
					right = new float[sampleWindowSize];
				}
			break;
		}

	}
}
