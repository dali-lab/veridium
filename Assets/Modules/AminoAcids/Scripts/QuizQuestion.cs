using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Modules.AminoAcids
{
    [Flags]
    public enum QuizAnswer
    {
        A = 1,
        B = 2,
        C = 4,
        D = 8,
    }

    [Serializable]
    public class LanguageTextList
    {
        public List<LanguageText> texts;
    }

    [CreateAssetMenu(fileName = "QuizQuestion", menuName = "Veridium/QuizQuestion", order = 1)]
    public class QuizQuestion : ScriptableObject
    {
        public List<LanguageText> questionText;
        public List<LanguageTextList> answers;
        public QuizAnswer correctAnswer;
        public int correctAnswerIndex => (int)correctAnswer;

        public QuizQuestion(List<LanguageText> questionText, List<LanguageTextList> answers, QuizAnswer correctAnswer)
        {
            this.questionText = questionText;
            this.answers = answers;
            this.correctAnswer = correctAnswer;
        }
    }
}
