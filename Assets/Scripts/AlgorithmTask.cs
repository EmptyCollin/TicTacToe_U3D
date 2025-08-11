using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AlgorithmTask
{
    private CancellationTokenSource _cts;
    private bool drop_result = false;
    private readonly ChessSearch _searcher = new ChessSearch();
    private Action<ChessAction> cb;

    public void task_start(ChessState[,] tile_data, int n_level, ChessState initSide, int thinking_time, Action<ChessAction> callback)
    {
        cb = callback;
        _cts = new CancellationTokenSource();
        drop_result = false;
        _cts.CancelAfter(thinking_time);
        Task.Run(() => do_search(tile_data, n_level, initSide, _cts.Token), _cts.Token);
    }

    private void do_search(ChessState[,] original_tile_data, int n_level, ChessState initSide, CancellationToken token)
    {
        try
        {
            // 深拷贝棋盘以免数据异步，比如悔棋
            ChessState[,] tile_data = new ChessState[original_tile_data.GetLength(0), original_tile_data.GetLength(1)];
            Array.Copy(original_tile_data, tile_data, original_tile_data.Length);
            _searcher.find_best_action(tile_data, n_level, initSide, token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("task thread over time");
        }
        finally
        {
            send_result_to_main();
        }
    }

    private void send_result_to_main()
    {
        // Debug.Log("send_result_to_main" + drop_result);
        if (drop_result)
        {
            return;
        }
        ChessAction ca = _searcher.get_best_action();
        ThreadDispatcher.Enqueue(() => cb(ca));
    }

    public void cancel_immediately_and_drop_result()
    {
        // Debug.Log("cancel_immediately_and_drop_result");
        _cts.Cancel();
        drop_result = true;
    }

}