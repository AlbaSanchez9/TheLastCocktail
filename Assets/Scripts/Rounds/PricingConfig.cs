using UnityEngine;

[CreateAssetMenu(menuName = "Config/PricingConfig")]
public class PricingConfig : ScriptableObject
{
    [Header("Precios base")]
    public int drinkOnlyPrice = 10;
    public int snackOnlyPrice = 6;
    public int bothPrice = 14; // menos que 10+6=16, pequeño descuento

    [Header("Penalizaciones por cliente perdido")]
    public int lostDrinkOnlyPenalty = 5;
    public int lostSnackOnlyPenalty = 3;
    public int lostBothPenalty = 7;

    [Header("Penalización entrega parcial (se fue con algo pendiente)")]
    public int partialDrinkServedPenalty = 2;  // tenía bebida pero faltaba snack
    public int partialSnackServedPenalty = 2;  // tenía snack pero faltaba bebida
}