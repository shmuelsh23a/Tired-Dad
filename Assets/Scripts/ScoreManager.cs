using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {
//{
    public static ScoreManager instance;
    public float prevFatherTiredness;
    public float prevBabyTiredness;
    public float prevMoral;
    public float prevGuilt;
    public float prevNeeds;
    public float newFatherTiredness;
    public float newBabyTiredness;
    public float newMoral;
    public float newGuilt;
    public float score;
    public int gameLevel = 0;

    void Awake()
    {

        if (instance != null)
        {
            Debug.Log("Singleton error!");
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public void CalculateScore() {
        prevFatherTiredness = Game.instance.fatherTiredness;
        prevBabyTiredness = Game.instance.babyTiredness;
        prevMoral = Game.instance.moral;
        prevGuilt = Game.instance.guilt;
        prevNeeds = Game.instance.needs;
        score = Game.instance.score + ((100 - prevNeeds) - prevGuilt) + (100 - prevMoral) + (100 - prevFatherTiredness);
        
	}

    public void CalculateNewStats() // Calculate new Stats for level 2 and upwords.
    {
        
        if (gameLevel > 0)
        {
            newFatherTiredness = prevFatherTiredness - Random.Range(10,prevFatherTiredness);
            newBabyTiredness = prevBabyTiredness - Random.Range(10,prevBabyTiredness);
            if (prevMoral >= (prevFatherTiredness - newFatherTiredness))
            {
                newMoral = prevMoral - (prevFatherTiredness - newFatherTiredness);
            }
            
            newGuilt = 100 - prevNeeds;
            gameLevel++;
        }
        else
        {
            newFatherTiredness = 0;
            newBabyTiredness = 0;
            newMoral = 0;
            newGuilt = 0;
            gameLevel++;
        }
}



 
}
