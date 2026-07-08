using UnityEngine;
using System.Collections;

public class Toys : MonoBehaviour {

	void Start () {
		Game.instance.RegisterGoods();
		Game.instance.RegisterNegatives();
		
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
			Game.instance.UnregisterNegatives();
            Game.instance.ToysOffset();
			Destroy (this.gameObject);
			
		}
	}
}
