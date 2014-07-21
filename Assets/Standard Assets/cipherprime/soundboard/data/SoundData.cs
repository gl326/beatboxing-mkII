using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SoundData : UniqueObject
{
    public string name = "New Sound";
    
    [Range(0,1)]
    public float volume = 1;
    
    [Range(-1,1)]
    public float pan;
    
    [Range(0, 256)]
    public int priority = 128;
    
    public bool isMuted;
    public bool isSoloed;
    public bool isSoloMuted;
    
	public List<AudioClip> clips = new List<AudioClip>();
    
    public bool shouldPlay
    {
    	get
    	{
    		return (!isMuted && !isSoloMuted);
    	}
    }
    
    public AudioClip GetClip()
    {
    	if( clips.Count == 1 )
    	{
    		return clip;
    	} else {
    		return clips[ (int)Random.Range(0, clips.Count) ];
    	}
    }
    
    public AudioClip clip
    {
    	get
    	{
    		if( clips.Count >= 1 )
    		{
    			return clips[0];
    		} else {
    			return null;
    		}
    	}
    	
    	set
    	{
    		if( clips == null || clips.Count == 0 )
    		{
    			clips = new List<AudioClip>();
    			clips.Add( value );
    		} else {
    			clips[0] = value;
    		}
    	}
    }
    
    public override string ToString()
    {
    	return name;
    }

}




[System.Serializable]
public class SoundReference : UniqueReference<SoundData>{}