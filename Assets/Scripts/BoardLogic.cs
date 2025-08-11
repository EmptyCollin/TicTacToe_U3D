using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BoardLogic : MonoBehaviour
{
    private int row = -1, column = -1;
    private ChessState cur_side;
    private ChessState player_side;

    // 对象缓存
    private GameObject tile_prefab;

    // 棋盘对象
    private GameObject[,] tile_objs;

    // 棋盘抽象数据
    private ChessState[,] tile_data;

    // 结果缓存
    private string cache_result = "";

    // 剪枝算法深度
    private int n_level = 3;
    bool enable_ai = false;

    // 步长超时定时器
    int round_outtime_handler = 0;

    // 步长预警定时器
    int round_warning_handler = 0;

    // AI延迟操作便于观察
    int ai_desicion_delay_handler = 0;

    // 记录栈，用于回退操作
    private List<(ChessState, ChessState[,])> history = new List<(ChessState, ChessState[,])>();

    // 线程控制以及中间队列
    AlgorithmTask task;
    // private bool ai_in_computing = false;
    // private CancellationTokenSource cts;
    // private ConcurrentQueue<ChessAction> ai_result_queue = new ConcurrentQueue<ChessAction>();

    // Start is called before the first frame update
    void Awake()
    {
        // 加载预制件
        tile_prefab = Resources.Load<GameObject>("Prefabs/tile");
        if (tile_prefab == null)
        {
            Debug.LogError("找不到路径为 'Assets/Resources/Prefabs/tile' 的预制件!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 新建或重开都走这里
    public void create_board(int r, int c, ChessState p_side, bool is_on, int n_lv)
    {
        Debug.Log($"create_board, {r}, {c}, {p_side}, {is_on}, {n_lv}");
        history.Clear();
        tile_data = new ChessState[r, c];

        player_side = p_side;
        n_level = n_lv;
        enable_ai = is_on;

        if (r != row || c != column)
        {
            Debug.Log("create a new board");
            // 删除旧节点
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            row = r;
            column = c;
            tile_objs = new GameObject[r, c];

            // 获取 board_outline的尺寸并计算tile的尺寸和缩放
            RectTransform rec_trans = gameObject.GetComponent<RectTransform>();
            float board_width = rec_trans.rect.width;
            float board_height = rec_trans.rect.height;

            Vector3 tile_size = tile_prefab.GetComponent<Renderer>().bounds.size;
            float tile_width = tile_size.x;
            float tile_height = tile_size.y;
            float scale = Math.Min(board_width, board_height) / (Math.Max(tile_width, tile_height) * Math.Max(column, row));
            tile_width *= scale;
            tile_height *= scale;

            // 计算起始位置, 这里默认居中了
            Vector3 start_position = transform.position - new Vector3(tile_width * column / 2, tile_height * row / 2);

            // 填充 tile_objs
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    // 计算每个 tile 的位置
                    Vector3 tile_position = new Vector3(
                        start_position.x + (j * tile_width) + tile_width / 2,
                        start_position.y + (i * tile_height) + tile_height / 2,
                        0);

                    // 实例化 tile
                    GameObject tile_instance = Instantiate(tile_prefab, transform);
                    tile_instance.transform.localPosition = tile_position;
                    tile_instance.transform.localScale = new Vector3(scale, scale, 1);
                    tile_instance.transform.name = $"tile {i}_{j}";
                    tile_objs[i, j] = tile_instance;
                    TileLogic tile_logic = tile_instance.GetComponent<TileLogic>();
                    tile_logic?.set_board(this, i, j);
                }
            }
        }

        initialize_board();
        round_start();
    }

    private void initialize_board()
    {
        // 重设每个对象和数据

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < row; j++)
            {
                GameObject tile_obj = tile_objs[i, j];
                tile_data[i, j] = ChessState.empty;
                TileLogic tile_logic = tile_obj.GetComponent<TileLogic>();
                tile_logic?.reset();
            }
        }

        cur_side = ChessState.circle;
    }

    public void update_ai_setting(bool b, int n)
    {
        enable_ai = b;
        n_level = n;
    }

    public void on_tile_click(int logic_x, int logic_y)
    {
        if (player_side != cur_side)
        {
            // 对方回合，通知UI弹提示
            GameController._instance.show_tips("plz waiting, cause ai thinking");
            return;
        }
        try_set_tile_state(cur_side, logic_x, logic_y);
    }

    private void random_choice_when_timeout()
    {
        // 遍历找出候选对象
        List<(int, int)> candidates = GameUtils.get_all_empty_tile(tile_data);

        // 随机挑选一个
        if (candidates.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, candidates.Count);
            var (row_idx, column_idx) = candidates[idx];
            set_tile_state(cur_side, column_idx, row_idx);
        }
        else
        {
            // 理论上无法进入这个分支，必然存在逻辑谬误
            Debug.LogError("unexpected stack visited random_choice_when_timeout");
        }

    }

    private void try_set_tile_state(ChessState state, int logic_x, int logic_y)
    {
        // Debug.Log($"try_set_tile_state, state {state}, {logic_x},{logic_y}");
        if (GameUtils.check_can_place(tile_data, logic_x, logic_y))
        {
            set_tile_state(state, logic_x, logic_y);
        }
        else
        {
            // 通知UI弹通知
            GameController._instance.show_tips("plz try again, cause place the chess on none-empty tile");
        }
        // GameUtils.print_tile_data(tile_data);
    }

    private void set_tile_state(ChessState state, int logic_x, int logic_y)
    {
        // 同步数据和表现
        tile_data[logic_y, logic_x] = state;
        GameObject tile_obj = tile_objs[logic_y, logic_x];
        TileLogic tile_logic = tile_obj.GetComponent<TileLogic>();
        tile_logic?.set_state(state);

        // 结果判定
        RoundResult result = GameUtils.check_result(tile_data);
        if (result != RoundResult.new_round)
        {
            game_over(result);
        }
        else
        {
            round_end();
        }
    }

    public void undo()
    {
        ChessState his_side;
        ChessState[,] his_tile_data;
        // 没有历史记录或者只有一步且不是自己下的悔不了
        if (history.Count == 0)
        {
            return;
        }
        else if (history.Count == 1)
        {
            (his_side, his_tile_data) = history[0];
            if (his_side != player_side)
            {
                return;
            }
        }

        task.cancel_immediately_and_drop_result();
        clear_timers();

        for (int i = history.Count - 1; i >= 0; i--)
        {
            // 悔棋直到自己的操作步
            (his_side, his_tile_data) = history[i];
            history.RemoveAt(i);
            if (his_side == player_side)
            {
                break;
            }

        }
        if (history.Count == 0)
        {
            // 一个不剩，重设棋盘
            initialize_board();
        }
        else
        {
            (his_side, his_tile_data) = history[history.Count - 1];
            tile_data = his_tile_data;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    ChessState tile = tile_data[i, j];
                    GameObject tile_obj = tile_objs[i, j];
                    TileLogic tile_logic = tile_obj.GetComponent<TileLogic>();
                    tile_logic.set_state(tile);
                }
            }
        }
        cur_side = player_side;

        round_start();

    }

    private void ai_immediately_do_desicion()
    {
        clear_timers();
        random_choice_when_timeout();
    }

    private void single_ai_do_desicion()
    {
        clear_timers();
        // 单线程先实现逻辑
        ChessAction ca = GameUtils.find_best_action(tile_data, n_level, cur_side);
        if (ca.side == ChessState.empty)
        {
            // 这个是无效值
            random_choice_when_timeout();
        }
        else
        { 
            try_set_tile_state(ca.side, ca.logic_x, ca.logic_y);
        }
        
    }

    private void mutil_ai_do_desicion_cb(ChessAction ca)
    {
        // Debug.Log(ca.ToString());
        clear_timers();
        if (ca.side == ChessState.empty)
        {
            // 这个是无效值
            random_choice_when_timeout();
        }
        else
        { 
            try_set_tile_state(ca.side, ca.logic_x, ca.logic_y);
        }
    }

    private void mutil_ai_do_desicion()
    {
        // 分线程进行剪枝计算
        task = new AlgorithmTask();
        task.task_start(tile_data, n_level, cur_side, 8000, mutil_ai_do_desicion_cb);
    }

    private void round_start()
    {
        // 设置当前玩家
        GameController._instance.set_cur_tile(cur_side);

        // 启用10秒倒计时定时器，并同步给UI
        round_outtime_handler = GameController._instance.start_timer(10, random_choice_when_timeout);
        GameController._instance.start_count_down(10);

        if (cur_side != player_side)
        {
            // AI 决策

            // ai_desicion_delay_handler = GameController._instance.start_timer(2, single_ai_do_desicion);
            mutil_ai_do_desicion();

        }
    }

    private void round_end()
    {
        // 清理倒计时定时器
        clear_timers();

        ChessState[,] copy_tile_data = new ChessState[tile_data.GetLength(0), tile_data.GetLength(1)];
        Array.Copy(tile_data, copy_tile_data, tile_data.Length);
        history.Add((cur_side, copy_tile_data));

        // 换手
        if (cur_side != ChessState.circle)
        {
            cur_side = ChessState.circle;
        }
        else
        {
            cur_side = ChessState.cross;
        }

        // 如果没开AI，将玩家也换边
        if (enable_ai != true)
        {
            player_side = cur_side;
        }

        round_start();
    }

    private void clear_timers()
    {
        if (round_outtime_handler > 0)
        {
            GameController._instance.cancel_timer(round_outtime_handler);
            round_outtime_handler = 0;
        }

        if (round_warning_handler > 0)
        {
            GameController._instance.cancel_timer(round_warning_handler);
            round_warning_handler = 0;
        }

        if (ai_desicion_delay_handler > 0)
        { 
            GameController._instance.cancel_timer(ai_desicion_delay_handler);
            ai_desicion_delay_handler = 0;
        }

        // cts?.Cancel();
    }

    private void game_over(RoundResult result)
    {
        history.Clear();
        clear_timers();
        switch (result)
        {
            case RoundResult.circle_win:
                cache_result = "circle is the winner";
                break;
            case RoundResult.cross_win:
                cache_result = "cross is the winner";
                break;
            case RoundResult.draw:
                cache_result = "circle and cross get the draw";
                break;
        }

        GameController._instance.change_stage(GameStage.ending);
    }

    public string get_winner()
    {
        return cache_result;
    }

}
