using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour {

	private float time = 0f;
	private float amount = 0f;
	private Vector3 trans = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}

	public Vector3 ShakeTrans(){
		return trans;
	}

	public void Shake(float a, float t){
		amount = a;
		time  = t;
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
