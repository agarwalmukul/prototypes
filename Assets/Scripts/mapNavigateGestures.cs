﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine.EventSystems;
using System;

public class mapNavigateGestures : MonoBehaviour {

	[SerializeField]
	[Range(.1f, 10f)]
	public float _panSpeed = 1.0f;

	[SerializeField]
	float _zoomSpeed = 0.25f;

	[SerializeField]
	float _rotationSpeed = 1.0f;

	[SerializeField]
	AbstractMap _mapManager;

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
	public GameObject parentMapGo;
	public GameObject rightHandPalm;
	public GameObject leftHandPalm;

	private Vector3 prevHandCenterPosition;
	private Vector3 currentHandCenterPosition;
	private float prevHandDistance;
	private float currentHandDistance;
	private float prevHandDiffRotation;
	private float currentHandDiffRotation;

	// this is true only when the pinch in both the hands has started. similar to buttonDown frame. only exists true for one frame. this is done to reset the prev... variables from
	// last time the pinch gesture was true.
	private bool manipulationFrame = false;

	private bool _isInitialized = false;

	void Awake(){
		_mapManager.OnInitialized += () =>
		{
			_isInitialized = true;
		};
	}


	// Use this for initialization
	void Start () {
		
	}
	/* prototype 1 functions		
	private Vector3 calcHandCenterPosition(){
		return ((rightHandPalm.transform.position + leftHandPalm.transform.position) * 0.5f);
	}
	private float calcHandDistance(){
		return ((rightHandPalm.transform.position - leftHandPalm.transform.position).magnitude);
	}
	private float calcHandDiffRotation(){
		Vector3 diff = rightHandPalm.transform.position - leftHandPalm.transform.position;
		Vector2 diffxz = diff.ToVector2xz ();
		diffxz.Normalize ();
		return (Mathf.Atan2 (diffxz.x, diffxz.y));
	}

	// Update is called once per frame
	void LateUpdate () {
		if (IsRightHandPinch && IsLeftHandPinch) {
			if (!manipulationFrame) {
				manipulationFrame = true;
				prevHandCenterPosition = calcHandCenterPosition ();
				currentHandCenterPosition = calcHandCenterPosition ();
				prevHandDistance = calcHandDistance ();
				currentHandDistance = calcHandDistance ();
				prevHandDiffRotation = calcHandDiffRotation ();
				currentHandDiffRotation = calcHandDiffRotation ();
			} else {
				// code to pan the map
				currentHandCenterPosition = calcHandCenterPosition ();
				Vector3 displacementCenterHand = 100.0f * (currentHandCenterPosition - prevHandCenterPosition);
				//Debug.Log(" " + displacementCenterHand.x + "," + displacementCenterHand.z);
				PanMapUsingHands (-displacementCenterHand.x, -displacementCenterHand.z);
				prevHandCenterPosition = currentHandCenterPosition;

				// code to zoom in/out the map
				currentHandDistance = calcHandDistance();
				float distanceDifferenceHands = ( currentHandDistance - prevHandDistance);
				ZoomMapUsingHands (distanceDifferenceHands);
				prevHandDistance = currentHandDistance;

				// code to rotate the map
				currentHandDiffRotation = calcHandDiffRotation();
				float rotationDifferenceHands = currentHandDiffRotation - prevHandDiffRotation;
				RotateMapUsingHands (rotationDifferenceHands);
				prevHandDiffRotation = currentHandDiffRotation;
			}
		} else {
			manipulationFrame = false;
		}
		// the map gameobject moves around on pan and zoom which makes it harder for the camera to see it. so I constrained its position
		this.transform.position = new Vector3(0, 0, 0);
	}

	*/


	private Vector3 calcHandCenterPosition(){
		return (rightHandPalm.transform.position);
	}
	private float calcHandDistance(){
		return (rightHandPalm.transform.position.y);
	}
	private float calcHandDiffRotation(){
		Vector3 diff = rightHandPalm.transform.rotation * Vector3.forward;
		//Vector3 diff = rightHandPalm.transform.position - leftHandPalm.transform.position;
		Vector2 diffxz = diff.ToVector2xz ();
		diffxz.Normalize ();
		return (Mathf.Atan2 (diffxz.x, diffxz.y));
	}

