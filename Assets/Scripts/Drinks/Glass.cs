using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Glass : MonoBehaviour
{
    private List<string> liquidIngredients = new List<string>();
    private List<string> solidIngredients = new List<string>();

    [SerializeField] private bool isDirty;

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

    private void OnDestroy()
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
            Debug.Log("Vaso caído al suelo");
            StartCoroutine(WaitThenNotifyFall());
        }
    }

    private IEnumerator WaitThenNotifyFall()
    {
        yield return new WaitForSeconds(timeOnFloor);
        spawner?.NotifyGlassFell(this);
        Destroy(gameObject);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        spawner?.NotifyGlassTaken(this);
    }

    public void SetSpawner(GlassSpawner s) => spawner = s;
    public GlassSpawner GetSpawner() => spawner;

    // Líquidos desde PourZone
    public void AddIngredient(string ingredient)
    {
        if (isDirty) return;
        liquidIngredients.Add(ingredient);
        glassLiquid?.SetVisible(true);
        ValidateRealTime();
        Debug.Log("Líquido añadido: " + ingredient);
    }

    // Sólidos desde PourZone
    public void AddSolidIngredient(string ingredient)
    {
        if (isDirty) return;
        solidIngredients.Add(ingredient);
        ValidateRealTime();
        Debug.Log("Sólido añadido: " + ingredient);
    }

    private void ValidateRealTime()
    {
        if (recipeManager == null) return;
        Color color = recipeManager.CheckGlassColor(this);
        if (color != Color.clear)
            glassLiquid?.SetColor(color);
        else
            glassLiquid?.SetColor(new Color(1f, 1f, 1f, 0.3f));
    }

    public void SetDrinkColor(Color color)
    {
        glassLiquid?.SetVisible(true);
        glassLiquid?.SetColor(color);
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
        isDirty = true;
        liquidIngredients.Clear();
        solidIngredients.Clear();
        glassLiquid?.SetVisible(false);
    }

    public void Clean()
    {
        isDirty = false;
        liquidIngredients.Clear();
        solidIngredients.Clear();
        glassLiquid?.SetVisible(false);
    }

    public bool IsDirty() => isDirty;

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