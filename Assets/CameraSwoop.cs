using UnityEngine;
using System.Collections;

public class CameraSwoop : MonoBehaviour {
	public Transform swoopLeft;
	public Transform swoopRight;

	private Transform target = null;

	// Use this for initialization
	void Start () {
	
	}

	public void SwoopLeft(){
		target = swoopLeft;
	}
	public void SwoopRight(){
		target = swoopRight;
	}
	public void UnSwoop(){
		target = null;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 ta = Vector3.zero;

		if (target==null){
			this.transform.localPosition = Vector3.Lerp (this.transform.localPosition,Vector3.zero,.2f);;
		}else{
			ta = target.localEulerAngles;
			this.transform.localPosition = Vector3.Lerp (this.transform.localPosition,target.localPosition,.3f);
		}
		while(Mathf.Abs(ta.y - this.transform.localEulerAngles.y)>180){ta.y -= 360f;}
		while(Mathf.Abs(ta.x - this.transform.localEulerAngles.x)>180){ta.x -= 360f;}
		while(Mathf.Abs(ta.z - this.transform.localEulerAngles.z)>180){ta.z -= 360f;}
		this.transform.localEulerAngles = Vector3.Slerp(this.transform.localEulerAngles,ta,.3f);
	}
}