	// Update is called once per frame
	void LateUpdate () {
		if (IsRightHandPinch) {
			if (!manipulationFrame) {
				manipulationFrame = true;
				prevHandCenterPosition = calcHandCenterPosition ();
				currentHandCenterPosition = calcHandCenterPosition ();
				prevHandDistance = calcHandDistance ();
				currentHandDistance = calcHandDistance ();
				prevHandDiffRotation = calcHandDiffRotation ();
				currentHandDiffRotation = calcHandDiffRotation ();
			} else {
				// code to pan the map
				currentHandCenterPosition = calcHandCenterPosition ();
				Vector3 displacementCenterHand = 100.0f * (currentHandCenterPosition - prevHandCenterPosition);
				//Debug.Log(" " + displacementCenterHand.x + "," + displacementCenterHand.z);
				ConvertWorldToLocalAxis(displacementCenterHand);
				PanMapUsingHands (-displacementCenterHand.x, -displacementCenterHand.z);
				prevHandCenterPosition = currentHandCenterPosition;

				// code to zoom in/out the map
				currentHandDistance = calcHandDistance();
				float distanceDifferenceHands = ( currentHandDistance - prevHandDistance);
				ZoomMapUsingHands (distanceDifferenceHands);
				prevHandDistance = currentHandDistance;

				// code to rotate the map
				currentHandDiffRotation = calcHandDiffRotation();
				float rotationDifferenceHands = currentHandDiffRotation - prevHandDiffRotation;
				RotateMapUsingHands (rotationDifferenceHands);
				prevHandDiffRotation = currentHandDiffRotation;
			}
		} else {
			manipulationFrame = false;
		}
		// the map gameobject moves around on pan and zoom which makes it harder for the camera to see it. so I constrained its position
		//this.transform.position = new Vector3(0, 0, 0);
		this.transform.localPosition = new Vector3(-1.536775f, -446.4198f, -0.7188137f);
	}

	// input to the function is the world axes vector, output of the function is a vector converted into the local axes of the map
	private Vector3 ConvertWorldToLocalAxis(Vector3 vec){


		return vec;


		/*
		// x is horizontal axis, z is posterior anterior axis. if the local map is rotated along the y axis, then the xmove, zmove values are not aligned to the current
		// viewing angle and will make the move along the local xmove, zmove axis. we want the map to move in xmove, zmove according to the user. so let's do some math
		float localXMove;
		float localZMove;
		Vector2 move = new Vector2 (xMove, zMove);
		Vector2 localMove = new Vector2 (0, 0);
		*/

	}

	public void OnPinchDetectionRightHand(bool value){
		//Debug.Log ("pinch detected right hand" + value);
		IsRightHandPinch = value;
	}
	public void OnPinchDetectionLeftHand(bool value){
		//Debug.Log ("pinch detected left hand" + value);
		IsLeftHandPinch = value;
	}

	void transitionBetweenHorizontalAndVerticalMap(float zoom){
		if (zoom < 14.0f && zoom >= 13.0f) {
			// rotate the canvas upwards to face the user
			float xRotationAngle = -1.0f * (14.0f - zoom) * (45.0f);
			//Vector3 rotation = new Vector3 (rotationAngle, 0, 0);
			parentMapGo.transform.rotation = Quaternion.Euler(xRotationAngle, 0, 0);

			// rotate the canvas to always be aligned with the ground
			//this.transform.localRotation = (zoom - 13.0f) * this.transform.localRotation; 
			//this.transform.localRotation *= (zoom - 14.0f) * this.transform.localRotation;
			//this.transform.localRotation.eulerAngles = new Vector3 (0.0f, (zoom - 13.0f) * this.transform.eulerAngles.y ,0.0f);
			float yRotationAngle = (zoom - 13.0f) * ((this.transform.localEulerAngles.y <= (360.0f - this.transform.localEulerAngles.y ))? this.transform.localEulerAngles.y : (this.transform.localEulerAngles.y -360.0f)) ;
			// setting the yRotationAngle when less than 0.1f was leading to random rotations, probably because of gimbal lock prevention
			if (yRotationAngle > 0.1f || yRotationAngle < -0.1f) {
				this.transform.localEulerAngles = new Vector3 (0.0f, yRotationAngle, 0.0f);
			} else {
				this.transform.localEulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
			}
			Debug.Log ((zoom - 13.0f) * this.transform.localEulerAngles.y + " ,"  + zoom);

			// bring the canvas closer to the user and at eye level so that the user can see what the map and the map
			// is within the grabbing distance of the user
			float positionChange = -1.0f * (14.0f - zoom) / 10.0f;
			// x, 0.7442,  0.001198292
			parentMapGo.transform.position = new Vector3 ( parentMapGo.transform.position.x, 0.7442f + (positionChange / 2.0f), positionChange * 2.0f );
			//x, 0.58, -0.2
		}
	}

