using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this thing loads the dubstep songs and sets the order to play them in and stuff?

public class MPlayerDubstep : MonoBehaviour {
	
	private List<int> playlist;
	private int playIndex = 0;
	private int section = 0;
	private Soundboard _soundboard;
	private MusicExtended _music;
	private MusicData song ; //whole song has all the pieces

	private bool offBeat = false;

	// Use this for initialization
	void Start () {
		_soundboard = Soundboard.Instance;
		song = _soundboard.music[0];

		Random.seed = (int)System.DateTime.Now.Ticks;
	
		_music = this.GetComponent<MusicExtended>();
		_music.LoadData(song);
		_music.Play();

		playlist = new List<int>();
		playlist.Add (1);
		playlist.Add (1);
		playlist.Add (2);
		playlist.Add (3);
		playlist.Add (4);

		playlist = shuffle(playlist);
		playlist.Insert (0, 1);

		section = 2;//0;
		play ();
	}

	void OnBeat(){
		if (!offBeat){
			Debug.Log ("BEAT");
		GameObject.FindWithTag("MainCamera").GetComponent<ScreenShake>().Shake(.25f,.1f);
		}else{
			Debug.Log ("beat");
			GameObject.FindWithTag("MainCamera").GetComponent<ScreenShake>().Shake(.1f,.05f);
		}

		offBeat = !offBeat;

		if (/*(_music.GetSection()!=section) ||*/ (_music.data.analysis.segments[_music.GetSegment()].loudness_start < -50f) ){
			playIndex += 1;
			if (playIndex >= playlist.Count){
				playlist = shuffle (playlist);
				playIndex = 0;
			}
			Debug.Log ("jumping to track "+playIndex+", section "+(playlist[playIndex]*2));

			section = playlist[playIndex]*2;
			play ();
		}
		
	}
	
	// Update is called once per frame
	/*void Update () {
		if(audio.time >= audio.clip.length)
		{section = -1; OnBeat();}
		}*/

	void play(){ //loads and starts playing the current song
		_music.JumpSection(section);
	}

	List<int> shuffle(List<int> list){ 
		//returns a shuffled version of the given array list
		foreach (int q in list){Debug.Log (q);}
		List<int> dupe = new List<int>();
		int i;
		while(list.Count > 0){
			i = Random.Range(0,list.Count);
			dupe.Add (list[i]);
			list.RemoveAt(i);
		}
		foreach (int q in dupe){Debug.Log (q);}
		return dupe;
	}
}
