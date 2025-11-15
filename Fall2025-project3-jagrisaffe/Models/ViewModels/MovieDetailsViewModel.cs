namespace Fall2025_project3_jagrisaffe.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<ReviewWithSentiment> Reviews { get; set; } = new List<ReviewWithSentiment>();
        public double AverageSentiment { get; set; }
    }

    public class ReviewWithSentiment
    {
        public string Text { get; set; }
        public double SentimentScore { get; set; }
    }
}