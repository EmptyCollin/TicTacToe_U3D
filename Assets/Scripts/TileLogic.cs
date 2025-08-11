using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileLogic : MonoBehaviour

{
    [SerializeField]
    private GameObject chess_circle;
    [SerializeField]
    private GameObject chess_cross;

    private GameObject[] chesses;

    private BoardLogic board;
    private int logic_x, logic_y;

    // Start is called before the first frame update
    void Awake()
    {
        if (chess_circle != null && chess_cross != null){
            chesses = new GameObject[] { chess_circle, chess_cross };
        }
        else{
            Debug.LogError("棋子初始化时未绑定对象");
        }

        // Debug.Log("初始化棋子成功");
    }

    public void set_board(BoardLogic b, int i, int j){
        board = b;
        logic_x = j;
        logic_y = i;
    }
    
    
    public void set_state(ChessState state)
    {
        // Debug.Log($"set_state at {logic_x}, {logic_y} with {state}");
        foreach(GameObject chess in chesses){
            chess.SetActive(false);
        }
        if (state == ChessState.circle){
            chess_circle.SetActive(true);
        }
        else if (state == ChessState.cross){
            chess_cross.SetActive(true);
        }
    }

    public void reset(){
        set_state(ChessState.empty);
    }

    void OnMouseDown()
    {
        board?.on_tile_click(logic_x, logic_y);
    }
}
