using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class ServerCallbacks : Bolt.GlobalEventListener
{
    public override void Connected(BoltConnection connection)
    {
        // In Bolt you use EventName.Create(); to create a new event, you then assign the properties you want and call eventObject.Send();
        var log = LogEventjp.Create();
        log.Message = string.Format("{0} connected", connection.RemoteEndPoint);
        log.Send();
    }

    public override void Disconnected(BoltConnection connection)
    {
        var log = LogEventjp.Create();
        log.Message = string.Format("{0} disconnected", connection.RemoteEndPoint);
        log.Send();
    }


}