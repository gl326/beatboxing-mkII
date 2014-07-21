using UnityEngine;
using System.Collections;

public class fist_position : MonoBehaviour {
	public Transform followTransform;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.localPosition = followTransform.localPosition;
	}
}
