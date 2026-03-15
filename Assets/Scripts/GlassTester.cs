using UnityEngine;

public class GlassTester : MonoBehaviour
{
    [SerializeField] private Glass glass;
    [SerializeField] private RecipeManager recipeManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            string cocktail = recipeManager.CheckGlass(glass);

            if (cocktail != null)
            {
                Debug.Log("Has preparado: " + cocktail);
            }
            else
            {
                Debug.Log("Cóctel incorrecto");
            }
        }
    }
}