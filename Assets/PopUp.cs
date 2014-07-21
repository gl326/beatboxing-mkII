using UnityEngine;
using System.Collections;

public class PopUp : MonoBehaviour {
	public Texture[] texList;
	public float life = .5f;
	public bool half = false;
	public int tex = 0;
	public float beat_target=0f;
	public bool player=false;
	private float beat_dist = 2f;
	private BeatBoxer _boxer;

	// Use this for initialization
	public void Start () {
		//if (beat_target>0f){Debug.Log ("queued a popup for beat "+beat_target);}
		//else{Debug.Log ("made new popup");}
		_boxer = GameObject.FindWithTag("Beatboxer").GetComponent<BeatBoxer>();
		//this.transform.localScale = Vector3.zero;
		GetComponent<MeshRenderer>().material.mainTexture = texList[tex];
		beat_dist = 2f;
	}
	
	// Update is called once per frame
	void Update () {
		if (_boxer.Beat()>=beat_target+1f){this.gameObject.SetActive(false);}
		else{
		float beats = _boxer.Beat();
		float dir = 1f;
		if (player){dir = -1f;}
		float sc = 1f;
			if (beats>=beat_target){sc = 2.5f*Mathf.Pow (1f - (beats - beat_target),1/2f); beat_dist = 0f;}

		this.transform.localScale = new Vector3(sc,sc,sc);


		this.transform.position = new Vector3(
			(1.5f*dir)+(dir*beat_dist*(Mathf.Ceil(Mathf.Max(0,beat_target-beats))-Mathf.Pow (Mathf.Max(0,(beats))%1f,2f))),
			this.transform.position.y,this.transform.position.z);

		/*
		if (!half){
			this.transform.localScale += ((1.5f*Vector3.one) - this.transform.localScale)*.2f;
			Color c = GetComponent<MeshRenderer>().material.color;
			GetComponent<MeshRenderer>().material.color = new Color(c.r,c.g,c.b,1f);
		}else{
			this.transform.localScale += ((.75f*Vector3.one) - this.transform.localScale)*.2f;
			Color c = GetComponent<MeshRenderer>().material.color;
			GetComponent<MeshRenderer>().material.color = new Color(c.r,c.g,c.b,0.5f);
		}
		if (life < 0f){
			this.gameObject.SetActive(false);
		}*/
		}
	}
}
