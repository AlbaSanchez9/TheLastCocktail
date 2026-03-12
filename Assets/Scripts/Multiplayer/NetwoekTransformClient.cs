using Unity.Netcode.Components;
using UnityEngine;

public class NetwoekTransformClient : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
