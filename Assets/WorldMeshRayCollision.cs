using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMeshRayCollision : MonoBehaviour {

	public GameObject leftEye;

	void Awake () {
		GetComponent<Camera> ().depthTextureMode = DepthTextureMode.Depth;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = leftEye.transform.position;
		this.transform.rotation = leftEye.transform.rotation;
	}
}
