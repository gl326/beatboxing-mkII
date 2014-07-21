using UnityEngine;
using System.Collections.Generic;

public class BGGeom : MonoBehaviour {
	public GameObject geoPrefab;
	public int geoNum = 15;
	public float minScale = 1f;
	public float maxScale = 5f;
	public float beatScale = 0.25f;
	public float beatForce = 1f;

	/*public float width = 40f;
	public float height = 20f;
	public float depth = 20f;

	public Transform top;
	public Transform bottom;
	public Transform left;
	public Transform right;
	public Transform front;
	public Transform back;*/

	private List<Transform> boxes;
	private BGFX fx;
	private BeatBoxer _boxer;

	// Use this for initialization
	void Start () {
		boxes = new List<Transform>();
		Transform box;
		for(int i=0;i<geoNum;i+=1){
			box = (Instantiate(geoPrefab) as GameObject).transform;
			boxes.Add (box);
			box.parent = this.transform;
			box.localEulerAngles = new Vector3(Random.Range(0f,360f),Random.Range(0f,360f),Random.Range(0f,360f));
			box.localPosition = new Vector3(Random.Range(-15f,15f),Random.Range(-4f,4f),Random.Range(-4f,4f));
			box.rigidbody.AddForce(new Vector3(Random.Range(-5f,5f),Random.Range(-5f,5f),Random.Range(-5f,5f)),ForceMode.Impulse);
		}

		fx = this.transform.parent.GetComponent<BGFX>();
		_boxer = GameObject.FindWithTag("Beatboxer").GetComponent<BeatBoxer>();
	}
	
	// Update is called once per frame
	void Update () {
		float scale = Mathf.Lerp (minScale,maxScale,fx.Volume())
				*	(1f + (beatScale*Mathf.Pow (Mathf.Abs((_boxer.Beat()%1f)-0.5f)/.5f,4f)));
		for(int i=0;i<geoNum;i+=1){
			boxes[i].localScale = new Vector3(scale,scale,scale);
		}
	}

	public void OnBeat(){
		//Debug.Log("cube beat!");
		for(int i=0;i<geoNum;i+=1){
			float dir = 1f;
			if (Mathf.FloorToInt(_boxer.Beat ()%2)==1){dir = -1f;}
			boxes[i].rigidbody.AddForce(
				Vector3.Normalize(boxes[i].localPosition*dir)*beatForce*fx.Volume(),ForceMode.Impulse);
		}
	}
}
