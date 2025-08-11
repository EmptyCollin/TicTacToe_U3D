using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AISetting : MonoBehaviour
{
    private Toggle ison_chk;

    public GameObject slider_diff;
    private Slider slider_comp;

    // 通过这里简单塞个操作回调
    private MainUI main_ui;


    void Awake()
    {
        ison_chk = gameObject.GetComponent<Toggle>();

        if (slider_diff == null)
        {
            Debug.Log("slider_diff not found");
        }
        slider_comp = slider_diff.GetComponent<Slider>();

        ison_chk.onValueChanged.AddListener(OnCheckBoxChanged);
        slider_comp.onValueChanged.AddListener((float v)=>main_ui?.update_ai_setting());
        OnCheckBoxChanged(ison_chk.isOn);
    }

    void OnCheckBoxChanged(bool b)
    {
        slider_diff.SetActive(b);
        main_ui?.update_ai_setting();
    }

    public void set_owner(MainUI main)
    {
        main_ui = main;
    }

    public (bool, int) get_ai_data()
    {
        return (ison_chk.isOn, (int)slider_comp.value);
    }

    public void set_ai_data(bool is_on, int n_level)
    {
        ison_chk.isOn = is_on;
        slider_comp.value = n_level;
    }

}
