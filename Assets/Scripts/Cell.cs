using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] Image _image;
    [Tooltip("正解のマスからどれくらい離れているかを示す色\n" +
        "正解のマス\n" +
        "正解のマスとの差分が2マス以内\n" +
        "5マス以内\n" +
        "8マス以内\n" +
        "それより遠い")]
    [SerializeField] Color[] _colors;
    private bool _isClick;
    private int _correctDistLevel = 0;
    public int PosY { get; set; }
    public int PosX { get; set; }
    /// <summary>正解のマスからどれくらい離れているかを曖昧にした値</summary>
    public int CorrectDistLevel
    {
        set
        {
            _correctDistLevel = value;
            if (_correctDistLevel < 0)
                _correctDistLevel = 0;
            else if (_correctDistLevel > 5)
                _correctDistLevel = 5;
        }
    }

    /// <summary>
    /// クリックされた時の処理<br/>
    /// Unityのボタンから呼ばれる事を想定している
    /// </summary>
    //public void OnClick()
    //{
    //    if (_isClick)
    //        return;
    //    _isClick = true;
    //    GameManager.Instance.OnClick(PosY, PosX);
    //    ColorChange();
    //}

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.Instance.OnClick(PosX, PosY, () =>
        {
            Debug.Log($"isClick {_isClick}");
            if (_isClick || !GameManager.Instance.IsMineTurn)
                return false;
            _isClick = true;
            ColorChange();
            return true;
        });
    }

    /// <summary>
    /// 色変え
    /// </summary>
    private void ColorChange()
    {
        _image.color = _colors[_correctDistLevel];
    }
}
