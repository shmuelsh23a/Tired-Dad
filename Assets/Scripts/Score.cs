using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public static Score instance;
    public Text fatherTirednessText;
    public Text babyTirednessText;
    public Text moralText;
    public Text guiltText;
    public Text needsText;
    public Text timeText;
    public Text scoreText;
    public Text versionText;
    public Text endingText;
    float prevScore;
    int fatherTirednessCounter;
    int babyTirednessCounter;
    int moralCounter;
    int guiltCounter;
    int scoreCounter;

    void Awake()
    {
        Object.DontDestroyOnLoad(ScoreManager.instance);
        if (instance != null)
        {
            Debug.Log("Singleton error!");
        }
        else
        {
            instance = this;
            
            return;
        }
    }
    // Use this for initialization
	void Start () {
        prevScore = ScoreManager.instance.score;
        ScoreManager.instance.CalculateNewStats();
        fatherTirednessCounter = (int)ScoreManager.instance.newFatherTiredness;
        babyTirednessCounter = (int)ScoreManager.instance.newBabyTiredness;
        moralCounter = (int) ScoreManager.instance.newMoral;
        guiltCounter = (int) ScoreManager.instance.newGuilt;
        scoreCounter = (int) ScoreManager.instance.score; 
        //ScoreManager SC = ScoreManager.instance.GetComponent<ScoreManager>();
       /* for (int i = (int)ScoreManager.instance.prevFatherTiredness; i == (int)ScoreManager.instance.newFatherTiredness; i--)
        {
            fatherTirednessCounter = i;
            Debug.Log(i);
            fatherTirednessText.text = "Tiredness " + fatherTirednessCounter.ToString();            
        }
        

        for (int i = (int)ScoreManager.instance.prevBabyTiredness; i == (int)ScoreManager.instance.newBabyTiredness; i--)
        {
            babyTirednessCounter = i;
            babyTirednessText.text = "Sleepiness " + babyTirednessCounter.ToString();
        }
        

        for (int i = (int)ScoreManager.instance.prevMoral; (int)i == ScoreManager.instance.newMoral; i--)
        {
            moralCounter = i;
            moralText.text = "Moral " + moralCounter.ToString();
        }
        

        for (int i = (int)ScoreManager.instance.prevGuilt; i == (int)ScoreManager.instance.newGuilt; i--)
        {
            guiltCounter = i;
            guiltText.text = "Guilt " + guiltCounter.ToString();
        }
        

        for (int i = (int)prevScore; i == (int)ScoreManager.instance.score; i++)
        {
            scoreCounter = i;
            scoreText.text = "Score " + scoreCounter.ToString();
        }*/
        scoreText.text = "Score " + scoreCounter.ToString();
        guiltText.text = "Guilt " + guiltCounter.ToString();
        moralText.text = "Moral " + moralCounter.ToString();
        babyTirednessText.text = "Sleepiness " + babyTirednessCounter.ToString();
        fatherTirednessText.text = "Tiredness " + fatherTirednessCounter.ToString();

	    
	}
	
	// Update is called once per frame
	void Update () {

        endingText.text = ("Do you want to play more? if yes, tap with one finger. if no, tap with two fingers");
        if (Input.touchCount == 2)
        {
            DontDestroyOnLoad(ScoreManager.instance);
            Application.LoadLevel("First Stage");
        }
        else if (Input.touchCount == 3)
        {
            Debug.Log("Quit");
            Application.Quit();
        }
	}
}
