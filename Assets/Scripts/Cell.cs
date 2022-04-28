using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] Image _image;
    [Tooltip("�����̃}�X����ǂꂭ�炢����Ă��邩�������F\n" +
        "�����̃}�X\n" +
        "�����̃}�X�Ƃ̍�����2�}�X�ȓ�\n" +
        "5�}�X�ȓ�\n" +
        "8�}�X�ȓ�\n" +
        "10�}�X�ȓ�\n" +
        "�����艓��")]
    [SerializeField] Color[] _colors;
    private bool _isClick;
    private int _correctDistLevel = 0;
    public int PosY { get; set; }
    public int PosX { get; set; }
    /// <summary>�N���b�N�ς݃t���O</summary>
    public bool IsClick
    {
        set
        {
            _isClick = value;
            ColorChange();
        }
    }
    /// <summary>�����̃}�X����ǂꂭ�炢����Ă��邩��B���ɂ����l</summary>
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
    /// �Z���̐F��ς���
    /// </summary>
    private void ColorChange()
    {
        _image.color = _colors[_correctDistLevel];
    }
}
