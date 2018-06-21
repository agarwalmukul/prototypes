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


	//public GameObject interactionCursor;
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

	private OVRInput.Controller _dominantHand = OVRInput.Controller.RTouch;
	private OVRInput.Controller _nonDominantHand = OVRInput.Controller.LTouch;


	// setup the plane detection class
	public ZEDPlaneDetectionManager testOne = new ZEDPlaneDetectionManager();
	//test.DetectPlaneAtHit(new Vector2());
	public GameObject frame;

	/**
	// calculate the position of the interaction cursor, put it 0.1 m in front of the touch controller
	private Vector3 calcInteractionCursorPosition(){
		Vector3 controllerPosition = OVRInput.GetLocalControllerPosition (_dominantHand);
		return controllerPosition + 0.1f * (OVRInput.GetLocalControllerRotation(_dominantHand) * Vector3.forward);
	}
	*/
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
			//Vector3 newZ = OVRInput.GetLocalControllerPosition(_dominantHand) + _cursorDistance * (_originCursorRotation * Vector3.forward);
			Vector3 newZ = _originCursorPosition;
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
	public Material strokeMat;
	public Material gridMat;

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

	public bool debugMode = false;
	private GameObject _debugGO;
	private LineRenderer _debugRenderer;

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
		_cursorRenderer.startWidth = 0.005f;
		_cursorRenderer.endWidth = 0.005f;
		//_cursorRenderer.SetWidth (0.005f, 0.005f);
		_cursorRenderer.material = new Material(strokeMat);
		_baseMeshRenderer = baseMesh.GetComponent<Renderer> ();
		// subscribe to the UI input action
		InteractionManage.Instance.onInteraction += onUIActivated;

		if (debugMode) {
			SetupDebug ();
		}
	}

	private string _modeDetermine = "idle";
	private string ModeDetermine{
		get{
			return _modeDetermine;
		}
		set{
			_modeDetermine = value;
		}
	}

	// this event is called when the user interacts with the UI elements
	void onUIActivated(stripColor value){
		if (ModeDetermine == "idle") {
			switch (value) {
			case stripColor.RED:
				strokeMat.color = Color.red;
				_cursorRenderer.material.color = Color.red;
				break;
			case stripColor.BLUE:
				strokeMat.color = Color.blue;
				_cursorRenderer.material.color = Color.blue;
				break;
			case stripColor.GREEN:
				strokeMat.color = Color.green;
				_cursorRenderer.material.color = Color.green;
				break;
			case stripColor.YELLOW:
				strokeMat.color = Color.yellow;
				_cursorRenderer.material.color = Color.yellow;
				break;
			}
		}else if (ModeDetermine == "drawing"){}
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
		//if (_originCursorPosition != new Vector3()) {
		//	return ((1.0f - CursorOrientationFactor) * (_originCursorRotation * Vector3.up) + CursorOrientationFactor * (_originCursorRotation * Vector3.right)).normalized;
		//}
		//else{
		/***
		 * experiment to see if we can predict what orientation the user wants the stroke in without lookahead like tilt brush
		 if (_currPoint != new Vector3 ()) {
			Vector3 curr = _currPoint - OVRInput.GetLocalControllerPosition (_dominantHand);
			Vector3 prev = _prevPoint - OVRInput.GetLocalControllerPosition (_dominantHand);
			Vector3 orientation = Vector3.Cross (curr, prev).normalized;

			return orientation;
		} else {
			return ((1.0f - CursorOrientationFactor) * (OVRInput.GetLocalControllerRotation (_dominantHand) * Vector3.up) + CursorOrientationFactor * (OVRInput.GetLocalControllerRotation (_dominantHand) * Vector3.right)).normalized;
		}
		***/
		
		return ((1.0f - CursorOrientationFactor) * (OVRInput.GetLocalControllerRotation (_dominantHand) * Vector3.up) + CursorOrientationFactor * (OVRInput.GetLocalControllerRotation (_dominantHand) * Vector3.right)).normalized;
			//}
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
		if (_grid==null) {
			_grid = GameObject.CreatePrimitive (PrimitiveType.Plane);
		}
		//_grid = GameObject.CreatePrimitive (PrimitiveType.Plane);
		_grid.transform.position = position;
		rotation *= Quaternion.Euler (90, 180, 0);
		_grid.transform.rotation = rotation;
		//plane.transform.LookAt(Vector3.Cross(position - 200.0f* (rotation * Vector3.forward), position - 200.0f* (rotation * Vector3.up)));
		Debug.Log(rotation + " " + _grid.transform.rotation + " " + rotation * Vector3.forward + " " +   rotation * Vector3.right + " " + rotation * Vector3.up);
		//plane.transform.rotation = Quaternion.Lerp (plane.transform.rotation, rotation);
		//plane.transform.LookAt(OVRInput.GetLocalControllerPosition(_dominantHand));
		_grid.transform.localScale = scale;
		MeshRenderer floorRenderer = _grid.GetComponent<MeshRenderer>();
		floorRenderer.material = gridMat;
		_grid.SetActive (true);
	}

	// Update is called once per frame
	void Update () {
		findPlaneClosestTo3DCursor ();
		// the interaction cursor is positioned where the cursor is
		//interactionCursor.transform.position = calcInteractionCursorPosition ();

		// debugging purposes
		if(debugMode){
			DrawDebug ();
		}

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
		// create the strip gameobject and setup initial parameters
		if (OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger, _dominantHand)) {
			GameObject go = createStrokeGameObject ();
			_currLine = go.AddComponent<MeshLineRenderer> ();
			_currLine.SetMaterial (new Material(strokeMat));
			_currLine.setWidth(StripWidth);
			_currLine.SetOrientation (calcCursorOrientation());
			//currLine.endWidth = 0.1f;
			numClicks = 0;
			_prevPoint = calcCursorPosition();
			ModeDetermine = "drawing";
		}
		// add strips to the existing stroke that you are drawing
		else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, _dominantHand)){
			//currLine.positionCount = numClicks + 1;
			//currLine.SetPosition (numClicks, OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch));
			_currLine.setWidth(StripWidth);
			_currPoint = calcCursorPosition();
			cleanPoint (_currLine, _currPoint);
			_currLine.SetOrientation (calcCursorOrientation());
			_prevPoint = _currPoint;
			numClicks++;
		}
		// when you stop drawing change the mode to idle so that you can respond to ui changes again
		else if (OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger, _dominantHand)){
			ModeDetermine = "idle";
		}


		/*
		if (CollisionDetector.CollidingBodyPart != null) {
			Debug.Log (CollisionDetector.CollidingBodyPart.transform.name);
		}*/

		// event system for detecting the frame where gripDown happens
		if (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, _dominantHand) > 0.9f) {
			if (_gripDownCount == 0) {
				_gripDownCount = 1;
				_gripDown = true;
			} else {
				_gripDown = false;
			}
		} else {
			if (_gripDownCount == 1) {
				_gripDownCount = 0;
			}
		}

		// assigning the origin of the drawing surface as this origin's x,y has to be defined in order to use the magnified surface.
		if (_gripDown) {
			if (_originCursorPosition == new Vector3 ()) {
				_originCursorPosition = calcCursorPosition ();
				_originCursorRotation = OVRInput.GetLocalControllerRotation (_dominantHand);
				Vector3 scale = new Vector3(0.02f,0.02f,0.02f);
				GenerateGrid (_originCursorPosition, _originCursorRotation, scale);
			} else {
				_originCursorPosition = new Vector3 ();
				_originCursorRotation = new Quaternion ();
				_grid.SetActive (false);
			}
		}
	}
	private GameObject _grid;
	private bool _gripDown = false;
	private int _gripDownCount = 0;

	void findPlaneClosestTo3DCursor(){
		//testOne.DetectPlaneAtHit (Vector3);
		// the origin is moved beyond the interaction cursor position so as not to detect the raycast collision with it
		Vector3 origin = OVRInput.GetLocalControllerPosition (_dominantHand) + 0.3f * (OVRInput.GetLocalControllerRotation(_dominantHand) * Vector3.forward);
		Ray ray = new Ray(origin, OVRInput.GetLocalControllerRotation(_dominantHand)* Vector3.forward);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100.0f)){
			Debug.Log ("hit detected" + hit.collider.transform.gameObject.name);
		}
		//Debug.Log (Camera.main.gameObject.name);
	}

	void SetupDebug(){
		_debugGO = new GameObject();
		_debugGO.AddComponent<LineRenderer> ();
		_debugGO.layer = 18;
		_debugRenderer = _debugGO.GetComponent<LineRenderer> ();
		_debugRenderer.positionCount = 2;
		_debugRenderer.startWidth = 0.005f;
		_debugRenderer.endWidth = 0.005f;
		//_cursorRenderer.SetWidth (0.005f, 0.005f);
		_debugRenderer.material = new Material(strokeMat);
	}

	void DrawDebug(){
		_debugRenderer.SetPosition (0, OVRInput.GetLocalControllerPosition (_dominantHand));
		_debugRenderer.SetPosition (1, OVRInput.GetLocalControllerPosition (_dominantHand) + 10.0f*(OVRInput.GetLocalControllerRotation (_dominantHand) * Vector3.forward));
	}

}
