using UnityEngine;
using System.Collections;

public class NoSleep : MonoBehaviour {

	void Start () {
		Game.instance.RegisterGoods(); //registers object in Game
		Game.instance.RegisterNegatives(); // Registers object as Negetive;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider coll)
	{
		Debug.Log (coll.gameObject);
		if (coll.gameObject.tag == "Player") 
		{
			Game.instance.UnregisterGoods (); //Unregisters object before destruction
			Game.instance.UnregisterNegatives();
            Game.instance.NoSleepOffset();
			Destroy (this.gameObject); //destroy the object
			
		}
	}
}
