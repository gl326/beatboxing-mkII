using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class EchoNest
{
	public EchoNestSubmitStatus status = EchoNestSubmitStatus.Idle;
	public EchoNestResponse response;
	
	public string api_key;


	WWW webRequest;
	WWWPoller poller;
	EchoNestAnalysis analysis;
	
	public EchoNest(){}
	
	public EchoNest( string api_key )
	{
		this.api_key = api_key;
	}
	
	public void Track( string filepath, EchoNestAnalysis analysis )
	{
		response = new EchoNestResponse();
		this.analysis = analysis;
		
		string url = "http://developer.echonest.com/api/v4/track/upload";
		
		//Get file name
		string[] file = filepath.Split('/');
		string name = file[ file.Length - 1 ];
		
		//Get Byte Data
		FileStream fs = new FileStream( filepath, FileMode.Open, FileAccess.Read );
		byte[] data = new byte[fs.Length];
		fs.Read(data, 0, data.Length);
		fs.Close();
		
		//Upload
		WWWForm form = new WWWForm();
		form.AddField("api_key", api_key);
		form.AddField("filetype", "mp3");
		form.AddBinaryData( "track", data, name, "audio/mpeg" );
		
		webRequest = new WWW( url, form );
		status = EchoNestSubmitStatus.Uploading;
		
		poller = new WWWPoller( webRequest, response );
	}
	
	
	public void SetupAnalysingPolling()
	{
		string url = "http://developer.echonest.com/api/v4/track/profile?api_key={0}&format={1}&id={2}&bucket={3}";
		url = System.String.Format( url, api_key, "json", response.track.id, "audio_summary");

		webRequest = new WWW(url);
		poller = new WWWPoller( webRequest, response );
	}
	
	public void SetupCompletePolling()
	{
		webRequest = new WWW( response.track.summary.analysis_url );
		poller = new WWWPoller( webRequest, analysis );
	}
	
	public void Update()
	{
		if( poller == null ) status = EchoNestSubmitStatus.Idle;
		
		switch( status )
		{
			case EchoNestSubmitStatus.Idle:
			break;
			
			case EchoNestSubmitStatus.Uploading:
				if( webRequest != null && webRequest.uploadProgress >= 1 )
				{
					status = EchoNestSubmitStatus.Pending;
				}
			break;
			
			case EchoNestSubmitStatus.Pending:
				if( poller.Poll() )
				{
					SetupAnalysingPolling();
					status = EchoNestSubmitStatus.Analysing;
				}
			break;
			
			case EchoNestSubmitStatus.Analysing:
				if( poller.Poll() )
				{
					if( response.track.status == EchoNestAnalysisStatus.Pending )
					{
						SetupAnalysingPolling();
					} else if ( response.track.status == EchoNestAnalysisStatus.Complete ) {
						SetupCompletePolling();
						status = EchoNestSubmitStatus.Complete;
					}
				}
			break;
			
			case EchoNestSubmitStatus.Complete:
				if( poller.Poll() )
				{
					status = EchoNestSubmitStatus.Idle;
					poller = null;
					
				}
			break;

		}
	
	}

	public float uploadProgress
	{
		get
		{
			if( webRequest != null )
			{
				return webRequest.uploadProgress;
			} else {
				return 0;
			}
		}
	}
	
}



public enum EchoNestSubmitStatus
{
	Idle,
	Uploading,
	Pending,
	Analysing,
	Complete
}