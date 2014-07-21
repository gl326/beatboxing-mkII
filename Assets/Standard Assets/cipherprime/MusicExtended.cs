using UnityEngine;
using System.Collections;


public class MusicExtended : Music {
	private float sectionOffset = (60f/145.12f)*1.5f;//.75f;

	public int GetSection(){
		int section = 0;
		while(audio.time+sectionOffset >= data.analysis.sections[section+1].start){
			section += 1;
			if (section>=data.analysis.sections.Length-1){break;}
		}
		//Debug.Log ("song at "+audio.time+", in section "+section+" which starts at "+data.analysis.sections[section].start);
		return section;
	}

	public int GetSegment(){
		int section = 0;
		while(audio.time+sectionOffset >= data.analysis.segments[section+1].start){
			section += 1;
			if (section>=data.analysis.segments.Length-1){break;}
		}
		//Debug.Log ("song at "+audio.time+", in section "+section+" which starts at "+data.analysis.sections[section].start);
		return section;
	}
	
	public void JumpSection(int section){
		audio.time = data.analysis.sections[section].start;
	}

	override public void Beat()
	{
		/*if( _soundboard != null ){ 
			_soundboard.Beat( beat );
		}*/

		gameObject.SendMessage("OnBeat");
	}
}
