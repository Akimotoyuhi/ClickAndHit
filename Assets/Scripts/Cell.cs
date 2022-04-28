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
        "�����艓��")]
    [SerializeField] Color[] _colors;
    private bool _isClick;
    private int _correctDistLevel = 0;
    public int PosY { get; set; }
    public int PosX { get; set; }
    /// <summary>�����̃}�X����ǂꂭ�炢����Ă��邩��B���ɂ����l</summary>
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
    /// �N���b�N���ꂽ���̏���<br/>
    /// Unity�̃{�^������Ă΂�鎖��z�肵�Ă���
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
    /// �F�ς�
    /// </summary>
    private void ColorChange()
    {
        _image.color = _colors[_correctDistLevel];
    }
}
