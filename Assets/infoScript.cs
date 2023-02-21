using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class infoScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject info;
    [SerializeField] private Color clickedColor;

    private Image thisImage;
    private bool isDisplaying = false;

    private void Awake()
    {
        thisImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDisplaying)
        {
            isDisplaying = true;
            info.SetActive(true);
            thisImage.color = clickedColor;
        }
        else
        {
            isDisplaying = false;
            info.SetActive(false);
            thisImage.color = Color.white;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!isDisplaying)
            {
                isDisplaying = true;
                info.SetActive(true);
                thisImage.color = clickedColor;
            }
            else
            {
                isDisplaying = false;
                info.SetActive(false);
                thisImage.color = Color.white;
            }
        }
    }
}
