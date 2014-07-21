using UnityEngine;
using System.Collections;

public class AngleSpeed : MonoBehaviour {

	public Vector3 speed = Vector3.zero;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.localEulerAngles += speed*Time.deltaTime;
	}
}
