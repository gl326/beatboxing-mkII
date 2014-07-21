using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;

[System.Serializable]
public class EchoNestAnalysis : IParseable {

	public EchoNestAnalysisMeta meta = new EchoNestAnalysisMeta();
	public EchoNestAnalysisTrack track = new EchoNestAnalysisTrack();
	public EchoNestAnalysisTime[] bars = new EchoNestAnalysisTime[0];
	public EchoNestAnalysisTime[] beats = new EchoNestAnalysisTime[0];
	public EchoNestAnalysisTime[] tatums = new EchoNestAnalysisTime[0];
	public EchoNestAnalysisTime[] sections = new EchoNestAnalysisTime[0];
	public EchoNestAnalysisSegment[] segments = new EchoNestAnalysisSegment[0];
	
	public float loudness_max;
	public float loudness_min;
	
	string[] keys = new string[]{ "C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "B"};
	string[] modality = new string[] {"minor", "major"};
	
	public EchoNestAnalysis(){}
	
	public EchoNestAnalysis( string text )
	{
		Parse(text);
	}
	
	public void Parse( string text )
	{
		Hashtable data =  EchoNestParse.Decode( text );
		
		meta = new EchoNestAnalysisMeta();
		track = new EchoNestAnalysisTrack();

		EchoNestParse.Populate( meta, data["meta"] as Hashtable );
		EchoNestParse.Populate( track, data["track"] as Hashtable );
		
		bars = EchoNestParse.PopulateTimeArray( data["bars"] as ArrayList );
		beats = EchoNestParse.PopulateTimeArray( data["beats"] as ArrayList );
		tatums = EchoNestParse.PopulateTimeArray( data["tatums"] as ArrayList );
		sections = EchoNestParse.PopulateTimeArray( data["sections"] as ArrayList );
		
		segments = EchoNestParse.PopulateSegmentArray( data["segments"] as ArrayList );
	}
	
	public int GetSectionFromTime( float time )
	{
		for( int i = 0; i < sections.Length; i++ )
		{
			if( time <= (sections[i].start + sections[i].duration) ) return i;
		}
		
		return 0;
	}
	
	public int GetIndexFromTime( float time, EchoNestAnalysisResolution resolution )
	{
		EchoNestAnalysisTime[] data = GetResolutionData( resolution );
		float percentage = time / (float)meta.seconds;
		int i = (int)((float)data.Length * percentage);
		i = (int)Mathf.Clamp(i, 0, data.Length - 1 );

		float offset = data[0].start;
		time += offset;

		for(int j = 0; j < data.Length; j++)
		{
			if( data[i].start > time )
			{
				if( i - 1 >= 0 ) i--;
				else break;
			} else if( data[i].start + data[i].duration < time ) {
				if( i + 1 < data.Length - 1 ) i++;
				else break;
			} else {
				break;
			}
		}

		return i;
	
	
	}
	
	
	public EchoNestAnalysisSegment GetSegmentDataFromTime( float time )
	{
		int i = GetIndexFromTime( time, EchoNestAnalysisResolution.Segment );
		return segments[i];
	}
	
	public void CalculateMaxima()
	{
		loudness_max = segments.OrderByDescending( x => x.loudness_max ).First().loudness_start;
		loudness_min = segments.OrderBy( x => x.loudness_max ).First().loudness_start;
	}
	
	public float RelativeLoudnessAt( int index )
	{
		if( index < 0 || index > segments.Length ) return 0;
		
		EchoNestAnalysisSegment segment = segments[index];
		
		return (segment.loudness_start - loudness_min ) / (loudness_max - loudness_min);
		
	}

	
	public EchoNestAnalysisTime[] GetResolutionData( EchoNestAnalysisResolution resolution )
	{
		EchoNestAnalysisTime[] result = segments;
		switch( resolution )
		{
			case EchoNestAnalysisResolution.Section:
				result = sections;
			break;
			
			case EchoNestAnalysisResolution.Bar:
				result = bars;
			break;
			
			case EchoNestAnalysisResolution.Beat:
				result = beats;
			break;
			
			case EchoNestAnalysisResolution.Tatum:
				result = tatums;
			break;
			
			case EchoNestAnalysisResolution.Segment:
			default:
				result = segments;
			break;
		}
		
		return result;
	}
	
	
	public string duration
	{
		get
		{
			return System.String.Format("{0}:{1:00}", (int)(meta.seconds / 60), (int)(meta.seconds % 60) ) ;
		}
	}
	
	public string key
	{
		get
		{
			return keys[ track.key ] + " " + modality[ track.mode ];
		}
	}
	
	public string counts
	{
		get
		{
			return sections.Length + "|" + bars.Length + "|" + beats.Length + "|" + segments.Length + "|" + tatums.Length;
		}
	}
	
	public bool isAnalyzed
	{
		get
		{
			return (meta != null && meta.analyzer_version != "" );
		}
	}
	
}

[System.Serializable]
public class EchoNestAnalysisMeta
{
	public string analyzer_version;
	public string detailed_status;
	public string filename;
	public string artist;
	public string album;
	public string title;
	public string genre;
	public int bitrate;
	public int sample_rate;
	public float seconds;
	public int status_code;
	public long timestamp;
	public float analysis_time;
}

[System.Serializable]
public class EchoNestAnalysisTrack
{
	public int num_samples;
	public float duration;
	public string sample_md5;
	public string decoder;
	public float offset_seconds;
	public float window_seconds;
	public int analysis_sample_rate;
	public int analysis_channels;
	public float end_of_fade_in;
	public float start_of_fade_out;
	public float loudness;
	public float tempo;
	public float tempo_confidence;
	public int time_signature;
	public float time_signature_confidence;
	public int key;
	public float key_confidence;
	public int mode;
	public float mode_confidence;
}

[System.Serializable]
public class EchoNestAnalysisTime
{
	public float start;
	public float duration;
	public float confidence;
}


[System.Serializable]
public class EchoNestAnalysisSegment : EchoNestAnalysisTime
{
	public float loudness_start;
	public float loudness_max_time;
	public float loudness_max;
	public float[] pitches;
	public float[] timbre;
}


public enum EchoNestAnalysisResolution
{
	Section,
	Bar,
	Beat,
	Tatum,
	Segment
}