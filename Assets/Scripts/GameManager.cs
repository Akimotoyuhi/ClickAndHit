using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private int _correctPosY;
    private int _correctPosX;
    /// <summary>�Q�[�����t���O</summary>
    private bool _isGame = false;
    private PhotonView _view;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    public void OnClick()
    {
        //if (_view.IsMine)
        //{
        //    Debug.Log("���[�ȁ[");
        //}
        //else
        //{
        //    Debug.Log("�̂��Ƃ��[�ȁ[");
        //    GameStart();
        //}
    }

    /// <summary>
    /// �Q�[���J�n
    /// </summary>
    public void GameStart()
    {
        _isGame = true;
        CreateField();
    }

    /// <summary>
    /// �Z�����N���b�N���ꂽ�Ƃ��ɌĂ΂��
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    public void OnClick(int y, int x)
    {
        if (!_isGame) return;
        if (y == _correctPosY && x == _correctPosX)
        {
            Debug.Log("�Q�[���I��");
            if (_isGame)
            {
                Debug.Log("�v���C���[�P�̏���");
            }
            else
            {
                Debug.Log("�v���C���[�Q�̏���");
            }
            _isGame = false;
        }
        else
        {
            if (y < 0) y *= -1;
            if (x < 0) x *= -1;
            int i = y + x;
            Debug.Log($"�n�Y���`�`�`�`wwwwwwww");
            Debug.Log($"{i}�}�X���炢����Ă邩��");
        }

        if (_isGame)
        {
            _isGame = false;
        }
        else
        {
            _isGame = true;
        }
    }

    /// <summary>
    /// �Ֆʍ\�z
    /// </summary>
    private void CreateField()
    {
        _cells = new Cell[_y, _x];
        for (int y = 0; y < _y; y++)
        {
            for (int x = 0; x < _x; x++)
            {
                Transform t = Instantiate(_cellPrefab).transform;
                t.SetParent(_mapTra, false);
                Cell c = t.gameObject.GetComponent<Cell>();
                c.PosY = y;
                c.PosX = x;
                _cells[y, x] = c;
            }
        }
        _correctPosX = Random.Range(0, _x);
        _correctPosY = Random.Range(0, _y);
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