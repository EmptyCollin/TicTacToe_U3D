using System;
using System.Collections.Generic;
using UnityEngine;


public static class GameUtils
{
    // 检查是否可以放置棋子
    public static bool check_can_place(ChessState[,] tile_data, int logic_x, int logic_y)
    {

        if (0 > logic_x || logic_x >= tile_data.GetLength(1))
        {
            return false;
        }

        if (0 > logic_y || logic_y >= tile_data.GetLength(0))
        {
            return false;
        }

        return tile_data[logic_y, logic_x] == ChessState.empty;
    }

    // 获取所有空的格子位置
    public static List<(int, int)> get_all_empty_tile(ChessState[,] tile_data)
    {
        int row = tile_data.GetLength(0);
        int column = tile_data.GetLength(1);
        List<(int, int)> candidates = new List<(int, int)>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (tile_data[i, j] == ChessState.empty)
                {
                    candidates.Add((i, j));
                }
            }
        }

        return candidates;
    }

    // 比照三阶棋盘的规则，假设n阶的规则需要n个棋子在同行列或对角线
    // 基于这个假设，传入的二维数组宽高相同
    public static RoundResult check_result(ChessState[,] tile_data)
    {
        ChessState chess_win = check_chess_win(tile_data);

        // 没有胜者，要再判断一次是否平局
        if (chess_win == ChessState.empty)
        {
            return check_draw(tile_data);
        }
        else
        {
            if (chess_win == ChessState.circle)
            {
                return RoundResult.circle_win;
            }
            else
            {
                return RoundResult.cross_win;
            }
        }
    }

    // 是否有一方获胜
    public static ChessState check_chess_win(ChessState[,] tile_data)
    {
        int max_cnt = tile_data.GetLength(0);
        int cur_c, cur_r, forward_c, forward_r;
        ChessState cur_data, base_data;

        // 检查行
        forward_c = 1;
        forward_r = 0;

        for (int i = 0; i < max_cnt; i++)
        {
            cur_c = 0;
            base_data = tile_data[i, cur_c];
            if (base_data == ChessState.empty)
            {
                continue;
            }
            cur_c += forward_c;
            bool skip_loop = false;
            while (cur_c < max_cnt)
            {
                cur_data = tile_data[i, cur_c];
                if (cur_data != base_data)
                {
                    skip_loop = true;
                    break;
                }
                cur_c += forward_c;
            }

            if (skip_loop)
            {
                continue;
            }

            return base_data;
        }

        // 检查列
        forward_c = 0;
        forward_r = 1;

        for (int i = 0; i < max_cnt; i++)
        {
            cur_r = 0;
            base_data = tile_data[cur_r, i];
            if (base_data == ChessState.empty)
            {
                continue;
            }
            cur_r += forward_r;
            bool skip_loop = false;
            while (cur_r < max_cnt)
            {
                cur_data = tile_data[cur_r, i];
                if (cur_data != base_data)
                {
                    skip_loop = true;
                    break;
                }
                cur_r += forward_r;
            }

            if (skip_loop)
            {
                continue;
            }

            return base_data;
        }

        // 检查对角线
        forward_c = 1;
        forward_r = 1;

        cur_r = 0;
        cur_c = 0;

        bool valid_result;

        base_data = tile_data[cur_r, cur_c];
        if (base_data != ChessState.empty)
        {
            cur_r += forward_r;
            cur_c += forward_c;
            valid_result = true;
            while (cur_r < max_cnt)
            {
                cur_data = tile_data[cur_r, cur_c];
                if (cur_data != base_data)
                {
                    valid_result = false;
                    break;
                }
                cur_r += forward_r;
                cur_c += forward_c;
            }

            if (valid_result)
            {
                return base_data;
            }
        }

        // 检查反对角线
        forward_c = 1;
        forward_r = -1;

        cur_r = max_cnt - 1;
        cur_c = 0;
        base_data = tile_data[cur_r, cur_c];
        if (base_data != ChessState.empty)
        {
            cur_r += forward_r;
            cur_c += forward_c;
            valid_result = true;
            while (0 <= cur_r)
            {
                cur_data = tile_data[cur_r, cur_c];
                if (cur_data != base_data)
                {
                    valid_result = false;
                    break;
                }
                cur_r += forward_r;
                cur_c += forward_c;
            }

            if (valid_result)
            {
                return base_data;
            }
        }
        return ChessState.empty;
    }

    // 是否平局
    public static RoundResult check_draw(ChessState[,] tile_data)
    {
        int max_cnt = tile_data.GetLength(0);
        for (int i = 0; i < max_cnt; i++)
        {
            for (int j = 0; j < max_cnt; j++)
            {
                if (tile_data[i, j] == ChessState.empty)
                {
                    return RoundResult.new_round;
                }
            }
        }
        return RoundResult.draw;
    }

    // 模拟下一步棋，深拷贝，返回结果棋盘
    // 下棋位置的合法性检查已经在进入前执行过
    // 其实用不上，不如直接回退操作省内存
    // public ChessState[,] tild_do_chess_action(ChessState[,] tile_data, ChessAction ca)
    // {
    //     ChessState[,] new_tile_data = new ChessState[tile_data.GetLength(0), tile_data.GetLength(1)];
    //     Array.Copy(tile_data, new_tile_data, tile_data.Length);

    //     ChessState side = ca.side;
    //     int logic_x = ca.logic_x;
    //     int logic_y = ca.logic_y;

    //     new_tile_data[logic_y, logic_x] = side;
    //     return new_tile_data;
    // }

    public static ChessState get_counter_side(ChessState side)
    {
        if (side == ChessState.circle)
        {
            return ChessState.cross;
        }
        else if (side == ChessState.cross)
        {
            return ChessState.circle;
        }
        return ChessState.empty;
    }

    // 剪枝算法找最优解
    public static ChessAction find_best_action(ChessState[,] tile_data, int max_depth, ChessState init_side)
    {
        ChessAction best_action = new ChessAction();
        int max_score = int.MinValue;

        // print_tile_data(tile_data);

        // 初始化填充,流程上这里不可能为空
        List<(int, int)> open_tiles = get_all_empty_tile(tile_data);

        foreach ((int r, int c) in open_tiles)
        {
            tile_data[r, c] = init_side;
            int cur_score = a_b_test(tile_data, max_depth, int.MinValue, int.MaxValue, get_counter_side(init_side), init_side);
            tile_data[r, c] = ChessState.empty;

            if (cur_score > max_score)
            {
                max_score = cur_score;
                best_action = new ChessAction(init_side, c, r);
            }
        }
        return best_action;
    }

    public static int evaluate_board(ChessState[,] tile_data, ChessState ai_side)
    {
        ChessState result = check_chess_win(tile_data);

        // AI 胜利，返回最大值, 玩家赢则为最小值
        if (result == ai_side)
        {
            return int.MaxValue;
        }
        else if (result == get_counter_side(ai_side))
        {
            return int.MinValue;
        }


        // 评估得分
        // 参照https://blog.csdn.net/m0_74766888/article/details/136971081，通过将剩余空格用某种填满，然后可能达成胜利的情况数量
        // 实际上这种方式略有点粗暴，3、4阶还不错，更高的时候有些笨，算是战未来
        // 增加贪婪式积分逻辑，看当前已有的棋子中可能达成胜利的情况，着眼当下
        // 增加权重，己方赢更重要，当下更重要
        return (
            (get_current_possible_win_cnt(tile_data, ai_side) * 2 - get_current_possible_win_cnt(tile_data, get_counter_side(ai_side))) * 2
            +
            get_furure_possible_win_cnt(tile_data, ai_side) * 2 - get_furure_possible_win_cnt(tile_data, get_counter_side(ai_side))
        );
    }

    private static int get_current_possible_win_cnt(ChessState[,] tile_data, ChessState use_side)
    {        
        int row = tile_data.GetLength(0);
        int column = tile_data.GetLength(1);
        ChessState counter_side = GameUtils.get_counter_side(use_side);
        // 计数
        int valid_cnt = 0;
        int cur_c, cur_r, forward_c, forward_r;
        ChessState cur_data;
        bool skip_loop = false;

        // 检查行
        forward_c = 1;
        forward_r = 0;

        for (int i = 0; i < row; i++)
        {
            cur_c = 0;
            skip_loop = false;
            while (cur_c < column)
            {
                cur_data = tile_data[i, cur_c];
                if (cur_data == counter_side)
                {
                    skip_loop = true;
                    break;
                }
                cur_c += forward_c;
            }

            if (skip_loop)
            {
                continue;
            }
            valid_cnt++;
            // Debug.Log($"row add at {i}");
        }

        // 检查列
        forward_c = 0;
        forward_r = 1;

        for (int i = 0; i < column; i++)
        {
            cur_r = 0;
            skip_loop = false;
            while (cur_r < row)
            {
                cur_data = tile_data[cur_r, i];
                if (cur_data == counter_side)
                {
                    skip_loop = true;
                    break;
                }
                cur_r += forward_r;
            }

            if (skip_loop)
            {
                continue;
            }
            valid_cnt++;
            // Debug.Log($"column add at {i}");
        }

        // 检查对角线
        forward_c = 1;
        forward_r = 1;

        cur_r = 0;
        cur_c = 0;

        skip_loop = false;
        while (cur_r < row && cur_c < column)
        {
            cur_data = tile_data[cur_r, cur_c];
            if (cur_data == counter_side)
            {
                skip_loop = true;
                break;
            }
            cur_r += forward_r;
            cur_c += forward_c;
        }
        if (!skip_loop)
        {
            valid_cnt++;
            // Debug.Log($"counter add");
        }


        // 检查反对角线
        forward_c = 1;
        forward_r = -1;

        cur_r = row - 1;
        cur_c = 0;

        skip_loop = false;
        while (0 <= cur_r && cur_c < column)
        {
            cur_data = tile_data[cur_r, cur_c];
            if (cur_data == counter_side)
            {
                skip_loop = true;
                break;
            }
            cur_r += forward_r;
            cur_c += forward_c;
        }

        if (!skip_loop)
        {
            valid_cnt++;
            // Debug.Log($"anti-counter add");
        }

        // print_tile_data(tile_data);
        // Debug.Log(valid_cnt);
        return valid_cnt;

    }

    private static int get_furure_possible_win_cnt(ChessState[,] original_tile_data, ChessState use_side)
    {

        // 简单起见直接深拷贝
        ChessState[,] tile_data = new ChessState[original_tile_data.GetLength(0), original_tile_data.GetLength(1)];
        Array.Copy(original_tile_data, tile_data, original_tile_data.Length);

        // 填充空格
        int row = tile_data.GetLength(0);
        int column = tile_data.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (tile_data[i, j] == ChessState.empty)
                {
                    tile_data[i, j] = use_side;
                }
            }
        }

        // 计数
        int valid_cnt = 0;
        int cur_c, cur_r, forward_c, forward_r;
        ChessState cur_data;
        bool skip_loop = false;

        // 检查行
        forward_c = 1;
        forward_r = 0;

        for (int i = 0; i < row; i++)
        {
            cur_c = 0;
            skip_loop = false;
            while (cur_c < column)
            {
                cur_data = tile_data[i, cur_c];
                if (cur_data != use_side)
                {
                    skip_loop = true;
                    break;
                }
                cur_c += forward_c;
            }

            if (skip_loop)
            {
                continue;
            }
            valid_cnt++;
            // Debug.Log($"row add at {i}");
        }

        // 检查列
        forward_c = 0;
        forward_r = 1;

        for (int i = 0; i < column; i++)
        {
            cur_r = 0;
            skip_loop = false;
            while (cur_r < row)
            {
                cur_data = tile_data[cur_r, i];
                if (cur_data != use_side)
                {
                    skip_loop = true;
                    break;
                }
                cur_r += forward_r;
            }

            if (skip_loop)
            {
                continue;
            }
            valid_cnt++;
            // Debug.Log($"column add at {i}");
        }

        // 检查对角线
        forward_c = 1;
        forward_r = 1;

        cur_r = 0;
        cur_c = 0;

        skip_loop = false;
        while (cur_r < row && cur_c < column)
        {
            cur_data = tile_data[cur_r, cur_c];
            if (cur_data != use_side)
            {
                skip_loop = true;
                break;
            }
            cur_r += forward_r;
            cur_c += forward_c;
        }
        if (!skip_loop)
        {
            valid_cnt++;
            // Debug.Log($"counter add");
        }


        // 检查反对角线
        forward_c = 1;
        forward_r = -1;

        cur_r = row - 1;
        cur_c = 0;

        skip_loop = false;
        while (0 <= cur_r && cur_c < column)
        {
            cur_data = tile_data[cur_r, cur_c];
            if (cur_data != use_side)
            {
                skip_loop = true;
                break;
            }
            cur_r += forward_r;
            cur_c += forward_c;
        }

        if (!skip_loop)
        {
            valid_cnt++;
            // Debug.Log($"anti-counter add");
        }

        // print_tile_data(tile_data);
        // Debug.Log(valid_cnt);
        return valid_cnt;

    }

    private static int a_b_test(ChessState[,] tile_data, int depth, int alpha, int beta, ChessState cur_side, ChessState ai_side)
    {
        // 递归结束
        List<(int, int)> open_tiles = get_all_empty_tile(tile_data);
        int result = evaluate_board(tile_data, ai_side);
        if (open_tiles.Count == 0 || depth == 0 || result == int.MaxValue || result == int.MinValue)
            return result;

        // AI,越大越好
        if (cur_side == ai_side)
        {
            int max_score = int.MinValue;

            foreach ((int r, int c) in open_tiles)
            {
                // 模拟一次操作，出栈后复原
                tile_data[r, c] = cur_side;
                int cur_score = a_b_test(tile_data, depth - 1, alpha, beta, get_counter_side(ai_side), ai_side);
                tile_data[r, c] = ChessState.empty;
                if (cur_score > max_score)
                {
                    max_score = cur_score;
                }
                alpha = Math.Max(alpha, cur_score);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return max_score;
        }
        // 目标是使玩家收益越小越好
        else
        {
            int min_score = int.MaxValue;
            foreach ((int r, int c) in open_tiles)
            {
                tile_data[r, c] = cur_side;
                int cur_score = a_b_test(tile_data, depth - 1, alpha, beta, ai_side, ai_side);
                tile_data[r, c] = ChessState.empty;
                if (cur_score < min_score)
                {
                    min_score = cur_score;
                }
                beta = Math.Min(beta, cur_score);
                // 这里的条件还是beta<=alpha
                // ！！！因为玩家收益是负值！！！
                if (beta <= alpha)
                {
                    break;
                }
            }
            return min_score;
        }
    }
    
    public static void print_tile_data(ChessState[,] tile_data)
    {
        string s = "print tile data with:\n";
        for (int i = tile_data.GetLength(0) -1; i >= 0; i--)
        {
            for (int j = 0; j < tile_data.GetLength(1); j++)
            {
                s += string.Format("{0,-20}", tile_data[i, j].ToString());
            }
            s += "\n";
        }
        Debug.Log(s);
    }

}

