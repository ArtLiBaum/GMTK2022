
using UnityEngine;

using Yarn.Unity;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private string StoryStartNode;

    private DialogueRunner _dialogueRunner;
    private void Start()
    {
        _dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    public void StartGame()
    {
        //Start Dialogue
        _dialogueRunner.StartDialogue(StoryStartNode);
    }
}
