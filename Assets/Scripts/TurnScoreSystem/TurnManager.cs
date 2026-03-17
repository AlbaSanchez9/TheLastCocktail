using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float turnDuration = 300f; 
    [SerializeField] private int maxAngryCustomers = 5;

    [Header("Eventos")]
    public UnityEvent OnTurnStarted;
    public UnityEvent OnTurnEnded;
    public UnityEvent OnGameOver;

    private float timeRemaining;
    private int angryCustomers = 0;
    private bool turnActive = false;

    public float TimeRemaining => timeRemaining;
    public bool TurnActive => turnActive;

    private void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        timeRemaining = turnDuration;
        angryCustomers = 0;
        turnActive = true;
        OnTurnStarted?.Invoke();
        Debug.Log("Turno iniciado");
    }

    private void Update()
    {
        if (!turnActive) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndTurn();
        }
    }

    public void RegisterAngryCustomer()
    {
        angryCustomers++;
        Debug.Log($"Clientes enfadados: {angryCustomers}/{maxAngryCustomers}");

        if (angryCustomers >= maxAngryCustomers)
        {
            turnActive = false;
            OnGameOver?.Invoke();
            Debug.Log("Game Over: demasiados clientes enfadados");
        }
    }

    private void EndTurn()
    {
        turnActive = false;
        OnTurnEnded?.Invoke();
        Debug.Log("Turno terminado");
    }
}