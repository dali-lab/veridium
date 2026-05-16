using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace Veridium.Modules.AminoAcids
{
    public class QuizScreen : MonoBehaviour
    {
        public TextMeshProUGUI questionText;
        public List<TextMeshProUGUI> answerTexts;
        public List<Button> answerButtons;
        public UnityEvent onQuizComplete;

        private Queue<QuizQuestion> questions;
        private QuizQuestion currentQuestion;

        void Start()
        {
            questions = new Queue<QuizQuestion>();
            onQuizComplete = new UnityEvent();

            for (int i = 0; i < answerButtons.Count; i++)
            {
                int index = i; // Capture the current index for the listener
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
            }
        }

        public void EnqueueQuestion(QuizQuestion question)
        {
            if (currentQuestion == null)
            {
                LoadQuestion(question);
                return;
            }
            questions.Enqueue(question);
        }

        public void EnqueueQuestions(IEnumerable<QuizQuestion> questionList)
        {
            foreach (var question in questionList)
            {
                EnqueueQuestion(question);
            }
        }

        private void LoadQuestion(QuizQuestion question)
        {
            currentQuestion = question;
            questionText.text = question.questionText.GetTextForCurrentLanguage();

            if (question.answers.Count != answerTexts.Count)
            {
                Debug.LogError("Number of answers does not match the number of answer texts.");
                return;
            }

            for (int i = 0; i < answerTexts.Count; i++)
            {
                answerTexts[i].text = question.answers[i].texts.GetTextForCurrentLanguage();
                answerTexts[i].color = Color.white;
                answerButtons[i].interactable = true;
            }
        }

        private void LoadNextQuestion()
        {
            if (questions.Count == 0)
            {
                Debug.Log("Quiz complete!");
                onQuizComplete.Invoke();
                LoadStandbyConfig();
                return;
            }

            LoadQuestion(questions.Dequeue());
        }

        private void CheckAnswer(int selectedIndex)
        {
            if (currentQuestion == null)
            {
                Debug.LogError("No question loaded.");
                return;
            }

            if (((1 << selectedIndex) & currentQuestion.correctAnswerIndex) != 0)
            {
                answerTexts[selectedIndex].color = Color.green;
                foreach (Button button in answerButtons) button.interactable = false;
                StartCoroutine(CorrectAnswer());
                Debug.Log("Correct answer!");
            }
            else
            {
                answerTexts[selectedIndex].color = Color.red;
                answerButtons[selectedIndex].interactable = false;
                Debug.Log("Wrong answer!");
            }
        }
        private IEnumerator CorrectAnswer()
        {
            yield return new WaitForSeconds(1f);
            LoadNextQuestion();
        }

        private void LoadStandbyConfig()
        {
            currentQuestion = null;
            questionText.text = Language.language switch
            {
                "German" => "Quizfragen werden hier erscheinen, um dein Wissen zu testen!",
                _ => "Quiz questions will appear here to test your knowledge."
            };

            for (int i = 0; i < answerTexts.Count; i++)
            {
                QuizAnswer answer = (QuizAnswer)i;
                answerTexts[i].text = Language.language switch
                {
                    "German" => $"Antwort {answer}",
                    _ => $"Answer {answer}"
                };
                answerTexts[i].color = Color.white;
                answerButtons[i].interactable = false;
            }
        }

        [ContextMenu("Pick correct answer")]
        public void PickCorrectAnswer()
        {
            if (currentQuestion == null) return;

            foreach (QuizAnswer answer in Enum.GetValues(typeof(QuizAnswer)))
            {
                if (!currentQuestion.correctAnswer.HasFlag(answer)) continue;

                int answerIndex = (int)Math.Log((int)answer, 2);
                CheckAnswer(answerIndex);
            }
        }
    }
}
