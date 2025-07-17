using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    Color color;
    Button button;
    public UnityAction<Color> OnClickColorButton;

    void Start()
    {
        color = GetComponent<Image>().color;
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClickButton);
    }

    void OnClickButton(){
        OnClickColorButton?.Invoke(color);
    }
}
