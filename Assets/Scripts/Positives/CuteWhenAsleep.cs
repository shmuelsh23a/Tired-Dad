using UnityEngine;
using System.Collections;

public class CuteWhenAsleep : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Game.instance.RegisterGoods();
		Game.instance.RegisterPositives();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider coll)
	{
		Debug.Log (coll.gameObject);
		if (coll.gameObject.tag == "Player") 
		{
			Game.instance.UnregisterGoods ();
			Game.instance.UnregisterPositives();
            Game.instance.CuteWhenAsleepOffset();
			Destroy (this.gameObject);
			
		}
	}
}