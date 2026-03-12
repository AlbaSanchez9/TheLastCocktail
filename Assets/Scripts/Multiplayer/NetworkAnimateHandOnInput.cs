using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkAnimateHandOnInput : NetworkBehaviour
{
    [SerializeField] private InputActionProperty triggerAction;
    [SerializeField] private InputActionProperty gripAction;

    [SerializeField]  private Animator animator;


    void Update()
    {
        if (IsOwner)
        {
            float triggerValue = triggerAction.action.ReadValue<float>();
            animator.SetFloat("Trigger", triggerValue);

            float gripValue = gripAction.action.ReadValue<float>();
            animator.SetFloat("Grip", gripValue);
        }
    }
}
