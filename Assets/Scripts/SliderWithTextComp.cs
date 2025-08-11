using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderWithTextComp : MonoBehaviour
{
    public string text_format;

    public GameObject text_obj;
    private TextMeshProUGUI text_comp;
    private Slider slider_comp;

    // Start is called before the first frame update
    void Awake()
    {
        if (text_obj == null)
        {
            Debug.Log("text obj not found");
        }
        text_comp = text_obj.GetComponent<TextMeshProUGUI>();
        slider_comp = gameObject.GetComponent<Slider>();
    }

    void Start()
    {
        slider_comp.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(slider_comp.value);
    }


    void OnSliderValueChanged(float value)
    {
        int int_value = (int)value;
        string s = string.Format(text_format, int_value);
        text_comp.text = s;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
