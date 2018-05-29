using System.Collections;
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

	private Vector3 _prevPoint = new Vector3(0,0,0);
	private Vector3 _currPoint = new Vector3(0,0,0);

	public GameObject handRight;
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
		return ((1.0f - CursorOrientationFactor) * (OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch) * Vector3.up) + CursorOrientationFactor * (OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch) * Vector3.right)).normalized;
		//}
	}
	// Draw the cursor using the width as a parameter
	void DrawCursor (float width){
		Vector3 firstPoint = OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch) - (width/2.0f) * calcCursorOrientation();
		Vector3 secondPoint = OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch) + (width/2.0f) * calcCursorOrientation();
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
		if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch)){
			CursorOrientationFactor += 0.02f;
			//StrokeOrientation = Vector3.RotateTowards (StrokeOrientation, OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch) * Vector3.right, 0.05f, 0.0f);
		} else if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch)){
			CursorOrientationFactor -= 0.02f;
			//StrokeOrientation = Vector3.RotateTowards (StrokeOrientation, OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTouch) * Vector3.up, 0.05f, 0.0f);
		}

		// Change the width of the strip on the basis of the joystick press down
		if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.RTouch)){
			StripWidth += stripWidthChangeFactor;
		} else if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickDown, OVRInput.Controller.RTouch)){
			StripWidth -= stripWidthChangeFactor;
		}

		// Start and stop the strip drawing process
		if (OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
			GameObject go = createStrokeGameObject ();
			_currLine = go.AddComponent<MeshLineRenderer> ();
			_currLine.SetMaterial (mat);
			_currLine.setWidth(StripWidth);
			_currLine.SetOrientation (calcCursorOrientation());
			//currLine.endWidth = 0.1f;
			numClicks = 0;
			_prevPoint = OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch);
		}
		else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)){
			//currLine.positionCount = numClicks + 1;
			//currLine.SetPosition (numClicks, OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch));
			_currLine.setWidth(StripWidth);
			_currPoint = OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch);
			cleanPoint (_currLine, _currPoint);
			_currLine.SetOrientation (calcCursorOrientation());
			numClicks++;
		}
	}
}
