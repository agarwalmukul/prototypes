using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum stripColor{
	GREEN, RED, BLUE, YELLOW
};
	
public class InteractionManager: MonoBehaviour {

	private static InteractionManager _instance;
	public static InteractionManager Instance { 
		get{
			if (_instance == null) {
				GameObject go = new GameObject ();
				go.AddComponent<InteractionManager> ();
			}
			return _instance;
		}
	}

	public delegate void Interaction(stripColor value);
	public event Interaction onInteraction;

	void Awake(){
		_instance = this;
	}

	public void interactionStateActivated(stripColor value){
		
		onInteraction (value);
	}
}
