using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public class HealthCollision : MonoBehaviour
{

	public PlayerMotor motorScript;
	public MeshGenerator meshScript;
	public FollowPlayer followScript;

	public MeshRenderer renderer;

	public RewindTime rewindScript;

	public GameObject life;
	List<GameObject> lifeElements = new List<GameObject>();

	public GameObject gameOver;

	private int maxHealth = 10;
	public int currentHealth ;

    // Start is called before the first frame update
    void Start()
    {
	    renderer = transform.GetChild(0).GetComponent<MeshRenderer>();    
	    motorScript = transform.GetComponent<PlayerMotor>();
	    rewindScript = transform.GetComponent<RewindTime>();
	    currentHealth = maxHealth;
	    lifeElements.Add(life);
	    for (int i=1; i<maxHealth; i++){
	    	GameObject tmpLife = Instantiate(life, life.transform /*.parent.transform*/, true);
	    	tmpLife.transform.position += Vector3.right * i * 2f * ((RectTransform)tmpLife.transform).rect.width;
	    	lifeElements.Add( tmpLife );

	    }
    }


    public void HandleCollision(){
		currentHealth --;
		//stop motor and folow script
    	motorScript.isPlaying = false;
		followScript.isPlaying = false;
		if (currentHealth==0) meshScript.obstacleIsRegistered = true;
    	if (currentHealth<0){
    		Die();
    		return;
    	}
    	//turn heart to gray
		lifeElements[currentHealth].transform.GetChild(1).gameObject.SetActive(false);
    	rewindScript.StartRewind();
	}

	void Die(){
		Debug.Log("you ded");
		gameOver.SetActive(true);
		StartCoroutine(CloseGameOver());
		motorScript.Reset();
		followScript.Reset();
		meshScript.InitialiseMesh();
		currentHealth = maxHealth;
		foreach (GameObject heart in lifeElements){
			heart.transform.GetChild(1).gameObject.SetActive(true);
		}
		life.SetActive(false);
		//this.enabled = false;

		//handle analytics
		AnalyticsResult result = Analytics.CustomEvent(
			"DieEvent",
			new Dictionary<string,object>{
				{"DieOnPic", meshScript.dieOnPic},
				{"DieOnWall", meshScript.dieOnWall}
			}
		);
		Debug.Log("Analytics result = "+result);
	
    	meshScript.OnDeath();
	}

    void OnGUI(){
		GUI.Label(new Rect(Screen.width-500, 60,200,100),"currentHealth = "+currentHealth);
	}


	IEnumerator CloseGameOver(){
		Vector3 initPos = gameOver.transform.position;
		yield return new WaitForSeconds(1f);
		float i = 0;
		while(i<Screen.height){
			i+=Time.deltaTime*4f/*vitesse de descente*/;
			gameOver.transform.position += Vector3.down * i;
			yield return new WaitForEndOfFrame(); 
		}
		gameOver.SetActive(false);
		gameOver.transform.position = initPos;
	}

	public void OnPlay(){
		life.SetActive(true);
		for (int i = 0; i < life.transform.GetChildCount(); ++i)
		{
		    life.transform.GetChild(i).gameObject.SetActive(true);
		}
	}

	public void endRewind(){
		motorScript.isPlaying = true;
		followScript.isPlaying = true;
		meshScript.obstacleIsRegistered = false;
	}
}
