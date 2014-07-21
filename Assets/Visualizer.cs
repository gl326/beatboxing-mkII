using UnityEngine;
using System.Collections;

public class Visualizer: MonoBehaviour {
	private BeatBoxer _boxer;
	private float min = 9999999f;
	private float max = -99999999f;
	private float scale = 16f;
	private float sections = 2f;
	public float[] spectrum_avg = new float[2];

	private int samples = 1024;

	void Start(){
		_boxer = GameObject.FindWithTag("Beatboxer").GetComponent<BeatBoxer>();
	}

	void Update() {
		float[] spectrum = _boxer.audio.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
		int i=0;
		float stotal = 0f;
		while(i<samples){
			stotal+=Mathf.Log(spectrum[i]);
			if ((i>0) && (Mathf.FloorToInt((float)i/(samples/sections))!=Mathf.FloorToInt(((float)i-1)/(samples/sections)))){
				spectrum_avg[Mathf.FloorToInt(((float)i-1)/(samples/sections))]=stotal/(samples/sections);
				stotal = 0f;
			}
			i++;
		}
		for(i=0;i<Mathf.FloorToInt(sections);i++){
				Debug.DrawLine(new Vector3((float)i/scale,0f,0f),new Vector3((float)i/scale,-spectrum_avg[i],0f),Color.cyan);
			}
		//if (-spectrum_avg[0]<min){min = -spectrum_avg[0]; Debug.Log ("new min: "+min);}
		//if (-spectrum_avg[0]>max && -spectrum_avg[0]<Mathf.Infinity ){max = -spectrum_avg[0]; Debug.Log ("new max: "+max);}
		/*int i = 1;
		while (i < samples-1) {
			Debug.DrawLine(new Vector3((i - 1)/scale, spectrum[i] + 10, 0), new Vector3((i)/scale, spectrum[i + 1] + 10, 0), Color.red);
			Debug.DrawLine(new Vector3((i - 1)/scale, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3((i)/scale, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
			Debug.DrawLine(new Vector3((i - 1)/scale, spectrum[i - 1] - 10, 1), new Vector3((i)/scale, spectrum[i] - 10, 1), Color.green);
			Debug.DrawLine(new Vector3((i - 1)/scale, Mathf.Log(spectrum[i - 1]), 3), new Vector3((i)/scale, Mathf.Log(spectrum[i]), 3), Color.yellow);
			i++;
		}*/
	}
}