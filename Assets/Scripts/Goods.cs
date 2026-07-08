using UnityEngine;
using System.Collections;

public class Goods : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Game.instance.RegisterGoods();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnControllerColliderHit(ControllerColliderHit coll)
	{
		//Debug.Log (coll.gameObject);
		if (coll.gameObject.tag == "Player") 
		{
			Game.instance.UnregisterGoods ();
			Destroy (this.gameObject);

		}
	}
}
