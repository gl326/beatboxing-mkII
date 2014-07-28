using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour {

	private float time = 0f;
	private float amount = 0f;
	private Vector3 trans = Vector3.zero;
	private ParticleSystem hurtFX;

	// Use this for initialization
	void Start () {
		hurtFX = this.GetComponentInChildren<ParticleSystem>();
		hurtFX.enableEmission = false;
	}

	public Vector3 ShakeTrans(){
		return trans;
	}

	public void Shake(float a, float t){
		amount = a;
		time  = t;
	}

	public void Hurt(){
		hurtFX.Emit (100);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(-trans);
		trans = Vector3.zero;

		if (time > 0f){time -= Time.deltaTime;}
		else{
			amount += (0f - amount)*.3f;
		}

		trans = new Vector3(Random.Range (-amount,amount),Random.Range (-amount,amount),0);
		transform.Translate(trans);
	}
}
