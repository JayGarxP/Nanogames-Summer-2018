using Assets.Scripts;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;        //Floating point variable to store the player's movement speed. //Makes cool toggle in inspector view UI lets you change this var
    public float tiltspeed;
    public float AccelerometerUpdateInterval = (float)(1.0 / 60.0);
    public float LowPassKernelWidthSec = 1.0f;

    private float LowPassFilterFactor;
    private Vector2 LowPassValue;

    private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.

    private int count; //objects picked up
    private XmlDocument doc;

    //TODO how make these Text a property instead of field
    // AND how set them programmatically, setting with the editor SUCKS
    public Text countText;          //Store a reference to the UI Text component which will display the number of pickups collected.
    public Text winText;            //Store a reference to the UI Text component which will display the 'You win' message.
    //public Text CountText
    //{
    //    get
    //    {
    //        return countText;
    //    }

    //    set
    //    {
    //        countText = value;
    //    }
    //}

    //TODO: How to manage strings for UI and objects in Unity? XML? Serialiazables?

    // Use this for initialization
    void Start()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
        count = 0; //no pickups at PlayerStart
                   //countText.text = "Count: " + count.ToString();
                   //doc = new XmlDocument();
                   //doc.PreserveWhitespace = true;
                   //try { doc.Load("UI.xml"); }
                   //catch (System.IO.FileNotFoundException)
                   //{ }

        //Initialze winText to a blank string since we haven't won yet at beginning.
        winText.text = "";

        countText.text = HelperClassTest.UpdateTextField("Count: ", count);

        LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthSec;
        //LowPassValue = Vector2.zero;
        LowPassValue = Input.acceleration;



    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        // we assume that the device is held parallel to the ground
        // and the Home button is in the right hand
        var dir = Vector2.zero;

        // remap the device acceleration axis to game coordinates:
        dir.y = Input.acceleration.y;
        dir.x = Input.acceleration.x;

       // clamp acceleration vector to the unit grid
        if (dir.sqrMagnitude > 1)
        {
            dir.Normalize();
        }

        LowPassValue = Vector2.Lerp(LowPassValue, Input.acceleration, LowPassFilterFactor);

        // Move object
        //rb2d.AddForce(dir * tiltspeed);
        rb2d.AddForce(LowPassValue * tiltspeed);
    }

    //OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    void OnTriggerEnter2D(Collider2D other)
    {
        //Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            //TODO: Can I group UI elements together, score being Count and a num value tied to actual score?
            countText.text = HelperClassTest.UpdateTextField("Count: ", count);

            if (count >= 2)
            //... then set the text property of our winText object to "You win!"
            winText.text = "You win!";
        }
    }
}

