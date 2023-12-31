using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO _plateKitchenObjectSO;

    private float _spawnPlateTimer;
    private float _spawnPlateTimerMax = 4f;
    private int _platesSpawnAmount;
    private int _platesSpawnAmountMax = 4;

    private void Update()
    {
        _spawnPlateTimer += Time.deltaTime;
        if (_spawnPlateTimer > _spawnPlateTimerMax)
        {
            _spawnPlateTimer = 0;

            if (_platesSpawnAmount < _platesSpawnAmountMax)
            {
                _platesSpawnAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())
        {
            if (_platesSpawnAmount > 0)
            {
                _platesSpawnAmount--;

                KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
