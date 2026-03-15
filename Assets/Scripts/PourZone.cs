using UnityEngine;

public class PourZone : MonoBehaviour
{
    [SerializeField] private Glass glass;

    [SerializeField] private float pourCooldown = 1f;

    private float lastPourTime;

    private void OnTriggerStay(Collider other)
    {
        IngredientBottle bottle = other.GetComponent<IngredientBottle>();

        if (bottle == null) return;

        float angle = Vector3.Angle(bottle.transform.up, Vector3.down);

        if (angle < 45f)
        {
            if (Time.time - lastPourTime > pourCooldown)
            {
                glass.AddIngredient(bottle.IngredientName);
                lastPourTime = Time.time;
            }
        }
    }
}