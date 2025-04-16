using System;
public class Card {
    public Action processCard {get; set;}
    public string question {get;}
    public string answer {get;}
    public string note {get;}
    public int points {get;}
    public string extra {get;}

    public Card(string question, string answer, string note, string points, string extra)
    {
        this.question = question;
        this.answer = answer;
        this.note = note;
        this.points = Int32.Parse(points);
        this.extra = extra;
    }
}