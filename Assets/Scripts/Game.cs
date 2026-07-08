using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	
	public static Game instance;
    public GameObject player;
	Vector3 spawner;
    public Text fatherTirednessText;
    public Text babyTirednessText;
    public Text moralText;
    public Text guiltText;
    public Text needsText;
    public Text timeText;
    public Text scoreText;
    public Text versionText;
    public Text endingText;
    public Text levelText;
	public float fatherTiredness;
	public float babyTiredness;
	public float moral;
	public float guilt;
	public float needs;
    float randomX;
    float randomZ;
    float spawnCheckerX;
    float spawnCheckerZ;
    float spawnCheckerXPlus;
    float spawnCheckerZPlus;
    float prevTimeChecker;
   /* int prevFatherTiredness;
    int prevBabyTiredness;
    int prevMoral;
    int prevGuilt;
    int prevNeeds;*/
    double version = 0.3;
    public float fatherTirednessOffset;
    public float babyTirednessOffset;
    public float needsOffset;
    public float moralOffset;
    public float attendingNeedsOffset;
    public float changedDiaperOffset;
    public float coffeeOffset;
    public float awakeAnywayOffset;
    public float almostAsleepOffset;
    public float bottleOffset;
    public float calmMovementOffset;
    public float calmMusicOffset;
    public float cuteWhenAsleepOffset;
    public float happyOffset;
    public float hugOffset;
    public float lullabyOffset;
    public float pacifierOffset;
    public float smileOffset;
    public float attentionOffset;
    public float barkingDogsOffset;
    public float carsOffset;
    public float cryOffset;
    public float foodOffset;
    public float itIsSoLateOffset;
    public float loudMusicOffset;
    public float mustBeEasierOffset;
    public float noSleepOffset;
    public float poopOffset;
    public float toysOffset;
    public float tummyHurtsOffset;
    public float burpNeedsOffset;
    public float burpBabyTirednessOffset;
    public float burpFatherTirednessOffset;
    public float kuchKuchKuMoralOffset;
    public float kuchKuchKuGuiltOffset;
    public float kuchKuchKubabyTirednessOffset;
    public float lightsOnFatherTirednessOffset;
    public float lightsOnbabyTirednessOffset;
    public float momIsAsleepMoralOffset;
    public float momIsAsleepFatherTirednessOffset;
    public float sleepingTigerBabyTirednessOffset;
    public float sleepingTigerFatherTirednessOffset;
    public float workTomorrowGuiltOffset;
    public float workTomorrowMoralOffset;
    public float changingDiaperBabyTirednessOffset;
    public int gameLevel = 0;
    int time;
    public int score;
	int positives = 0;
	int negatives = 0;
    int halfAndHalf = 0;
    int goods = 0;
    int yieldTime;
    bool goodsInternalCounter = true;
    public bool needPoop = false;
    bool needFood = false;
    bool needAttention = false;
    bool areThereNeeds = false;
	public Transform coffee;
	public Transform awakeAnyway;
	public Transform almostAsleep;
	public Transform bottle;
	public Transform calmMovement;
	public Transform calmMusic;
	public Transform cuteWhenAsleep;
	public Transform happy;
	public Transform hug;
	public Transform lullaby;
	public Transform pacifier;
	public Transform smile;
    public Transform sofa;
    public Transform bed;
    public Transform chair;
    public Transform changingTable;
    public Transform attention;
    public Transform barkingDog;
    public Transform cars;
    public Transform cry;
    public Transform food;
    public Transform itIsSoLate;
    public Transform loudMusic;
    public Transform mustBeEasier;
    public Transform noSleep;
    public Transform poop;
    public Transform toys;
    public Transform tummyHurts;
    public Transform burp;
    public Transform kuchKuchKu;
    public Transform lightOn;
    public Transform momIsAsleep;
    public Transform sleepingTiger;
    public Transform workTomorrow;
    GameObject changinTableObject;
    bool running;
    
    

    #region "Main Code Area"
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
            //DontDestroyOnLoad(this);
            return;
        }
	}
	
	// Use this for initialization
	void Start () 
    {

        //if (gameLevel > 0)
        //{
            //ScoreManager.instance.CalculateScore();
        //}
        ScoreManager.instance.CalculateNewStats();
        Debug.Log(ScoreManager.instance.gameLevel);
        fatherTiredness = ScoreManager.instance.newFatherTiredness;
        babyTiredness = ScoreManager.instance.newBabyTiredness;
        moral = ScoreManager.instance.newMoral;
        guilt = ScoreManager.instance.newGuilt;
        gameLevel = ScoreManager.instance.gameLevel;
        score = (int) ScoreManager.instance.score;
        levelText.text = "Level " + gameLevel.ToString();
        scoreText.text = "Score " + score.ToString();
        FrnitureSpawner();
        RandomNeedsDecider();
        prevTimeChecker = Time.timeSinceLevelLoad;
        versionText.text = version.ToString();
        running = true;
	}
	
	// Update is called once per frame
	public void Update () 
    {
        EndGame();
        if (running == true)
        {
            TimeManeger();
            FatherTierdnedssMenager();
            MoralMenager();
            GuiltMenager();
            BabyTierdnedssMenager();
            NeedsMenager();
            float rangeStart = Random.Range(0.0f, 11.0f); // randomizing the spawning so not every fram will spawn an object
            float rangeend = Random.Range(0.0f, 11.0f);
            if ((rangeStart > rangeend) && (goods < 10) && (goodsInternalCounter == true))
            {

                goodsInternalCounter = false; // makes sure that a new spanning action won't start while another spawnning action is under way due to corutine
                StartCoroutine(SpawnObjects());
            }
            else
            {
                return;
            }
        }
        
	}
    #endregion
    #region "Goods Managers"
    public void RegisterGoods ()
	{
		goods++;

	}
	
	public void UnregisterGoods()
	{
		goods--;
	}

    public void RegisterHalfAndHalf()
    {
        halfAndHalf++;
    }

    public void UnregisterHalfandHalf()
    {
        halfAndHalf--;
    }
	
	public void RegisterPositives ()
	{
		positives++;
		
	}
	public void UnregisterPositives()
	{
		positives--;
	}

	public void RegisterNegatives ()
	{
		negatives++;
		
	}
	
	
	public void UnregisterNegatives()
	{
		negatives--;
	}
