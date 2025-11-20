using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Veridium.Animation;
using UnityEngine.Events;

namespace Veridium.Modules.AminoAcids {
    public class AwaitQuiz : AwaitAny
    {
        public QuizScreen quizScreen;
        public List<QuizQuestion> questions;
        [Tooltip("How many questions to select from the list. If 0, all questions will be asked.")]
        public int selectAtRandom = 0;

        public override void Play()
        {
            base.Play();
            List<QuizQuestion> randomQuestions = questions.SelectRandom(selectAtRandom).ToList();
            quizScreen.EnqueueQuestions(selectAtRandom <= 0 ? randomQuestions : questions);
            quizScreen.onQuizComplete.AddListener(OnQuizComplete);
        }

        private void OnQuizComplete()
        {
            quizScreen.onQuizComplete.RemoveListener(OnQuizComplete);
            CompleteAction();
        }
    }
}