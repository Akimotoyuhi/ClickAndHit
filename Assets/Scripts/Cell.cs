using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] Image _image;
    private bool _isClick;
    public int PosY { get; set; }
    public int PosX { get; set; }

    public void OnChecked()
    {
        if (_isClick)
            return;
        _isClick = true;
        GameManager.Instance.OnClick(PosY, PosX);
        _image.color = Color.black;
    }
}