#endregion

    #region "Offset Managers"
    public void JustAte() // Set food counter off, changes needs acordigly
    {
        needFood = false;
        if (needs > bottleOffset)
        {
            needs = needs - bottleOffset; // Changes baby's needs when Bottle item is collected.
        }
        else if ((needs < bottleOffset) && (needs > 0))
        {
            needs = 0;
        }
        Debug.Log("Just Ate");
    }

    public void GotPacifier()
    {
        needAttention = false;
        if (needs > pacifierOffset)
        {

            needs = needs - pacifierOffset;
        }
        else if ((needs < pacifierOffset) && (needs > 0))
        {
            needs = 0;
        }
            Debug.Log("Got Attention");
    }

    public void HasPoop()
    {
        if (needPoop == false)
        {
            needPoop = true;
        }
        else
        {
            needs = needs + poopOffset;
        }  
    }

    public void ChangedDaiper()
    {
        needPoop = false;
        if (needs > attendingNeedsOffset)
        {
            needs = needs - attendingNeedsOffset;
        }
        else if ((needs < attendingNeedsOffset) && (needs > 0))
        {
            needs = 0;
        }
        
        if (guilt > changedDiaperOffset)
        {
            guilt = guilt - changedDiaperOffset;
        }
        else if ((guilt < changedDiaperOffset) && (guilt > 0))
        {
            guilt = 0;
        }

        if (babyTiredness > changingDiaperBabyTirednessOffset)
        {
            babyTiredness = babyTiredness - changingDiaperBabyTirednessOffset;
        }
        else if ((babyTiredness < changingDiaperBabyTirednessOffset) && (babyTiredness > 0))
        {
            babyTiredness = 0;
        }

    }


    public void Hungry()
    {
        if (needFood == false)
        {
            needFood = true;
        }
        else
        {
            needs = needs + foodOffset;
        }
    }

    public void WantsAttention()
    {
        if (needAttention == false)
        {
            needAttention = true;
        }
        else
        {
            needs = needs + attentionOffset;
        }
    }

    public void CoffeeOffset() // Changes father's tireness when Coffee item is collected.
    {
        if (fatherTiredness > coffeeOffset)
        {
            fatherTiredness = fatherTiredness - coffeeOffset;
        }
        else if ((fatherTiredness < coffeeOffset) && (fatherTiredness > 0))
        {
            fatherTiredness = 0;
        }
    }

    public void AwakeAnywayOffset() // Changes father's tireness when Awake Anyway item is collected.
    {
        if (fatherTiredness > awakeAnywayOffset)
        {
            fatherTiredness = fatherTiredness - awakeAnywayOffset;
        }
        else if ((fatherTiredness < awakeAnywayOffset) && (fatherTiredness > 0))
        {
            fatherTiredness = 0;
        }
    }

    public void ItIsSoLateOffset() // Changes father's tireness when It Is So Late item is collected.
    {
        fatherTiredness = fatherTiredness + itIsSoLateOffset;
    }

    public void AlmostAsleepOffset() // Changes father's moral when Almost Asleep item is collected.
    {
        if (moral > almostAsleepOffset)
        {
            moral = moral - almostAsleepOffset;
        }
        else if ((moral < almostAsleepOffset) && (moral > 0))
        {
            moral = 0;
        }

    }

    public void CuteWhenAsleepOffset() // Changes father's moral when Cute When Asleep item is collected.
    {
        if (moral > cuteWhenAsleepOffset)
        {
            moral = moral - cuteWhenAsleepOffset;
        }
        else if ((moral < cuteWhenAsleepOffset) && (moral > 0))
        {
            moral = 0;
        }
    }

    public void SmileOffset() // Changes father's moral when Smile When Asleep item is collected.
    {
        if (moral > smileOffset)
        {
            moral = moral - smileOffset;
        }
        else if ((moral < smileOffset) && (moral > 0))
        {
            moral = 0;
        }

    }

    public void CryOffset() // Changes father's moral when Cry When Asleep item is collected.
    {
        moral = moral + cryOffset;
    }

    public void NoSleepOffset() // Changes father's moral when No Sleep When Asleep item is collected.
    {
        moral = moral + noSleepOffset;
    }

    public void ToysOffset() // Changes father's moral when Toys When Asleep item is collected.
    {
        moral = moral + toysOffset;
    }

    public void HappyOffset() // Changes father's guilt when Happy item is collected.
    {
        if (guilt > happyOffset)
        {
            guilt = guilt - happyOffset;
        }
        else if ((guilt < happyOffset) && (guilt > 0))
        {
            guilt = 0;
        }
    }

    public void HugOffset() // Changes father's guilt when Hug item is collected.
    {
        if (guilt > hugOffset)
        {
            guilt = guilt - hugOffset;
        }
        else if ((guilt < hugOffset) && (guilt > 0))
        {
            guilt = 0;
        }
    }

    public void MustBeEasierOffset() // Changes father's guilt when Must Be Easier item is collected.
    {
        guilt = guilt + mustBeEasierOffset;
    }

    public void TummyHurtsOffset() // Changes father's guilt when Tummy Hurts item is collected.
    {
        guilt = guilt + tummyHurtsOffset;
    }

    public void CalmMovementOffset() // Changes baby's tireness when Calm Movement item is collected.
    {
        babyTiredness = babyTiredness + calmMovementOffset;
    }

    public void CalmMusicOffset() // Changes baby's tireness when Calm Music item is collected.
    {
        babyTiredness = babyTiredness + calmMusicOffset;
    }

    public void LullabyOffset() // Changes baby's tireness when Lullaby item is collected.
    {
        babyTiredness = babyTiredness + lullabyOffset;
    }

    public void BarkingDogsOffset() // Changes baby's tireness when Barking Dogs item is collected.
    {
        if (babyTiredness > barkingDogsOffset)
        {
            babyTiredness = babyTiredness - barkingDogsOffset;
        }
        else if ((babyTiredness < barkingDogsOffset) && (babyTiredness > 0))
        {
            babyTiredness = 0;
        }
    }

    public void CarsOffset() // Changes baby's tireness when Cars item is collected.
    {
        if (babyTiredness > carsOffset)
        {
            babyTiredness = babyTiredness - carsOffset;
        }
        else if ((babyTiredness < carsOffset) && (babyTiredness > 0))
        {
            babyTiredness = 0;
        }
    }

    public void LoudMusicOffset() // Changes baby's tireness when Loud Music item is collected.
    {
        if (babyTiredness > loudMusicOffset)
        {
            babyTiredness = babyTiredness - loudMusicOffset;
        }
        else if ((babyTiredness < loudMusicOffset) && (babyTiredness > 0))
        {
            babyTiredness = 0;
        }
    }

    public void BurpOffset() // Changes baby's tireness and needs and father's tiredness when Burp item is collected.
    {
        babyTiredness = babyTiredness + burpBabyTirednessOffset;
        if (needs > burpNeedsOffset)
        {
            needs = needs - burpNeedsOffset;
        }
        else if ((needs < burpNeedsOffset) && (needs > 0))
        {
            needs = 0;
        }
        fatherTiredness = fatherTiredness + burpFatherTirednessOffset;
    }

    public void KuchKuchKuOffset() // Changes Father's moral and guilt and baby tiredness when KuchKuchKu item is collected.
    {
        if (guilt > kuchKuchKuGuiltOffset)
        {
            guilt = guilt - kuchKuchKuGuiltOffset;
        }
        else if ((guilt < kuchKuchKuGuiltOffset) && (guilt > 0))
        {
            guilt = 0;
        }
        if (moral > kuchKuchKuMoralOffset)
        {
            moral = moral - kuchKuchKuMoralOffset;
        }
        else if ((moral < kuchKuchKuMoralOffset) && (moral > 0))
        {
            moral = 0;
        }
        babyTiredness = babyTiredness - kuchKuchKubabyTirednessOffset;
    }

    public void LightsOnOffset() // Changes Father's tiredness and baby tiredness when Lights On item is collected.
    {
        if (fatherTiredness > lightsOnFatherTirednessOffset)
        {
            fatherTiredness = fatherTiredness - lightsOnFatherTirednessOffset;
        }
        else if ((fatherTiredness < lightsOnFatherTirednessOffset) && (fatherTiredness > 0))
        {
            fatherTiredness = 0;
        }
        if (babyTiredness > lightsOnbabyTirednessOffset)
        {
            babyTiredness = babyTiredness - lightsOnbabyTirednessOffset;
        }
        else if ((babyTiredness < lightsOnbabyTirednessOffset) && (babyTiredness > 0))
        {
            babyTiredness = 0;
        }
    }

    public void MomIsAsleepOffset() // Changes Father's tiredness and moral when Mom Is Asleep item is collected.
    {
        fatherTiredness = fatherTiredness + momIsAsleepFatherTirednessOffset;
        if (moral > momIsAsleepMoralOffset)
        {
            moral = moral - momIsAsleepMoralOffset;
        }
        else if ((moral < momIsAsleepMoralOffset) && (moral > 0))
        {
            moral = 0;
        }
    }

    public void SleepingTigerOffset() // Changes Father's tiredness and baby tiredness when Sleeping Tiger item is collected.
    {
        fatherTiredness = fatherTiredness + sleepingTigerFatherTirednessOffset;
        babyTiredness = babyTiredness + sleepingTigerBabyTirednessOffset;
    }

    public void WorkTomorrowOffset() // Changes Father's moral and guilt when Work Tomorrowp item is collected.
    {
        if (moral > workTomorrowMoralOffset)
        {
            moral = moral - workTomorrowMoralOffset;
        }
        else if ((moral < workTomorrowMoralOffset) && (moral > 0))
        {
            moral = 0;
        }
        guilt = guilt + workTomorrowGuiltOffset;
    }
