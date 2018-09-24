using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compassUI : MonoBehaviour {

	private static compassUI _instance;
	public static compassUI Instance{
		get
		{
			return _instance;
		}
	}

	public GameObject compassOutline;

	void Awake(){
		_instance = this;
	}

	public delegate void onCompassClicked();
	public static event onCompassClicked clickComplete;

	void OnTriggerEnter(){
		clickComplete ();
		compassOutline.SetActive(true);
	}

	void OnTriggerExit(){
		compassOutline.SetActive(false);
	}

	/*
	//mapNavigateGestures del = new mapNavigateGestures();
	// Use this for initialization

	private delegate void fingerHover();
	//private fingerHover handle;
	private event fingerHover onComplete;

	void Start () {
		onComplete = mapNavigateGestures.OnCompassFingerEnter;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(){
		//mapNavigateGestures.onReceivingSignal ();
		//handle();
		onComplete();
	}
	*/
}
