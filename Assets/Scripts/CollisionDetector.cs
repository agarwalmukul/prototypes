using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour {

	private static GameObject _collidingBodyPart;

	public static GameObject CollidingBodyPart {
		get{ 
			return _collidingBodyPart;
		}
		private set { 
			_collidingBodyPart = value;
		}
	}

	void OnTriggerEnter(Collider col){
		CollidingBodyPart = col.gameObject;
	}
	void OnTriggerExit (Collider col){
		CollidingBodyPart = null;
	}
}
