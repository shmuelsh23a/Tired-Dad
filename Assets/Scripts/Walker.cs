using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour
{
    //public float speed;
    public Transform player;
    public enum tags { Player, Positive, Negative };
    public int tempo = 0;
    public float tempoSleep = 0;
    public CharacterController controller;
    float prevTouchBeganTime = 0;
    float prevTouchEndedTime = 0;
    float prevTouchBeganTime1 = 0;
    float prevTouchEndedTime1 = 0;
    //float intervalEndOne = 0;
    //float intervalEndTwo = 0;
    //Vector3 stright = new Vector3 (0, 0, 1);
    //private Touch previousTouch;
    Vector2 firstTouchPrevPos = new Vector2();
    Vector2 firstTouchEndPos = new Vector2();
    float touchBeganTime;
    float touchEndedTime;
    Vector2 xPosition = new Vector2();
    public Touch firstTouch;
    //bool changing = false;
    bool needPoop;

    // Use this for initialization
    void Start()
    {
        //		firstTouchBeganTime = Time.timeSinceLevelLoad ;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount == 1)
        { //Check if there is input 
            firstTouch = Input.GetTouch(0);
            //			Debug.Log ("st" + firstTouch.fingerId);

            if (firstTouch.phase == TouchPhase.Began)
            {  //takes position of touch in begining of stage to see if there was a swipe
                firstTouchPrevPos = firstTouch.position; //Debug.Log ("begun" + firstTouch.position);
                touchBeganTime = Time.timeSinceLevelLoad;//Debug.Log (touchBeganTime);
                prevTouchBeganTime = prevTouchBeganTime1;

            }


            if (firstTouch.phase == TouchPhase.Ended)
            { // takes position of touch at the end of its stage to see if there was a swipe
                firstTouchEndPos = firstTouch.position; //Debug.Log ("ended" + firstTouch.position);
                touchEndedTime = Time.timeSinceLevelLoad; //Debug.Log (touchEndedTime);
                prevTouchEndedTime = prevTouchEndedTime1;
                //}

                if ((firstTouch.phase == TouchPhase.Ended)
                    && (((touchBeganTime - prevTouchBeganTime) > 0.5f) && (touchBeganTime - prevTouchBeganTime) < 2))
                {
                    //&& (((touchEndedTime - prevTouchEndedTime) > 0.5f) && (touchEndedTime - prevTouchEndedTime) < 2)){ //moves player if tempo is above 0
                    if (tempo < 5)
                    {
                        tempo++;
                    }
                    if (tempoSleep < 1)
                    {
                       tempoSleep = tempoSleep + (float) 0.01;
                    }
                    //Debug.Log ("tempo = " + tempo);
                }
                else if (((touchBeganTime - prevTouchBeganTime) < 0.5f) && ((touchBeganTime - prevTouchBeganTime) > 2)) // reduces tempo if touch isn't continuous 
                {
                    if (tempo > 0)
                    {
                        tempo--;
                    }
                    if (tempoSleep > 0)
                    {
                        tempoSleep = tempoSleep - (float) 0.1;
                    }

                    //Debug.Log ("tempo = " + tempo);
                }
            }
            prevTouchBeganTime1 = touchBeganTime; //Debug.Log ("firstTouchPrevPos" + prevTouchBeganTime + " current " + touchBeganTime);
            prevTouchEndedTime1 = touchEndedTime;
        }
        else // if there was no touch for 2 seconds, reduces tempo.
        {
            touchBeganTime = Time.timeSinceLevelLoad;
            touchEndedTime = Time.timeSinceLevelLoad;
            if (((touchBeganTime - prevTouchBeganTime) > 2))
            {
                if (tempo > 0)
                {
                    tempo--;
                }
                if (tempoSleep > 0)
                {
                    tempoSleep = tempoSleep - (float)0.1; ;
                }
                //Debug.Log ("tempo = " + tempo);
            }
        }

        //Debug.Log("en" + firstTouch.fingerId);
        xPosition = firstTouchEndPos - firstTouchPrevPos; // measures the vector of the swipe
        if ((xPosition.x > 75.0f) && (firstTouch.phase == TouchPhase.Ended))
        {
            controller.transform.rotation = controller.transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0)); // rotates player 90 degrees
            ZeroVectors();

        }

        else if ((xPosition.x < -75.0f) && (firstTouch.phase == TouchPhase.Ended))
        {
            controller.transform.rotation = controller.transform.rotation * Quaternion.Euler(new Vector3(0, -90, 0));
            ZeroVectors();
        }


        if (tempo > 0) //&& (firstTouch.phase == TouchPhase.Ended)) //moves the player
        {
            controller.SimpleMove(transform.forward);

        }



    }




    void ZeroVectors() //zeeros stuff up for next time.
    {
        xPosition = Vector3.zero;
        firstTouchEndPos = Vector3.zero;
        firstTouchPrevPos = Vector3.zero;

    }

   // IEnumerator OnControllerColliderHit(ControllerColliderHit coll)
   // {


     //   if (coll.gameObject.tag == "ChangingTable")
     //   {
      //      needPoop = Game.instance.GetComponent<Game>().needPoop; // checks if the baby needs poop
       //     if (needPoop == true) // if needs poop, starts changing
       //     {
          //      tempo = 0;
           //     yield return new WaitForSeconds(5);
          //      Game.instance.ChangedDaiper();     
          //      tempo = 1;
           // }
         //   else
           // {
              //  yield break;
         //   }
     //   }


  //  }
}	





			//Touch secondTouch = Input.GetTouch (1);
			//Vector3 rightRotator = new Vector3 (0, 90, 0);
			//Vector3 leftRotator = new Vector3 (0, -90, 0);


				//Vector2 firstTouchPrevPos = (firstTouch.position - firstTouch.deltaPosition);
				//yPosition = firstTouch.position - firstTouchPrevPos;




			//if (


			//	tempo ++;
				//Debug.Log (tempo);
		//	} 

			//else*/ 
			//if ((firstTouch.deltaTime != previousTouch.deltaTime) && (tempo > 0))
			//{ 
			//	tempo --;
				//Debug.Log (tempo);
			//}

			//if (tempo > 0)
			//{
			//	player.transform.position = player.transform.position + stright;

			//}


			
//for (int i = 0; i < Input.touches.Length; i++)
//{
//Debug.Log(Input.touches.Length);
//if (firstTouch.phase == TouchPhase.Began)
//{

//}
//}

