using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AudioSource))]
public class Music : Audio 
{

	public MusicData data;

	private float _nextPlayTime;
	private double _startDspTime;
	
	private Soundboard _soundboard;
	private double _lastTime;
	
	void Start()
	{
		_soundboard = Soundboard.Instance;
	}

	public void Play()
	{
		audio.clip = data.audioClip;
		audio.Play();
	}
	
	public void LoadData( MusicData data )
	{
		this.data = data;
		audio.clip = this.data.audioClip;
	}
	
	virtual public void Beat()
	{
		if( _soundboard != null ){ 
			_soundboard.Beat( beat );
		}
	}
	
	
	public int NearestBeat()
	{
		if( timeToNextBeat < timeToPrevBeat )
		{
			return beat;
		} else {
			return beat - 1;
		}
	}
	
	public float TimeToNearestBeat()
	{
		return Mathf.Min( timeToNextBeat, timeToPrevBeat );
	}
	
	void Update()
	{
		if(audio.isPlaying)
		{
			if(audio.time >= audio.clip.length)
			{
				audio.Stop();
				audio.Play();
			}
			
			
			if( timeToNextBeat < _lastTime) Beat();
			_lastTime = timeToNextBeat;
		}
	}

	public int measure
	{
		get{ return (int)( dspTime / data.secondsPerMeasure ); }
	}
	
	public int beat
	{
		get{ return (int)( dspTime / data.secondsPerBeat ); }
	}
	
	public float timeToNextBeat
	{
		get{ return (float)dspTime - (beat * data.secondsPerBeat); }
	}
	
	public float timeToPrevBeat
	{
		get{ return Mathf.Abs( (float)dspTime - ((beat - 1) * data.secondsPerBeat) ); }
	}
	
	double dspTime
	{
		get{ 
			return (double)audio.time;
		}
	}
	
	
	public float time
	{
		set{ audio.time = value; }
		get{ return audio.time; }
	}
	
	public float length
	{
		get{ return audio.clip.length; }
	}

}


