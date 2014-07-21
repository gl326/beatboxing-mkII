using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class EchoNestParse  {

	public static Hashtable Decode( string text )
	{
		return MiniJSON.jsonDecode( text ) as Hashtable;
	}
	
	public static void Populate( object metadata, Hashtable data )
	{
		if( data == null ) return;

		FieldInfo[] fields = metadata.GetType().GetFields();
		
		foreach( FieldInfo fieldInfo in fields )
		{				
			switch( fieldInfo.FieldType.ToString() )
			{
				case "System.Int32":
					fieldInfo.SetValue( metadata, Convert.ToInt32( data[ fieldInfo.Name ] ) );
				break;
				
				case "System.Single":
					fieldInfo.SetValue( metadata, Convert.ToSingle( data[ fieldInfo.Name ] ));
				break;
				
				case "System.Int64":
					fieldInfo.SetValue( metadata, Convert.ToInt64( data[ fieldInfo.Name ] ));
				break;
		
				case "System.String":
					fieldInfo.SetValue( metadata, data[ fieldInfo.Name ] );
				break;
			}

		}
	}
	
	
	public static EchoNestAnalysisTime[] PopulateTimeArray( ArrayList list )
	{
		EchoNestAnalysisTime[] result = new EchoNestAnalysisTime[ list.Count ];
		
		for( int i = 0; i < list.Count; i++ )
		{
			EchoNestAnalysisTime t = new EchoNestAnalysisTime();
			t.start = Convert.ToSingle( (list[i] as Hashtable)["start"] );
			t.duration = Convert.ToSingle( (list[i] as Hashtable)["duration"] );
			t.confidence = Convert.ToSingle( (list[i] as Hashtable)["confidence"] );
			
			result[i] = t;
		}
		
		return result;  
	}
	
	public static EchoNestAnalysisSegment[] PopulateSegmentArray( ArrayList list )
	{
		EchoNestAnalysisSegment[] result = new EchoNestAnalysisSegment[ list.Count ];
		
		for( int i = 0; i < list.Count; i++ )
		{
			EchoNestAnalysisSegment t = new EchoNestAnalysisSegment();
			t.start = Convert.ToSingle( (list[i] as Hashtable)["start"] );
			t.duration = Convert.ToSingle( (list[i] as Hashtable)["duration"] );
			t.confidence = Convert.ToSingle( (list[i] as Hashtable)["confidence"] );
			
			t.loudness_start = Convert.ToSingle( (list[i] as Hashtable)["loudness_start"] );
			t.loudness_max_time = Convert.ToSingle( (list[i] as Hashtable)["loudness_max_time"] );
			t.loudness_max = Convert.ToSingle( (list[i] as Hashtable)["loudness_max"] );
			
			t.pitches = PopulateFloatArray( (list[i] as Hashtable)["pitches"] as ArrayList );
			t.timbre = PopulateFloatArray( (list[i] as Hashtable)["timbre"] as ArrayList );
			
			result[i] = t;
		}
		
		return result;
	}
	
	
	static float[] PopulateFloatArray( ArrayList list )
	{
		float[] result = new float[ list.Count ];
		
		for( int i = 0; i < list.Count; i++ )
		{
			result[i] = Convert.ToSingle( list[i] );
		}
		
		return result;
	}
	
}
