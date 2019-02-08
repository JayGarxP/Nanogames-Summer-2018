using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://doc.photonengine.com/en-us/bolt/current/demos-and-tutorials/bolt-basics/bolt-102-getting-started
// follow along photon bolt 101; Do not attach [BoltGlobalBehavior] scripts as component to any gameobjects!!!
[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    List<string> logMessages = new List<string>();

    // Broadcast event so that PlayerController knows a tentacle is in scene
    public event EventHandler RaiseTentacleTipSceneInstantiatedEvent;

    public override void SceneLoadLocalDone(string sceneName)
    {
        // randomize a position; may have to convert to camera units???
        var spawnPosition = new Vector3(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2), 0);

        // instantiate cube; all BoltPrefabs are accessed through a static class
       BoltNetwork.Instantiate(BoltPrefabs.TentacleTipScene, spawnPosition, Quaternion.identity);
    }

    public override void OnEvent(LogEventjp evnt)
    {
        logMessages.Insert(0, evnt.Message);
    }


    void OnGUI()
    {
        //DrawLogBoxGUI();
    }

    void DrawLogBoxGUI()
    {
        // only display max the 5 latest log messages
        int maxMessages = Mathf.Min(5, logMessages.Count);

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height - 100, 400, 100), GUI.skin.box);

        for (int i = 0; i < maxMessages; ++i)
        {
            GUILayout.Label(logMessages[i]);
        }

        GUILayout.EndArea();
    }
}
