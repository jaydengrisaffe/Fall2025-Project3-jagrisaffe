async function fetchMovieDetails(movieId) {
    // Example movie info; replace with backend DB fetch
    const movie = {
        title: "The Example Movie",
        genre: "Action",
        year: 2025,
        imdbLink: "https://www.imdb.com/title/tt1234567/",
        poster: "",
        actors: ["Alice Smith", "Bob Johnson", "Charlie Lee"]
    };

    document.getElementById('movieTitle').textContent = movie.title;
    document.getElementById('movieGenre').textContent = movie.genre;
    document.getElementById('movieYear').textContent = movie.year;
    document.getElementById('movieIMDB').href = movie.imdbLink;
    document.getElementById('movieIMDB').textContent = "IMDB";

    const actorsList = document.getElementById('movieActors');
    actorsList.innerHTML = '';
    movie.actors.forEach(actor => {
        const li = document.createElement('li');
        li.textContent = actor;
        actorsList.appendChild(li);
    });

    // Fetch 10 AI reviews with sentiment
    const reviews = await getAIReviewsAndSentiment(movie.title, movie.actors);

    const tableBody = document.getElementById('movieReviews');
    tableBody.innerHTML = '';

    let sentimentScore = 0;

    reviews.forEach(r => {
        const tr = document.createElement('tr');
        tr.innerHTML = `<td>${r.review}</td><td>${r.sentiment}</td>`;
        tableBody.appendChild(tr);

        // Calculate average sentiment
        if(r.sentiment.toLowerCase() === 'positive') sentimentScore += 1;
        if(r.sentiment.toLowerCase() === 'negative') sentimentScore -= 1;
    });

    const avg = sentimentScore / reviews.length;
    let avgSentiment = 'Neutral';
    if(avg > 0) avgSentiment = 'Positive';
    if(avg < 0) avgSentiment = 'Negative';
    document.getElementById('movieAverageSentiment').textContent = avgSentiment;
}

async function getAIReviewsAndSentiment(movieTitle, actors) {
    const endpoint = "https://fall2025-aif-eastus2.cognitiveservices.azure.com/openai/deployments/gpt-4.1-nano/chat/completions?api-version=2023-07-01-preview";

    const prompt = `
You are an AI that:
1. Generates 10 short, unique movie reviews for "${movieTitle}".
2. For each review, provide the sentiment as Positive, Negative, or Neutral.
Return JSON array like:
[
  {"review": "Great movie!", "sentiment": "Positive"},
  {"review": "Not my taste", "sentiment": "Negative"},
  ...
]`;

    const response = await fetch(endpoint, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "api-key": apiKey
        },
        body: JSON.stringify({
            messages: [{ role: "user", content: prompt }],
            max_tokens: 500,
            temperature: 0.7
        })
    });

    const data = await response.json();
    const content = data.choices[0].message.content;

    // Parse JSON array
    try {
        return JSON.parse(content);
    } catch(e) {
        console.error("Error parsing AI response:", content, e);
        return [];
    }
}

// On page load
document.addEventListener('DOMContentLoaded', () => {
    const movieId = document.getElementById('movieTitle').dataset.id;
    fetchMovieDetails(movieId);
});
