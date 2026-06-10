using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArithmeticGameplay : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text questionText;

    public Button answer1;
    public Button answer2;
    public Button answer3;

    public TMP_Text answer1Text;
    public TMP_Text answer2Text;
    public TMP_Text answer3Text;

    [Header("Health")]
    public Image healthImage;

    [Tooltip("Assign 4 sprites: index 0 = 1 wrong answer, index 1 = 2 wrong, index 2 = 3 wrong, index 3 = 4 wrong (game over)")]
    public Sprite[] healthStages = new Sprite[4];

    int correctAnswerIndex;
    int wrongAnswerCount;

    void Start()
    {
        wrongAnswerCount = 0;
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        int target = Random.Range(-20, 21);

        questionText.text = target.ToString();

        string correctExpression = CreateExpression(target);

        List<string> answers = new List<string>();
        answers.Add(correctExpression);

        while (answers.Count < 3)
        {
            int wrongTarget = target;

            while (wrongTarget == target)
            {
                wrongTarget = Random.Range(-20, 21);
            }

            string wrongExpression = CreateExpression(wrongTarget);

            if (!answers.Contains(wrongExpression))
                answers.Add(wrongExpression);
        }

        // Shuffle
        for (int i = 0; i < answers.Count; i++)
        {
            int rand = Random.Range(i, answers.Count);

            string temp = answers[i];
            answers[i] = answers[rand];
            answers[rand] = temp;
        }

        answer1Text.text = answers[0];
        answer2Text.text = answers[1];
        answer3Text.text = answers[2];

        correctAnswerIndex = answers.IndexOf(correctExpression);
    }

    string CreateExpression(int result)
    {
        int x = Random.Range(-10, 11);
        int y = result - x;

        return $"({x})+({y})";
    }

    public void SelectAnswer(int buttonIndex)
    {
        if (buttonIndex == correctAnswerIndex)
        {
            Debug.Log("Correct!");
            GenerateQuestion();
        }
        else
        {
            Debug.Log("Wrong!");

            wrongAnswerCount++;

            if (wrongAnswerCount <= healthStages.Length && healthStages[wrongAnswerCount - 1] != null)
            {
                healthImage.sprite = healthStages[wrongAnswerCount - 1];
            }

            if (wrongAnswerCount >= healthStages.Length)
            {
                Debug.Log("Game Over!");
                answer1.interactable = false;
                answer2.interactable = false;
                answer3.interactable = false;
            }
        }
    }
}