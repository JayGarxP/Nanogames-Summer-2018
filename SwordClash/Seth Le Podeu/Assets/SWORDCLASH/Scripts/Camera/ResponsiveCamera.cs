using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * https://gist.github.com/ditzel/d3dde9d60df08e714f9c571db81f3937#file-cameraaspectratioscaler-cs
 * Source code from indie dev youtuber ditzel AKA DitzelGames ~Aug 2016
 * Actaully SUCKS! Cannot use it...
 */


public class ResponsiveCamera : MonoBehaviour
{
  
    //EDITOR_FIELDS
    public Camera defaultStartCamera;
    public Camera PortraitCamera;
    public Camera LandscapeCamera;

    private Camera _activeCamera;




    // Use this for initialization
    void Start()
    {
        //defaultStartCamera = GetComponent<Camera>();
        _activeCamera = defaultStartCamera;

    }

    // Update is called once per frame
    void Update()
    {
        checkScreenOrientation();

    }

    void SetActiveCamera(Camera newActiveCamera)
    {

        _activeCamera.enabled = false;
        _activeCamera = newActiveCamera;
        _activeCamera.enabled = true;
        
        //Only needed for 3D sound.
        //_activeCamera.GetComponent<AudioListener>().enabled = true;


    }

    void checkScreenOrientation()
    {
        // Instead of polling each frame, consider using an event system.
        // There are UnityEvents and Input.ScreenOrientation would also be MUCH better.
        if (Screen.width <= Screen.height)
        {
            SetActiveCamera(PortraitCamera);
        }
        else
        {
            SetActiveCamera(LandscapeCamera);
        }
    }


    
}