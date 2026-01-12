using System;
using System.Collections.Generic;

enum QuestionLevel
{
    Easy = 1,
    Medium = 2,
    Hard = 3
}

enum ExamType
{
    Midterm = 1,
    Final = 2
}

abstract class Question
{
    private string header;
    private int degree;
    private QuestionLevel level;

    protected Question(string header, int degree, QuestionLevel level)
    {
        this.header = header;
        this.degree = degree;
        this.level = level;
    }

    public string Header { get { return header; } }
    public int Degree { get { return degree; } }
    public QuestionLevel Level { get { return level; } }

    public abstract void Display(int number);
    public abstract int CheckAnswer();
}

class TrueFalseQuestion : Question
{
    private int correctAnswer;

    public TrueFalseQuestion(string header, int degree, QuestionLevel level, int correctAnswer)
        : base(header, degree, level)
    {
        this.correctAnswer = correctAnswer;
    }

    public override void Display(int number)
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Q{number}: {Header}");
        Console.WriteLine("[1] True");
        Console.WriteLine("[2] False");
        Console.Write("Your Answer: ");
    }

    public override int CheckAnswer()
    {
        int answer = int.Parse(Console.ReadLine());
        return answer == correctAnswer ? Degree : 0;
    }
}

class ChooseOneQuestion : Question
{
    private List<string> choices;
    private int correctAnswer;

    public ChooseOneQuestion(string header, int degree, QuestionLevel level,
                             List<string> choices, int correctAnswer)
        : base(header, degree, level)
    {
        this.choices = choices;
        this.correctAnswer = correctAnswer;
    }

    public override void Display(int number)
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Q{number}: {Header}");

        for (int i = 0; i < choices.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {choices[i]}");
        }

        Console.Write("Your Answer: ");
    }

    public override int CheckAnswer()
    {
        int answer = int.Parse(Console.ReadLine());
        return answer == correctAnswer ? Degree : 0;
    }
}

class MultipleChoiceQuestion : Question
{
    private List<string> choices;
    private List<int> correctAnswers;

    public MultipleChoiceQuestion(string header, int degree, QuestionLevel level,
                                  List<string> choices, List<int> correctAnswers)
        : base(header, degree, level)
    {
        this.choices = choices;
        this.correctAnswers = correctAnswers;
    }

    public override void Display(int number)
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Q{number}: {Header}");

        for (int i = 0; i < choices.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {choices[i]}");
        }

        Console.Write("Your Answers (ex: 1,2): ");
    }

    public override int CheckAnswer()
    {
        string input = Console.ReadLine();
        string[] answers = input.Split(',');

        int score = 0;

        for (int i = 0; i < answers.Length; i++)
        {
            int ans = int.Parse(answers[i]);
            if (correctAnswers.Contains(ans))
                score += Degree / correctAnswers.Count;
        }

        return score;
    }
}

class StudentResult
{
    public string Name { get; }
    public ExamType Exam { get; }
    public int Score { get; }
    public int Total { get; }
    public string Status { get; }

    public StudentResult(string name, ExamType exam, int score, int total)
    {
        Name = name;
        Exam = exam;
        Score = score;
        Total = total;
        Status = score >= total / 2 ? "Pass" : "Fail";
    }
}

class ExamSystem
{
    static List<Question> questionBank = new List<Question>();
    static List<StudentResult> results = new List<StudentResult>();

    static QuestionLevel SelectLevel()
    {
        Console.WriteLine("[1] Easy");
        Console.WriteLine("[2] Medium");
        Console.WriteLine("[3] Hard");
        return (QuestionLevel)int.Parse(Console.ReadLine());
    }

    static Question CreateTrueFalse()
    {
        Console.Write("Header: ");
        string header = Console.ReadLine();

        QuestionLevel level = SelectLevel();

        Console.Write("Degree: ");
        int degree = int.Parse(Console.ReadLine());

        Console.WriteLine("[1] True");
        Console.WriteLine("[2] False");
        int correct = int.Parse(Console.ReadLine());

        return new TrueFalseQuestion(header, degree, level, correct);
    }

