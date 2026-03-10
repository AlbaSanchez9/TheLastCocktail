using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimations : MonoBehaviour
{
    [SerializeField] private InputActionProperty triggerAction;
    [SerializeField] private InputActionProperty gripAction;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();
        animator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAction.action.ReadValue<float>();
        animator.SetFloat("Grip", gripValue);
    }
}
