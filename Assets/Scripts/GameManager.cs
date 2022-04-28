using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    /// <summary>セルのプレハブ</summary>
    [SerializeField] GameObject _cellPrefab;
    /// <summary>マップ生成位置</summary>
    [SerializeField] Transform _mapTra;
    /// <summary>セル保存用</summary>
    private Cell[,] _cells;
    /// <summary>y軸にセルを生成する数</summary>
    [SerializeField] int _y = 10;
    /// <summary>x軸にセルを生成する数</summary>
    [SerializeField] int _x = 10;
    [SerializeField] Text _text;
    /// <summary>マッチングボタン</summary>
    [SerializeField] GameObject _matchButton;
    /// <summary>背景</summary>
    [SerializeField] Image _background;
    /// <summary>オーナーの背景色</summary>
    [SerializeField] Color _backgroundOwnerColor = Color.white;
    /// <summary>オーナーじゃない時の背景色</summary>
    [SerializeField] Color _backgroundUnOwnerColor = Color.white;
    /// <summary>正解ポジション</summary>
    private int _correctPosY;
    private int _correctPosX;
    /// <summary>経過ターン</summary>
    private int _currentTurn;
    /// <summary>ゲーム中フラグ</summary>
    private bool _isGame = false;
    private PhotonView _view;
    public static GameManager Instance { get; private set; }
    /// <summary>現在どちらのターンか</summary>
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
    }

    /// <summary>
    /// ゲーム開始
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
            _view.RPC(nameof(OnClick), RpcTarget.All, new object[] { x, y });
    }

    /// <summary>
    /// セルがクリックされたときに呼ばれる
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    [PunRPC]
    private void OnClick(int x, int y)
    {
        //if (!_isGame) return;
        //if (_currentTurn % 2 == 0)
        //{
        //    if (!_view.IsMine)
        //        return;
        //}
        //else
        //{
        //    if (_view.IsMine)
        //        return;
        //}
        if (y == _correctPosY && x == _correctPosX)
        {
            Debug.Log("ゲーム終了");
            if (_isGame)
            {
                Debug.Log("プレイヤー１の勝ち");
            }
            else
            {
                Debug.Log("プレイヤー２の勝ち");
            }
            _isGame = false;
        }
        else
        {
            if (y < 0) y *= -1;
            if (x < 0) x *= -1;
            int i = y + x;
            Debug.Log($"ハズレ～～～～wwwwwwww");
            Debug.Log($"{i}マスくらい離れてるかも");
        }

        _currentTurn++;

        //if (_isGame)
        //{
        //    _isGame = false;
        //}
        //else
        //{
        //    _isGame = true;
        //}
    }

    /// <summary>
    /// 盤面構築
    /// </summary>
    [PunRPC]
    private void CreateField(int correctNumX, int correctNumY)
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
                int yy = y;
                int xx = x;
                if (yy < 0) yy *= -1;
                if (xx < 0) xx *= -1;
                int i = yy + xx;
                c.CorrectDistLevel = i;
                _cells[y, x] = c;
            }
        }
        _correctPosX = correctNumX;
        _correctPosY = correctNumY;
        SetPanel();
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

    private void SetPanel()
    {
        if (_view.IsMine)
        {
            _background.color = _backgroundOwnerColor;
            _text.text = "プレイヤー１" + _correctPosX + _correctPosY;
        }
        else
        {
            _background.color = _backgroundUnOwnerColor;
            _text.text = "プレイヤー２" + _correctPosX + _correctPosY;
        }
    }
}

public enum GameState : byte
{
    Start,
    End,
}