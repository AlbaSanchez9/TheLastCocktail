using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassSpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject glassPrefab;
    [SerializeField] private Transform[] slots;
    [SerializeField] private float respawnDelay = 3f;

    private Glass[] slotGlass;
    private bool[] slotOccupied;
    private bool[] slotRespawning;

    private void Awake()
    {
        int count = slots.Length;
        slotGlass = new Glass[count];
        slotOccupied = new bool[count];
        slotRespawning = new bool[count];
    }

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
            SpawnGlassAt(i);
    }

    // Llamado desde WashZone
    public void NotifyGlassBeingWashed(Glass glass)
    {
        int idx = FindSlotOf(glass);

        if (idx >= 0)
        {
            slotGlass[idx] = null;
            slotOccupied[idx] = false;

            if (!slotRespawning[idx])
                StartCoroutine(RespawnAfterDelay(idx));
        }
        else
        {
            // El vaso fue agarrado y soltado fuera del slot,
            // respawnear en el primer hueco libre
            RespawnFirstFreeSlot();
        }
    }

    // Llamado desde Glass cuando cae al suelo
    public void NotifyGlassFell(Glass glass)
    {
        int idx = FindSlotOf(glass);
        Debug.Log($"NotifyGlassFell llamado, idx={idx}");

        if (idx >= 0)
        {
            slotGlass[idx] = null;
            slotOccupied[idx] = false;

            if (!slotRespawning[idx])
                StartCoroutine(RespawnAfterDelay(idx));
        }
        else
        {
            RespawnFirstFreeSlot();
        }
    }

    // Llamado desde Glass cuando el jugador lo agarra
    public void NotifyGlassTaken(Glass glass)
    {
        int idx = FindSlotOf(glass);
        if (idx >= 0)
            slotOccupied[idx] = false;
        // Nota: slotGlass[idx] sigue apuntando al vaso
        // para poder encontrarlo si cae o va a la washzone
    }

    private void RespawnFirstFreeSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slotGlass[i] == null && !slotRespawning[i])
            {
                StartCoroutine(RespawnAfterDelay(i));
                return;
            }
        }
    }

    private IEnumerator RespawnAfterDelay(int slotIndex)
    {
        slotRespawning[slotIndex] = true;
        yield return new WaitForSeconds(respawnDelay);
        SpawnGlassAt(slotIndex);
        slotRespawning[slotIndex] = false;
    }

    private void SpawnGlassAt(int slotIndex)
    {
        if (glassPrefab == null || slots[slotIndex] == null) return;

        GameObject go = Instantiate(
            glassPrefab,
            slots[slotIndex].position,
            slots[slotIndex].rotation
        );

        Glass glass = go.GetComponent<Glass>();
        if (glass != null)
        {
            glass.SetSpawner(this);
            glass.UnlockGlass(); // ← AÑADE ESTO
            slotGlass[slotIndex] = glass;
            slotOccupied[slotIndex] = true;
        }

        Debug.Log($"Vaso spawneado en hueco {slotIndex}");
    }

    private int FindSlotOf(Glass glass)
    {
        for (int i = 0; i < slotGlass.Length; i++)
            if (slotGlass[i] == glass) return i;
        return -1;
    }
}