using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimerMgr : MonoBehaviour
{
    private static TimerMgr _instance;
    private Dictionary<int, Coroutine> timer_dict = new Dictionary<int, Coroutine>();
    private int cur_idx = 1; // 从1开始，便于逻辑判空

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
    }

    // 启动定时器并返回唯一ID
    public int start_timer(float duration, System.Action callback)
    {
        int idx = cur_idx++;
        timer_dict[idx] = StartCoroutine(TimerRoutine(idx, duration, callback));
        return idx;
    }

    private IEnumerator TimerRoutine(int id, float duration, System.Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
        timer_dict.Remove(id);
    }

    // 通过ID提前终止
    public void cancel_timer(int id)
    {
        if (timer_dict.TryGetValue(id, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            timer_dict.Remove(id);
        }
    }
}