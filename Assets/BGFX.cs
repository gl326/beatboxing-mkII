using UnityEngine;
using System.Collections;

public class BGFX : MonoBehaviour {
	private BeatBoxer _boxer;
	private LineWave beatLine;
	private Transform beatLineTrans;
	private ParticleSystem particle;
	//private Visualizer soundInfo;

	public float maxAmp = 10f;
	public float minFreq = 2f;
	public float maxFreq = 4f;

	public Color minColor;
	public Color maxColor;
	public float minParticleSpeed = 0f;
	public float maxParticleSpeed = 10f;
	public float minParticleSize = 4f;
	public float maxParticleSize = 10f;

	public float soundMin = -18f;
	public float soundMax = -4f;

	public Transform line;
	public Transform particles;
	public Transform lights;
	public BGGeom geom;

	private float ratio;

	// Use this for initialization
	void Start () {
		_boxer = GameObject.FindWithTag("Beatboxer").GetComponent<BeatBoxer>();
		beatLine = GetComponentInChildren<LineWave>();
		particle = GetComponentInChildren<ParticleSystem>();
		beatLineTrans = this.transform.GetChild (0);
	}
	
	// Update is called once per frame
	void Update () {
		beatLine.walkManual = -(0f*_boxer.Beat());//-Mathf.Pow(2f*((_boxer.Beat()+.5f)%1f),2f);
		beatLine.amp += ((maxAmp * (1f-Mathf.Pow(_boxer.Beat()%1f,1f/2f))*Mathf.Sign (Mathf.Sin (_boxer.Beat()*Mathf.PI)))-beatLine.amp)*0.4f;
		beatLineTrans.eulerAngles = new Vector3(0f,50f*Mathf.Sin (_boxer.Beat()*Mathf.PI),0f);
			//Mathf.Max (1f,Mathf.Min (maxAmp,Mathf.Tan(-(Mathf.PI*_boxer.Beat())-(Mathf.PI/2f))));
			//Mathf.Pow (Mathf.Abs (Mathf.Sin (Mathf.PI*_boxer.Beat())), 2f)*maxAmp;))
		//beatLine.freq = minFreq + ((maxFreq-minFreq)*Mathf.Pow (Mathf.Abs (Mathf.Sin (Mathf.PI*_boxer.Beat())), 1f/2f));

		int samples = 512;
		float[] spectrum = new float[samples];
		spectrum = _boxer.audio.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
		int i=0;
		float stotal = 0f;
		while(i<samples){
//			Debug.Log(Mathf.Log(spectrum[i]));
			if (Mathf.Log(spectrum[i])!=-Mathf.Infinity){stotal+=Mathf.Log(spectrum[i]);}
			i++;
		}

		//Debug.Log(stotal+","+stotal/(float)samples);

		ratio = (Mathf.Max (soundMin,Mathf.Min (soundMax,(stotal/(float)samples)))-soundMin)/(soundMax-soundMin);
		particle.startColor = Color.Lerp (minColor,maxColor,Mathf.Pow (ratio,3f));
		particle.startSpeed = Mathf.Lerp (minParticleSpeed,maxParticleSpeed,Mathf.Pow (ratio,3f))*(1f+(9f*Mathf.Pow(1f - (_boxer.Beat()%1f),8f)));
		particle.startSize = Mathf.Lerp (minParticleSize,maxParticleSize,ratio);
	}

	public void OnBeat(){
		if (geom.enabled){geom.OnBeat();}
	}

	public float Volume(){
		return ratio;
	}
}
