using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Glass : MonoBehaviour
{
    private List<string> ingredients = new List<string>();
    [SerializeField] private bool isDirty;

    [Header("Caída")]
    [SerializeField] private float fallYThreshold = -1f;
    [SerializeField] private float timeOnFloor = 3f;
    private bool falling = false;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private GlassSpawner spawner;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

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
            //spawner?.NotifyGlassFell(this);
            //Destroy(gameObject);
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

    public void AddIngredient(string ingredient)
    {
        if (isDirty) return;
        ingredients.Add(ingredient);
        Debug.Log("Ingredientes actuales: " + string.Join(", ", ingredients));
    }

    public IReadOnlyList<string> GetIngredients() => ingredients;

    public void MakeDirty()
    {
        isDirty = true;
        ingredients.Clear();
    }

    public void Clean()
    {
        isDirty = false;
        ingredients.Clear();
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