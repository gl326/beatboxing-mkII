using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class PlayerHP : MonoBehaviour {
	private int quality = 16;
	private float hpRadius = 1.75f;
	private float staminaRadius = 2.2f;

	private float hpFill = 1f;
	private float staminaFill = 1f;

	private float hpY = 0f;
	private float stamY = -0.1f;

	private MeshFilter _mesh;
	private MeshFilter _mesh2;
	private Player _player;

	//public Color hpColor;
	//public Color staminaColor;


	// Use this for initialization
	void Start () {
		_player = this.transform.parent.GetComponent<Player>();
		_mesh = GetComponent<MeshFilter>();
		_mesh2 = this.transform.GetChild (0).GetComponent<MeshFilter>();
		_mesh.mesh = new Mesh();
		_mesh2.mesh = new Mesh();

		HpDraw ();
		StaminaDraw ();
	}

	void StaminaDraw(){
		_mesh2.mesh.Clear();
		
		int verts = Mathf.CeilToInt(quality/2f*staminaFill);
		
		if (verts>=2){
			Vector3[] vertices = new Vector3[verts + 1];
			vertices[0] = Vector3.zero; vertices[0].y = stamY;
			for(int i=0;i<verts;i+=1){
				vertices[i+1] = new Vector3(
					-Mathf.Cos(((float)i/(float)verts)*Mathf.PI*staminaFill)*staminaRadius,
					stamY,
					-Mathf.Sin(((float)i/(float)verts)*Mathf.PI*staminaFill)*staminaRadius);
			}
			
			//Color32[] colors32 = new Color32[verts + 1];
			//for(int i=0;i<verts+1;i+=1){colors32[i] = staminaColor;}
			
			int[] triangles = new int[(verts-1)*3];
			for(int i=0;i<(verts-1);i+=1){
				//Debug.Log ("drawing triangle "+(i+1));
				triangles[(i*3)] = 0;
				triangles[(i*3)+1] = i+1;
				triangles[(i*3)+2] = i+2;
			}
			
			Vector2[] uvs = new Vector2[vertices.Length];
			for(int i = 0; i < uvs.Length; i+=1) {
				uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
			}
			
			_mesh2.mesh.vertices = vertices;
			_mesh2.mesh.triangles = triangles;
			_mesh2.mesh.uv = uvs;
//			_mesh2.mesh.colors32 = colors32;
			_mesh2.mesh.RecalculateNormals();
		}
	}

	void HpDraw(){
		_mesh.mesh.Clear();

		int verts = Mathf.CeilToInt(quality*hpFill);

		if (verts>=2){
		Vector3[] vertices = new Vector3[verts + 1];
		vertices[0] = Vector3.zero;
		for(int i=0;i<verts;i+=1){
			vertices[i+1] = new Vector3(
						-Mathf.Sin(((float)i/(float)verts)*Mathf.PI*2*hpFill)*hpRadius,
						hpY,
						-Mathf.Cos(((float)i/(float)verts)*Mathf.PI*2*hpFill)*hpRadius);
		}
		
		//Color32[] colors32 = new Color32[verts + 1];
		//for(int i=0;i<verts+1;i+=1){colors32[i] = hpColor;}

		int[] triangles = new int[(verts)*3];
		for(int i=0;i<(verts-1);i+=1){
				//Debug.Log ("drawing triangle "+(i+1));
			triangles[(i*3)] = 0;
			triangles[(i*3)+1] = i+1;
			triangles[(i*3)+2] = i+2;
		}
			triangles[((verts-1)*3)] = 0;
			triangles[((verts-1)*3)+1] = (verts-1)+1;
			triangles[((verts-1)*3)+2] = 1;

		Vector2[] uvs = new Vector2[vertices.Length];
		for(int i = 0; i < uvs.Length; i+=1) {
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		}

		_mesh.mesh.vertices = vertices;
		_mesh.mesh.triangles = triangles;
		_mesh.mesh.uv = uvs;
	//	_mesh.mesh.colors32 = colors32;
		_mesh.mesh.RecalculateNormals();
		}
	}
	
	// Update is called once per frame
	void Update () {
		//////////////HP//////////
		float hpp = hpFill;
		float hpT = ((float)_player.hp/(float)_player.hpMax);

		if (Mathf.Abs (hpFill - hpT) >= .01f){
		hpFill += (hpT - hpFill) * 0.1f;
		}
		else{
			hpFill = (hpT);
		}

		if (hpFill!=hpp){
			HpDraw();
		}

		//////////////stamina//////////
		float sp = staminaFill;
		float sT = ((float)_player.stamina/(float)_player.staminaMax);
		
		if (Mathf.Abs (staminaFill - sT) >= .01f){
			staminaFill += (sT - staminaFill) * 0.1f;
		}
		else{
			staminaFill = (sT);
		}
		
		if (staminaFill!=sp){
			StaminaDraw();
		}

	}
}
