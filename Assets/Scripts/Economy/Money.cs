using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Money : XRGrabInteractable
{
    private int value = 0;

    public void SetValue(int amount) => value = amount;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!RoundManager.Instance.IsRoundActive()) return;
        base.OnSelectEntered(args);
        GameManager.Instance.AddMoney(value);
        CollectMoneyRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void CollectMoneyRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}