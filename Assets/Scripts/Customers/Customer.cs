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
    private GameManager gameManager;

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

    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 2f;
    private Transform exitPoint;

    private CustomerState state;

    private float currentPatience;
    private float drinkTimer;

    private Order order;

    private Glass servedGlass;
    private GameObject spawnedMoney;

    private CustomerManager manager;

    //private bool hasDrink = false;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Start()
    {
        // Decidir tipo de pedido al inicio
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
    }

    public void SetOrder(Order newOrder)
    {
        order = newOrder;
        UpdateOrderText();
        //if (orderText != null)
        //    orderText.text = order.Recipe.CocktailName;
    }

    public void SetManager(CustomerManager m)
    {
        manager = m;
    }

    public void SetExitPoint(Transform exit)
    {
        exitPoint = exit;
    }

    public void SetSnackOrder(SnackType snack)
    {
        snackOrder = snack;
        UpdateOrderText();
        //if (orderText != null && order != null)
        //{
        //    orderText.text = $"Bebida: {order.Recipe.CocktailName}\nSnack: {snackOrder}";
        //}
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
        if (!wantsDrink) return;
        if (drinkServed) return;
        if (state != CustomerState.Waiting)
            return;

        if (cocktailName == order.Recipe.CocktailName)
        {
            Debug.Log("Pedido de bebida correcto!");

            GameManager.Instance.AddCorrectDrink();

            drinkServed = true;
            servedGlass = glass;

            //glass.transform.position = barPoint.position;
            //glass.LockGlass();

            if (locationType == CustomerLocationType.Bar && barPoint != null)
            {
                // En barra el vaso se coloca y se bloquea en el barPoint
                glass.transform.position = barPoint.position;
                glass.LockGlass();
            }
            // En mesa el jugador deja el vaso donde quiera, no se bloquea

            // Si solo quería bebida, pasa a beber directamente
            if (!wantsSnack || snackServed)
            {
                StartDrinking();
            }
            else
            {
                // Tiene pendiente el snack: bonus de paciencia
                ApplyPartialDeliveryBonus();
            }

            //drinkTimer = drinkTime;
            //state = CustomerState.Drinking;
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

            if (!wantsDrink || drinkServed) StartDrinking();
            else ApplyPartialDeliveryBonus();
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

    // Aplica el bonus de paciencia una sola vez al recibir entrega parcial
    private void ApplyPartialDeliveryBonus()
    {
        if (patienceBonusApplied) return;
        patienceBonusApplied = true;

        currentPatience += partialDeliveryPatienceBonus;

        // No dejar que supere el máximo original para no romper la barra
        currentPatience = Mathf.Min(currentPatience, patience);

        Debug.Log($"Entrega parcial recibida. Paciencia aumentada en {partialDeliveryPatienceBonus}s");

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

        if (currentPatience < patience * 0.3f)
        {
            Debug.Log("Cliente impaciente!");
        }

        if (currentPatience <= 0f)
        {
            // Si había recibido algo parcialmente, deja algo de dinero antes de irse
            bool receivedPartial = (wantsDrink && drinkServed) || (wantsSnack && snackServed);

            if (receivedPartial)
            {
                Debug.Log("Cliente se fue sin esperar más, pero había recibido algo. Deja algo de dinero.");
                SpawnMoney();
            }
            else
            {
                Debug.Log("Cliente se fue enfadado por esperar demasiado");
            }

            LeaveBar(false);
        }
    }

    private void HandleDrinking()
    {
        drinkTimer -= Time.deltaTime;

        if (drinkTimer <= 0)
        {
            FinishDrink();
        }
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
        {
            Debug.Log("Cliente intenta irse sin pagar!");
            targetPosition = exitPoint.position;
        }
        else
        {
            //spawnedMoney = Instantiate(
            //    moneyPrefab,
            //    barPoint.position + Vector3.up * 0.1f,
            //    Quaternion.identity
            //);
            SpawnMoney();
        }

        state = CustomerState.FinishedDrink;
    }

    private void SpawnMoney()
    {
        //if (moneyPrefab != null)
        //{
        //    spawnedMoney = Instantiate(
        //        moneyPrefab,
        //        barPoint.position + Vector3.up * 0.1f,
        //        Quaternion.identity
        //    );
        //}

        if (moneyPrefab != null)
        {
            spawnedMoney = Instantiate(
                moneyPrefab,
                // En barra usa barPoint, en mesa spawna delante del cliente
                locationType == CustomerLocationType.Bar && barPoint != null
                    ? barPoint.position + Vector3.up * 0.1f
                    : transform.position + Vector3.up * 0.1f,
                Quaternion.identity
            );
        }
    }

    private void CheckMoneyCollected()
    {
        if (spawnedMoney == null)
        {
            LeaveBar(true);
        }
    }

    //public void ReceiveDrink()
    //{
    //    hasDrink = true;

    //    Debug.Log("Cliente recibió su bebida");
    //}

    private void LeaveBar(bool wasServed)
    {
        state = CustomerState.Leaving;

        if (manager != null)
            manager.CustomerLeft(this);

        if (wasServed)
        {
            Debug.Log("Cliente se va satisfecho");
        }

        Destroy(gameObject);
    }
}





