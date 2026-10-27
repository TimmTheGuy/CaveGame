using UnityEngine;

[CreateAssetMenu(menuName = "Text Object")]
public class TalkObject : ScriptableObject
{
    public enum speechType
    {
        Talking,
        Question
    }
    public speechType DialogueType;

    [System.Serializable]
    public struct DataDype
    {
        public string Dialogue;
        public TalkObject Answer;
        public int CamPos;
        public bool ForceLeave;
    }

    public DataDype[] DialogueTree;
}
