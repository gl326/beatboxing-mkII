using UnityEngine;
using System.Collections;

public class Metronome : MonoBehaviour {
	private BeatBoxer _boxer;
	private Transform bar;
	private Transform ball;
	private Transform debug;
	private Material ringMat;
	private float barLength;
	private Color debugColor;

	// Use this for initialization
	void Start () {
		_boxer = GameObject.FindWithTag("Beatboxer").GetComponent<BeatBoxer>();
		ball = this.transform.GetChild(1);
		bar = this.transform.GetChild (0);
		debug = this.transform.GetChild (2);
		debugColor = debug.GetComponent<MeshRenderer>().material.color;

		barLength = bar.localScale.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float cos = Mathf.Sin (Mathf.PI*_boxer.Beat());
		ball.localPosition = new Vector3(barLength*Mathf.Pow (Mathf.Abs (cos), 2f/3f)*Mathf.Sign(cos), ball.localPosition.y, ball.localPosition.z);

		cos = Mathf.Max (0,Mathf.Tan ((Mathf.PI*_boxer.Beat())+(Mathf.PI/2))/10f);

		//ring.localScale = new Vector3(10f*Mathf.Abs(cos),.1f,10f*Mathf.Abs(cos));
		//ringMat.color = new Color(ringMat.color.r,ringMat.color.g,ringMat.color.b,(1 - Mathf.Pow (Mathf.Abs(cos),3)));
	}

	public void UpdateInput(bool didit){
		debug.position = ball.position;
		debug.GetComponent<MeshRenderer>().enabled = true;
		debug.GetComponent<ParticleSystem>().Clear();
		if (didit){
		debug.GetComponent<MeshRenderer>().material.color = debugColor;
		debug.GetComponent<ParticleSystem>().Play();
		debug.GetComponent<ParticleSystem>().time = 0f;
		debug.GetComponent<ParticleSystem>().playbackSpeed = _boxer.BPM ()/60f;
		debug.GetComponent<ParticleSystem>().startSize = 1f+(6f*Mathf.Pow (Mathf.Cos (Mathf.PI*_boxer.Beat()),4f));
		}else{
		debug.GetComponent<ParticleSystem>().Stop();
			debug.GetComponent<MeshRenderer>().material.color = new Color(debugColor.r*0.5f,debugColor.g*0.5f,debugColor.b*0.5f);
		}
	}
	public void HideInput(){
		debug.GetComponent<MeshRenderer>().enabled = false;
		debug.GetComponent<ParticleSystem>().Stop();
	}
}
