using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MathMissingNumberGame : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string questionText;
        public int optionA;
        public int optionB;
        public int optionC;
        public int correctAnswer;
    }

    [Header("UI References")]
    public TMP_Text levelText;
    public TMP_Text scoreText;
    public TMP_Text questionText;
    public TMP_Text feedbackText;

    public Button answerButtonA;
    public Button answerButtonB;
    public Button answerButtonC;
    public Button nextButton;

    public TMP_Text answerButtonAText;
    public TMP_Text answerButtonBText;
    public TMP_Text answerButtonCText;

    [Header("Levels")]
    public List<LevelData> levels = new List<LevelData>();

    private int currentLevelIndex = 0;
    private int score = 0;
    private bool hasAnswered = false;

    void Start()
    {
        nextButton.gameObject.SetActive(false);
        LoadLevel();
    }

    void LoadLevel()
    {
        if (currentLevelIndex >= levels.Count)
        {
            ShowGameComplete();
            return;
        }

        LevelData currentLevel = levels[currentLevelIndex];

        levelText.text = "Level " + (currentLevelIndex + 1);
        scoreText.text = "Score: " + score;
        questionText.text = currentLevel.questionText;
        feedbackText.text = "";

        answerButtonAText.text = currentLevel.optionA.ToString();
        answerButtonBText.text = currentLevel.optionB.ToString();
        answerButtonCText.text = currentLevel.optionC.ToString();

        answerButtonA.interactable = true;
        answerButtonB.interactable = true;
        answerButtonC.interactable = true;

        nextButton.gameObject.SetActive(false);
        hasAnswered = false;
    }

    public void ChooseAnswerA()
    {
        CheckAnswer(levels[currentLevelIndex].optionA);
    }

    public void ChooseAnswerB()
    {
        CheckAnswer(levels[currentLevelIndex].optionB);
    }

    public void ChooseAnswerC()
    {
        CheckAnswer(levels[currentLevelIndex].optionC);
    }

    void CheckAnswer(int selectedAnswer)
    {
        if (hasAnswered)
            return;

        hasAnswered = true;

        LevelData currentLevel = levels[currentLevelIndex];

        if (selectedAnswer == currentLevel.correctAnswer)
        {
            feedbackText.text = "Correct!";
            score++;
            scoreText.text = "Score: " + score;
        }
        else
        {
            feedbackText.text = "Wrong! Correct answer is " + currentLevel.correctAnswer;
        }

        answerButtonA.interactable = false;
        answerButtonB.interactable = false;
        answerButtonC.interactable = false;

        nextButton.gameObject.SetActive(true);
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        LoadLevel();
    }

    void ShowGameComplete()
    {
        levelText.text = "Finished";
        questionText.text = "You completed all levels!";
        feedbackText.text = "Final Score: " + score + " / " + levels.Count;
        scoreText.text = "Score: " + score;

        answerButtonA.gameObject.SetActive(false);
        answerButtonB.gameObject.SetActive(false);
        answerButtonC.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }
}