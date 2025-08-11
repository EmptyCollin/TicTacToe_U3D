using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class ChessSearch
{
    // GameUtils定义为了静态类，各种不方便，这里也是实验性的，不要污染GameUtils
    // 如果返回默认值就是一轮都算不出，直接超时了
    private (ChessAction ca, int ab_score) current_best = (new ChessAction(ChessState.empty, 0, 0), 0);

    public void find_best_action(ChessState[,] tile_data, int max_depth, ChessState init_side, CancellationToken token)
    {
        int max_score = int.MinValue;

        // 初始化填充,流程上这里不可能为空
        List<(int, int)> open_tiles = GameUtils.get_all_empty_tile(tile_data);

        // 实际测试下来，如果棋盘过大，剪枝的效率其实很低
        // 应该是规则限的太死了，要链长与边长相等，导致评估函数输出大量相同值
        // 因此在棋盘过大时这里近似于一个指数，用一个相对大数作为约束吧，实际上应该更加性能来定
        int threshold = 100000000;
        int asume_cnt = 1;
        int init_cnt = open_tiles.Count;
        int d = max_depth;
        while (d > 0 && init_cnt > 1)
        {
            asume_cnt *= init_cnt;
            init_cnt--;
            d--;
            if (asume_cnt > threshold)
            {
                break;
            }
        }
        if (asume_cnt <= threshold)
        {
            foreach ((int r, int c) in open_tiles)
            {
                tile_data[r, c] = init_side;
                int cur_score = a_b_test(tile_data, max_depth, int.MinValue, int.MaxValue, GameUtils.get_counter_side(init_side), init_side, token);
                tile_data[r, c] = ChessState.empty;

                if (cur_score > max_score)
                {
                    max_score = cur_score;
                    current_best = (new ChessAction(init_side, c, r), cur_score);

                }
            }
        }
        // 这里直接搜索一层作为代替
        else
        {
            foreach ((int r, int c) in open_tiles)
            {

                tile_data[r, c] = init_side;
                int cur_score = GameUtils.evaluate_board(tile_data, init_side);
                tile_data[r, c] = ChessState.empty;

                if (cur_score > max_score)
                {
                    max_score = cur_score;
                    current_best = (new ChessAction(init_side, c, r), cur_score);

                }

                // 最少跑一次
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException("over time");
                }
            }

        }

    }

    private int a_b_test(ChessState[,] tile_data, int depth, int alpha, int beta, ChessState cur_side, ChessState ai_side, CancellationToken token)
    {

        // 加了个提前中断
        if (token.IsCancellationRequested)
        {
            throw new OperationCanceledException("over time");
        }
            
        // 递归结束
        List<(int, int)> open_tiles = GameUtils.get_all_empty_tile(tile_data);
        int result = GameUtils.evaluate_board(tile_data, ai_side);
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
                int cur_score = a_b_test(tile_data, depth - 1, alpha, beta, GameUtils.get_counter_side(ai_side), ai_side, token);
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
                int cur_score = a_b_test(tile_data, depth - 1, alpha, beta, ai_side, ai_side, token);
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

    // 获取当前最优解（线程安全）
    public ChessAction get_best_action()
    {
        return current_best.ca;
    }
    
}