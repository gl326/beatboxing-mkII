using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Soundboard : Settings<Soundboard> {

	ProvidesEvent OnBeat;

	[Range(0,1)]
    public float volume = 1f;
    
	[UniqueReference( typeof(MusicData) )]
    public List<MusicData> music = new List<MusicData>();
    
    [UniqueReference( typeof(SoundData) )]
    public List<SoundData> sounds = new List<SoundData>();
    
    public string apiKey = "MHDVHZRVPHEW9G7VD";


	Music _currentMusic;
	MusicData _lastData;

	/*void Update(){ //debug
		for(int i = 0; i < music.Count; i++ )
		{
			Debug.Log( music[i].guid) ;
		}
	}*/
	
	
	MusicData GetMusicById( string id )
	{

		for(int i = 0; i < music.Count; i++ )
		{
		 	if( music[i].guid == id ) return music[i];
		}
		
		return null;
	}
	
	public static MusicData GetMusicData( MusicReference reference )
	{
		return Instance.GetMusicById( reference.id );
	}
	
	public static Music PlayMusic( MusicReference reference )
	{
		return PlayMusic( Instance.GetMusicById( reference.id ) );
	}
	
	public static Music PlayMusic( int i )
	{
		return PlayMusic( Instance.music[i] );
	}
	
	public static Music PlayMusic( MusicData data )
	{
		if( data == Instance._lastData ) return Instance._currentMusic;
	
		if( Instance._currentMusic == null)
		{
			GameObject go = new GameObject( "Music" );
			DontDestroyOnLoad( go );
			go.transform.parent = Instance.transform;
			
			Instance._currentMusic = go.AddComponent<Music>();
		}
		
		if( Instance._currentMusic.data != data )
		{
			Instance._currentMusic.LoadData( data );
			Instance._currentMusic.Play();
		}
		
		Instance._lastData = data;

		return Instance._currentMusic;
	}
    
    public void Beat( int beat )
	{
		FireEvent( OnBeat, beat );
	}
	
	
	
	
	
	
	
	
	
	SoundData GetSoundById( string id )
	{

		for(int i = 0; i < sounds.Count; i++ )
		{
		 	if( sounds[i].guid == id ) return sounds[i];
		}
		
		return null;
	}
	
	public static Sound PlaySound( SoundReference reference, float volume = 1f )
	{
		return PlaySound( Instance.GetSoundById( reference.id ).name, volume );
	}
	
	
	public static Sound PlaySound( SoundReference reference, Vector3 position, float volume = 1f )
	{
		return PlaySound( Instance.GetSoundById( reference.id ).name, position, volume );
	}
	
    
    public static Sound PlaySound(string name, float volume = 1f)
    {
        return PlaySound( name, Vector3.zero, volume );
    }
    
    public static Sound PlaySound(string name, Vector3 position, float volume = 1f)
    {
    	 
    	SoundData data = Instance.sounds.Where( d => d.name == name ).First();
    	if( data != null && data.shouldPlay )
    	{
    		Sound a = Sound.PlayClipAtPoint( data.GetClip(), position, volume * data.volume * Instance.volume);
            a.transform.parent = Instance.transform;
            return a;
    	} else {
    		return null;
    	}
    }
    
    public void UpdateSolos()
    {
    	bool hasSolo = false;
    	foreach( SoundData data in sounds )
    	{
    		if( data.isSoloed )
    		{
    			hasSolo = true;
    			break;
    		}
    	}
    	
    	if( hasSolo )
    	{
			foreach( SoundData data in sounds )
			{
				if( !data.isSoloMuted && !data.isSoloed )
				{
					data.isSoloMuted = true;
				} else if( data.isSoloMuted && data.isSoloed ) {
					data.isSoloMuted = false;
				}
			}
		} else {
			foreach( SoundData data in sounds )
			{
				data.isSoloMuted = false;
			}
		}
    }
    
    
    public Music currentMusic { get { return _currentMusic; } }
   
}
