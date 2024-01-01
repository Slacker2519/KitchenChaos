using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawn;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnrecipeSuccess;
    public event EventHandler OnrecipeFail;
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO _recipeListSO;
    private List<RecipesSO> _waitingRecipeSOList;
    [SerializeField] private float _spawnRecipeTimerMax = 4;
    private float _spawnRecipeTimer;
    [SerializeField] private int _watingRecipesMax = 4;
    private int _successfulRecipesAmount;

    private void Awake()
    {
        Instance = this;
        _waitingRecipeSOList = new List<RecipesSO>();
    }

    private void Update()
    {
        _spawnRecipeTimer -= Time.deltaTime;
        if (_spawnRecipeTimer < 0)
        {
            _spawnRecipeTimer = _spawnRecipeTimerMax;
            if (_waitingRecipeSOList.Count < _watingRecipesMax)
            {
                RecipesSO waitingRecipeSO = _recipeListSO.RecipeSOList[Random.Range(0, _recipeListSO.RecipeSOList.Count)];
                _waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < _waitingRecipeSOList.Count; i++)
        {
            RecipesSO waitingRecipeSO = _waitingRecipeSOList[i];

            if (waitingRecipeSO.KitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.KitchenObjectSOList)
                {
                    // Cycling through all ingredients in the Recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // Cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            // Ingredient matches
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        // This Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    // Player delivered the correct recipe!
                    _successfulRecipesAmount++;
                    _waitingRecipeSOList.RemoveAt(i);

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);

                    OnrecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // No matches found!
        // Player did not delivered a correct recipe
        OnrecipeFail?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipesSO> GetWaitingRecipesSOList()
    {
        return _waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return _successfulRecipesAmount;
    }
}
