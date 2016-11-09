using System.ComponentModel.DataAnnotations;

namespace Duncan.PEMS.Entities.Users
{
    public class PasswordQuestion
    {
        //public const string DummyAnswer = "******";
        public enum QuestionState
        {
            Invalid,
            Empty,
            New,
//            QuestionChangedNeedAnswer,
            Changed,
            NoChange
        }

        public PasswordQuestion()
        {
            QuestionNumber = 0;
        }

        public PasswordQuestion(int questionNumber)
        {
            QuestionNumber = questionNumber;
        }

        public PasswordQuestion(int questionNumber, string question, string answer)
        {
            QuestionNumber = questionNumber;
            Question = question;
            Answer = answer;
        }

        [Required]
        [Display(Name = "Question")]
        public string Question { get; set; }

        public int QuestionNumber { get; set; }

        [Required]
        [Display(Name = "Answer")]
        public string Answer { get; set; }
    }
}