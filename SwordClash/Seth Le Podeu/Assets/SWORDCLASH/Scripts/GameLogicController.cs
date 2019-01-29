using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SwordClash Game Logic Controller; needs some way to talk to Nanogames menu scene
public class GameLogicController : MonoBehaviour
{
    public short NumberofRounds;

    // Food tentacles fight over
    [SerializeField]
    private GameObject Snack;
    [SerializeField]
    private Sprite[] SnackSpriteArray;


    private short PlayerOnePoints;
    private short PlayerTwoPoints;
   

    // Center of camera game screen in world units
    private Vector3 CenterCameraCoord;
    private SpriteRenderer SnackSprite;
    private Rigidbody2D SnackBody;


    // Start is called before the first frame update
    void Start()
    {
        PlayerOnePoints = 0;
        PlayerTwoPoints = 0;

        CenterCameraCoord = Camera.main.ScreenToWorldPoint(new Vector2((Screen.width / 2),
            (Screen.height / 2)));
        // Zero out z coordinate, for some reason setting it to zero in ScreenToWorldPoint results in -10 for z...
        CenterCameraCoord.z = 0;

        //SnackSprite = Snack.GetComponent<SpriteRenderer>();
        //SnackBody = Snack.GetComponent<Rigidbody2D>();

        SpawnFoodInCenter();

    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    // Monobehavior reset when component is first dropped into scene, set default editor fields here
    void Reset()
    {
        NumberofRounds = 3;
    }

    private void SpawnFoodInCenter()
    {
        Snack = Instantiate(Snack, CenterCameraCoord, Quaternion.identity);
        SnackBody = Snack.GetComponent<Rigidbody2D>();
        SnackSprite = Snack.GetComponent<SpriteRenderer>();
        AssignRandomSnackSprite();
    }

    private void AssignRandomSnackSprite()
    {
        // Assuming that the SnackSpriteArray is of size 20, with 20 food sprites set in the Unity Editor Inspector
        //  Pick a random sprite to be the current SnackSprite
        SnackSprite.sprite = SnackSpriteArray[Random.Range(0, 20)];
    }

    // For inbetween rounds
    private void NextRoundFoodInCenter()
    {
        AssignRandomSnackSprite();
        SnackBody.position = (CenterCameraCoord);
    }

    public void OnFoodEaten(string EaterPlayerID)
    {
        if (EaterPlayerID == "Player1")
        {
            PlayerOnePoints += 1;
        }
        else if (EaterPlayerID == "Player2")
        {
            PlayerTwoPoints += 1;
        }
        else
        {
            // Bad playerID ...
        }

        // Best outta 3 rounds means whoever hits 2 wins is the big match winner.
        short pointsToWin = (short)((NumberofRounds / 2) + 1);

        if (PlayerOnePoints >= pointsToWin)
        {
            // Player one wins!!!
        }
        else if (PlayerTwoPoints >= pointsToWin)
        {
            // Player two wins...
        }

        // Spawn in new food
        NextRoundFoodInCenter();
    }

}
