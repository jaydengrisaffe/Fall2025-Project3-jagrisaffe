namespace Fall2025_project3_jagrisaffe.Services
{
    public interface IAIService
    {
        Task<List<string>> GenerateMovieReviews(string movieTitle, int count = 10);
        Task<List<string>> GenerateActorTweets(string actorName, int count = 20);
    }
}