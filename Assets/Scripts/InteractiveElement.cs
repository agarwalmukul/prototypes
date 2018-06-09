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

	// manage the hover state of the cuboid button
	void clickState(bool value){
		Renderer rend = GetComponent<Renderer> ();
		if (value) {
			rend.material.shader = Shader.Find ("Custom/ToyCubeOutline");
		} 
		else {
			rend.material.shader = Shader.Find ("Standard");
		}
	}

	void OnTriggerEnter(){
		//onInteraction (colorValue);
		//InteractionManager.Instance.state++;
		clickState(true);
		InteractionManager.Instance.interactionStateActivated(colorValue);

	}

	void OnTriggerExit(){
		clickState (false);
	}
}
