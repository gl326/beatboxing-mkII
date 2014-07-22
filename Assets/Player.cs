using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public float beatAllowance = .25f;

	public int staminaMax = 8;
	[HideInInspector] public int stamina;
	public int hpMax = 8;
	[HideInInspector] public int hp;

	public bool attackingShort=false;
	public bool attackingLong=false;
	public bool blockingShort=false;
	public bool blockingLong=false;
	private bool triedAttack = false;

	public BeatAnimation[] animationList;
	private string anim="idle";
	private Animator animation;
	//private AnimationState animator;

	private enum inputs{rest,left,up,right,down}
	private inputs input;

	public bool bigAttack = false;
	private bool bigBlock = false;
	public int healMeasures = 4;
	private int passedMeasures = 0;

	private float oldTime;
	private float animationSpeedOffset = 1f;

	public bool left = false;

	public ParticleSystem rightParticles;
	public ParticleSystem leftParticles;

	private Metronome _metronome;

	BeatBoxer _boxer;

	public Transform apollo;
	private Vector3 origin;
	public float hurtShake = 1f;
	private int hurtBeat = -99;

	// Use this for initialization
	void Start () {
		_boxer = GameObject.FindWithTag("Beatboxer").GetComponent<BeatBoxer>();
		_metronome = GameObject.FindWithTag("Metronome").GetComponent<Metronome>();
		hp = hpMax;
		stamina = staminaMax;
		animation = GetComponentInChildren<Animator>();
		oldTime = Time.time;

		rightParticles.enableEmission = false;
		leftParticles.enableEmission = false;
	
		origin = apollo.localPosition;
		//animator = this.GetComponent<Animator>();
	}

	void OnMeasure(){
		//stamina = Mathf.Min (staminaMax, stamina+1);
		passedMeasures += 1;
		if (passedMeasures >= healMeasures){hp = Mathf.Min (hpMax, hp+1); passedMeasures = 0;}
	}

	public void LoseStamina(){
		stamina = Mathf.Max (0,stamina-1);
	}

	void OnBeat(){
		rightParticles.enableEmission = false;
		leftParticles.enableEmission = false;
		triedAttack = false;
		string oldAnim = anim;

		if (bigAttack){bigAttack = false; attackingLong = true;}
		else{
		if (bigBlock){bigBlock = false;}
		else{
		switch(input){
		case inputs.rest: 
			attackingShort=false;
			attackingLong=false;
			blockingShort=false;
			blockingLong=false;
			stamina = Mathf.Min (staminaMax,stamina+1); 
			anim = "idle";
					_metronome.HideInput();
			break;
		case inputs.right: 
			attackingShort=true;
			attackingLong=false;
			blockingShort=false;
			blockingLong=false; 
					left = !left;
					if (left){anim = "punch1"; rightParticles.enableEmission = true;}
					else{anim = "punch2"; leftParticles.enableEmission = true;}
			break;
		case inputs.left: 
			attackingShort=false;
			attackingLong=false;
			blockingShort=true;
			blockingLong=false;
					anim = "block";
			break;
		case inputs.up: 
			attackingShort=false;
			attackingLong=false;
			blockingShort=false;
			blockingLong=false;
			bigAttack = true;
					LoseStamina();
					anim = "uppercut";
			break;
		case inputs.down: 
			attackingShort=false;
			attackingLong=false;
			blockingShort=false;
			blockingLong=true;
			//bigBlock = true; //makes big block 2 beats
					anim = "duck";
			break;
		}
		}
		}



		if (!attackingLong){animation.SetTrigger(anim);}
		oldTime = Time.time;

		input = inputs.rest;
	}

	void Hurt(){
		hp -= 1;
		bigAttack = false;
		attackingLong = false;
		attackingShort = false;
		blockingShort = false;
		blockingLong = false;
		LoseStamina();
		hurtBeat = Mathf.FloorToInt(_boxer.Beat());
	}

	public bool didInput(){
		return triedAttack;
	}

	int animIndex(string a, int def = 0){
		for (int i=0;i<animationList.Length;i+=1){
			if (animationList[i].name == a){
				return i;
				break;
			}
		}
		return def;
	}

	void Update(){
		float timeToNext = (60f/_boxer.BPM());
		float animLength = animation.GetCurrentAnimatorStateInfo(0).length/(float)animationList[animIndex(anim)].beats;
		animation.speed = (animLength/timeToNext)*animationSpeedOffset;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float beat = _boxer.Beat()%1f;
		if (Mathf.FloorToInt(_boxer.Beat())==hurtBeat){
			float shake = hurtShake*(1f - Mathf.Pow (beat,2f));
			apollo.localPosition = origin + 
				new Vector3(-shake+Random.Range(0f,2f*shake),-shake+Random.Range(0f,2f*shake),-shake+Random.Range(0f,2f*shake));
		}else{apollo.localPosition = origin;}

		if ((Input.GetButtonDown("Uppercut") || Input.GetButtonDown("Punch") 
		     || Input.GetButtonDown("Block") || Input.GetButtonDown("Dodge"))
		    && input==inputs.rest && stamina > 0 && !bigAttack && !bigBlock && !triedAttack){ //input
			triedAttack = true;
		if ((beat >= 1f-beatAllowance || beat<=_boxer.beatDelay) ){ //it was on time
			if (Input.GetButtonDown("Uppercut")){input = inputs.up;}
			if (Input.GetButtonDown("Punch")){input = inputs.right;}
			if (Input.GetButtonDown("Block")){input = inputs.left;}
			if (Input.GetButtonDown("Dodge")){input = inputs.down;}
		}
			_metronome.UpdateInput((input!=inputs.rest));
		}
	}
}

[System.Serializable]
public class BeatAnimation //animations with some beat info
{ 
	public string name;
	public int beats;
	
	public BeatAnimation(string name, int beats)
	{
		this.name = name;
		this.beats = beats;
	}
	
}
