using UnityEngine;
using TMPro;

public class OrderUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orderText;

    public void UpdateOrder(string cocktailName)
    {
        orderText.text = "Pedido: " + cocktailName;
    }
}