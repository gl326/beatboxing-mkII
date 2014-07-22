using UnityEngine;

/// <summary>
/// Beat boxer.
/// Carries the list of possible music clips
/// determines what music is being played, and informs everything else of the game timing.
/// </summary>

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class BeatBoxer : MonoBehaviour {

	public int hpMax = 36;
	public int hp = 0;
	public bool doubleTime = false;
	public List<Pattern> soundSet; //list of moves. 0 is *always* the intro
	public List<Attack> attackSet;
	public BeatAnimation[] animationList;

	private string anim = "idle";

	private List<Pattern> playlist;
	private Pattern song;
	private int songIndex = 0;
	private float timePassed = 0f;

	public float measureLength = 4f;

	private List<Attack> attackStream; //stream of attacks to be used
	private int currentFrame = 0;
	private Attack currentAttack = null;
	private AttackFrame frameData;
	public float beatDelay = .25f;//.08f;
	private float beatTimer = 0f;

	private PlayerParticles _playerParts;
	private PlayerParticles _enemyParts;
	private BGFX _bg;

	public GameObject popup;
	public GameObject boxerModel;

	private Player _player;
	private Transform _enemy;
	private Transform _enemyModel;
	private Animator animation;
	private List<GameObject> popUps;
	private Camera _mainCamera;
	//private int QueuedFrames = 0;

	public float hurtShake = 0.5f;
	private int hurtBeat = -99;

	
	private float beats; //gives an accurate measure of the totalsong progress in beats

	public ColorSet colorSet;

	// Use this for initialization
	void Start () {
		Random.seed = (int)System.DateTime.Now.Ticks;
		hp = hpMax;

		_player = GameObject.FindWithTag("Player").GetComponent<Player>();
		_playerParts = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerParticles>();
		popUps = new List<GameObject>();
		GameObject[] pups = GameObject.FindGameObjectsWithTag("Respawn");
		foreach (GameObject g in pups){popUps.Add (g); g.SetActive(false);}

		_enemy = GameObject.FindWithTag("Enemy").transform;
		_enemyParts = _enemy.GetComponentInChildren<PlayerParticles>();
		_enemy.GetComponentInChildren<MeshRenderer>().material.color = colorSet.HPColor; //set HP color

		_bg = GameObject.FindWithTag("BG").GetComponent<BGFX>();
		_bg.line.GetComponent<LineRenderer>().material.SetColor("_TintColor",colorSet.lineColor);
		_bg.minColor = colorSet.particleColor1;
		_bg.maxColor = colorSet.particleColor2;
		_bg.lights.GetChild(0).light.color = colorSet.lightColorA;
		_bg.lights.GetChild(1).light.color = colorSet.lightColorB;
		_bg.lights.GetChild(2).light.color = colorSet.lightColorC;

		_mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

		_enemyModel = (Instantiate(boxerModel) as GameObject).transform;
		_enemyModel.parent = _enemy;
		_enemyModel.localPosition = Vector3.zero;
		animation = _enemy.GetComponentInChildren<Animator>();

		attackStream = new List<Attack>();
		frameData = new AttackFrame();

		playlist = new List<Pattern>();
		foreach (Pattern p in soundSet){
			for(int q=0;q<p.commonality;q+=1){
				playlist.Add(p);
			}
		}
		Shuffle ();
		QueueAttacks(soundSet[0]);
		//QueuedFrames += PatternBeats (soundSet[0]);
		Play(soundSet[0]); //always play intro first
	}

	AttackFrame LoadFrame(Attack attk, int frame){
		return attk.frames[frame];
	}
	AttackFrame LoadFrame(string attk, int frame){
		for (int i=0;i<attackSet.Count;i+=1){
			if (attackSet[i].name == attk){
				return attackSet[i].frames[frame];
			}
		}
		return null;
	}
	

	void Play(Pattern p){ //start playing a new pattern
		song = p;
		timePassed = 0f;

		audio.Stop ();
		audio.clip = p.clip;
		audio.Play ();

		QueueAttacks(playlist[(songIndex +1)%playlist.Count]); //queue attacks for the NEXT move, so we can always look ahead.
	}

	void QueueAttacks(Pattern p){ //given a pattern, queue attacks to the stream that fill it exactly
		int beatsToFill = PatternBeats (p);
		List<Attack> attackDeck = new List<Attack>();
		float queue_beat = Mathf.Floor(beats)+1f;
		for (int i=0;i<attackStream.Count;i+=1){queue_beat += (float)attackStream[i].frames.Count;}
		//int new_attacks = 0;

		while(beatsToFill > 0){
			//Debug.Log ("Filling "+beatsToFill+" beats for pattern "+p.name);
			if (attackDeck.Count == 0){ //refill attack deck
				foreach (PatternAttack a in p.attacks){
					for (int i=0;i<a.weight;i+=1){ //weight changes proportion of occurence
						attackDeck.Insert (Mathf.Max (Random.Range(0,attackDeck.Count),0),GetAttack (a.attack));
					}
				}
			}

			//foreach(Attack a in attackDeck){Debug.Log (a.name);}

			if (attackDeck.Count > 0){
				while(attackDeck[0].frames.Count > beatsToFill){ //ignore attacks that can't fit
				//Debug.Log ("ignoring "+attackDeck[0].name);
				attackDeck.RemoveAt(0);
					if (attackDeck.Count == 0){break;}
				}}

			if (attackDeck.Count > 0){ //add the top attack to the stream, and reduce how much time is left to fill
				//Debug.Log ("adding "+attackDeck[0].name+", taking away "+attackDeck[0].frames.Count+" of "+beatsToFill+" beats");
				attackStream.Add (attackDeck[0]);
				for(int q=0;q<attackDeck[0].frames.Count;q+=1){
					if (attackDeck[0].frames[q].attackingShort){ActivatePopUp(_player.transform.position+(3f*Vector3.up),0,queue_beat+(float)q); }
					if (attackDeck[0].frames[q].attackingLong){ActivatePopUp(_player.transform.position,1,queue_beat+(float)q);}
					if (attackDeck[0].frames[q].blockingShort){ActivatePopUp(_player.transform.position+(3f*Vector3.up),2,queue_beat+(float)q);}
					if (attackDeck[0].frames[q].blockingLong){ActivatePopUp(_player.transform.position,3,queue_beat+(float)q);}
					Debug.Log ("queued frame "+(queue_beat+(float)q));
				}
				beatsToFill -= attackDeck[0].frames.Count;
				queue_beat += attackDeck[0].frames.Count;
				//new_attacks += 1;
				//QueuedFrames += attackDeck[0].frames.Count;
				attackDeck.RemoveAt(0);
			}
		}
		//QueuedFrames += PatternBeats (p);
		//Debug.Log ("queued "+new_attacks+" attacks to fill "+PatternBeats (p)+" beats. total queued frames is "+QueuedFrames);
	}

	void Shuffle(){
		List<Pattern> dupe = new List<Pattern>();
		while(playlist.Count > 0){
			int i = Random.Range (0,playlist.Count);
			dupe.Add (playlist[i]);
			playlist.RemoveAt(i);
		}

		playlist = dupe;
	}

	void OnBeat(){
		GameObject.FindWithTag("MainCamera").GetComponent<ScreenShake>().Shake(.05f,.1f);
		beatTimer = beatDelay;
		_bg.OnBeat();

		currentFrame += 1;
		if (currentAttack==null || currentFrame>=currentAttack.frames.Count){
			currentFrame = 0;
			currentAttack = attackStream[0]; 
			//QueuedFrames-= attackStream[0].frames.Count;
			//Debug.Log ("Popped "+attackStream[0].frames.Count+" frames, leaving "+QueuedFrames+" more queued popup frames for "+(attackStream.Count-1)+" attacks");
			attackStream.RemoveAt(0);
		}
		frameData = LoadFrame(currentAttack,currentFrame);
		Debug.Log ("Using "+currentAttack.name+", frame "+(currentFrame+1)+"/"+(currentAttack.frames.Count));

		if (frameData.anim!="" && frameData.anim!=anim){animation.SetTrigger(anim);}
		
		if (frameData.attackingShort && frameData.attackingLong){_playerParts.Swipe(Vector3.left);}
		if (frameData.attackingShort && !frameData.attackingLong){_playerParts.Swipe(Vector3.down);}
		if (!frameData.attackingShort && frameData.attackingLong){_playerParts.Swipe(Vector3.back);}
	}

	void DelayedBeat(){
		if (song!=soundSet[0]){ // intro doesn't start attack stuff yet
			if (!_player.bigAttack && _player.stamina<=0){
				ActivatePopUp(_player.transform.position+(2.25f*Vector3.up),6,beats,true);
			}
			_player.SendMessage("OnBeat");
		}
			
			


			/////resolve player states

			if ( //hurting the player, which resets their state
			    (frameData.attackingShort || frameData.attackingLong) &&
				!(frameData.attackingShort && _player.blockingShort) && 
				!(frameData.attackingLong && _player.blockingLong))
			{
			_player.SendMessage("Hurt");
			ActivatePopUp(_player.transform.position+(1.5f*Vector3.up),5,beats,true);
			_playerParts.Hit();
			}

		if ( //Player attacked
		    (_player.attackingShort) || (_player.attackingLong))
		{
			if (_player.attackingLong){_enemyParts.Swipe (Vector3.up);}
			if (_player.attackingShort){_enemyParts.Swipe (Vector3.left);}
			//if (_player.attackingShort && !_player.left){_enemyParts.Swipe (Vector3.forward);}
			if ( //getting hurt by the player
				(_player.attackingShort && !frameData.blockingShort) || 
				(_player.attackingLong && !frameData.blockingLong))
			{
			if (_player.attackingLong){ActivatePopUp(_player.transform.position+(1.5f*Vector3.up),5,beats); hp -= 3;}
				else{ActivatePopUp(_player.transform.position+(1.5f*Vector3.up),4,beats); hp -= 1;}
				_enemyParts.Hit();
				hurtBeat = Mathf.FloorToInt(beats);
			}else{
				_player.LoseStamina(); //player attacked and whiffed
				_enemyParts.Shield();
			}
		}

			////the rest is just popups
		//if (_player.attackingShort){ActivatePopUp(_player.transform.position+(3f*Vector3.up),0,beats,true);}
		if (_player.attackingLong){ActivatePopUp(_player.transform.position,1,beats,true);}
		if (_player.blockingShort && frameData.attackingShort){_playerParts.Shield();}
		if (_player.blockingLong && frameData.attackingLong){_playerParts.Dodge();}
			if (_player.bigAttack){ActivatePopUp(_player.transform.position,1,Mathf.Floor (beats)+1f,true);}
	}

	void ActivatePopUp(Vector3 t, int spr, float beat_target, bool player = false){
		//Debug.Log ("trying to activate popup");
		bool didit = false;
		GameObject g = null;
		foreach (GameObject pp in popUps){
					if (!pp.activeInHierarchy){
						pp.SetActive(true);
						g = pp;
						break;
						}
				}
		if (g==null){
			//Debug.Log ("gotta instatiate a popup");
			g = Instantiate(popup) as GameObject;
			//Debug.Log ("instantiated "+g);
			g.SetActive(true);
			popUps.Add(g);
		}

			PopUp p = g.GetComponent<PopUp>();
			p.player = player;
			p.tex = spr;
			p.beat_target = Mathf.Floor (beat_target);
			//p.life = 60f/BPM ();
			p.transform.position = t +(2f*Vector3.forward);
			p.Start();

	
			}

	int PatternBeats(Pattern p){
		return Mathf.RoundToInt(p.clip.length*BPM(p)/60f); //how many beats before it switches
	}

	public float BPM(){
		if (doubleTime){return song.bpm*2f;}
			else{return song.bpm;}
	}
	float BPM(Pattern p){
		if (doubleTime){return p.bpm*2f;}
		else{return p.bpm;}
	}

	Attack GetAttack(string str){ //get an attack by name
			foreach (Attack a in attackSet){
				if (a.name==str){return a;}
			}

			return null;
		}

	//low fidelity for effects and junk
	void Update(){
		////enemy hurt shake
		if (Mathf.FloorToInt(beats)==hurtBeat){
			float shake = hurtShake*(1f - Mathf.Pow (beats%1f,2f));
			_enemyModel.localPosition = 
				new Vector3(-shake+Random.Range(0f,2f*shake),-shake+Random.Range(0f,2f*shake),-shake+Random.Range(0f,2f*shake));
		}else{_enemyModel.localPosition = Vector3.zero;}

		////enemy animation speed
		float timeToNext = (60f/BPM());
		float animLength = animation.GetCurrentAnimatorStateInfo(0).length/(float)animationList[animIndex(anim)].beats;
		animation.speed = (animLength/timeToNext);
		
		///////background color
		_mainCamera.backgroundColor = Color.Lerp (colorSet.bgColor1,colorSet.bgColor2,Mathf.Pow (Mathf.Abs(.5f-(beats%1f))/.5f,4f));
	}

	// high fidelity for inputs
	void FixedUpdate () {
		float timeP = timePassed;
		timePassed += Time.deltaTime;

		beats += (Time.deltaTime/60*BPM ());

		///////update beat
		if (Mathf.FloorToInt(timePassed*BPM()/60f)!=Mathf.FloorToInt(timeP*BPM()/60f)){ //new beat
			//currentFrame += 1;
			OnBeat ();
			if (Mathf.Floor(beats)%measureLength == 0f){_player.SendMessage("OnMeasure");}

			if (Mathf.FloorToInt(timePassed*BPM()/60f) >= PatternBeats(song)){ //start a new pattern

				songIndex += 1;
				if (songIndex >= playlist.Count){
					songIndex = 0;
				}
				Play(playlist[songIndex]);

				if (songIndex>=playlist.Count-1){Shuffle();} 
						//shuffle the beat before we start the next pattern, 
							//so we can preview the next attack

			}

		}

		if (beatTimer > 0f){
			beatTimer -= Time.deltaTime;
			if (beatTimer<=0f){
				beatTimer = 0f;
				DelayedBeat();
			}
		}
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

	public float Beat(){
		return beats;
	}
}

//------------------------------------------------------------------------------------------------

[System.Serializable]
public class Pattern
{ 
	public string name;
	public AudioClip clip;
	public float bpm;
	public int commonality;
	public List<PatternAttack> attacks;
	
	public Pattern( AudioClip clip, float bpm, int weight, List<PatternAttack> l )
	{
		this.clip = clip;
		this.bpm = bpm;
		this.commonality = weight;
		this.attacks = l;
	}

}

[System.Serializable]
public class PatternAttack
{ 
	public string attack;
	public int weight = 1;
	public List<AttackRule> rules;
	
	public PatternAttack(string attk, int w, List<AttackRule> l)
	{
		this.attack = attk;
		this.weight = w;
		this.rules = l;
	}
	
}

[System.Serializable]
public class AttackRule
{ 
	public enum attackRuleTypes{startBeat, endBeat, followedBy}

	public attackRuleTypes rule;
	public float value;
	
	public AttackRule(attackRuleTypes r, float v)
	{
		this.rule = r;
		this.value = v;
	}
	
}

[System.Serializable]
public class Attack //a series of preset states over several beats
{ 
	public string name;
	public List<AttackFrame> frames;

	
	public Attack(string str, List<AttackFrame> l, string a="")
	{
		this.name = name;
		this.frames = l;
	}
	
}

[System.Serializable]
public class AttackFrame //one beat of an attack
{ 
	public string anim = "";
	public bool blockingShort = false;
	public bool blockingLong = false;
	public bool attackingShort = false;
	public bool attackingLong = false;
	
	public AttackFrame(bool bS, bool bL, bool aS, bool aL, string anim )
	{
		blockingShort = bS;
		blockingLong = bL;
		attackingShort = aS;
		attackingLong = aL;
		this.anim = anim;
	}
	public AttackFrame()
	{
		blockingShort = false;
		blockingLong = false;
		attackingShort = false;
		attackingLong = false;
		anim = "";
	}
	
}

[System.Serializable]
public class ColorSet //a series of preset states over several beats
{ 
	public Color bgColor1;
	public Color bgColor2;
	public Color particleColor1;
	public Color particleColor2;
	public Color lineColor;
	public Color lightColorA;
	public Color lightColorB;
	public Color lightColorC;
	public Color HPColor;
	
	public ColorSet(
			Color bgColor1,Color bgColor2,Color particleColor1,Color particleColor2,
			Color lineColor,
			Color lightColorA,Color lightColorB,Color lightColorC,
			Color HPColor)
	{
		this.bgColor1 = bgColor1;
		this.bgColor2 = bgColor2;
		this.particleColor1 = particleColor1;
		this.particleColor2 = particleColor2;
		this.lineColor = lineColor;
		this.lightColorA = lightColorA;
		this.lightColorB = lightColorB;
		this.lightColorC = lightColorC;
		this.HPColor = HPColor;
	}
	
}