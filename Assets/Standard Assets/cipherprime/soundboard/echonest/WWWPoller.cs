using UnityEngine;
using System.Collections;

public class WWWPoller {

	WWW webRequest;
	IParseable destination;
	float t;
	float timeout = 2;
	
	public WWWPoller( WWW webRequest, IParseable destination )
	{
		this.webRequest = webRequest;
		this.destination = destination;
	}
	
	public bool Poll()
	{
		if( Time.time - t > timeout ) return false;
	
		if( webRequest == null ) return false;
		if( webRequest.error != null) return false;
		
		if( webRequest.isDone )
		{
			destination.Parse( webRequest.text );
			return true;
		}
		
		t = Time.time;
		
		return false;
	}
	
}
