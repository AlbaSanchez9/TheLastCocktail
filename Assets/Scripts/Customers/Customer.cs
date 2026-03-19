using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    private enum CustomerState
    {
        Waiting,
        Drinking,
        FinishedDrink,
        Leaving
    }

    [Header("Location")]
    [SerializeField] private CustomerLocationType locationType;

    [Header("Patience")]
    [SerializeField] private float patience = 30f;
    [SerializeField] private float partialDeliveryPatienceBonus = 15f;
    [SerializeField] private Slider patienceBar;

    [Header("Drink")]
    [SerializeField] private float drinkTime = 6f;
    [SerializeField] private Transform barPoint;

    [Header("Economy")]
    [SerializeField] private float stealChance = 0.2f;
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private float moneyDistanceFromClient = 0.3f;

    [Header("Order Config")]
    [SerializeField] private float drinkOnlyChance = 0.35f;
    [SerializeField] private float snackOnlyChance = 0.20f;

    [Header("Snack")]
    private SnackType snackOrder;
    private bool snackFailedAlready = false;

    private bool wantsDrink = false;
    private bool wantsSnack = false;
    private bool drinkServed = false;
    private bool snackServed = false;
    private bool patienceBonusApplied = false;

    [SerializeField] private TextMeshProUGUI orderText;

    [SerializeField] private float moveSpeed = 2f;
    private Transform exitPoint;

    private CustomerState state;
    private float currentPatience;
    private float drinkTimer;

    private Order order;
    private Glass servedGlass;
    private GameObject spawnedMoney;
    private CustomerManager manager;

    private Transform tableCenter;
    private bool hasTableCenter = false;

    private void Awake()
    {
        if (GameManager.Instance == null)
            Debug.LogWarning("GameManager no encontrado");
    }

    private void Start()
    {
        float roll = Random.value;
        if (roll < drinkOnlyChance)
        {
            wantsDrink = true;
            wantsSnack = false;
        }
        else if (roll < drinkOnlyChance + snackOnlyChance)
        {
            wantsDrink = false;
            wantsSnack = true;
        }
        else
        {
            wantsDrink = true;
            wantsSnack = true;
        }

        state = CustomerState.Waiting;
        currentPatience = patience;

        if (patienceBar != null)
            patienceBar.maxValue = patience;

        UpdateOrderText();

        StartCoroutine(RotateToTableCenter());
    }

    private IEnumerator RotateToTableCenter()
    {
        yield return null;
        yield return null;
        yield return null;

        if (hasTableCenter && tableCenter != null)
        {
            Vector3 dirToCenter = tableCenter.position - transform.position;
            dirToCenter.y = 0;
            if (dirToCenter != Vector3.zero)
            {
                // La cara apunta a +X, hay que rotar -90 grados en Y
                Quaternion rotation = Quaternion.LookRotation(dirToCenter);
                transform.rotation = rotation * Quaternion.Euler(0, 90f, 0);
                Debug.Log($"Cliente rotado hacia mesa: {transform.rotation.eulerAngles}");
            }
        }
    }

    public void SetOrder(Order newOrder)
    {
        order = newOrder;
        UpdateOrderText();
    }

    public void SetManager(CustomerManager m) => manager = m;
    public void SetExitPoint(Transform exit) => exitPoint = exit;

    public void SetTableCenter(Transform center)
    {
        tableCenter = center;
        hasTableCenter = true;
    }

    public void SetSnackOrder(SnackType snack)
    {
        snackOrder = snack;
        UpdateOrderText();
    }

    private void UpdateOrderText()
    {
        if (orderText == null) return;

        if (wantsDrink && wantsSnack && order != null)
            orderText.text = $"Bebida: {order.Recipe.CocktailName}\nSnack: {snackOrder}";
        else if (wantsDrink && order != null)
            orderText.text = $"Bebida: {order.Recipe.CocktailName}";
        else if (wantsSnack)
            orderText.text = $"Snack: {snackOrder}";
    }

    public void TryServeDrink(string cocktailName, Glass glass)
    {
        if (!wantsDrink || drinkServed) return;
        if (state != CustomerState.Waiting) return;

        if (cocktailName == order.Recipe.CocktailName)
        {
            Debug.Log("Pedido de bebida correcto!");
            GameManager.Instance.AddCorrectDrink();

            drinkServed = true;
            servedGlass = glass;

            if (locationType == CustomerLocationType.Bar && barPoint != null)
            {
                glass.transform.position = barPoint.position;
                glass.LockGlass();
            }

            if (!wantsSnack || snackServed)
                StartDrinking();
            else
                ApplyPartialDeliveryBonus();
        }
        else
        {
            Debug.Log("Bebida incorrecta!");
            GameManager.Instance.AddWrongDrink();
        }
    }

    public bool TryServeSnack(SnackType deliveredSnack)
    {
        if (!wantsSnack || snackServed) return false;

        if (deliveredSnack == snackOrder)
        {
            snackServed = true;
            GameManager.Instance.AddCorrectSnack();

            if (!wantsDrink || drinkServed)
                StartDrinking();
            else
                ApplyPartialDeliveryBonus();

            return true;
        }
        else
        {
            Debug.Log("Snack incorrecto");
            if (!snackFailedAlready)
            {
                GameManager.Instance.AddWrongSnack();
                snackFailedAlready = true;
            }
            return false;
        }
    }

    private void ApplyPartialDeliveryBonus()
    {
        if (patienceBonusApplied) return;
        patienceBonusApplied = true;

        currentPatience = Mathf.Min(currentPatience + partialDeliveryPatienceBonus, patience);
        Debug.Log($"Entrega parcial. Paciencia +{partialDeliveryPatienceBonus}s");

        if (patienceBar != null)
            patienceBar.value = currentPatience;
    }

    private void StartDrinking()
    {
        drinkTimer = drinkTime;
        state = CustomerState.Drinking;
    }

    private void Update()
    {
        if (!RoundManager.Instance.IsRoundActive()) return;

        switch (state)
        {
            case CustomerState.Waiting:
                HandleWaiting();
                break;
            case CustomerState.Drinking:
                HandleDrinking();
                break;
            case CustomerState.FinishedDrink:
                CheckMoneyCollected();
                break;
        }

        if (state == CustomerState.FinishedDrink && spawnedMoney == null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                exitPoint.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void HandleWaiting()
    {
        currentPatience -= Time.deltaTime;

        if (patienceBar != null)
        {
            patienceBar.value = currentPatience;
            if (currentPatience < patience * 0.3f)
                patienceBar.fillRect.GetComponent<Image>().color = Color.red;
        }

        if (currentPatience <= 0f)
        {
            bool receivedPartial = (wantsDrink && drinkServed) || (wantsSnack && snackServed);
            if (receivedPartial)
                SpawnMoney();

            LeaveBar(false);
        }
    }

    private void HandleDrinking()
    {
        drinkTimer -= Time.deltaTime;
        if (drinkTimer <= 0)
            FinishDrink();
    }

    private void FinishDrink()
    {
        Debug.Log("Cliente terminó su bebida");

        if (servedGlass != null)
        {
            servedGlass.MakeDirty();
            servedGlass.UnlockGlass();
        }

        if (Random.value < stealChance)
            Debug.Log("Cliente intenta irse sin pagar!");
        else
            SpawnMoney();

        state = CustomerState.FinishedDrink;
    }

    private void SpawnMoney()
    {
        if (moneyPrefab == null) return;

        Vector3 spawnPos;

        if (locationType == CustomerLocationType.Bar && barPoint != null)
        {
            spawnPos = barPoint.position + Vector3.up * 0.1f;
        }
        else if (tableCenter != null)
        {
            Vector3 dirToCenter = (tableCenter.position - transform.position).normalized;
            dirToCenter.y = 0;
            spawnPos = transform.position + dirToCenter * moneyDistanceFromClient + Vector3.up * 0.1f;
        }
        else
        {
            spawnPos = transform.position + Vector3.up * 0.1f;
        }

        spawnedMoney = Instantiate(moneyPrefab, spawnPos, Quaternion.identity);
    }

    private void CheckMoneyCollected()
    {
        if (spawnedMoney == null)
            LeaveBar(true);
    }

    private void LeaveBar(bool wasServed)
    {
        state = CustomerState.Leaving;
        manager?.CustomerLeft(this);

        if (!wasServed)
            RoundManager.Instance.AddLostCustomer();

        Destroy(gameObject);
    }

    public string GetCocktailName() => (wantsDrink && order != null) ? order.Recipe.CocktailName : "-";
    public string GetSnackName() => wantsSnack ? snackOrder.ToString() : "-";
    public bool WantsDrink() => wantsDrink;
    public bool WantsSnack() => wantsSnack;
}