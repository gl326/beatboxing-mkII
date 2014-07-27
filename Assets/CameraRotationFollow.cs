using UnityEngine;
using System.Collections;

public class CameraRotationFollow : MonoBehaviour {
	private Transform cam;

	// Use this for initialization
	void Start () {
		cam = GameObject.FindWithTag("MainCamera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.eulerAngles = cam.eulerAngles;
		this.transform.position = cam.position;
		this.transform.Translate(-cam.GetComponent<ScreenShake>().ShakeTrans());
	}
}
