using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class keyScript : MonoBehaviour
{
    [Header("Required Fields")]
    [SerializeField] private Image UpKeyArrow;
    [SerializeField] private Image UpKeyOutline;
    [SerializeField] private Image RightKeyArrow;
    [SerializeField] private Image RightKeyOutline;
    [SerializeField] private Image LeftKeyArrow;
    [SerializeField] private Image LeftKeyOutline;
    [SerializeField] private Image ShiftKeyOutline;
    [SerializeField] private TextMeshProUGUI ShiftKeyText;
    [SerializeField] private Image CtrlKeyOutline;
    [SerializeField] private TextMeshProUGUI CtrlKeyText;
    [SerializeField] private Image ZKeyOutline;
    [SerializeField] private TextMeshProUGUI ZKeyText;

    [Header("Colors")]
    [SerializeField] private Color UpKeyDownColor;
    [SerializeField] private Color SideKeyDownColor;
    [SerializeField] private Color ShiftKeyDownColor;
    [SerializeField] private Color CtrlKeyDownColor;
    [SerializeField] private Color ZKeyDownColor;

    private bool zKeyPressed = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            UpKeyArrow.color = UpKeyDownColor;
            UpKeyOutline.color = UpKeyDownColor;
        }
        else
        {
            UpKeyArrow.color = Color.white;
            UpKeyOutline.color = Color.white;
        }

        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            RightKeyArrow.color = Color.white;
            RightKeyOutline.color = Color.white;
            LeftKeyArrow.color = Color.white;
            LeftKeyOutline.color = Color.white;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RightKeyArrow.color = SideKeyDownColor;
            RightKeyOutline.color = SideKeyDownColor;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            LeftKeyArrow.color = SideKeyDownColor;
            LeftKeyOutline.color = SideKeyDownColor;
        }
        else
        {
            RightKeyArrow.color = Color.white;
            RightKeyOutline.color = Color.white;
            LeftKeyArrow.color = Color.white;
            LeftKeyOutline.color = Color.white;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
        {
            ShiftKeyOutline.color = Color.white;
            ShiftKeyText.color = Color.white;
            CtrlKeyOutline.color = Color.white;
            CtrlKeyText.color = Color.white;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            ShiftKeyOutline.color = ShiftKeyDownColor;
            ShiftKeyText.color = ShiftKeyDownColor;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            CtrlKeyOutline.color = CtrlKeyDownColor;
            CtrlKeyText.color = CtrlKeyDownColor;
        }
        else
        {
            ShiftKeyOutline.color = Color.white;
            ShiftKeyText.color = Color.white;
            CtrlKeyOutline.color = Color.white;
            CtrlKeyText.color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!zKeyPressed)
            {
                ZKeyOutline.color = ZKeyDownColor;
                ZKeyText.color = ZKeyDownColor;

                zKeyPressed = true;
            }
            else
            {
                ZKeyOutline.color = Color.white;
                ZKeyText.color = Color.white;

                zKeyPressed = false;
            }
        }
    }
}
