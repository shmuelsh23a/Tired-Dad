using UnityEngine;
using System.Collections;

public class ChangingTable : MonoBehaviour
{

    bool needPoop;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnTriggerEnter(Collider coll)
    {
      
        
        if (coll.gameObject.tag == "Player")
        {
            needPoop = Game.instance.GetComponent<Game>().needPoop; // checks if the baby needs poop
            if (needPoop == true) // if needs poop, starts changing
            {
                Walker walker = coll.gameObject.GetComponent<Walker>();
                walker.enabled = !walker.enabled;
                yield return new WaitForSeconds(3);
                Game.instance.ChangedDaiper();
                walker.enabled = true;
                walker.controller.transform.rotation = walker.controller.transform.rotation * Quaternion.Euler(new Vector3(0, -180, 0));
                walker.tempo = 1;    
            }
            else
            {
                yield break;
            }
        }
    }
}
