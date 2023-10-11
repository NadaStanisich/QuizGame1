public class Quiz
{
    public int id { get; set; }
    public string question { get; set; }
    public List<string> choices { get; set; }
    public int correctanswerindex { get; set; }


    public Quiz() {
        // This is required for Entity Framework
    }   

    public Quiz(int ID, string Question, List<string> Choices, int CorrectAnswerIndex)
    {
        id = ID;
        question = Question;
        choices = Choices;
        correctanswerindex = CorrectAnswerIndex;
    }
}

    
  