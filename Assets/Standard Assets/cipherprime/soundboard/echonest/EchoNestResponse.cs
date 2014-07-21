using UnityEngine;
using System.Collections;


[System.Serializable]
public class EchoNestResponse : IParseable{

	[SerializeField]
	public EchoNestStatus status;
	
	[SerializeField]
	public EchoNestTrack track;

	public EchoNestResponse(){}
	public EchoNestResponse( string text )
	{
		Parse( text );
	}
	
	public void Parse( string text )
	{
		Hashtable data = EchoNestParse.Decode( text );
		Hashtable response = data["response"] as Hashtable;
		status = new EchoNestStatus();
		track = new EchoNestTrack();
		
		EchoNestParse.Populate( status, response["status"] as Hashtable );
		
		if( status.code == 0 )
		{
		
			EchoNestParse.Populate( track, response["track"] as Hashtable );
			Hashtable t = response["track"] as Hashtable;
			switch( t["status"] as string )
			{
				case "unknown":
					track.status = EchoNestAnalysisStatus.Unknown;
				break;
			
				case "pending":
					track.status = EchoNestAnalysisStatus.Pending;
				break;
			
				case "complete":
					track.status = EchoNestAnalysisStatus.Complete;
					track.summary = new EchoNestSummary();
					EchoNestParse.Populate( track.summary, t["audio_summary"] as Hashtable );
					
				break;
			
				case "error":
					track.status = EchoNestAnalysisStatus.Error;
				break;
			}
		} else {
			track.status = EchoNestAnalysisStatus.Error;
		}
	}
	
	
	
}

[System.Serializable]
public class EchoNestStatus
{
	public string version;
	public int code;
	public string message;
}


[System.Serializable]
public class EchoNestTrack
{
	public string id;
	public EchoNestAnalysisStatus status;
	public EchoNestSummary summary;
	public string artist;
	public string title;
	public string analyzer_version;
	public string release;
	public string audio_md5;
	public int bitrate;
	public int samplerate;
	public string md5;
	
	public string catalog;
	public string foreign_id;
	public string foreign_release_id;
	public string preview_url;
	public string release_image;
	public string song_id;
}

[System.Serializable]
public class EchoNestSummary
{
	public string analysis_url;
	public float danceablity;
	public float duration;
	public float energy;
	public int key;
	public float loudness;
	public float mode;
	public float speechiness;
	public float acousticness;
	public float liveness;
	public float tempo;
	public int time_signature;
}


public enum EchoNestAnalysisStatus
{
	Unknown,
	Pending,
	Complete,
	Error
}
