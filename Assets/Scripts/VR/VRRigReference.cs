using UnityEngine;

public class VRRigReference : MonoBehaviour
{
    public static VRRigReference Singleton;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;


    void Awake()
    {
        Singleton = this;
    }
}