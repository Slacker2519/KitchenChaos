using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipesSO : ScriptableObject
{
    public List<KitchenObjectSO> KitchenObjectSOList;
    public string RecipeName;
}
