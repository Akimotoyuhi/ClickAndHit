using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject m_cellPrefab;
    [SerializeField] Transform m_mapTra;
    private Cell[,] m_cells;
    [SerializeField] int m_y = 10;
    [SerializeField] int m_x = 10;
    private int m_correctPosY;
    private int m_correctPosX;
    private bool m_isPlaying = false;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_isPlaying = true;
        CreateField();
    }

    public void OnClicked(int y, int x)
    {
        if (y == m_correctPosY && x == m_correctPosX)
        {
            Debug.Log("ゲーム終了");
            if (m_isPlaying)
            {
                Debug.Log("プレイヤー１の勝ち");
            }
            else
            {
                Debug.Log("プレイヤー２の勝ち");
            }
            m_isPlaying = false;
        }
        else
        {
            if (y < 0) y *= -1;
            if (x < 0) x *= -1;
            int i = y + x;
            Debug.Log($"ハズレ～～～～wwwwwwww");
            Debug.Log($"{i}マスくらい離れてるかも");
        }

        if (m_isPlaying)
        {
            m_isPlaying = false;
        }
        else
        {
            m_isPlaying = true;
        }
    }

    private void CreateField()
    {
        m_cells = new Cell[m_y, m_x];
        for (int y = 0; y < m_y; y++)
        {
            for (int x = 0; x < m_x; x++)
            {
                Transform t = Instantiate(m_cellPrefab).transform;
                t.SetParent(m_mapTra, false);
                Cell c = t.gameObject.GetComponent<Cell>();
                c.PosY = y;
                c.PosX = x;
                m_cells[y, x] = c;
            }
        }
        m_correctPosX = Random.Range(0, m_x);
        m_correctPosY = Random.Range(0, m_y);
    }
}
