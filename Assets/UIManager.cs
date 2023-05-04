using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TextMeshProUGUI _ammoText;

    private UIManager _instance;
    public UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void UpdateAmmo(int ammoAmout)
    {
        _ammoText.text = ammoAmout.ToString();
    }

    public void UpdateLives(int livesAmount)
    {
        _livesText.text = livesAmount.ToString();
    }
}
