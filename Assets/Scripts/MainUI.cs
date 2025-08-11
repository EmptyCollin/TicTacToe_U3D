using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUI : MonoBehaviour
{
    public GameObject grp_tips;
    private Tips tips;
    public GameObject time_bar;
    private TimeCountDown time_count_down;
    public GameObject cur_player;
    private TileLogic cur_tile;

    public GameObject btn_reset;
    public GameObject btn_undo;
    public GameObject chk_ai;
    private AISetting ai_setting;

    void Awake()
    {
        if (grp_tips == null)
        {
            Debug.Log("grp_tips not found");
        }
        tips = grp_tips.GetComponent<Tips>();

        if (time_bar == null)
        {
            Debug.Log("time_bar not found");
        }
        time_count_down = time_bar.GetComponent<TimeCountDown>();

        if (cur_player == null)
        {
            Debug.Log("cur_player not found");
        }
        cur_tile = cur_player.GetComponent<TileLogic>();

        if (btn_reset == null)
        {
            Debug.Log("btn_reset not found");
        }
        btn_reset.GetComponent<Button>().onClick.AddListener(() => GameController._instance.create_board());

        if (btn_undo == null)
        {
            Debug.Log("btn_undo not found");
        }
        btn_undo.GetComponent<Button>().onClick.AddListener(() => GameController._instance.req_board_undo());

        if (chk_ai == null)
        {
            Debug.Log("chk_ai not found");
        }
        ai_setting = chk_ai.GetComponent<AISetting>();
        ai_setting.set_owner(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void set_active(bool b)
    {
        gameObject.SetActive(b);
    }


    public void show_tips(string content)
    {
        tips.show_tips(content);
    }

    public void start_count_down(float time)
    {
        time_count_down.start_count_down(time);
    }

    public void set_cur_tile(ChessState state)
    {
        cur_tile.set_state(state);
    }

    public void set_ai_data(bool enable_ai, int n_lv)
    {
        ai_setting.set_ai_data(enable_ai, n_lv);
    }

    public void update_ai_setting()
    {
        (bool enable_ai, int n_lv) = ai_setting.get_ai_data();
        GameController._instance.change_ai_setting(enable_ai, n_lv);        
    }

}
