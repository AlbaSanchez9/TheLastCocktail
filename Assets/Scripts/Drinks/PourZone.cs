using UnityEngine;

public class PourZone : MonoBehaviour
{
    [SerializeField] private Glass glass;
    [SerializeField] private float pourCooldown = 1f;

    private float lastPourTime;

    private void OnTriggerStay(Collider other)
    {
        if (!RoundManager.Instance.IsRoundActive()) return;

        IngredientBottle bottle = other.GetComponent<IngredientBottle>();

        if (bottle != null)
        {
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

    private void OnTriggerEnter(Collider other)
    {
        if (!RoundManager.Instance.IsRoundActive()) return;

        SolidIngredient solid = other.GetComponent<SolidIngredient>();
        if (solid != null)
        {
            solid.PlaceInGlass(glass);
        }
    }
}