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
    /// <summary>ゲーム終了時に表示するパネル</summary>
    [SerializeField] GameObject _gameEndPanel;
    /// <summary>ゲーム終了時に表示するテキスト</summary>
    [SerializeField] Text _gameEndText;
    /// <summary>勝ち負けを表示するテキストを表示するまでの時間</summary>
    [SerializeField] float _gameEndTextViewDuration;
    /// <summary>正解位置</summary>
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
        _gameEndPanel.SetActive(false);
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
        {
            _view.RPC(nameof(Evaluation), RpcTarget.All, new object[] { x, y });
            _view.RPC(nameof(CellClicked), RpcTarget.Others, new object[] { x, y });
        }
    }

    /// <summary>
    /// 勝敗判定
    /// </summary>
    [PunRPC]
    private void Evaluation(int x, int y)
    {
        if (y == _correctPosY && x == _correctPosX)
        {
            _gameEndPanel.SetActive(true);
            _gameEndText.text = "終了！！！！";
            
            DOVirtual.DelayedCall(_gameEndTextViewDuration, () =>
            {
                if (IsMineTurn)
                {
                    _gameEndText.text = "勝利！！！";
                }
                else
                {
                    _gameEndText.text = "敗北...";
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
    /// 盤面構築
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
    /// 画面の情報を更新する
    /// </summary>
    private void SetPanel()
    {
        if (IsMineTurn)
        {
            _text.text = "君のターン";
        }
        else
        {
            _text.text = "相手のターン";
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
    /// セルが押された事を同期する
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