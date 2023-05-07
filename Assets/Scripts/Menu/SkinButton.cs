using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int _id;
    [SerializeField] private GameObject _skinWindow;
    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerPrefs.SetInt("Skin", _id);
        _skinWindow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _img.color = new Color(255 / 255f, 241 / 255f, 11 / 255f, 255 / 255f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _img.color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
    }
}
