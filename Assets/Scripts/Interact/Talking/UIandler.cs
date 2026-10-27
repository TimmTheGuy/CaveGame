using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIandler : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public TMP_Text TXT;
    public GameObject LayoutGroup;

    List<GameObject> buttonlist = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(string txt)
    {
        TXT.text = txt;
    }

    public void AskQuestion(TalkObject buttontxt)
    {
        GetComponent<Animator>().SetBool("TALKING", true);
        GetComponent<Animator>().Play("OpenSelect");
        foreach(TalkObject.DataDype line in buttontxt.DialogueTree)
        {
            GameObject temp = Instantiate(ButtonPrefab, LayoutGroup.transform);
            temp.GetComponent<UIButtonTest>().UpdateText(line.Dialogue);
            buttonlist.Add(temp);
        }
    }

    public void StartTalking()
    {
        GetComponent<Animator>().Play("Talk");
        GetComponent<Animator>().SetBool("TALKING", true);
    }

    public void UpdateSelect(int selected)
    {
        foreach(GameObject tempbut in buttonlist)
        {
            tempbut.GetComponent<UIButtonTest>().ChangeColor(Color.white);
        }
        buttonlist[selected].GetComponent<UIButtonTest>().ChangeColor(Color.yellow);
    }

    public void ResetButtons()
    {
        if (buttonlist.Count > 0)
        {
            foreach(GameObject tempbut in buttonlist)
            {
                Destroy(tempbut);
            }
        }
        buttonlist.Clear();
    }

    public void LeaveTalking()
    {
        GetComponent<Animator>().SetBool("TALKING", false);
    }
}
