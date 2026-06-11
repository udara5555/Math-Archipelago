using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArithmeticGameplay : MonoBehaviour
{
    enum OperationMode { Addition, Subtraction, Multiplication, Division }
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
    int correctCount;
    OperationMode currentMode;

    void Start()
    {
        wrongAnswerCount = 0;
        correctCount = 0;
        currentMode = OperationMode.Addition;
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        int a, b;
        string op;

        if (currentMode == OperationMode.Division)
        {
            // For division: pick divisor and quotient, compute dividend
            // so that dividend ÷ divisor = quotient (always integer)
            int divisor = Random.Range(2, 11);
            int quotient;
            do
            {
                quotient = Random.Range(2, 11);
            } while (quotient == divisor);

            a = divisor * quotient; // dividend
            b = divisor;
            op = "\u00F7";
        }
        else
        {
            // Addition, Subtraction, or Multiplication: pick two distinct positive numbers
            do
            {
                a = Random.Range(1, 11);
                b = Random.Range(1, 11);
            } while (a == b);

            if (currentMode == OperationMode.Multiplication)
                op = "\u00D7";
            else if (currentMode == OperationMode.Subtraction)
                op = "\u2212";
            else
                op = "+";
        }

        // Build all 4 sign combinations: (±a) op (±b)
        int[] signs = { 1, -1 };
        List<(string expression, int value)> combos = new List<(string, int)>();

        foreach (int sa in signs)
        {
            foreach (int sb in signs)
            {
                int va = sa * a;
                int vb = sb * b;
                int result;

                switch (currentMode)
                {
                    case OperationMode.Subtraction:
                        result = va - vb;
                        break;
                    case OperationMode.Multiplication:
                        result = va * vb;
                        break;
                    case OperationMode.Division:
                        result = va / vb;
                        break;
                    default:
                        result = va + vb;
                        break;
                }

                combos.Add(($"({va}){op}({vb})", result));
            }
        }

        // Pick one combo as the correct answer
        int correctPick = Random.Range(0, combos.Count);
        string correctExpression = combos[correctPick].expression;
        int target = combos[correctPick].value;

        questionText.text = target.ToString();

        // Collect wrong answers (different value from target)
        List<(string expression, int value)> wrongCombos = new List<(string, int)>();
        for (int i = 0; i < combos.Count; i++)
        {
            if (i != correctPick && combos[i].value != target)
                wrongCombos.Add(combos[i]);
        }

        // Shuffle wrong combos
        for (int i = 0; i < wrongCombos.Count; i++)
        {
            int rand = Random.Range(i, wrongCombos.Count);
            var temp = wrongCombos[i];
            wrongCombos[i] = wrongCombos[rand];
            wrongCombos[rand] = temp;
        }

        List<string> answers = new List<string>();
        answers.Add(correctExpression);
        answers.Add(wrongCombos[0].expression);
        answers.Add(wrongCombos[1].expression);

        // Shuffle all 3 answers
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


    public void SelectAnswer(int buttonIndex)
    {
        if (buttonIndex == correctAnswerIndex)
        {
            Debug.Log("Correct!");
            correctCount++;

            if (correctCount >= 5)
            {
                if (currentMode == OperationMode.Addition)
                {
                    Debug.Log("Switching to Subtraction!");
                    currentMode = OperationMode.Subtraction;
                    correctCount = 0;
                }
                else if (currentMode == OperationMode.Subtraction)
                {
                    Debug.Log("Switching to Multiplication!");
                    currentMode = OperationMode.Multiplication;
                    correctCount = 0;
                }
                else if (currentMode == OperationMode.Multiplication)
                {
                    Debug.Log("Switching to Division!");
                    currentMode = OperationMode.Division;
                    correctCount = 0;
                }
                else if (currentMode == OperationMode.Division)
                {
                    Debug.Log("Congratulations!");
                    answer1.gameObject.SetActive(false);
                    answer2.gameObject.SetActive(false);
                    answer3.gameObject.SetActive(false);
                    return;
                }
            }

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