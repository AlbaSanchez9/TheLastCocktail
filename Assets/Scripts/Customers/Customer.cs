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

    [Header("Patience")]
    [SerializeField] private float patience = 30f;
    [SerializeField] private Slider patienceBar;

    [Header("Drink")]
    [SerializeField] private float drinkTime = 6f;
    [SerializeField] private Transform barPoint;

    [Header("Economy")]
    [SerializeField] private float stealChance = 0.2f;
    [SerializeField] private GameObject moneyPrefab;
    private GameManager gameManager;

    private CustomerState state;

    private float currentPatience;
    private float drinkTimer;

    private Order order;

    private Glass servedGlass;
    private GameObject spawnedMoney;

    //private bool hasDrink = false;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
    }

    public void SetOrder(Order newOrder)
    {
        order = newOrder;
        Debug.Log("Cliente pide: " + order.Recipe.CocktailName);
    }


    public void TryServeDrink(string cocktailName, Glass glass)
    {
        if (state != CustomerState.Waiting)
            return;

        if (cocktailName == order.Recipe.CocktailName)
        {
            Debug.Log("Pedido correcto!");

            servedGlass = glass;

            glass.transform.position = barPoint.position;

            drinkTimer = drinkTime;

            state = CustomerState.Drinking;
        }
        else
        {
            Debug.Log("Bebida incorrecta!");
        }
    }

    private void Start()
    {
        state = CustomerState.Waiting;

        currentPatience = patience;

        if (patienceBar != null)
            patienceBar.maxValue = patience;
    }

    private void Update()
    {
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
    }

    private void HandleWaiting()
    {
        currentPatience -= Time.deltaTime;

        if (patienceBar != null)
            patienceBar.value = currentPatience;

        if (currentPatience <= 0f)
        {
            Debug.Log("Cliente se fue enfadado por esperar demasiado");
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
        }

        if (Random.value < stealChance)
        {
            Debug.Log("Cliente intenta irse sin pagar!");
        }
        else
        {
            spawnedMoney = Instantiate(
                moneyPrefab,
                barPoint.position + Vector3.up * 0.1f,
                Quaternion.identity
            );
        }

        state = CustomerState.FinishedDrink;
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

        if (wasServed)
        {
            Debug.Log("Cliente se va satisfecho");
        }

        Destroy(gameObject);
    }

    public void MoveTo(Transform target)
    {
        transform.position = target.position;
    }
}





