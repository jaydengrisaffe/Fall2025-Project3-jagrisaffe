using Azure.AI.OpenAI;
using Fall2025_project3_jagrisaffe.Services;
using OpenAI.Chat;
using System.ClientModel;

namespace YourProjectName.Services
{
    public class AIService : IAIService
    {
        private readonly Uri _endpoint;
        private readonly ApiKeyCredential _apiCredential;
        private readonly string _deploymentName;

        public AIService(IConfiguration configuration)
        {
            _endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]!);
            _apiCredential = new ApiKeyCredential(configuration["AzureOpenAI:ApiKey"]!);
            _deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
        }

        public async Task<List<string>> GenerateMovieReviews(string movieTitle, int count = 10)
        {
            ChatClient client = new AzureOpenAIClient(_endpoint, _apiCredential).GetChatClient(_deploymentName);

            var messages = new ChatMessage[]
            {
                new SystemChatMessage($"You represent a group of {count} film critics with different opinions. When you receive a question, respond as each critic with each response separated by a '|', but don't indicate which critic you are."),
                new UserChatMessage($"How would you review the movie {movieTitle} in 50 words or less per review?")
            };

            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
            string response = result.Value.Content[0].Text;

            // Split by | and clean up
            var reviews = response
                .Split('|')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Take(count)
                .ToList();

            return reviews;
        }

        public async Task<List<string>> GenerateActorTweets(string actorName, int count = 20)
        {
            ChatClient client = new AzureOpenAIClient(_endpoint, _apiCredential).GetChatClient(_deploymentName);

            var messages = new ChatMessage[]
            {
                new SystemChatMessage($"You represent a group of {count} Twitter/X users with different opinions about actors. When you receive a question, respond as each user with each tweet separated by a '|', but don't indicate which user you are. Keep tweets short."),
                new UserChatMessage($"Generate tweets about the actor {actorName}.")
            };

            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
            string response = result.Value.Content[0].Text;

            var tweets = response
                .Split('|')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Take(count)
                .ToList();

            return tweets;
        }
    }
}