using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    /// <summary>�Z���̃v���n�u</summary>
    [SerializeField] GameObject _cellPrefab;
    /// <summary>�}�b�v�����ʒu</summary>
    [SerializeField] Transform _mapTra;
    /// <summary>�Z���ۑ��p</summary>
    private Cell[,] _cells;
    /// <summary>y���ɃZ���𐶐����鐔</summary>
    [SerializeField] int _y = 10;
    /// <summary>x���ɃZ���𐶐����鐔</summary>
    [SerializeField] int _x = 10;
    [SerializeField] Text _text;
    /// <summary>�}�b�`���O�{�^��</summary>
    [SerializeField] GameObject _matchButton;
    /// <summary>�w�i</summary>
    [SerializeField] Image _background;
    /// <summary>�I�[�i�[�̔w�i�F</summary>
    [SerializeField] Color _backgroundOwnerColor = Color.white;
    /// <summary>�I�[�i�[����Ȃ����̔w�i�F</summary>
    [SerializeField] Color _backgroundUnOwnerColor = Color.white;
    /// <summary>�Q�[���I�����ɕ\������p�l��</summary>
    [SerializeField] GameObject _gameEndPanel;
    /// <summary>�Q�[���I�����ɕ\������e�L�X�g</summary>
    [SerializeField] Text _gameEndText;
    /// <summary>����������\������e�L�X�g��\������܂ł̎���</summary>
    [SerializeField] float _gameEndTextViewDuration;
    /// <summary>�����ʒu</summary>
    private int _correctPosY;
    private int _correctPosX;
    /// <summary>�o�߃^�[��</summary>
    private int _currentTurn;
    /// <summary>�Q�[�����t���O</summary>
    private bool _isGame = false;
    private PhotonView _view;
    public static GameManager Instance { get; private set; }
    /// <summary>���݂ǂ���̃^�[����</summary>
    public bool IsMineTurn
    {
        get
        {
            if (_view.IsMine && _currentTurn % 2 == 0 || !_view.IsMine && _currentTurn % 2 != 0)
                return true;
            return false;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _view = GetComponent<PhotonView>();
        _gameEndPanel.SetActive(false);
    }

    /// <summary>
    /// �Q�[���J�n
    /// </summary>
    public void GameStart()
    {
        _isGame = true;
        if (_view.IsMine)
        {
            int x = Random.Range(0, _x);
            int y = Random.Range(0, _y);
            _view.RPC(nameof(CreateField), RpcTarget.All, new object[] { x, y });
        }
        _matchButton.SetActive(false);

    }

    public void OnClick(int x, int y, System.Func<bool> onClick)
    {
        if (_isGame && onClick())
        {
            _view.RPC(nameof(Evaluation), RpcTarget.All, new object[] { x, y });
            _view.RPC(nameof(CellClicked), RpcTarget.Others, new object[] { x, y });
        }
    }

    /// <summary>
    /// ���s����
    /// </summary>
    [PunRPC]
    private void Evaluation(int x, int y)
    {
        if (y == _correctPosY && x == _correctPosX)
        {
            _gameEndPanel.SetActive(true);
            _gameEndText.text = "�I���I�I�I�I";
            
            DOVirtual.DelayedCall(_gameEndTextViewDuration, () =>
            {
                if (IsMineTurn)
                {
                    _gameEndText.text = "�����I�I�I";
                }
                else
                {
                    _gameEndText.text = "�s�k...";
                }
            });

            _isGame = false;
        }
        else
        {
            _currentTurn++;
            SetPanel();
        }
    }

    /// <summary>
    /// �Ֆʍ\�z
    /// </summary>
    [PunRPC]
    private void CreateField(int correctNumX, int correctNumY)
    {
        _cells = new Cell[_x, _y];
        for (int y = 0; y < _y; y++)
        {
            for (int x = 0; x < _x; x++)
            {
                Transform t = Instantiate(_cellPrefab).transform;
                t.SetParent(_mapTra, false);
                Cell c = t.gameObject.GetComponent<Cell>();
                c.PosY = y;
                c.PosX = x;
                c.gameObject.name = $"Cell,{x} {y}";
                int yy = correctNumY - y;
                int xx = correctNumX - x;
                if (yy < 0) yy *= -1;
                if (xx < 0) xx *= -1;
                int i = yy + xx;
                c.CorrectDistLevel = i;
                _cells[x, y] = c;
            }
        }
        _correctPosX = correctNumX;
        _correctPosY = correctNumY;
        SetPanel();
    }

    /// <summary>
    /// ��ʂ̏����X�V����
    /// </summary>
    private void SetPanel()
    {
        if (IsMineTurn)
        {
            _text.text = "�N�̃^�[��";
        }
        else
        {
            _text.text = "����̃^�[��";
        }
        if (_view.IsMine)
        {
            _background.color = _backgroundOwnerColor;
        }
        else
        {
            _background.color = _backgroundUnOwnerColor;
        }
    }

    /// <summary>
    /// �Z���������ꂽ���𓯊�����
    /// </summary>
    [PunRPC]
    private void CellClicked(int x, int y)
    {
        _cells[x, y].IsClick = true;
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (byte)GameState.Start:
                GameStart();
                break;
            default:
                break;
        }
    }
}

public enum GameState : byte
{
    Start,
    End,
}