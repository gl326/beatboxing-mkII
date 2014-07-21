using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class EnemyHP : MonoBehaviour {
	private int quality = 16;
	private float hpRadius = 1.75f;
	//private float staminaRadius = 2.2f;

	private float hpFill = 1f;
	//private float staminaFill = 1f;

	private float hpY = 0f;
	//private float stamY = -0.1f;

	private MeshFilter _mesh;
	//private MeshFilter _mesh2;
	private BeatBoxer _player;

	//public Color hpColor;
	//public Color staminaColor;


	// Use this for initialization
	void Start () {
		_player = GameObject.FindWithTag("Beatboxer").GetComponent<BeatBoxer>();
		_mesh = GetComponent<MeshFilter>();
		_mesh.mesh = new Mesh();

		HpDraw ();
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


	}
}
