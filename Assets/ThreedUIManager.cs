using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreedUIManager : MonoBehaviour {

	public GameObject interactionCursor;
	public GameObject rightHandGo;
	private Animator interactionCursorAnimator;

	//private TrailRenderer cursorTrailTrail;
	public GameObject cursorTrailPrefab;
	//private GameObject trail;

	private bool _isRightHandPinch;

	public bool IsRightHandPinch {
		get {
			return _isRightHandPinch;
		}
		set {
			_isRightHandPinch = value;
		}
	}

	private bool _isLeftHandPinch;

	public bool IsLeftHandPinch {
		get {
			return _isLeftHandPinch;
		}
		set {
			_isLeftHandPinch = value;
		}
	}

	// Use this for initialization
	void Start () {
		interactionCursorAnimator = interactionCursor.transform.GetComponent<Animator> ();
		//interactionCursorTrail = interactionCursor.transform.GetComponent<TrailRenderer> ();
		//trail = (GameObject) Instantiate(cursorTrailPrefab, interactionCursor.transform);
		//cursorTrailTrail = trail.transform.GetComponent<TrailRenderer> ();
		//cursorTrailTrail.enabled = false;
		Instantiate(cursorTrailPrefab, interactionCursor.transform);
		interactionCursor.transform.GetComponentInChildren<TrailRenderer> ().enabled = false;
		//Debug.Log (interactionCursor.transform.GetComponentInChildren<TrailRenderer> ().enabled);
	}
	
	// Update is called once per frame
	void Update () {
		/**
		if (IsRightHandPinch && !interactionCursor.activeSelf) {
			interactionCursor.SetActive (true);
			interactionCursorAnimator.SetBool ("expandIn", true);
		} else if (!IsRightHandPinch && interactionCursor.activeSelf) {
			interactionCursor.SetActive (false);
			interactionCursorAnimator.SetBool ("expandIn", false);
		}
		**/
		//Debug.Log
		if (rightHandGo.activeSelf && interactionCursor.transform.childCount == 0) {
			//trail = Instantiate<GameObject>(cursorTrailPrefab, interactionCursor.transform);
			Instantiate<GameObject>(cursorTrailPrefab, interactionCursor.transform);
			//cursorTrailTrail = trail.transform.GetComponent<TrailRenderer> ();
			interactionCursor.transform.GetComponentInChildren<TrailRenderer> ().enabled = false;
			//interactionCursorTrail.enabled = false;
			//Debug.Log("prefab spawned");
		}

		if (IsRightHandPinch) {
			interactionCursorAnimator.SetBool ("expandIn", true);
			// because of the delay in the animation frame and the hand moving, the interaction cursor changes its position
			interactionCursor.transform.localPosition = new Vector3 (0, 0, 0);
			//cursorTrailTrail.enabled = true;
			interactionCursor.transform.GetComponentInChildren<TrailRenderer> ().enabled = true;
			//Debug.Log ( " sda" + interactionCursor.transform.GetComponentInChildren<TrailRenderer> ());
			//interactionCursorTrail.enabled = true;
		} else if (!IsRightHandPinch) {
			interactionCursorAnimator.SetBool ("expandIn", false);
			//cursorTrailTrail.enabled = false;
			interactionCursor.transform.GetComponentInChildren<TrailRenderer> ().enabled = false;
			//interactionCursorTrail.enabled = false;
		}
		//Debug.Log (interactionCursor.transform.GetComponentInChildren<TrailRenderer> ().enabled);
	}

	public void OnPinchDetectionRightHand(bool value){
		//Debug.Log ("pinch detected right hand" + value);
		IsRightHandPinch = value;
	}
	public void OnPinchDetectionLeftHand(bool value){
		//Debug.Log ("pinch detected left hand" + value);
		IsLeftHandPinch = value;
	}
}
