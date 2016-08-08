using UnityEngine;
using System.Collections;

public class FXCombiner : MonoBehaviour {

    public Camera targetCamera;
    public ParticleSystem effect;
    public GameObject prop;
    public float radius;

	// Use this for initialization
	void Start () {
        if (effect == null) effect = gameObject.GetComponentInChildren<ParticleSystem>();
        if (GetComponent<Camera>() == null) targetCamera = Camera.main;
        if (prop == null) prop = this.gameObject;
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 cameraVec = (prop.transform.position - targetCamera.transform.position);

        effect.transform.position = Vector3.Lerp(prop.transform.position, targetCamera.transform.position, radius / cameraVec.magnitude); ;



    }
}
