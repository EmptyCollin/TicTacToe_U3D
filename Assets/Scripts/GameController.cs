using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public static GameController _instance;
    private TimerMgr timer_mgr;
    // 流程控制
    private GameStage game_stage;
    private BoardLogic board;
    private MainUI main_ui;
    private LoginUI login_ui;

    // row, column, side, enable_ai, n_level
    private (int, int, ChessState, bool, int) cache_game_setting = (3, 3, ChessState.circle, false, 2);

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 销毁重复实例
        }

        timer_mgr = gameObject.GetComponent<TimerMgr>();
    }


    private void Start()
    {
        // 获取场景对象
        GameObject board_node = GameObject.Find("board_outline");
        if (board_node == null)
        {
            Debug.LogError("找不到名为 'board_outline' 的对象!");
        }
        board = board_node.GetComponent<BoardLogic>();

        GameObject main_ui_node = GameObject.Find("main_ui");
        if (main_ui_node == null)
        {
            Debug.LogError("找不到名为 'main_ui' 的对象!");
        }
        main_ui = main_ui_node.GetComponent<MainUI>();

        GameObject login_ui_node = GameObject.Find("login_ui");
        if (login_ui_node == null)
        {
            Debug.LogError("找不到名为 'login_ui' 的对象!");
        }
        login_ui = login_ui_node.GetComponent<LoginUI>();

        game_stage = GameStage.init;
        check_stage();
    }

    private void check_stage()
    {
        switch (game_stage)
        {
            case GameStage.init:
                show_login_ui();
                hide_main_ui();
                break;

            case GameStage.gaming:
                show_main_ui();
                hide_login_ui();
                create_board();
                break;

            case GameStage.ending:
                show_result_ui();
                hide_main_ui();
                break;
        }
    }

    private void show_login_ui()
    {
        login_ui.set_active(true);
        login_ui.set_content("");
    }

    private void hide_login_ui()
    {
        login_ui.set_active(false);
    }

    private void show_main_ui()
    {
        main_ui.set_active(true);
    }

    private void hide_main_ui()
    {
        main_ui.set_active(false);
    }

    private void show_result_ui()
    {
        string winner = board.get_winner();
        login_ui.set_active(true);
        login_ui.set_content(winner);
    }

    public void set_game_setting(int r, int c, ChessState state, bool enable_ai, int n_level)
    {
        cache_game_setting = (r, c, state, enable_ai, n_level);
    }

    public void change_stage(GameStage new_stage)
    {
        if (game_stage != new_stage)
        {
            game_stage = new_stage;
            check_stage();
        }

    }

    public void show_tips(string content)
    {
        main_ui.show_tips(content);
    }

    public void start_count_down(float time)
    {
        main_ui.start_count_down(time);
    }

    public int start_timer(float duration, System.Action callback)
    {
        if (timer_mgr == null)
        {
            return 0;
        }
        return timer_mgr.start_timer(duration, callback);
    }

    public void cancel_timer(int time_id)
    {
        timer_mgr?.cancel_timer(time_id);
    }

    public void create_board()
    {
        (int row, int column, ChessState state, bool enable_ai, int n_lv) = cache_game_setting;
        board.create_board(row, column, state, enable_ai, n_lv);
        main_ui.set_ai_data(enable_ai, n_lv);
    }

    public void change_ai_setting(bool enable_ai, int n_lv)
    {
        (int row, int column, ChessState state, _, _) = cache_game_setting;
        cache_game_setting = (row, column, state, enable_ai, n_lv);
        board.update_ai_setting(enable_ai, n_lv);
        login_ui.asyn_ai_setting(enable_ai, n_lv);
    }

    public void set_cur_tile(ChessState state)
    {
        main_ui.set_cur_tile(state);
    }

    public void req_board_undo()
    {
        board.undo();
    }

}
