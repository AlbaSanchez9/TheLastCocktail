using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Glass : NetworkBehaviour
{
    private List<string> liquidIngredients = new List<string>();
    private List<string> solidIngredients = new List<string>();

    private NetworkVariable<bool> isDirty = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Color> drinkColor = new NetworkVariable<Color>(
        new Color(1f, 1f, 1f, 0.3f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> liquidVisible = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Líquido visual")]
    [SerializeField] private GlassLiquid glassLiquid;

    [Header("Caída")]
    [SerializeField] private float fallYThreshold = -1f;
    [SerializeField] private float timeOnFloor = 3f;
    private bool falling = false;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private GlassSpawner spawner;
    private RecipeManager recipeManager;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        recipeManager = FindFirstObjectByType<RecipeManager>();
        if (grab != null)
            grab.selectEntered.AddListener(OnGrabbed);
    }

    public override void OnNetworkSpawn()
    {
        drinkColor.OnValueChanged += (old, newColor) => glassLiquid?.SetColor(newColor);
        liquidVisible.OnValueChanged += (old, visible) => glassLiquid?.SetVisible(visible);
        isDirty.OnValueChanged += (old, dirty) => { if (dirty) glassLiquid?.SetVisible(false); };
    }

    private new void OnDestroy()
    {
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void Update()
    {
        if (falling) return;
        if (transform.position.y < fallYThreshold)
        {
            falling = true;
            StartCoroutine(WaitThenNotifyFall());
        }
    }

    private IEnumerator WaitThenNotifyFall()
    {
        spawner?.NotifyGlassFell(this);
        yield return new WaitForSeconds(timeOnFloor);
        if (IsServer) GetComponent<NetworkObject>().Despawn();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        spawner?.NotifyGlassTaken(this);
    }

    public void SetSpawner(GlassSpawner s) => spawner = s;
    public GlassSpawner GetSpawner() => spawner;

    public void AddIngredient(string ingredient)
    {
        if (isDirty.Value) return;
        if (!IsServer) { AddIngredientRpc(ingredient); return; }
        liquidIngredients.Add(ingredient);
        liquidVisible.Value = true;
        ValidateRealTime();
        SyncIngredientsClientRpc(
            string.Join("|", liquidIngredients),
            string.Join("|", solidIngredients)
        );
    }

    public void AddSolidIngredient(string ingredient)
    {
        if (isDirty.Value) return;
        if (!IsServer) { AddSolidIngredientRpc(ingredient); return; }
        solidIngredients.Add(ingredient);
        ValidateRealTime();
        SyncIngredientsClientRpc(
            string.Join("|", liquidIngredients),
            string.Join("|", solidIngredients)
        );
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddIngredientRpc(string ingredient)
    {
        liquidIngredients.Add(ingredient);
        liquidVisible.Value = true;
        ValidateRealTime();
        SyncIngredientsClientRpc(
            string.Join("|", liquidIngredients),
            string.Join("|", solidIngredients)
        );
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AddSolidIngredientRpc(string ingredient)
    {
        solidIngredients.Add(ingredient);
        ValidateRealTime();
        SyncIngredientsClientRpc(
            string.Join("|", liquidIngredients),
            string.Join("|", solidIngredients)
        );
    }

    [ClientRpc]
    private void SyncIngredientsClientRpc(string liquidsJoined, string solidsJoined)
    {
        if (IsServer) return;
        liquidIngredients = new List<string>(
            liquidsJoined.Length > 0 ? liquidsJoined.Split('|') : new string[0]);
        solidIngredients = new List<string>(
            solidsJoined.Length > 0 ? solidsJoined.Split('|') : new string[0]);
    }

    private void ValidateRealTime()
    {
        if (recipeManager == null) return;
        Color color = recipeManager.CheckGlassColor(this);
        drinkColor.Value = color != Color.clear ? color : new Color(1f, 1f, 1f, 0.3f);
    }

    public void SetDrinkColor(Color color)
    {
        if (!IsServer) return;
        liquidVisible.Value = true;
        drinkColor.Value = color;
    }

    public IReadOnlyList<string> GetIngredients()
    {
        var all = new List<string>();
        all.AddRange(liquidIngredients);
        all.AddRange(solidIngredients);
        return all;
    }

    public IReadOnlyList<string> GetLiquidIngredients() => liquidIngredients;
    public IReadOnlyList<string> GetSolidIngredients() => solidIngredients;

    public void MakeDirty()
    {
        if (!IsServer) return;
        isDirty.Value = true;
        liquidIngredients.Clear();
        solidIngredients.Clear();
        SyncIngredientsClientRpc("", "");
    }

    public void Clean()
    {
        if (!IsServer) return;
        isDirty.Value = false;
        liquidIngredients.Clear();
        solidIngredients.Clear();
        liquidVisible.Value = false;
        SyncIngredientsClientRpc("", "");
    }

    public bool IsDirty() => isDirty.Value;

    public void LockGlass()
    {
        if (grab != null) grab.enabled = false;
        if (rb != null) rb.isKinematic = true;
    }

    public void UnlockGlass()
    {
        if (grab != null) grab.enabled = true;
        if (rb != null) rb.isKinematic = false;
    }
}