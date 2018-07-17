using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectSpawner : MonoBehaviour {

	public GameObject cube;

	// Use this for initialization
	void Start () {
		
	}

	public void spawnCube(){
		//GameObject clone = cube;
		Object.Instantiate(cube);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
