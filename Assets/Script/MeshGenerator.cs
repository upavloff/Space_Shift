using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class MeshGenerator : MonoBehaviour
{
	public Mesh mesh1;	
	public Mesh mesh2;	
	private Mesh currentMesh;	

	public MeshCollider meshCollider1;
	public MeshCollider meshCollider2;

	public Transform player; 
	public PlayerMotor motorScript;

	Vector3[] vertices;
	int[] triangles;
	Color[] colors;

	bool[] obstacleOld;
	bool[] obstacleCurrent;
	public bool obstacleIsRegistered = true;
	/*public Vector3[] centerMeshOld;
	public Vector3[] centerMeshNew;*/

	float[] perlinGirthList;

	public Gradient gradient;

	private int xSize = 40;
	private int zSize = 80;
	private float girth = 160f;

	public float smooth = .2f;
	public float minPic = 0.8f;
	public float maxPic = 1.25f;
	private float picProbability = 0.01f;

	private float minPerlinGirth;
	private float maxPerlinGirth;

	private float orgXrand;
	private float orgZrand;

	public float angleMax = Mathf.PI/2f;//3f/2f; //en degré
	private float distanceBetweenPoints = 1600f;

	private int nbRepeat = 0;

	private Vector3 initA = new Vector3(0,3,-4);
	private Vector3 initB = new Vector3(0,2,-2);
	private Vector3 initC = new Vector3(0,1,0);

	public Vector3 pointA;
	public Vector3 pointB;
	public Vector3 pointC;

	public float DistPlayer = 0f;
	public float distMean = 1f;
	private int meanLength = 100;
	public Vector3 closestPoint = new Vector3();
	public int closestPointIndex = new int();

    // Start is called before the first frame update
    void Start()
    {
    	orgXrand = Random.Range(0f,9999f);
    	orgZrand = Random.Range(0f,9999f);

    	mesh1 = new Mesh();
    	transform.GetChild(0).GetComponent<MeshFilter>().mesh = mesh1;
    	meshCollider1 = transform.GetChild(0).GetComponent<MeshCollider>();


    	mesh2 = new Mesh();
    	transform.GetChild(1).GetComponent<MeshFilter>().mesh = mesh2;
    	meshCollider2 = transform.GetChild(1).GetComponent<MeshCollider>();

    	InitialiseMesh();

    	motorScript = player.GetComponent<PlayerMotor>();
        
    }

    public void InitialiseMesh(){
    	pointA = initA;
    	pointB = initB;
    	pointC = initC;
    	
    	CalculateNewPoint();
    	CreateShape();
    	UpdateMesh(mesh1, meshCollider1);

    	currentMesh = mesh1;
    	obstacleOld = obstacleCurrent;

    	CalculateNewPoint();
    	CreateShape();
    	UpdateMesh(mesh2, meshCollider2);
    }

    void FixedUpdate(){
    	//closestPoint = meshCollider1.ClosestPoint(player.position);
    	if (motorScript.isPlaying){
	    	float smallestDist = 10000f;
	    	int i = 0;
	    	foreach (Vector3 vertex in currentMesh.vertices){
	    	//for (int i = 0; i < currentMesh.vertices.Length; i++){
	    		//float currenDist = Vector3.Distance(currentMesh.vertices[i], player.position);
	    		float currenDist = Vector3.Distance(vertex, player.position);
	    		if (currenDist<=smallestDist){
	    			smallestDist = currenDist;
	    			closestPoint = vertex;
	    			closestPointIndex = i;
	    		}
	    		i++;
	    	}
	    	DistPlayer = smallestDist;
	    	distMean = distMean + (DistPlayer - distMean)/meanLength;
	    	//meanLength++;
	    }else if( ! motorScript.isPlaying && !obstacleIsRegistered){
	    	bool pic = obstacleOld[closestPointIndex];
	    	if ( closestPointIndex+1 < obstacleOld.Length )   pic = pic || obstacleOld[closestPointIndex+1] ;
	    	if (closestPointIndex-1>=0) 					  pic = pic || obstacleOld[closestPointIndex-1] ;
	    	if (closestPointIndex+xSize < obstacleOld.Length) pic  = pic || obstacleOld[closestPointIndex+xSize]  ;
	    	if (closestPointIndex-xSize>=0)  				  pic = pic || obstacleOld[closestPointIndex-xSize];

	    	Debug.Log("the obstavle is  "+ pic );
	    	//Debug.Log("the obstavle is a ");
	    	obstacleIsRegistered = true;
	    	//obstacleIsRegistered put false in healthScipt
	    	if (pic){
	    		picProbability /= 2;
    		}else{
    			girth =  Mathf.RoundToInt(girth*1.1f); 
    		}
    		UpdateLastMesh();
	    }
    }

    void LateUpdate(){    	
    	if (Vector3.Distance(player.position,pointA)<girth){
    		newMesh();
    		//keep track of nbMesh pass
    		AnalyticsResult result = Analytics.CustomEvent(
    			"MeshPass",
    			new Dictionary<string,object>{
    				{"MeshNumber", nbRepeat-2},
    				{"MeshGirth", girth}
    			}

    		);
    		Debug.Log("Analytics result = "+result);
    		Debug.Log(" nbrepeat "+nbRepeat);
    	}
    }

    public void CreateShape(){
    	//-------------------------------------   VERTICES / COLORS
    	orgZrand += nbRepeat * zSize;
    	nbRepeat ++;

    	vertices = new Vector3[ (xSize+1) * (zSize+1) ];
    	colors = new Color[(xSize+1) * (zSize+1)];
    	perlinGirthList = new float[(xSize+1) * (zSize+1)];
    	obstacleCurrent = new bool[(xSize+1) * (zSize+1)];
    	
    	//centerMeshNew = new Vector3[(xSize+1) * (zSize+1)];

		minPerlinGirth = girth;
		maxPerlinGirth = 0f;

    	int index = 0;

		Vector3 init1 = new Vector3(0,0,0);
    	Vector3 init2 = new Vector3(0,0,0);

    	for (int z=0; z<=zSize; z++)
    	{
    		init1 = QuadraticCurve((float)z/(float)zSize);
    		init2 = QuadraticCurve((float)(z+1)/(float)zSize);
    		//get the direction of the curv (gradient)
    		Vector3 dir = (init2 - init1).normalized;

    		Vector3 dirNorm = new Vector3(-dir.y*dir.z, 0.5f*dir.x*dir.z, 0.5f*dir.y*dir.x).normalized;
    		//dir = (pointB - pointA).normalized;
    		//if (z==0) test = dir;
    
    		//vertices[index] = init;
    		//index++;
    		for (int x=0; x<=xSize; x++)
    		{
    			float PerlinGirth = 0f;
    			bool obstacleIsPic = false;
    			//if (x<=xSize/2){
    			if (x<=xSize-5){	//if the 5 last info copy the begining for a smooth circle
					PerlinGirth = girth - 10*Mathf.PerlinNoise(smooth*x+orgXrand/2,smooth*z+orgZrand/2) - 10*Mathf.PerlinNoise(x+orgXrand,z+orgZrand);
					//add some pikes in the tunel
					//if (z>1 && z<zSize-1) PerlinGirth -=(Random.Range(0f,1f)>0.7f ? minPic*girth : maxPic*girth)*(Random.Range(0f,1f)>0.99f ? 1 : 0);
					if (z>1 && z<zSize-1 && Random.Range(0f,1f)> (1f - picProbability)){ 
						PerlinGirth -=(Random.Range(0f,1f)>0.7f ? minPic*girth : maxPic*girth);
						obstacleIsPic = true;
					}
    			}else{
    				PerlinGirth = perlinGirthList[(z+1)*xSize - x + z ]; 
    				obstacleIsPic = obstacleCurrent[(z+1)*xSize - x + z ];
    			}
    			perlinGirthList[index] = PerlinGirth;
    			obstacleCurrent[index] = obstacleIsPic;
    			//centerMeshNew[index] = init1;
    			
    			vertices[index] = init1 + Quaternion.AngleAxis((float)x*360f/(float)xSize,dir) * dirNorm * PerlinGirth;
    			index++;

    			if (PerlinGirth>maxPerlinGirth){
    				maxPerlinGirth = PerlinGirth;
    			}
    			if (PerlinGirth<minPerlinGirth){
    				minPerlinGirth = PerlinGirth;
    			}
    		}
    	}

    	//-------------------------------------   TRIANGLES

	    triangles = new int[xSize*zSize*6];

    	int vert = 0;
    	int tris = 0;

    	for (int z=0; z<zSize; z++)
    	{
	    	for (int x=0; x<xSize; x++)
	    	{
		    	triangles[tris + 0] = vert + 0;
		    	triangles[tris + 1] = vert + xSize + 1;
		    	triangles[tris + 2] = vert + 1;
		    	triangles[tris + 3] = vert + 1;
		    	triangles[tris + 4] = vert + xSize + 1;
		    	triangles[tris + 5] = vert + xSize + 2;

		    	vert ++;
		    	tris += 6;
	    	}
    		vert ++;
    	}

    	//------------------------------------   Colors
    	int i = 0;
    	for (int z=0; z<=zSize; z++)
    	{
    		for (int x=0; x<=xSize; x++)
    		{
    			float valueGrad = Mathf.InverseLerp(minPerlinGirth, maxPerlinGirth, perlinGirthList[i] );
    			colors[i] = gradient.Evaluate( valueGrad );
    			i++;
    		}
    	}

    }

    public void UpdateMesh(Mesh mesh, MeshCollider meshCollider){    	
    	mesh.Clear();

    	mesh.vertices = vertices;
    	mesh.triangles = triangles;
    	mesh.colors = colors;

    	mesh.RecalculateNormals();
    	meshCollider.sharedMesh = mesh;
    }

    public void UpdateLastMesh(){
		CreateShape();
    	if (nbRepeat%2!=0){
    		UpdateMesh(mesh2, meshCollider2);
    	}else{
    		UpdateMesh(mesh1, meshCollider1);
    	}
    	nbRepeat--;
    	Debug.Log("nbReapt is "+nbRepeat);
    }


    public Vector3 Lerp( Vector3 a, Vector3 b, float t ){
		return t*b + (1-t)*a;
	}

    public Vector3 QuadraticCurve(float t){
    	Vector3 p0 = Lerp(pointA, pointB, t);
		Vector3 p1 = Lerp(pointB, pointC, t);
		return Lerp(p0,p1, t); 
    }

    private void OnDrawGizmos(){
    	Gizmos.color = Color.red;
    	Gizmos.DrawSphere(pointA, 15f);
    	Gizmos.color = Color.green;
    	Gizmos.DrawSphere(pointB, 15f);
    	Gizmos.color = Color.yellow;
    	Gizmos.DrawSphere(pointC, 15f);

    	/*if (vertices==null)
    		return;
    	for (int i=0; i < vertices.Length; i++)
    	{
    		Gizmos.color = Color.blue;
    		if (i== vertices.Length-1){
    			Gizmos.DrawSphere(vertices[i], 5f);
    		}else{
    			Gizmos.DrawSphere(vertices[i], 5f);
    		}
    		//Debug.Log("present"); 	
    	}*/

    	/*for (int i=0; i < mesh1.vertices.Length/10; i++)
    	{
    		Gizmos.color = Color.blue;
    		if (obstacleOld[i]){
	    		Gizmos.color = Color.red;
    			Gizmos.DrawSphere(mesh1.vertices[i], 2f);
    		}else{
    			Gizmos.DrawSphere(mesh1.vertices[i], 2f);
    		}
    		//Debug.Log("present"); 	
    	}*/


    	/*Gizmos.color = Color.red;
    	foreach (Vector3 pos in centerMeshOld){
    		Gizmos.DrawSphere(pos, 7f);
    	}*/

    	//draw sphere closeet point
    	/*Gizmos.color = Color.yellow;
    	Gizmos.DrawSphere(closestPoint, 7f);
    	Gizmos.color = Color.green;
    	Gizmos.DrawSphere(currentMesh.vertices[closestPointIndex], 8f);*/

    }

    private void CalculateNewPoint(){
    	//calculate new random point according to previous point to stay smooth
    	pointA = pointC;
    	Vector3 dir = (pointC - pointB).normalized;

    	pointB = pointC + dir * distanceBetweenPoints;

    	Vector3 dirNorm = new Vector3(-dir.y*dir.z, 0.5f*dir.x*dir.z, 0.5f*dir.y*dir.x).normalized;
    	//tweak previous direction from a random angle
    	float theta = angleMax;//Random.Range(0f, 1f)*maxAngle;
    	Vector3 newDir = (Mathf.Cos(theta)*dir + Mathf.Sin(theta)*dirNorm).normalized;

    	pointC = pointB + Quaternion.AngleAxis(Random.Range(0f,360f),dir) * newDir * distanceBetweenPoints;
    }

    public void newMesh(){
    	//centerMeshOld = (Vector3[]) centerMeshNew.Clone();
    	obstacleOld = obstacleCurrent;
    	CalculateNewPoint();
    	CreateShape();
    	if (nbRepeat%2!=0){
    		UpdateMesh(mesh1, meshCollider1);
    		currentMesh = mesh2;   ///current mesh is the closest one not the one we build
    	}else{
    		UpdateMesh(mesh2, meshCollider2);
    		currentMesh = mesh1;
    	}
    	
    }

    void OnGUI(){
		GUI.Label(new Rect(Screen.width-500, 80,200,100),"DistPlayer = "+DistPlayer);
		GUI.Label(new Rect(Screen.width-500, 100,200,100),"MeanDist = "+distMean );
		//GUI.Label(new Rect(Screen.width-500, 120,200,100),"counter = "+meanLength);
	}

	public void OnPlay(){
		obstacleIsRegistered = false;
	}
}
