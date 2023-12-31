using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float _footStepTimerMax = .1f;
    private float _footStepTimer;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        _footStepTimer -= Time.deltaTime;
        if (_footStepTimer < 0) 
        {
            _footStepTimer = _footStepTimerMax;
            if (player.IsWalking())
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootStepSound(player.transform.position, volume);
            }
        }
    }
}
