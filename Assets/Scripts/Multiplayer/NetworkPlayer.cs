using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public Renderer[] meshToDisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (var mesh in meshToDisable)
                mesh.enabled = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;
        if (VRRigReference.Singleton == null) return;

        var rig = VRRigReference.Singleton;

        // Root: posición mundial directa — NetworkTransform la sincroniza
        root.position = rig.root.position;
        root.rotation = rig.root.rotation;

        // Cabeza y manos: posición LOCAL respecto al root del avatar
        // Así el offset entre jugadores desaparece porque cada avatar
        // se mueve relativo a su propio root
        head.localPosition = rig.root.InverseTransformPoint(rig.head.position);
        head.localRotation = Quaternion.Inverse(rig.root.rotation) * rig.head.rotation;

        leftHand.localPosition = rig.root.InverseTransformPoint(rig.leftHand.position);
        leftHand.localRotation = Quaternion.Inverse(rig.root.rotation) * rig.leftHand.rotation;

        rightHand.localPosition = rig.root.InverseTransformPoint(rig.rightHand.position);
        rightHand.localRotation = Quaternion.Inverse(rig.root.rotation) * rig.rightHand.rotation;
    }
}