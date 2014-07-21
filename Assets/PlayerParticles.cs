using UnityEngine;
using System.Collections;

public class PlayerParticles : MonoBehaviour {
	public Transform hitFX;
	public Transform dodgeFX;
	public Transform shieldFX;
	public Transform swipeFX;

	private Vector3 swipeOrigin = Vector3.zero;
	private Vector3 swipeEnd;
	private Vector3 swipeStart;
	public float swipeDist = 2f;
	private int swipeBeat = 0;

	private BeatBoxer _boxer;




	// Use this for initialization
	void Start () {
		swipeOrigin = swipeFX.localPosition;
		swipeEnd = swipeOrigin;
		swipeStart = swipeEnd;

		swipeFX.particleSystem.enableEmission = false;
		hitFX.particleSystem.enableEmission = false;
		shieldFX.particleSystem.enableEmission = false;
		dodgeFX.particleSystem.enableEmission = false;

		_boxer = GameObject.FindWithTag("Beatboxer").GetComponent<BeatBoxer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.FloorToInt(_boxer.Beat())!=swipeBeat){
			swipeFX.particleSystem.enableEmission = false;
		}else{
			//Debug.Log("swiping @ "+(Mathf.Pow ((_boxer.Beat()%1f),1f/2f)*100f)+"%, emission is "+swipeFX.particleSystem.enableEmission);
			Vector3 swipeDir = Vector3.Normalize(swipeEnd-swipeStart);
			swipeFX.localPosition = Vector3.Lerp (swipeStart,swipeEnd,Mathf.Pow ((_boxer.Beat()%1f)/.75f,1f/3f))  
				/*+Vector3.Lerp (swipeFX.localPosition,
				              swipeFX.localPosition + (Quaternion.AngleAxis(90, swipeDir) * swipeDir*(swipeDist/3f)),
				              Mathf.Pow (1f - (Mathf.Abs ((_boxer.Beat()%1f)-.5f)/.5f),1f/2f))*/
					;
			swipeFX.particleSystem.enableEmission = true;
		}

		Color oldColor = shieldFX.GetComponentInChildren<MeshRenderer>().material.GetColor("_TintColor");
		shieldFX.GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", 
		                            new Color(oldColor.r,oldColor.g,oldColor.b,(oldColor.a)-(0.5f*60f/_boxer.BPM()*Time.deltaTime*2f)));
	}

	public void Swipe(Vector3 direction){
		Debug.Log ("swipe! "+direction);

		swipeFX.localPosition = swipeOrigin - (direction*swipeDist*1.5f);
		swipeStart = swipeFX.localPosition;
		Vector3 victor = new Vector3(-.5f+Random.Range(0f,1f),-.5f+Random.Range(0f,1f),-.5f+Random.Range(0f,1f));
		swipeEnd = swipeOrigin + (direction*swipeDist) + victor;

		swipeBeat = Mathf.FloorToInt(_boxer.Beat());
		swipeFX.particleSystem.Clear();
		swipeFX.particleSystem.Play();
		swipeFX.particleSystem.enableEmission = true;
		swipeFX.particleSystem.startDelay = 60f/_boxer.BPM()*.05f;
	}

	public void Hit(){
		hitFX.particleSystem.Emit(20);
	}
	public void Dodge(){
		dodgeFX.particleSystem.Emit(30);
	}
	public void Shield(){
		shieldFX.particleSystem.Emit(20);
		Color oldColor = shieldFX.GetComponentInChildren<MeshRenderer>().material.GetColor("_TintColor");
		shieldFX.GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", new Color(oldColor.r,oldColor.g,oldColor.b,.5f));
		Debug.Log (shieldFX.GetComponentInChildren<MeshRenderer>().material.GetColor("_TintColor"));
	}
}
