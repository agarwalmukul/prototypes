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
	/*
	public Vector3 CurrPoint{
		get{
			return _currPoint;
		}
		private set{ 
			_currPoint = value;
		}
	}*/

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
		float threshold = 0.005f/OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).magnitude;
		if ((_currPoint - _prevPoint).magnitude < threshold) {
		} else {
			currLine.AddPoint (_currPoint);
			_prevPoint = _currPoint;
		}



	}

	// Use this for initialization

	void Start () {
		//instance = new CollisionDetector ();
		numClicks = 0;
		//Debug.Log (instance.CollidingBodyPart);
	}

	GameObject createStroke ()
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

		// Change the width of the strip on the basis of the joystick press down
		if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.RTouch)){
			StripWidth += stripWidthChangeFactor;
		} else if(OVRInput.Get (OVRInput.Button.PrimaryThumbstickDown, OVRInput.Controller.RTouch)){
			StripWidth -= stripWidthChangeFactor;
		}

		// Start and stop the strip drawing process
		if (OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) {
			GameObject go = createStroke ();
			_currLine = go.AddComponent<MeshLineRenderer> ();
			_currLine.SetMaterial (mat);
			_currLine.setWidth(StripWidth);
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
			numClicks++;
		}
	}
}
