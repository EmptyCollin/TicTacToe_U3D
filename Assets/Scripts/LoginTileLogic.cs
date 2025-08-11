using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoginTileLogic : MonoBehaviour

{

    [SerializeField]
    public ChessState state;

    [SerializeField]
    public GameObject login_ui_panel;
    private LoginUI login_ui;

    void Awake()
    {
        if (login_ui_panel == null)
        {
            Debug.LogError("未设定login_ui_panel");
        }
        login_ui = login_ui_panel.GetComponent<LoginUI>();
    }

    void OnMouseDown()
    {
        login_ui?.on_tile_click(state);
    }
}
