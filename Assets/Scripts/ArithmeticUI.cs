using UnityEngine;

public class ArithmeticUI : MonoBehaviour
{
    public GameObject instructionsImage;

    void Start()
    {
        instructionsImage.SetActive(false);
    }

    public void ToggleInstructions()
    {
        instructionsImage.SetActive(!instructionsImage.activeSelf);
    }
}
