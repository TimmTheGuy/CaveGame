using UnityEngine;

public class PlayAnimOnInteraact : InteractableBase
{
    public string BoolName;
    Animator Anim;
    bool toggled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInteracted()
    {
        toggled = toggled ? false : true;
        Anim.SetBool(BoolName, toggled);
    }
}
