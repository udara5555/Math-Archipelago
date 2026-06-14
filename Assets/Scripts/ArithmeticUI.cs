using UnityEngine;

public class ArithmeticUI : MonoBehaviour
{
    public GameObject instructionsImage;
    public GameObject instructionsPanel;

    [Tooltip("Assign all content GameObjects inside InstructionsPanel: Addition, Substraction, Multiplication, Division")]
    public GameObject[] instructionContents;

    void Start()
    {
        instructionsImage.SetActive(false);
        instructionsPanel.SetActive(false);
    }

    public void ToggleInstructions()
    {
        instructionsImage.SetActive(!instructionsImage.activeSelf);
    }

    public void ShowInstructionPanel(GameObject content)
    {
        // Disable all content first
        foreach (var item in instructionContents)
        {
            item.SetActive(false);
        }

        // Enable the panel and the selected content
        instructionsPanel.SetActive(true);
        content.SetActive(true);
    }

    public void CloseInstructionsPanel()
    {
        instructionsPanel.SetActive(false);
    }
}
