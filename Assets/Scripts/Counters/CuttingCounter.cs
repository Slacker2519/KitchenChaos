using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnPogressChangedArgs> OnPogressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipeSO[] _cuttingRecipiSOArray;
    private int _cuttingProgress;

    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            // There's no KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (HasRecipiWithInput(player.GetKitchenObject().GetKitchenObjectSO))
                {
                    // Player carrying something that can be cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _cuttingProgress = 0;
                    CuttingRecipeSO cuttingRecipiSO = GetCuttingRecipiSOWithInput(GetKitchenObject().GetKitchenObjectSO);

                    OnPogressChanged?.Invoke(this, new IHasProgress.OnPogressChangedArgs
                    {
                        progressNormalized = (float)_cuttingProgress / cuttingRecipiSO.cuttingProgressMax
                    });
                }
            }
            else
            {
                // Player not carrying anything
            }
        }
        else
        {
            // There's a KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player's carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                // Player's not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(PlayerController player)
    {
        if (HasKitchenObject() && HasRecipiWithInput(GetKitchenObject().GetKitchenObjectSO))
        {
            // There's a KitchenObject here AND it can be cut
            _cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipiSO = GetCuttingRecipiSOWithInput(GetKitchenObject().GetKitchenObjectSO);

            OnPogressChanged?.Invoke(this, new IHasProgress.OnPogressChangedArgs
            {
                progressNormalized = (float)_cuttingProgress / cuttingRecipiSO.cuttingProgressMax
            });

            if (_cuttingProgress >= cuttingRecipiSO.cuttingProgressMax)
            {
                KitchenObjectSO kitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO);
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(kitchenObjectSO, this);
            }
        }
    }

    private bool HasRecipiWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipiSO = GetCuttingRecipiSOWithInput(inputKitchenObjectSO);
        return cuttingRecipiSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipiSO = GetCuttingRecipiSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipiSO != null)
        {
            return cuttingRecipiSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingRecipeSO GetCuttingRecipiSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipiSO in _cuttingRecipiSOArray)
        {
            if (cuttingRecipiSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipiSO;
            }
        }

        return null;
    }
}