    static Question CreateChooseOne()
    {
        Console.Write("Header: ");
        string header = Console.ReadLine();

        QuestionLevel level = SelectLevel();

        Console.Write("Degree: ");
        int degree = int.Parse(Console.ReadLine());

        List<string> choices = new List<string>();
        for (int i = 1; i <= 4; i++)
        {
            Console.Write($"Choice {i}: ");
            choices.Add(Console.ReadLine());
        }

        Console.Write("Correct Answer: ");
        int correct = int.Parse(Console.ReadLine());

        return new ChooseOneQuestion(header, degree, level, choices, correct);
    }

    static Question CreateMultipleChoice()
    {
        Console.Write("Header: ");
        string header = Console.ReadLine();

        QuestionLevel level = SelectLevel();

        Console.Write("Degree: ");
        int degree = int.Parse(Console.ReadLine());

        List<string> choices = new List<string>();
        for (int i = 1; i <= 4; i++)
        {
            Console.Write($"Choice {i}: ");
            choices.Add(Console.ReadLine());
        }

        Console.Write("Correct Answers (1,2): ");
        string input = Console.ReadLine();
        string[] arr = input.Split(',');

        List<int> correctAnswers = new List<int>();
        for (int i = 0; i < arr.Length; i++)
            correctAnswers.Add(int.Parse(arr[i]));

        return new MultipleChoiceQuestion(header, degree, level, choices, correctAnswers);
    }

    public static void DoctorMode()
    {
        Console.Clear();
        Console.Write("Number of Questions: ");
        int count = int.Parse(Console.ReadLine());

        for (int i = 0; i < count; i++)
        {
            Console.Clear();
            Console.WriteLine("[1] True False");
            Console.WriteLine("[2] Choose One");
            Console.WriteLine("[3] Multiple Choice");

            int choice = int.Parse(Console.ReadLine());
            Question question = null;

            switch (choice)
            {
                case 1:
                    question = CreateTrueFalse();
                    break;
                case 2:
                    question = CreateChooseOne();
                    break;
                case 3:
                    question = CreateMultipleChoice();
                    break;
            }

            questionBank.Add(question);
        }
    }

    public static void StudentMode()
    {
        Console.Clear();
        Console.Write("Student Name: ");
        string name = Console.ReadLine();

        Console.WriteLine("[1] Midterm");
        Console.WriteLine("[2] Final");
        ExamType exam = (ExamType)int.Parse(Console.ReadLine());

        QuestionLevel level = SelectLevel();

        List<Question> examQuestions = new List<Question>();
        for (int i = 0; i < questionBank.Count; i++)
        {
            if (questionBank[i].Level == level)
                examQuestions.Add(questionBank[i]);
        }

        int score = 0;
        int total = 0;

        Console.Clear();
        Console.WriteLine("===== Exam Started =====");
        int questionsCount = exam == ExamType.Final ? examQuestions.Count : examQuestions.Count / 2;


        for (int i = 0; i < questionsCount; i++)
        {
            examQuestions[i].Display(i + 1);
            score += examQuestions[i].CheckAnswer();
            total += examQuestions[i].Degree;
        }


        results.Add(new StudentResult(name, exam, score, total));
        Console.WriteLine($"Result: {score} / {total}");
    }

    public static void ShowResults()
    {
        Console.Clear();
        Console.WriteLine("\tName\tExam\tScore\tStatus");
        Console.WriteLine("\t================================");
        for (int i = 0; i < results.Count; i++)
        {
            var r = results[i];
            Console.WriteLine($"\t{r.Name}\t{r.Exam}\t{r.Score}/{r.Total}\t{r.Status}");
            Console.WriteLine("\t--------------------------------");
        }
    }
}

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Educational Institution for Examinations");
            Console.WriteLine("========================================================");
            Console.WriteLine("[1] Doctor Mode");
            Console.WriteLine("[2] Student Mode");
            Console.WriteLine("[3] Show Results");
            Console.WriteLine("[4] Exit");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    ExamSystem.DoctorMode();
                    break;
                case 2:
                    ExamSystem.StudentMode();
                    break;
                case 3:
                    ExamSystem.ShowResults();
                    break;
                case 4:
                    return;
            }

            Console.WriteLine("\nPress Enter...");
            Console.ReadLine();
        }
    }
}
