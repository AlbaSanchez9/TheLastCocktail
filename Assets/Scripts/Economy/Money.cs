using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Money : XRGrabInteractable
{
    [SerializeField] private int value = 10;

    private GameManager gameManager;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        gameManager = FindFirstObjectByType<GameManager>();

        gameManager.AddMoney(value);

        Destroy(gameObject);
    }
}