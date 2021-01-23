using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	public Mesh mesh1;	
	public Mesh mesh2;	

	public Transform player; 

	Vector3[] vertices;
	int[] triangles;
	Color[] colors;

	float[] perlinGirthList;

	public Gradient gradient;

	private int xSize = 40;
	private int zSize = 80;
	private float girth = 160f;

	public float smooth = .2f;

	private float minPerlinGirth;
	private float maxPerlinGirth;

	private float orgXrand;
	private float orgZrand;

	public float angleMax = Mathf.PI/2f;//3f/2f; //en degré
	private float distanceBetweenPoints = 1600f;

	private int nbRepeat = 0;

	public Vector3 pointA = new Vector3(0,3,-4);
	public Vector3 pointB = new Vector3(0,2,-2);
	public Vector3 pointC = new Vector3(0,1,0);

    // Start is called before the first frame update
    void Start()
    {
    	orgXrand = Random.Range(0f,9999f);
    	orgZrand = Random.Range(0f,9999f);

    	mesh1 = new Mesh();
    	transform.GetChild(0).GetComponent<MeshFilter>().mesh = mesh1;

    	mesh2 = new Mesh();
    	transform.GetChild(1).GetComponent<MeshFilter>().mesh = mesh2;
    	
    	CalculateNewPoint();
    	CreateShape();
    	UpdateMesh(mesh1);

    	CalculateNewPoint();
    	CreateShape();
    	UpdateMesh(mesh2);

    	//InitFunction(detail);
        
    }

    void LateUpdate(){
    	if (Vector3.Distance(player.position,pointA)<girth){
    		newMesh();
    	}
    }

    public void CreateShape(){
    	//-------------------------------------   VERTICES / COLORS
    	orgZrand += nbRepeat * zSize;
    	nbRepeat ++;

    	vertices = new Vector3[ (xSize+1) * (zSize+1) ];
    	colors = new Color[(xSize+1) * (zSize+1)];
    	perlinGirthList = new float[(xSize+1) * (zSize+1)];

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
    			float PerlinGirth;
    			if (x<=xSize/2){
					PerlinGirth = girth - 10*Mathf.PerlinNoise(smooth*x+orgXrand/2,smooth*z+orgZrand/2) - 10*Mathf.PerlinNoise(x+orgXrand,z+orgZrand);
    			}else{
    				PerlinGirth = perlinGirthList[(z+1)*xSize - x + z ]; 
    			}
    			perlinGirthList[index] = PerlinGirth;
    			
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

    public void UpdateMesh(Mesh mesh){
    	mesh.Clear();

    	mesh.vertices = vertices;
    	mesh.triangles = triangles;
    	mesh.colors = colors;

    	mesh.RecalculateNormals();
    }


    public Vector3 Lerp( Vector3 a, Vector3 b, float t ){
		return t*b + (1-t)*a;
	}

    private Vector3 QuadraticCurve(float t){
    	Vector3 p0 = Lerp(pointA, pointB, t);
		Vector3 p1 = Lerp(pointB, pointC, t);
		return Lerp(p0,p1, t); 

    }

    private void OnDrawGizmos(){
    	Gizmos.color = Color.red;
    	Gizmos.DrawSphere(pointA, 20f);
    	Gizmos.color = Color.green;
    	Gizmos.DrawSphere(pointB, 20f);
    	Gizmos.color = Color.yellow;
    	Gizmos.DrawSphere(pointC, 20f);

    	if (vertices==null)
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
    		
    	}
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
    	CalculateNewPoint();
    	CreateShape();
    	if (nbRepeat%2!=0){
    		UpdateMesh(mesh1);
    	}else{
    		UpdateMesh(mesh2);
    	}
    }
}
