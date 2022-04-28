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
        "10マス以内\n" +
        "それより遠い")]
    [SerializeField] Color[] _colors;
    private bool _isClick;
    private int _correctDistLevel = 0;
    public int PosY { get; set; }
    public int PosX { get; set; }
    /// <summary>クリック済みフラグ</summary>
    public bool IsClick
    {
        set
        {
            _isClick = value;
            ColorChange();
        }
    }
    /// <summary>正解のマスからどれくらい離れているかを曖昧にした値</summary>
    public int CorrectDistLevel
    {
        set
        {
            int i = value;
            if (i == 0)
                i = 0;
            else if (i <= 2)
                i = 1;
            else if (i <= 5)
                i = 2;
            else if (i <= 8)
                i = 3;
            else if (i <= 10)
                i = 4;
            else
                i = 5;
            _correctDistLevel = i;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.Instance.OnClick(PosX, PosY, () =>
        {
            if (_isClick || !GameManager.Instance.IsMineTurn)
                return false;
            IsClick = true;
            return true;
        });
    }

    /// <summary>
    /// セルの色を変える
    /// </summary>
    private void ColorChange()
    {
        _image.color = _colors[_correctDistLevel];
    }
}
