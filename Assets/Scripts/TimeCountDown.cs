using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCountDown : MonoBehaviour
{
    private Scrollbar scrollbar;
    private float remains_time = 0;
    private float total_time = 1;
    private bool need_tick = false;

    // Start is called before the first frame update
    void Awake()
    {
        scrollbar = gameObject.GetComponent<Scrollbar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (need_tick == false)
        {
            return;
        }

        if (remains_time > 0)
        {
            remains_time -= Time.deltaTime;
        }
        else
        {
            remains_time = 0;
            need_tick = false;
        }
        scrollbar.size = Math.Max(0, remains_time) / total_time;
    }

    public void start_count_down(float time)
    {
        remains_time = time;
        total_time = time;
        need_tick = true;
    }
}