#endregion
#region "Spwaners"
    IEnumerator SpawnObjects()
	{

		yieldTime = Random.Range (1, 5);
		yield return new WaitForSeconds (yieldTime);
        bool goodOrBad = false;
        while (goodOrBad == false) // decides if to spawn negative, positive or half and half object
        {
            int positiveOrNegative = Random.Range(1, 4);
            switch (positiveOrNegative)
            {
                case 1:
                    if (halfAndHalf < 5)
                    {
                       // bool spawned = false;

                       // while (spawned == false) // spawner itself
                       // {
                            Randomizer();
                            if ((Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZ), transform.forward, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZPlus), -transform.forward, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(spawnCheckerXPlus, 0.5f, randomZ), transform.right, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(spawnCheckerX, 0.5f, randomZ), -transform.right, 1.5f) == false))
                            {

                                int decider = Random.Range(1, 7); // decides which object to spawn


                                switch (decider)
                                {
                                    case 1:
                                        Instantiate(burp, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 2:
                                        Instantiate(kuchKuchKu, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;

                                    case 3:
                                        Instantiate(lightOn, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 4:
                                        Instantiate(momIsAsleep, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 5:
                                        Instantiate(sleepingTiger, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 6:
                                        Instantiate(workTomorrow, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;                                  
                                }
                            }
                            else
                            {
                                goodsInternalCounter = true;
                                yield break;
                            }
                       // }
                        //Debug.Log (spawned);
                       // spawned = false;
                        goodsInternalCounter = true;
                        goodOrBad = true;
                    }
                    break;
                case 2:
                    if (negatives < 5)
                    {
                       // bool spawned = false;

                      //  while (spawned == false)
                      //  {
                            Randomizer();
                            if ((Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZ), transform.forward, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZPlus), -transform.forward, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(spawnCheckerXPlus, 0.5f, randomZ), transform.right, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(spawnCheckerX, 0.5f, randomZ), -transform.right, 1.5f) == false))
                            {

                                int decider = Random.Range(1, 13);
                                switch (decider)
                                {
                                    case 1:
                                        Instantiate(attention, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;

                                    case 2:
                                        Instantiate(barkingDog, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;

                                    case 3:
                                        Instantiate(cars, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 4:
                                        Instantiate(cry, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 5:
                                        Instantiate(food, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                     //   spawned = true;
                                        break;

                                    case 6:
                                        Instantiate(itIsSoLate, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 7:
                                        Instantiate(loudMusic, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                     //   spawned = true;
                                        break;

                                    case 8:
                                        Instantiate(mustBeEasier, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 9:
                                        Instantiate(noSleep, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 10:
                                        Instantiate(poop, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                     //   spawned = true;
                                        break;

                                    case 11:
                                        Instantiate(toys, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                     //   spawned = true;
                                        break;

                                    case 12:
                                        Instantiate(tummyHurts, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                     //   spawned = true;
                                        break;
                                }
                            }
                            else
                            {
                                goodsInternalCounter = true; // tells script spawnning action was unsucessful but complete and another can start
                                yield break;
                            }
                      //  }
                        //Debug.Log (spawned);
                        //spawned = false; // ends spawner while loop
                        goodsInternalCounter = true; // tels script spawnning action is complete and another can start
                        goodOrBad = true; // ends primary while loop
                    }
                    break;
                case 3:
                    if (positives < 5)
                    {
                       // bool spawned = false;

                      //  while (spawned == false)
                      //  {
                            Randomizer();
                            if ((Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZ), transform.forward, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZPlus), -transform.forward, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(spawnCheckerXPlus, 0.5f, randomZ), transform.right, 1.5f) == false)
                                          && (Physics.Raycast(new Vector3(spawnCheckerX, 0.5f, randomZ), -transform.right, 1.5f) == false))
                            {

                                int decider = Random.Range(1, 12);


                                switch (decider)
                                {
                                    case 1:
                                        Instantiate(coffee, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;

                                    case 2:
                                        Instantiate(awakeAnyway, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 3:
                                        Instantiate(almostAsleep, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;

                                   // case 4:
                                    //    Instantiate(bottle, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                    //    break;

                                    case 4:
                                        Instantiate(calmMovement, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 5:
                                        Instantiate(calmMusic, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;

                                    case 6:
                                        Instantiate(cuteWhenAsleep, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 7:
                                        Instantiate(happy, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 8:
                                        Instantiate(hug, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                        break;

                                    case 9:
                                        Instantiate(lullaby, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;

                                    //case 10:
                                       // Instantiate(pacifier, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                      //  spawned = true;
                                       // break;

                                    case 10:
                                        Instantiate(smile, spawner, Quaternion.identity);
                                        //Debug.Log ("Spawned");
                                       // spawned = true;
                                        break;
                                }


                            }
                            else
                            {
                                goodsInternalCounter = true;
                                yield break;
                            }

                     //   }
                        //Debug.Log (spawned);
                       // spawned = false;
                        goodsInternalCounter = true;
                        goodOrBad = true;
                    }
                    break;
            }
        }
		
	}


    public void FrnitureSpawner()
    {
        int wallDecider = Random.Range(1, 5); //Determines on which wall the changing table will be placed
        switch (wallDecider)
        {
            case 1: //instantiate on the "east" wall
               Instantiate(changingTable, new Vector3(4.8f, 0.5f, Random.Range(-8f,7.4f)), Quaternion.identity);
                break;
            case 2: //instantiate on the "west" wall
                Instantiate(changingTable, new Vector3(-4.8f, 0.5f, Random.Range(-8.0f, 7.4f)), Quaternion.identity);
                break;
            case 3: //instantiate on the "north" wall
                Transform changinTableTransform =  Instantiate (changingTable, new Vector3(Random.Range(-4.8f, 5.0f), 0.5f, 8.0f), Quaternion.identity) as Transform;
                changinTableObject = changinTableTransform.gameObject;
                changinTableObject.transform.rotation = changinTableObject.transform.rotation * Quaternion.Euler(0, 90, 0);
                break;
            case 4: //instantiate on the "south" wall
                changinTableTransform = Instantiate(changingTable, new Vector3(Random.Range(-4.8f, 5.0f), 0.5f, -8.8f), Quaternion.identity) as Transform;
                changinTableObject = changinTableTransform.gameObject;
                changinTableObject.transform.rotation = changinTableObject.transform.rotation * Quaternion.Euler(0, 270, 0);
                break;

        }

        for (int i = 0; i < 2; i++)
            {
                bool spawned = false;
                while (spawned == false)
                {
                    Randomizer();
                    if ((Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZ), transform.forward, 2.0f) == false)
                        && (Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZPlus), -transform.forward, 2.0f) == false)
                        && (Physics.Raycast(new Vector3(spawnCheckerXPlus, 0.5f, randomZ), transform.right, 2.0f) == false)
                        && (Physics.Raycast(new Vector3(spawnCheckerX, 0.5f, randomZ), -transform.right, 2.0f) == false))
                    {
                        switch (i)
                        {
                            case 0:
                                Instantiate(bed, spawner, Quaternion.identity);
                                spawned = true;
                                break;

                           // case 1:
                               // Instantiate(changingTable, spawner, Quaternion.identity);
                              //  spawned = true;
                             //   break;

                            case 1:
                                Instantiate(chair, spawner, Quaternion.identity);
                                spawned = true;
                                break;
 
                            default:
                                spawned = true;
                                break;
                        }
                    }
                }
            }
        }
#endregion

#region "Randomizers"
    public void Randomizer()
	{
	    randomX = Random.Range(-5.0f,5.0f);
		randomZ = Random.Range(-8.0f,8.0f);
		spawnCheckerX = (randomX - 1.5f);
		spawnCheckerZ = (randomZ - 1.5f);
		spawnCheckerXPlus = (randomX + 1.5f);
		spawnCheckerZPlus = (randomZ + 1.5f);
		spawner = new Vector3 (randomX, 0.5f, randomZ);
		 		
				
	}

    public void RandomNeedsDecider()
    {
        int randomess = Random.Range(1, 4);
        switch (randomess)
        {
            case 1:
                needs = 0;
                break;
            case 2:
                needs = Random.Range(10, 51);
                break;
            case 3:
                needs = Random.Range(51, 76);
                break;
        }
    }

#endregion

#region "Game Managers"
    public void FatherTierdnedssMenager() // Maneges Father's tiredness bar
    {
            Walker walker = player.GetComponent<Walker>();
            if (walker.tempo == 0)
            {
                fatherTiredness = fatherTiredness + fatherTirednessOffset * Time.deltaTime;
            }
            else
            {
                fatherTiredness = fatherTiredness +0.5f * Time.deltaTime; //if player is walking, he is lees tired
            }
            int fatherTirednessInt = (int)fatherTiredness;
            fatherTirednessText.text = "Tiredness " + fatherTirednessInt.ToString();

  
    }


    public void MoralMenager()
    {
        
        moral = moral + moralOffset * Time.deltaTime  ; //moral rises (worsen) as time goes by
        int moralInt = (int)moral;
        moralText.text = "Morale " + moralInt.ToString();
    }

    public void GuiltMenager()
    {
       // if (needs < 10)
      //  {
            guilt = needs;
      //  }
        if ((needs > 10) && (needs < 20)) // guilt is proportional to needs. The higher the needs bar, the faster the guilt bar is rising.
        {
            guilt = needs + 0.2f * Time.deltaTime;
        }
        else if ((needs > 20) && (needs < 40)) 
        {
            guilt = needs + 1.5f * Time.deltaTime ;
        }
        else if ((needs > 40) && (needs < 60))
        {
            guilt = guilt + 2.0f * Time.deltaTime;
        }
        else if ((needs > 60) && (needs < 80))
        {
            guilt = needs + 2.5f * Time.deltaTime;
        }
        else if ((needs > 80) && (needs < 90))
        {
            guilt = needs + 3.0f * Time.deltaTime;
        }
        else if (needs > 90)
        {
            guilt = needs + 5.0f * Time.deltaTime;
        }
        int guiltInt = (int)guilt;
        guiltText.text = "Guilt " + guiltInt.ToString();

    }

    public void BabyTierdnedssMenager()
    {
        Walker walker = player.GetComponent<Walker>();
        if (walker.tempoSleep > 0)
        {
            babyTiredness = (babyTiredness) + (walker.tempoSleep * babyTirednessOffset) ; // when player is walking, the baby is becoming more tired faster.
        }
        else
        {
            babyTiredness = babyTiredness + (float)0.01;
        }
        int babyTirednessInt = (int)babyTiredness;        
        babyTirednessText.text = "Sleepiness " + babyTirednessInt.ToString();

    }

    public void NeedsMenager() // maneges the needs
    {        
        NeedsRandomizer(); // starts by randomizing a need
        NeedsChecker(); // checks if there is a need
        if (areThereNeeds == true)
        {
            needs = needs + 1.0f * Time.deltaTime;
        }
        else
        {
            needs = needs + (float)0.1 * Time.deltaTime;
        }
        NeedsGUI(); // updates the GUI
    }

    public void NeedsDecider() // randomize the baby's needs
    {
        int needsDecider = Random.Range(1,4);
       // bool needsSpawner;
        switch (needsDecider)
        {
            case 1:
                if (needAttention == false)
                {
                    needAttention = true;
                    //needsSpawner = false;
                        //while (needsSpawner == false)
                            Randomizer();
                            //if ((Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZ), transform.forward, 1.0f) == false)
                              //  && (Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZPlus), -transform.forward, 1.0f) == false)
                              //  && (Physics.Raycast(new Vector3(spawnCheckerXPlus, 0.5f, randomZ), transform.right, 1.0f) == false)
                              //  && (Physics.Raycast(new Vector3(spawnCheckerX, 0.5f, randomZ), -transform.right, 1.0f) == false))
                        //{
                            Instantiate(pacifier, spawner, Quaternion.identity);
                           // needsSpawner = true;
                       // }
                }
                else
                {
                    needs = needs + needsOffset;
                }
            break;
            case 2:
                if (needFood == false)
                {
                    needFood = true;
                    //needsSpawner = false;
                   // while (needsSpawner == false)
                        Randomizer();
                      //  if ((Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZ), transform.forward, 1.0f) == false)
                       //     && (Physics.Raycast(new Vector3(randomX, 0.5f, spawnCheckerZPlus), -transform.forward, 1.0f) == false)
                       //     && (Physics.Raycast(new Vector3(spawnCheckerXPlus, 0.5f, randomZ), transform.right, 1.0f) == false)
                       //     && (Physics.Raycast(new Vector3(spawnCheckerX, 0.5f, randomZ), -transform.right, 1.0f) == false))
                      //  {
                            Instantiate(bottle, spawner, Quaternion.identity); 
                           // needsSpawner = true;
                       // }
                }
                else
                {
                    needs = needs + needsOffset;
                }
                break;
            case 3:
                if (needPoop == false)
                {
                    needPoop = true;
                }
                else
                {
                    needs = needs + needsOffset;
                }
                break;
    
        }
        
    }

    public void NeedsRandomizer() // randomizes the baby's needs
    {
        float timeChecker = Time.timeSinceLevelLoad;
        if (timeChecker > prevTimeChecker + 1.0f)
        {
            int needsRandomizer;
            if ((needs > 20) && (needs < 60))
            {
                needsRandomizer = Random.Range(1, 101);
                if (needsRandomizer > 95)
                {
                    NeedsDecider();
                }
            }
            else if ((needs > 60) && (needs < 80))
            {
                needsRandomizer = Random.Range(1, 101);
                if (needsRandomizer > 80)
                {
                    NeedsDecider();
                }
            }
            else if (needs > 80)
            {
                needsRandomizer = Random.Range(1, 101);
                if (needsRandomizer > 50)
                {
                    NeedsDecider();
                }
            }
            prevTimeChecker = timeChecker;
        }
        else
        {
            return;
        }
    }

    public void NeedsChecker()
    {
        if ((needFood == true) || (needPoop == true) || (needAttention == true))
        {
            areThereNeeds = true;
        }
        else if ((needFood == false) && (needPoop == false) && (needAttention == false))
        {
            areThereNeeds = false;
        }
    }

    public void NeedsGUI()
    {
        int needsInt = (int)needs;
        needsText.text = "Needs: " + needsInt.ToString();
        if (needFood == true)
        { 
            needsText.text = needsText.text + " Food";
            Debug.Log("Need Food " + needFood);
        }
        if (needPoop == true)
        { 
            needsText.text = needsText.text + " Poop";
            Debug.Log("Need Poop " + needPoop);
        }
        if (needAttention == true)
        {
            needsText.text = needsText.text + " Attention";
            Debug.Log("Need Attention " + needAttention);
        }
    }

    void TimeManeger() // Keeps track of game time
    {
        
            time = (int)Time.timeSinceLevelLoad;
            timeText.text = "Time: " + time.ToString();
        
    }

    void EndGame()
    {                
        
        Walker walker = player.GetComponent<Walker>();
        if (babyTiredness > 99)
        {
            
            running = false;         
            walker.enabled = !walker.enabled;
            endingText.text = ("Baby is asleep, you can go to sleep now. Tap with one finger to continue");
            if (Input.touchCount == 1)
            {
                ScoreManager.instance.CalculateScore();
                DontDestroyOnLoad(ScoreManager.instance);
                Application.LoadLevel("score");
            }
            /*else if (Input.touchCount == 3)
            {
                Debug.Log("Quit");
                Application.Quit();
            }*/

        }

        if (moral > 99)
        {
            
            running = false;
            walker.enabled = !walker.enabled;
            endingText.text = ("You are depressed, you lost. Tap with one finger to continue");
            if (Input.touchCount == 1)
            {
                ScoreManager.instance.CalculateScore();
                DontDestroyOnLoad(ScoreManager.instance);
                Application.LoadLevel("score");
            }
            /*else if (Input.touchCount == 3)
            {
                Debug.Log("Quit");
                Application.Quit();
            }*/

        }

        if (fatherTiredness > 99)
        {
            
            running = false;
            walker.enabled = !walker.enabled;
            endingText.text = ("You fell asleep.  Tap with one finger to continue");
            if (Input.touchCount == 1)
            {
                ScoreManager.instance.CalculateScore();
                DontDestroyOnLoad(ScoreManager.instance);
                Application.LoadLevel("score");
            }
           /* else if (Input.touchCount == 3)
            {
                Debug.Log("Quit");
                Application.Quit();
            }*/

        }
        
    }

   /* void CalculateScore()
    {
        prevFatherTiredness = (int) fatherTiredness;
        prevBabyTiredness = (int) babyTiredness;
        prevMoral = (int) moral;
        prevGuilt = (int) guilt;
        prevNeeds = (int) needs;
        score = score + ((100 - prevNeeds) - prevGuilt) + (100 - prevMoral) + (100 - prevFatherTiredness);
        score = (int)ScoreManager.instance.score;
    }

    public void CalculateNewStats() // Calculate new Stats for level 2 and upwords.
    {
        
        if (gameLevel > 1)
        {
            fatherTiredness = prevFatherTiredness - Random.Range(10,prevFatherTiredness);
            babyTiredness = prevBabyTiredness - Random.Range(10,prevBabyTiredness);
            if (prevMoral >= (prevFatherTiredness - fatherTiredness))
            {
                moral = prevMoral - (prevFatherTiredness - fatherTiredness);
            }
            
            guilt = 100 - prevNeeds;
            gameLevel++;
        }
        else
        {
            fatherTiredness = 0;
            babyTiredness = 0;
            moral = 0;
            guilt = 0;
            gameLevel++;
        }
}*/

#endregion		
}
//public void SpawnGoods()
//{
	
	
//	bool spawned = false;
	
//	while (spawned == false)
//	{
	//	Randomizer();
	//	if ((Physics.Raycast (new Vector3 (spawnCheckerX, 0.5f, spawnCheckerZ),new Vector3 (randomX, 0.5f, randomZ), 1.0f) == false) 
		//    && (Physics.Raycast (new Vector3 (spawnCheckerXPlus, 0.5f, spawnCheckerZPlus),new Vector3 (randomX, 0.5f, randomZ), 1.0f) == false) 
		//    && (Physics.Raycast (new Vector3 (spawnCheckerXPlus, 0.5f, spawnCheckerZ),new Vector3 (randomX, 0.5f, randomZ), 1.0f) == false) 
		//    && (Physics.Raycast (new Vector3 (spawnCheckerX, 0.5f, spawnCheckerZPlus),new Vector3 (randomX, 0.5f, randomZ), 1.0f) == false))
	//	{
			
		//	Instantiate(coffee, spawner, Quaternion.identity);
			//Debug.Log ("Spawned");
		//	spawned = true;
			
	//	}
	//	else 
	//	{
		//	return;
	//	}
		
//	}
	//Debug.Log (spawned);
	//spawned = false;
//}