using UnityEngine;
using System.Collections;

public class WorkTomorrow : MonoBehaviour
{

    void Start()
    {
        Game.instance.RegisterGoods(); //registers object in Game
        Game.instance.RegisterHalfAndHalf(); // Registers object as Half and Half;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider coll) //to detect impact with player
    {
        Debug.Log(coll.gameObject);
        if (coll.gameObject.tag == "Player")
        {
            Game.instance.UnregisterGoods(); //Unregisters object before destruction
            Game.instance.UnregisterHalfandHalf();
            Game.instance.WorkTomorrowOffset();
            Destroy(this.gameObject); //destroy the object

        }
    }
}
