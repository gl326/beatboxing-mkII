using UnityEngine;
using System.Collections;

public class CameraSwoop : MonoBehaviour {
	public Transform swoopLeft;
	public Transform swoopRight;
	public Transform swoopHit;

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
	public void SwoopHit(){
		target = swoopHit;
	}
	public void UnSwoop(){
		target = null;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 ta = Vector3.zero;
		if (target!=null){ta = target.localEulerAngles;}
		//normalize rotation
		while((ta.y - this.transform.localEulerAngles.y)>180){ta.y -= 360f;}
		while((ta.x - this.transform.localEulerAngles.x)>180){ta.x -= 360f;}
		while((ta.z - this.transform.localEulerAngles.z)>180){ta.z -= 360f;}
		while((ta.y - this.transform.localEulerAngles.y)<-180){ta.y += 360f;}
		while((ta.x - this.transform.localEulerAngles.x)<-180){ta.x += 360f;}
		while((ta.z - this.transform.localEulerAngles.z)<-180){ta.z += 360f;}

		if (target==null){
			this.transform.localPosition = Vector3.Lerp (this.transform.localPosition,Vector3.zero,.1f);
			this.transform.localEulerAngles = Vector3.Slerp(this.transform.localEulerAngles,ta,.2f);
		}else{
			this.transform.localPosition = Vector3.Lerp (this.transform.localPosition,target.localPosition,.1f);
			this.transform.localEulerAngles = Vector3.Slerp(this.transform.localEulerAngles,ta,.05f);
		}

		//this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x,this.transform.eulerAngles.y,0f);
	}
}
