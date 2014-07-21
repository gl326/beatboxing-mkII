using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System;

//EditorApplication.update

[CustomEditor(typeof(Soundboard))]
public class SoundboardEditor : Editor {

	Dictionary< MusicData, EchoNest > echoNestRequesters = new Dictionary<MusicData, EchoNest>();

	public override void OnInspectorGUI()
	{
	
		Soundboard soundboard = target as Soundboard;
		
		soundboard.volume = EditorGUILayout.Slider( "Volume", soundboard.volume, 0, 1 );
		
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		
		EditorGUILayout.LabelField("Music", EditorStyles.boldLabel);
		/*
		if( soundboard.music.spectrum != null )
		{
			Rect rect = GUILayoutUtility.GetRect( new GUIContent(), GUIStyle.none, new GUILayoutOption[]{ GUILayout.Height(64) } );
			GUI.DrawTexture( rect, soundboard.music.spectrum );
		}
		*/
		
		if( soundboard.music.Count == 0 )
		{
			if( GUILayout.Button("Add Music") )
			{
				soundboard.music.Add( new MusicData() );
			}
		}

		
		for( int i = 0; i < soundboard.music.Count; i++ )
		{
		
			MusicData music = soundboard.music[i];
//			if( Soundboard.Instance.currentMusic != null && music.audioClip == Soundboard.Instance.currentMusic.data.audioClip ) GUI.color = Color.cyan;
			
			EditorGUILayout.BeginHorizontal();
			
				bool showMusic = ComponentGUILayout.Foldout( (i + 1).ToString(), music );
				ComponentGUILayout.AddRemoveButtons<MusicData>( soundboard.music, i );
				music.audioClip = EditorGUILayout.ObjectField( music.audioClip, typeof(AudioClip), false ) as AudioClip;
			
			EditorGUILayout.EndHorizontal();
			
			if( showMusic && music.analysis != null )
			{	
				EditorGUI.indentLevel++;
				
				if( music.analysis.isAnalyzed )
				{
				
					Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 3);
					rect.width /= 2;
					rect.height /= 3;
									
				
					music.analysis.meta.title = EditorGUI.TextField( rect, music.analysis.meta.title , EditorStyles.boldLabel );
					rect.y += rect.height;
					music.analysis.meta.artist = EditorGUI.TextField( rect, music.analysis.meta.artist, EditorStyles.label );
					rect.y += rect.height;
					EditorGUI.LabelField( rect, music.analysis.duration );
					
					
					rect.y -= 2 * rect.height;
					rect.x += rect.width;

		
					EditorGUI.LabelField( rect, music.analysis.track.tempo + " bpm, " + music.analysis.track.time_signature +"/4" );
					rect.y += rect.height;
					EditorGUI.LabelField( rect, music.analysis.key);
					rect.y += rect.height;
					EditorGUI.LabelField( rect, music.analysis.counts);
					
					
				} else {
					if( music.audioClip == null )
					{
						EditorGUILayout.HelpBox( "Add an audio clip for analysis. Must be an MP3 below 20 MB.", MessageType.Info);
					} else {
						if( !ShowAnalysisProgress( music ) )
						{
							 
							if( GUILayout.Button("Analyze") )
							{
								Analyze(music, soundboard.apiKey);
							}
						}
					}
				}
				
				EditorGUI.indentLevel--;
			}
		
			GUI.color = Color.white;
		}
		
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
				
		


		EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);
		
		if( soundboard.sounds.Count == 0 )
		{
			
			if( GUILayout.Button("Add Sound") )
			{
				soundboard.sounds.Add( new SoundData() );
			}
		}
		
		soundboard.UpdateSolos();
		
		for( int i = 0; i < soundboard.sounds.Count; i++ )
		{
			
		
			SoundData data = soundboard.sounds[i];
			if( data.clips.Count == 0 ) data.clips = new List<AudioClip>( new AudioClip[1] );
			
			EditorGUILayout.BeginHorizontal();
			
				
				bool showClips = ComponentGUILayout.Foldout( (i + 1).ToString() , data );
			
				EditorGUILayout.BeginVertical();
					
					EditorGUILayout.BeginHorizontal();
						ComponentGUILayout.AddRemoveButtons<SoundData>( soundboard.sounds, i );

						data.name = EditorGUILayout.TextField( data.name );
						data.clips[0] = EditorGUILayout.ObjectField( data.clips[0], typeof(AudioClip), false ) as AudioClip;
					EditorGUILayout.EndHorizontal();
					
					
					EditorGUILayout.BeginHorizontal();
						GUI.color = (data.isMuted || data.isSoloMuted) ? Color.red : Color.white;
						if( GUILayout.Button("M" , EditorStyles.miniButtonLeft, GUILayout.Width(20)) )
						{
							data.isMuted = !data.isMuted;
						}

						GUI.color = data.isSoloed ? Color.green : Color.white;
						if( GUILayout.Button("S" , EditorStyles.miniButtonRight, GUILayout.Width(20)) )
						{
							data.isSoloed = !data.isSoloed;
						}

						GUI.color = Color.white;
						
						data.volume = GUILayout.HorizontalSlider( data.volume, 0, 1 );
					EditorGUILayout.EndHorizontal();
					
					
					if( showClips )
					{
		
						EditorGUILayout.BeginHorizontal();			

							EditorGUILayout.BeginVertical();
			
								EditorGUILayout.LabelField("Samples:");
			
								for( int j = 0; j < data.clips.Count; j++ )
								{
									AudioClip clip = data.clips[j];
									EditorGUILayout.BeginHorizontal();
					
										EditorGUILayout.LabelField( (j + 1) + " ", GUILayout.Width(40) );
										data.clips[j] = EditorGUILayout.ObjectField( clip, typeof(AudioClip), false ) as AudioClip;
					
										ComponentGUILayout.AddRemoveButtons<AudioClip>( data.clips, j );
					
									EditorGUILayout.EndHorizontal();
								}
			
							EditorGUILayout.EndVertical();

						EditorGUILayout.EndHorizontal();
			
					}
						
				EditorGUILayout.EndVertical();
				
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			
		}
		
		if( GUI.changed )
		{
			EditorUtility.SetDirty( target );
		}
		
		
	}
	
	
	bool ShowAnalysisProgress( MusicData music )
	{
		if( echoNestRequesters == null ) return false;
		
		EchoNest echoNest = null;
		if( !echoNestRequesters.TryGetValue( music, out echoNest ) ) return false;		

		echoNest.Update();
		
		Rect r = EditorGUILayout.GetControlRect(false);
		switch( echoNest.status )
		{	
			case EchoNestSubmitStatus.Uploading:
				EditorGUI.ProgressBar(r, echoNest.uploadProgress, "Uploading...");	
			break;
		
			case EchoNestSubmitStatus.Pending:
				EditorGUI.ProgressBar(r, 1, "Waiting...");
			break;
		
			case EchoNestSubmitStatus.Analysing:
				EditorGUI.ProgressBar(r, 1, "Analysing...");
			break;
		
			case EchoNestSubmitStatus.Complete:
				music.trackInfo = echoNest.response.track;
				EditorGUI.ProgressBar(r, 1, "Parsing...");
			break;	
		}

		GUILayout.Space(16);
		
		return true;
		
	}
	
	
	
	void Analyze( MusicData music, string api_key )
	{
	
		if( echoNestRequesters == null )
		{
			echoNestRequesters = new Dictionary<MusicData, EchoNest>();
		}
		
		EchoNest echoNest = new EchoNest( api_key );
		
		echoNestRequesters[music] = echoNest;
		
		string path = AssetDatabase.GetAssetPath( music.audioClip );
		echoNest.Track( path, music.analysis );
	}
	
	
	
}

[CustomPropertyDrawer( typeof( MusicReference ) )]
public class MusicReferenceDrawer : UniqueReferenceDrawer<Soundboard, MusicData>{}

[CustomPropertyDrawer( typeof( SoundReference ) )]
public class SoundReferenceDrawer : UniqueReferenceDrawer<Soundboard, SoundData>{}