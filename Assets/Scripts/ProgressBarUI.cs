using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject _hasProgressGameObject;
    [SerializeField] private Image _barImage;
    private IHasProgress _hasProgress;

    private void Start()
    {
        _hasProgress = _hasProgressGameObject.GetComponent<IHasProgress>();
        if (_hasProgress == null)
        {
            Debug.LogError("Game Object " + _hasProgressGameObject + "does not have a component that implements IHasProgress!");
        }

        _hasProgress.OnPogressChanged += HasProgress_OnPogressChanged;

        _barImage.fillAmount = 0f;

        Hide();
    }

    private void HasProgress_OnPogressChanged(object sender, IHasProgress.OnPogressChangedArgs e)
    {
        _barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f || e.progressNormalized == 1) 
        { 
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
