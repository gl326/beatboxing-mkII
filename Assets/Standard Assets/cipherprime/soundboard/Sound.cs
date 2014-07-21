using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Sound : Audio {

	bool _clipStarted = false;
	bool _isDelayed = false;
	
	bool _isMuted = false;
//	bool _isSoloed = false;

	public static Sound PlayClipAtPoint( AudioClip clip, Vector3 position, float volume = 1f )
	{
		GameObject go = new GameObject("Sound " + clip.name);
		go.transform.position = position;
		
		Sound sound = go.AddComponent<Sound>();
		sound.PlayClip(clip, volume);
		
		return sound;
	}

	void Start()
	{
		DontDestroyOnLoad( gameObject );
	}

	void Update()
	{
		if( audio.isPlaying && _isDelayed) _isDelayed = false;
		if( _clipStarted && !(_isDelayed || audio.isPlaying) )
		{
			Destroy( gameObject );
		}
	}
	
	public bool isMuted
	{
		get
		{
			return _isMuted;
		}
	}
	

	public Sound PlayClip( AudioClip clip, float volume = 1f )
	{
		audio.clip = clip;
		audio.volume = volume;
		audio.Play();
		_clipStarted = true;
		return this;
	}
	
}
