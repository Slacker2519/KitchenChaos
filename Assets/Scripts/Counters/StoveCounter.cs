using System;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public enum States
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    public event EventHandler<IHasProgress.OnPogressChangedArgs> OnPogressChanged;
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public class OnStateChangeEventArgs : EventArgs
    {
        public States State;
    }

    [SerializeField] private FryingRecipeSO[] _fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] _burningRecipeSOArray;

    private States _state;
    private float _fryingTimer;
    private FryingRecipeSO _fryingRecipeSO;
    private float _burningTimer;
    private BurningRecipeSO _burningRecipeSO;

    private void Start()
    {
        _state = States.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (_state)
            {
                case States.Idle:
                    break;
                case States.Frying:
                    _fryingTimer += Time.deltaTime;

                    OnPogressChanged?.Invoke(this, new IHasProgress.OnPogressChangedArgs
                    {
                        progressNormalized = _fryingTimer / _fryingRecipeSO.fryingTimerMax
                    });

                    if (_fryingTimer > _fryingRecipeSO.fryingTimerMax)
                    {
                        // Fried
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);
                        _state = States.Fried;
                        _burningTimer = 0;
                        _burningRecipeSO = GetBurningRecipiSOWithInput(GetKitchenObject().GetKitchenObjectSO);

                        OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                        {
                            State = _state,
                        });
                    }
                    break;
                case States.Fried:
                    _burningTimer += Time.deltaTime;

                    OnPogressChanged?.Invoke(this, new IHasProgress.OnPogressChangedArgs
                    {
                        progressNormalized = _burningTimer / _burningRecipeSO.burniningTimerMax
                    });

                    if (_burningTimer > _burningRecipeSO.burniningTimerMax)
                    {
                        // Burned
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_burningRecipeSO.output, this);
                        _state = States.Burned;

                        OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                        {
                            State = _state,
                        });
                    }
                    break;
                case States.Burned:
                    OnPogressChanged?.Invoke(this, new IHasProgress.OnPogressChangedArgs
                    {
                        progressNormalized = 0f
                    });
                    break;
            }
        }
    }

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
                    // Player carrying something that can be fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    _fryingRecipeSO = GetFryingRecipiSOWithInput(GetKitchenObject().GetKitchenObjectSO);

                    _state = States.Frying;
                    _fryingTimer = 0;

                    OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                    {
                        State = _state,
                    });

                    OnPogressChanged?.Invoke(this, new IHasProgress.OnPogressChangedArgs
                    {
                        progressNormalized = _fryingTimer / _fryingRecipeSO.fryingTimerMax
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
            }
            else
            {
                // Player's not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                _state = States.Idle;

                OnStateChange?.Invoke(this, new OnStateChangeEventArgs
                {
                    State = _state,
                });

                OnPogressChanged?.Invoke(this, new IHasProgress.OnPogressChangedArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipiWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipiSO = GetFryingRecipiSOWithInput(inputKitchenObjectSO);
        return fryingRecipiSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipiSO = GetFryingRecipiSOWithInput(inputKitchenObjectSO);
        if (fryingRecipiSO != null)
        {
            return fryingRecipiSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipiSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipiSO in _fryingRecipeSOArray)
        {
            if (fryingRecipiSO.input == inputKitchenObjectSO)
            {
                return fryingRecipiSO;
            }
        }

        return null;
    }

    private BurningRecipeSO GetBurningRecipiSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipiSO in _burningRecipeSOArray)
        {
            if (burningRecipiSO.input == inputKitchenObjectSO)
            {
                return burningRecipiSO;
            }
        }

        return null;
    }
}
