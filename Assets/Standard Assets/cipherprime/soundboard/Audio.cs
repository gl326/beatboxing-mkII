using UnityEngine;
using System.Collections;

[RequireComponent( typeof(AudioSource) )]
public class Audio : MonoBehaviour {

	public Audio Delay( float time )
	{
	//	_isDelayed = true;
		audio.Stop();
		audio.PlayDelayed( time );
		return this;
	}
	
	public Audio Priority( int priority )
	{
		audio.Stop();
		audio.priority = priority;
		audio.Play();
		return this;
	} 
	
	public Audio RandomPitch( float min, float max )
	{
		audio.pitch = Random.Range( min, max );
		return this;
	}
	
	public Audio Pitch( float pitch )
	{
		audio.pitch = pitch;
		return this;
	}
	
	public float Pitch()
	{
		return audio.pitch;
	}
	
	public Audio Loop()
	{
		audio.loop = true;
		return this;
	}

	public Audio Doppler( float doppler )
	{
		audio.dopplerLevel = doppler;
		return this;
	}

	public Waveform Waveform()
	{
		Waveform w = gameObject.GetComponent<Waveform>();
		if( w == null ) {
			w = gameObject.AddComponent<Waveform>();
		}
		return w;
	}
	
}
