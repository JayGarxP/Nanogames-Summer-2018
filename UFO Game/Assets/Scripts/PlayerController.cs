using Assets.Scripts;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;        //Floating point variable to store the player's movement speed. //Makes cool toggle in inspector view UI lets you change this var

    //TODO how make these Text a property instead of field
    //TODO: How to manage strings for UI and objects in Unity? XML? Serialiazables?
    // AND how set them programmatically, setting with the editor SUCKS
    public Text countText;          //Store a reference to the UI Text component which will display the number of pickups collected.
    public Text winText;            //Store a reference to the UI Text component which will display the 'You win' message.
    public Text touchCountText;

    private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.

    private int count; //objects picked up

    // Use this for initialization
    void Start()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
        count = 0; //no pickups at PlayerStart

        //Initialze winText to a blank string since we haven't won yet at beginning.
        winText.text = "";

        countText.text = HelperClassTest.UpdateTextField("Count: ", count);

    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        //rb2d.AddForce(LowPassValue * tiltspeed);
    }

    void Update()
    {
        touchCountText.text = HelperClassTest.UpdateTextField("touchCount: ", Input.touchCount);
        //perforce test auto-checkout file WORKS
        //Test perforce from within UNITY auto-checkout DOES NOT WORK!!!!
        //Sadly will have to use P4V since the integrated unity tool does not work for crap.


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
            winText.text = "You win YEET!";
        }
    }
}

