using UnityEngine;
using UnityEngine.InputSystem;
using System;


public class Speaker : InteractableBase
{
    public TalkObject CurrentDialogue;
    int currentLine;
    public Transform[] CamPos;
    int currentCam;

    [Header("Other Stuff")]
    GameObject MainCamera;
    bool Returning;
    PlayerInput ACTONS;
    GameObject SpeechUI;
    Transform returnpos;
    bool IsUse;
    bool buttondown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCam = 0;
        currentLine = 0;
        MainCamera = GameObject.FindWithTag("MainCamera");
        SpeechUI = GameObject.FindWithTag("UITEXT");
        Returning = false;
        ACTONS = GetComponent<PlayerInput>();
        ACTONS.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsUse)
        {
            Transform TargetPos = Returning ? returnpos : CamPos[currentCam];
            if (Returning)
            {
                ACTONS.enabled = false;
            }

            if(Vector3.Distance(MainCamera.transform.position, TargetPos.position) > 0.1f)
            {
                MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, TargetPos.position, Time.deltaTime * 5);
                MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, TargetPos.rotation, Time.deltaTime * 5);
            }
            else
            {
                if (Returning)
                {
                    print("Done");
                    Returning = false;
                    EndTalking();
                }
            }

            if(ACTONS.actions["ESC"].ReadValue<float>() > 0)
            {
                leave();
            }

            if(ACTONS.actions["Interact"].WasPressedThisFrame())
            {
                SpeechUI.GetComponent<UIandler>().ResetButtons();
                UpdateDialogue();
            }
        }
    }
    
    void leave()
    {
        currentLine = 0;
        Returning = true;
        SpeechUI.GetComponent<UIandler>().ResetButtons();

        switch(CurrentDialogue.DialogueType)
        {
            case TalkObject.speechType.Talking:
            SpeechUI.GetComponent<Animator>().Play("Quiet");
            break;
            case TalkObject.speechType.Question:
            SpeechUI.GetComponent<Animator>().Play("CloseSelect");
            break;
        }
        SpeechUI.GetComponent<UIandler>().LeaveTalking();
    }

    void NewDialogue(TalkObject newdio)
    {
        currentLine = 0;
        switch(newdio.DialogueType)
        {
            case TalkObject.speechType.Talking:
            SpeechUI.GetComponent<UIandler>().StartTalking();
            break;
            case TalkObject.speechType.Question:
            SpeechUI.GetComponent<UIandler>().AskQuestion(newdio);
            break;
        }
        CurrentDialogue = newdio;
        currentCam = CurrentDialogue.DialogueTree[currentLine].CamPos;
        
        if(CurrentDialogue.DialogueType == TalkObject.speechType.Talking)
        {
            currentLine = -1;
            UpdateDialogue();
        }
    }

    void UpdateDialogue()
    {
        if(CurrentDialogue.DialogueType == TalkObject.speechType.Talking)
        {
            currentLine += 1;
            if(currentLine > CurrentDialogue.DialogueTree.Length - 1)
            {
                leave();
            }
            SpeechUI.GetComponent<UIandler>().UpdateText(CurrentDialogue.DialogueTree[currentLine].Dialogue);
           currentCam = CurrentDialogue.DialogueTree[currentLine].CamPos;
            if(CurrentDialogue.DialogueTree[currentLine].Answer != null)
            {
                bool forceLeave = CurrentDialogue.DialogueTree[currentLine].ForceLeave;
                NewDialogue(CurrentDialogue.DialogueTree[currentLine].Answer);
                if(forceLeave)
                {
                    leave();
                }
            }
        }
        else
        {
            if(CurrentDialogue.DialogueTree[currentLine].Answer != null)
            {
                NewDialogue(CurrentDialogue.DialogueTree[currentLine].Answer);
            }
            else
            {
                leave();
            }
        }
    }

    public override void OnInteracted()
    {
        StartTalking2();
    }

    public void StartTalking2()
    {
        MainCamera.transform.parent = transform;
        ACTONS.enabled = true;
        Returning = false;
        currentCam = 0;
        currentLine = 0;
        returnpos = GameObject.FindWithTag("Player").GetComponent<PLAYER>().CamParent.transform;
        IsUse = true;
        NewDialogue(CurrentDialogue);
    }

    public void EndTalking()
    {
        currentLine = 0;
        ACTONS.enabled = false;
        IsUse = false;
        Returning = false;
        GameObject.FindWithTag("Player").GetComponent<PLAYER>().ResetCam();
        print("No more talk");
    }

    public void OnMove(InputValue val)
    {
        if(!buttondown && CurrentDialogue.DialogueType == TalkObject.speechType.Question)
        {
            currentLine = Mathf.Clamp(currentLine - Convert.ToInt32(val.Get<Vector2>().y), 0, CurrentDialogue.DialogueTree.Length - 1);
            buttondown = true;
            SpeechUI.GetComponent<UIandler>().UpdateSelect(currentLine);
            print(currentLine);
        }
        else
        {
            if(val.Get<Vector2>() == new Vector2(0, 0))
            {
                buttondown = false;
            }
        }
    }
}
