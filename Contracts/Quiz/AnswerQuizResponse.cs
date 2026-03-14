namespace VocabTrainer.Api.Contracts.Quiz;

public record AnswerQuizResponse(
      bool IsCorrect,
      string CorrectAnswer,
      int CorrectCount,
      int WrongCount,
      int Score,
      bool IsFinished,
      QuizSummaryResponse? Summary
  );