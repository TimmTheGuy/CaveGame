using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIButtonTest : MonoBehaviour
{
    public TMP_Text texts;
    Image imsg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imsg = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(string Text)
    {
        texts.text = Text;
    }

    public void ChangeColor(Color c)
    {
        imsg.color = c;
    }
}
