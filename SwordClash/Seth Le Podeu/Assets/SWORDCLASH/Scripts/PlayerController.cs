using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region EDITOR_FIELDS
    
    #endregion

    // Use this for initialization
    void Start()
    {
       

    }

    ////FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    //void FixedUpdate()
    //{
        
    //}

    void Update()
    {
        


    }


    ////OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    //Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
    //    if (other.gameObject.CompareTag("PickUp"))
    //    {
    //        other.gameObject.SetActive(false);
    //        count += 1;
    //        //TODO: Can I group UI elements together, score being Count and a num value tied to actual score?
    //        countText.text = HelperClassTest.UpdateTextField("Count: ", count);

    //        if (count >= 2)
    //            //... then set the text property of our winText object to "You win!"
    //            winText.text = "You win YEET!";
    //    }
    //}
}