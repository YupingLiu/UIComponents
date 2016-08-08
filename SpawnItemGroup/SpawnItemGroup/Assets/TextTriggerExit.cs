using UnityEngine;
using System.Collections;

public class TextTriggerExit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if ( null != other.gameObject )
        {
            Debug.Log("OnTriggerEnter");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ( null != other.gameObject )
        {
            Debug.Log("OnTriggerExit");
        }
    }


}