	void ZoomMapUsingHands(float zoomFactor)
	{
		var zoom = Mathf.Max(0.0f, Mathf.Min(_mapManager.Zoom + zoomFactor * _zoomSpeed, 21.0f));
		transitionBetweenHorizontalAndVerticalMap (zoom);
		_mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, zoom);
		//_mapManager.
	}

	void PanMapUsingHands(float xMove, float zMove)
	{
		if (Math.Abs(xMove) > 0.0f || Math.Abs(zMove) > 0.0f)
		{
			// Get the number of degrees in a tile at the current zoom level.
			// Divide it by the tile width in pixels ( 256 in our case)
			// to get degrees represented by each pixel.
			// Keyboard offset is in pixels, therefore multiply the factor with the offset to move the center.
			float factor = (_panSpeed) * (Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom));
			var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + zMove * factor * 2.0f, _mapManager.CenterLatitudeLongitude.y + xMove * factor * 4.0f);
			_mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
		}
	}

	void RotateMapUsingHands (float rotateFactor){
		//this.transform.RotateAround (this.transform.position, new Vector3 (0, 1, 0), _rotationSpeed * rotateFactor);
		//this.transform.RotateAround (this.transform.position, this.transform.rotation * Vector3.up, _rotationSpeed * rotateFactor);

		// if zoom level is less than 14.0f, this means the canvas is being rotated from horizontal to vertical and will remain vertical, 
		//so user input for rotation should not be taken into account in this case
		if(_mapManager.Zoom > 14.0f){
			this.transform.localRotation *= Quaternion.Euler(0, _rotationSpeed * rotateFactor, 0);
			//this.transform.rotation = Quaternion.Euler(0, _rotationSpeed * rotateFactor, 0);
		}
	}
}

/*
	{
		[SerializeField]
		[Range(.1f, 10f)]
		public float _panSpeed = 1.0f;

		[SerializeField]
		float _zoomSpeed = 0.25f;

		[SerializeField]
		public Camera _referenceCamera;

		[SerializeField]
		AbstractMap _mapManager;

		private Vector3 _origin;
		private Vector3 _mousePosition;
		private Vector3 _mousePositionPrevious;
		private bool _shouldDrag;
		private bool _isInitialized = false;

		void Awake()
		{
			if (null == _referenceCamera)
			{
				_referenceCamera = GetComponent<Camera>();
				if (null == _referenceCamera) { Debug.LogErrorFormat("{0}: reference camera not set", this.GetType().Name); }
			}
			_mapManager.OnInitialized += () =>
			{
				_isInitialized = true;
			};
		}


		private void LateUpdate()
		{
			if (!_isInitialized) { return; }

			if (Input.touchSupported && Input.touchCount > 0)
			{
				HandleTouch();
			}
			else
			{
				HandleMouseAndKeyBoard();
			}
		}

		void HandleMouseAndKeyBoard()
		{
			// zoom
			float scrollDelta = 0.0f;
			scrollDelta = Input.GetAxis("Mouse ScrollWheel");
			ZoomMapUsingTouchOrMouse(scrollDelta);

			//pan mouse
			PanMapUsingTouchOrMouse();
		}

		void HandleTouch()
		{
			float zoomFactor = 0.0f;
			//pinch to zoom.
			switch (Input.touchCount)
			{
			case 1:
				{
					PanMapUsingTouchOrMouse();
				}
				break;
			case 2:
				{
					// Store both touches.
					Touch touchZero = Input.GetTouch(0);
					Touch touchOne = Input.GetTouch(1);

					// Find the position in the previous frame of each touch.
					Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
					Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

					// Find the magnitude of the vector (the distance) between the touches in each frame.
					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

					// Find the difference in the distances between each frame.
					zoomFactor = 0.01f * (touchDeltaMag - prevTouchDeltaMag);
				}
				ZoomMapUsingTouchOrMouse(zoomFactor);
				break;
			default:
				break;
			}
		}

		void ZoomMapUsingTouchOrMouse(float zoomFactor)
		{
			var zoom = Mathf.Max(0.0f, Mathf.Min(_mapManager.Zoom + zoomFactor * _zoomSpeed, 21.0f));

			_mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, zoom);
		}

		void PanMapUsingKeyBoard(float xMove, float zMove)
		{
			if (Math.Abs(xMove) > 0.0f || Math.Abs(zMove) > 0.0f)
			{
				// Get the number of degrees in a tile at the current zoom level.
				// Divide it by the tile width in pixels ( 256 in our case)
				// to get degrees represented by each pixel.
				// Keyboard offset is in pixels, therefore multiply the factor with the offset to move the center.
				float factor = (_panSpeed) * (Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom));
				var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + zMove * factor * 2.0f, _mapManager.CenterLatitudeLongitude.y + xMove * factor * 4.0f);
				_mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
			}
		}

		void PanMapUsingTouchOrMouse()
		{
			if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				var mousePosScreen = Input.mousePosition;
				_mousePosition = mousePosScreen;

				if (_shouldDrag == false)
				{
					_shouldDrag = true;
					_origin = mousePosScreen;
				}
			}
			else
			{
				_shouldDrag = false;
			}

			if (_shouldDrag == true)
			{

				var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
				if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
				{
					_mousePositionPrevious = _mousePosition;

					var offsetDelta = _origin - _mousePosition;
					var offset = new Vector3(offsetDelta.x, 0f, offsetDelta.y);
					offset = Camera.main.transform.rotation * offset;

					if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
					{
						PanMapUsingKeyBoard(offset.x, offset.z);
					}
					_origin = _mousePosition;
				}
				else
				{
					if (EventSystem.current.IsPointerOverGameObject())
					{
						return;
					}
					_mousePositionPrevious = _mousePosition;
					_origin = _mousePosition;
				}
			}
		}
	}
}
*/