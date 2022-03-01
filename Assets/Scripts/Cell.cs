using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int PosY { get; set; }
    public int PosX { get; set; }

    public void OnChecked()
    {
        GameManager.Instance.OnClick(PosY, PosX);
    }
}
