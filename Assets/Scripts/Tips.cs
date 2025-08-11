using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tips : MonoBehaviour {
    public GameObject panel_tips;
    public GameObject txt_tips;
    private TextMeshProUGUI tips_txt;

    void Awake()
    {
        panel_tips.SetActive(false);
        tips_txt = txt_tips.GetComponent<TextMeshProUGUI>();
    }

    public void show_tips(string content)
    {
        tips_txt.text = content;
        panel_tips.SetActive(true);
        StartCoroutine(HideAfterDelay(2f)); // 2秒后隐藏
    }

    IEnumerator HideAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        panel_tips.SetActive(false);
    }
}