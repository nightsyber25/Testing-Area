using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmButton : MonoBehaviour , IClicked
{
    public void OnClick()
    {
        Debug.Log("Click buttojn");
    }
}
