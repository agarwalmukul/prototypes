using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum stripColor{
	GREEN, RED, BLUE, YELLOW
};
	
public class InteractionManage: MonoBehaviour {

	private static InteractionManage _instance;
	public static InteractionManage Instance { 
		get{
			if (_instance == null) {
				GameObject go = new GameObject ();
				go.AddComponent<InteractionManage> ();
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
