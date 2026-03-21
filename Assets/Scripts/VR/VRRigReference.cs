using UnityEngine;

public class VRRigReference : MonoBehaviour
{
    public static VRRigReference Singleton;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [Header("Mallas del XR Origin a ocultar (solo para el jugador local)")]
    public Renderer[] localMeshesToHide; // arrastra aquí Left Hand Model, Right Hand Model, etc.

    void Awake()
    {
        Singleton = this;

        // Se ocultan inmediatamente al arrancar
        // porque este script solo existe en la máquina local
        foreach (var r in localMeshesToHide)
            r.enabled = false;
    }
}