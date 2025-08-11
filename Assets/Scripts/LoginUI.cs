using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [SerializeField]
    public GameObject title_label;
    private TextMeshProUGUI title_txt;

    [SerializeField]
    public GameObject sld_n_grid;
    private Slider n_grid_slider;

    [SerializeField]
    public GameObject chk_ai;
    private AISetting ai_setting;
    private string config_path = "Assets/Configuraion.txt";

    void Awake()
    {
        if (title_label == null)
        {
            Debug.Log("title label not found");
        }
        title_txt = title_label.GetComponent<TextMeshProUGUI>();


        if (sld_n_grid == null)
        {
            Debug.Log("sld_n_grid not found");
        }
        n_grid_slider = sld_n_grid.GetComponent<Slider>();

        if (chk_ai == null)
        {
            Debug.Log("chk_ai not found");
        }
        ai_setting = chk_ai.GetComponent<AISetting>();

        read_config();
    }


    // Update is called once per frame
    void Update()
    {

    }


    public void on_tile_click(ChessState state)
    {
        int grid_length = (int)n_grid_slider.value;
        bool ai_on;
        int ai_deep;
        (ai_on, ai_deep) = ai_setting.get_ai_data();
        // Debug.Log($"{state} is down, n level {grid_length}, use ai {ai_chk.isOn}, ai deep {ai_deep}");

        // 如果选择后手且AI没开则强制打开AI，否则流程走不下去
        if (state == ChessState.cross && ai_on == false)
        {
            ai_on = true;
            ai_deep = 3;
        }
        GameController._instance.set_game_setting(grid_length, grid_length, state, ai_on, ai_deep);
        GameController._instance.change_stage(GameStage.gaming);

        save_config();
    }

    private void save_config()
    {
        string content = package_config();
        File.WriteAllText(config_path, content);
    }

    private void read_config()
    {
        if (File.Exists(config_path))
        {
            string content = File.ReadAllText(config_path);
            unpack_config(content);
        }
    }

    private string package_config()
    {
        int grid_length = (int)n_grid_slider.value;
        (bool ai_on, int ai_deep) = ai_setting.get_ai_data();
        string s = "";
        s += $"grid_length={grid_length}\n";
        s += $"ai_on={ai_on}\n";
        s += $"ai_deep={ai_deep}";
        return s;
    }

    private void unpack_config(string content)
    {
        try
        {
            int grid_length = 3;
            bool ai_on = false;
            int ai_deep = 3;
            string[] single_contents = content.Split("\n");
            foreach (string single_content in single_contents)
            {
                string[] d = single_content.Split("=");
                string key = d[0];
                string value = d[1];

                switch (key)
                {
                    case "grid_length":
                        grid_length = Convert.ToInt32(value);
                        break;
                    case "ai_on":
                        ai_on = Convert.ToBoolean(value);
                        break;
                    case "ai_deep":
                        ai_deep = Convert.ToInt32(value);
                        break;
                }
            }
            n_grid_slider.value = grid_length;
            ai_setting.set_ai_data(ai_on, ai_deep);
        }
        catch
        {
            Debug.LogError("Broken Configuraion.txt, use default vaule");
        }
        
    }

    public void asyn_ai_setting(bool ai_on, int ai_deep)
    {
        ai_setting.set_ai_data(ai_on, ai_deep);
        save_config();
    }

    public void set_active(bool b)
    {
        gameObject.SetActive(b);
    }

    public void set_content(string s)
    {
        if (s == "")
        {
            s = "Welcome to TicTacToe";
        }

        title_txt.text = s;

    }
}
