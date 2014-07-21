using UnityEngine;
using System.Collections;

[System.Serializable]
public class MusicData : UniqueObject
{

	public AudioClip audioClip;
	
	public EchoNestAnalysis analysis = new EchoNestAnalysis();
	public EchoNestTrack trackInfo = new EchoNestTrack();
	
	public EchoNestAnalysisResolution resolution;
	
	public float length
	{
		get{  return audioClip.length; }
	}
	
	public float secondsPerMeasure
	{
		get{ return secondsPerBeat * 4; }
	}
	
	public float secondsPerBeat
	{
		get{ return 1 / (float)bpm * 60; }
	}
	
	public float bpm
	{
		get{ return analysis.track.tempo; }
	}
	
	public string name 
	{
		get{ return analysis.meta.title; }
	}
	
	public int timeSignature
	{
		get{ return analysis.track.time_signature; }
	}
	
	public override string ToString()
	{
		return name;
	}
}



[System.Serializable]
public class MusicReference : UniqueReference<MusicData>{}