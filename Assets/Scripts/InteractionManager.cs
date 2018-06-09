using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum stripColor{
	Green, Red, Blue, Yellow
};
	
public static class InteractionManager {



	public static void interactionStateActivated(stripColor value){
		Debug.Log (value);
	}
}
