using Npgsql;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Quiz API");
app.MapGet("/quiz/{id}", (int id) => getQuiz(id));
app.MapGet("/quizzes", () => getQuizzes());

app.MapPost("/quiz", (Quiz quiz) => addQuiz(quiz));


app.Run();



Quiz getQuiz(int id) {
    Quiz quiz = null;
    using (var conn = getDbConnection()) {
        try {
            conn.Open();
            using (var cmd = new NpgsqlCommand($"SELECT * FROM \"Quizzes\" WHERE \"id\" = {id}", conn)) {
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        quiz = new Quiz(reader.GetInt32(0), reader.GetString(1), new List<string>(), reader.GetInt32(2));
                    }
                }
            }
            if (quiz != null) {
                using (var cmd = new NpgsqlCommand($"SELECT \"text\" FROM \"Choices\" WHERE \"quizid\" = {id}", conn)) {
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            quiz.choices.Add(reader.GetString(0));
                        }
                    }
                }
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }
    return quiz;
}

List<Quiz> getQuizzes() {
    List<Quiz> quizzes = new List<Quiz>();
    using (var conn = getDbConnection()) {
        try {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT * FROM \"Quizzes\"", conn)) {
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        Quiz quiz = new Quiz(reader.GetInt32(0), reader.GetString(1), new List<string>(), reader.GetInt32(2));
                        quizzes.Add(quiz);
                    }
                }
            }
            foreach (var quiz in quizzes) {
                using (var cmd = new NpgsqlCommand($"SELECT \"text\" FROM \"Choices\" WHERE \"quizid\" = {quiz.id}", conn)) {
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            quiz.choices.Add(reader.GetString(0));
                        }
                    }
                }
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }
    return quizzes;
}



void addQuiz(Quiz quiz) {
    using (var conn = getDbConnection()) {
        try {
            conn.Open();
            using (var cmd = new NpgsqlCommand($"INSERT INTO \"Quizzes\" (\"id\", \"question\", \"correctanswerindex\") VALUES ({quiz.id}, '{quiz.question}', {quiz.correctanswerindex})", conn)) {
                cmd.ExecuteNonQuery();
            }
            foreach (var choice in quiz.choices) {
                bool isCorrect = quiz.choices.IndexOf(choice) == quiz.correctanswerindex;
                using (var cmd = new NpgsqlCommand($"INSERT INTO \"Choices\" (\"text\", \"iscorrect\", \"quizid\") VALUES ('{choice}', {isCorrect}, {quiz.id})", conn)) {
                    cmd.ExecuteNonQuery();
                }
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }
}
NpgsqlConnection getDbConnection() {
    return new NpgsqlConnection("User Id=postgres;Password=JDivision1979!2023!;Server=db.degdsykyssiponzjmhbq.supabase.co;Port=5432;Database=postgres");
}