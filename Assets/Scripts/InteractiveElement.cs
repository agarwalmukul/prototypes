using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveElement : MonoBehaviour {

	public stripColor colorValue;


	//public delegate void Interaction(stripColor value);
	//public event Interaction onInteraction;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(){
		//onInteraction (colorValue);
		InteractionManager.Instance.state++;
		InteractionManager.Instance.interactionStateActivated(colorValue);

	}
}
