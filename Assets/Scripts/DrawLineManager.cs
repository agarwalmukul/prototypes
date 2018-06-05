﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DrawLineManager : MonoBehaviour {
	
	//public CollisionDetector instance;
	/*
	public List<Texture> brushes = new List<Texture> ();
	public enum operations
	{
		Draw,
		Delete,
		Move,
		Grab,
		Paint
	}
	public operations currentOperation;
	*/
	private float stripWidthChangeFactor = 0.001f;
	private float _stripWidth = 0.1f;
	public float StripWidth {
		get{
			return _stripWidth;
		}
		private set{
			if (value >= 0.005f) {
				_stripWidth = value;
			} else {
				_stripWidth = 0.005f;
			}
		}
	}
	//private Vector3 _cursorPosition;
	//public Vector3 CursorPosition{
	//	get{ return _cursorPosition; }
	//	set{ }
	//}
	private float _cursorDistance = 0.1f;
	private float _cursorDistanceSpeed = 0.005f;
	private Vector3 _originCursorPosition;
	private Quaternion _originCursorRotation;
	private bool _gripButtonDown = false;
	private OVRInput.Controller _dominantHand = OVRInput.Controller.RTouch;
	private OVRInput.Controller _nonDominantHand = OVRInput.Controller.LTouch;

	private Vector3 calcCursorPosition(){
		//return OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch) + 0.1f * (OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch) * Vector3.forward);
		Vector3 controllerPosition = OVRInput.GetLocalControllerPosition (_dominantHand);


		//return OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch) + _cursorDistance * (OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch) * Vector3.forward);

		if (_originCursorPosition != new Vector3()) {
			Vector3 newX = _originCursorRotation * Vector3.right;
			Vector3 newY = _originCursorRotation * Vector3.up;
			Vector3 diff = controllerPosition - _originCursorPosition;
			Vector3 scaledDiffNewX = (((_cursorDistance - 0.1f) * 20.0f) * Vector3.Dot (diff, newX)) * newX.normalized;
			Vector3 scaledDiffNewY = (((_cursorDistance - 0.1f) * 20.0f) * Vector3.Dot (diff, newY)) * newY.normalized;
			//Vector3 finalVector = ( * (diffNewX + diffNewY));
			Vector3 newZ = OVRInput.GetLocalControllerPosition(_dominantHand) + _cursorDistance * (_originCursorRotation * Vector3.forward);
			return scaledDiffNewX + scaledDiffNewY + newZ;
			//return new Vector3 (_originCursorPosition.x+((controllerPosition.x-_originCursorPosition.x) * ((_cursorDistance-0.1f)*20.0f)), _originCursorPosition.y+((controllerPosition.y-_originCursorPosition.y) * ((_cursorDistance-0.1f)*20.0f)), controllerPosition.z) + _cursorDistance * (Vector3.forward);
		} else {
			return OVRInput.GetLocalControllerPosition (_dominantHand) + _cursorDistance * (OVRInput.GetLocalControllerRotation(_dominantHand) * Vector3.forward);
		}

	}

	private Vector3 _prevPoint = new Vector3(0,0,0);
	private Vector3 _currPoint = new Vector3(0,0,0);

	public GameObject baseMesh;
	private MeshLineRenderer _currLine;
	private int numClicks;
	public Material mat;

	void cleanPoint (MeshLineRenderer currLine, Vector3 newPoint)
	{
		/* if the distance between the previous and the current point is below a threshold, then we do not add the point to the strip
		 * but this approach did not work, because if we make the threshold large, then regular strip does not appear until the
		 * distance is greater than the threshold, and if we make the threshold small we still have the problem of discontinuous
		 * orientation of the strip
		if ((_currPoint - _prevPoint).magnitude < 0.002f) {
		} else {
			currLine.AddPoint (_currPoint);
			_prevPoint = _currPoint;
		}
		*/
		/* Another experiment is a point is added to a strip only if the controller is moving at a velocity greater than a threshold.
		 * This did not work out, because if the user is moving at a slow speed and then speeds up, it makes one strip which does not
		 * look great
		if(OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).magnitude > 0.1f){
			currLine.AddPoint (_currPoint);
		}
		*/

		/* Experiment where if the controller is moving fast, more points are added as opposed to when it is moving slow
		float threshold = 0.005f/OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).magnitude;
		if ((_currPoint - _prevPoint).magnitude < threshold) {
		} else {
			currLine.AddPoint (_currPoint);
			_prevPoint = _currPoint;
		}
		*/
		currLine.AddPoint (_currPoint);
		/*
		// Experiment: Do not draw a strip if the the displacement vector between the current and last frame coincides
		// or is close to the orientation of the tracked controller.
		Vector3 rot = new Vector3 (1,0,0);
		Quaternion qu = OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch);
		rot = qu * Vector3.forward;
		if (Vector3.Dot (_currPoint - _prevPoint, rot) < 0.5f) {
			currLine.AddPoint (_currPoint);
			_prevPoint = _currPoint;
		}
		*/
	}

	private GameObject _cursor;
	private LineRenderer _cursorRenderer;
	private Renderer _baseMeshRenderer;
	// Use this for initialization

	void Start () {
		//instance = new CollisionDetector ();
		numClicks = 0;
		//Debug.Log (instance.CollidingBodyPart);
		_cursor = new GameObject();
		_cursor.AddComponent<LineRenderer> ();
		_cursorRenderer = _cursor.GetComponent<LineRenderer> ();
		_cursorRenderer.positionCount = 2;
		_cursorRenderer.SetWidth (0.005f, 0.005f);

		_baseMeshRenderer = baseMesh.GetComponent<Renderer> ();
	}
	// This renders the visual indicator for the orientation of the strip that will be generated
	private float _cursorOrientationFactor = 0.0f;
	public float CursorOrientationFactor {
		get {
			return _cursorOrientationFactor;
		}
		set { 
			if (value > 1.0f) {
				_cursorOrientationFactor = 1.0f;
			} else if (value < 0.0f) {
				_cursorOrientationFactor = 0.0f;
			} else {
				_cursorOrientationFactor = value;
			}
		}
	}
	public Vector3 calcCursorOrientation (){
		//get{
		return ((1.0f - CursorOrientationFactor) * (OVRInput.GetLocalControllerRotation (_dominantHand) * Vector3.up) + CursorOrientationFactor * (OVRInput.GetLocalControllerRotation (_dominantHand) * Vector3.right)).normalized;
		//}
	}
	// Draw the cursor using the width as a parameter
	void DrawCursor (float width){
		Vector3 firstPoint = calcCursorPosition() - (width/2.0f) * calcCursorOrientation();
		Vector3 secondPoint = calcCursorPosition() + (width/2.0f) * calcCursorOrientation();
		_cursorRenderer.SetPosition(0, firstPoint);
		_cursorRenderer.SetPosition (1, secondPoint);
	}

	GameObject createStrokeGameObject ()
	{
		GameObject go = new GameObject ();
		go.AddComponent<MeshFilter> ();
		go.AddComponent<MeshRenderer> ();
		if (CollisionDetector.CollidingBodyPart != null) {
			go.transform.SetParent (CollisionDetector.CollidingBodyPart.transform);
		}
		return go;
	}

	// to display the grid plane on which the user is drawing strips
	void GenerateGrid(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		GameObject plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		plane.transform.position = position;
		plane.transform.rotation = rotation;
		plane.transform.localScale = scale;
		plane.AddComponent<BoxCollider> ();
		//plane.AddComponent<MeshRenderer> ();
		Texture2D gridImage = new Texture2D(64,64);
		int borderSize = 1;
		Color gridColor = Color.cyan;
		Color borderColor = Color.black;
		Collider floorCollider = plane.GetComponent<Collider>();
		Vector3 floorSize = new Vector3(floorCollider.bounds.size.x, floorCollider.bounds.size.z);
		for (int x = 0; x < gridImage.width; x++)
		{
			for (int y = 0; y < gridImage.height; y++)
			{
				if (x < borderSize || x > gridImage.width - borderSize || y < borderSize || y > gridImage.height - borderSize)
				{
					gridImage.SetPixel(x, y, new Color(borderColor.r, borderColor.g, borderColor.b, 50));
				}
				else gridImage.SetPixel(x, y, new Color(gridColor.r, gridColor.g, gridColor.b, 50));
			}
			gridImage.Apply();
		}
		MeshRenderer floorRenderer = plane.GetComponent<MeshRenderer>();
		floorRenderer.material.mainTexture = gridImage;
		floorRenderer.material.mainTextureScale = new Vector2(floorCollider.bounds.size.x, floorCollider.bounds.size.z);
		floorRenderer.material.mainTextureOffset = new Vector2(.5f, .5f);
	}

	// Update is called once per frame
	void Update () {
		//if (instance != null) {
		//Debug.Log (CollisionDetector.CollidingBodyPart.transform);
		//}
		/*
		else {
			instance = new CollisionDetector ();
		}
		*/
		DrawCursor (StripWidth);
		// change the orientation of the strip on the basis of the joystick press right
		if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickRight, _dominantHand)){
			CursorOrientationFactor += 0.02f;
			//StrokeOrientation = Vector3.RotateTowards (StrokeOrientation, OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch) * Vector3.right, 0.05f, 0.0f);
		} else if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickLeft, _dominantHand)){
			CursorOrientationFactor -= 0.02f;
			//StrokeOrientation = Vector3.RotateTowards (StrokeOrientation, OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch) * Vector3.up, 0.05f, 0.0f);
		}

		/*
		// change the transparency of the avatar if the controller button 'A' is pressed. This was done so that I could see the 3D strokes that I have made more clearly
		if (OVRInput.Get (OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
			_baseMeshRenderer.material.color = new Color(_baseMeshRenderer.material.color.r, _baseMeshRenderer.material.color.g, _baseMeshRenderer.material.color.b, 0.0f);
			//baseMesh.SetActive (false);
		} else {
			//baseMesh.SetActive (true);
			_baseMeshRenderer.material.color = new Color(_baseMeshRenderer.material.color.r, _baseMeshRenderer.material.color.g, _baseMeshRenderer.material.color.b, 1.0f);
		}
		*/

		// Lets try giving the user a control on determining the distance of the 3D cursor from the controller. "A" button brings the cursor closer and "B" button pushes it away
		if (OVRInput.Get (OVRInput.Button.Two, _dominantHand)) {
			_cursorDistance += _cursorDistanceSpeed;
		}
		if (OVRInput.Get (OVRInput.Button.One, _dominantHand)) {
			_cursorDistance -= _cursorDistanceSpeed;
		}

		// Change the width of the strip on the basis of the joystick press down
		if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickUp, _dominantHand)){
			StripWidth += stripWidthChangeFactor;
		} else if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickDown, _dominantHand)){
			StripWidth -= stripWidthChangeFactor;
		}

		// Start and stop the strip drawing process
		if (OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger, _dominantHand)) {
			GameObject go = createStrokeGameObject ();
			_currLine = go.AddComponent<MeshLineRenderer> ();
			_currLine.SetMaterial (mat);
			_currLine.setWidth(StripWidth);
			_currLine.SetOrientation (calcCursorOrientation());
			//currLine.endWidth = 0.1f;
			numClicks = 0;
			_prevPoint = calcCursorPosition();
		}
		else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, _dominantHand)){
			//currLine.positionCount = numClicks + 1;
			//currLine.SetPosition (numClicks, OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch));
			_currLine.setWidth(StripWidth);
			_currPoint = calcCursorPosition();
			cleanPoint (_currLine, _currPoint);
			_currLine.SetOrientation (calcCursorOrientation());
			numClicks++;
		}


		if (CollisionDetector.CollidingBodyPart != null) {
			Debug.Log (CollisionDetector.CollidingBodyPart.transform.name);
		}

		// assigning the origin of the drawing surface as this origin's x,y has to be defined in order to use the magnified surface.

		if (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, _dominantHand) > 0.9f && !_gripButtonDown) {
			if (_originCursorPosition == new Vector3 ()) {
				_originCursorPosition = calcCursorPosition ();
				_originCursorRotation = OVRInput.GetLocalControllerRotation (_dominantHand);
				Vector3 scale = new Vector3 (0.02f, 0.02f, 0.02f);
				GenerateGrid (_originCursorPosition, _originCursorRotation, scale);
			} else {
				_originCursorPosition = new Vector3 ();
				_originCursorRotation = new Quaternion ();
			}
			_gripButtonDown = true;
		} else {
			_gripButtonDown = false;
		}


	}
}
