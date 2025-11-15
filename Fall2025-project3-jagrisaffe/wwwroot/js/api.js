const AI_ENDPOINT = "https://fall2025-aif-eastus2.cognitiveservices.azure.com/";
async function fetchAIReviews(prompt) {
    const response = await fetch(`${AI_ENDPOINT}openai/deployments/gpt-4.1-nano/completions?api-version=2023-03-15-preview`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "api-key": API_KEY
        },
        body: JSON.stringify({
            "prompt": prompt,
            "max_tokens": 500
        })
    });
    const data = await response.json();
    return data.choices[0].text.split("\n").filter(r => r.trim() !== "");
}

function analyzeSentiment(texts) {
    // Placeholder for VaderSharp2 sentiment analysis logic
    // Example: return array of scores [-1, 1]
    return texts.map(t => (Math.random() * 2 - 1).toFixed(2));
}

async function fetchMovieAIReviews() {
    const movieTitle = document.getElementById("movie-title-header").innerText;
    const reviews = await fetchAIReviews(`Generate 10 short reviews for the movie "${movieTitle}"`);
    const sentiments = analyzeSentiment(reviews);

    const tbody = document.querySelector("#ai-reviews-table tbody");
    tbody.innerHTML = "";
    let total = 0;
    reviews.forEach((review, i) => {
        total += parseFloat(sentiments[i]);
        const tr = document.createElement("tr");
        tr.innerHTML = `<td>${review}</td><td>${sentiments[i]}</td>`;
        tbody.appendChild(tr);
    });

    document.getElementById("movie-sentiment").innerText = `Average Sentiment: ${(total / reviews.length).toFixed(2)}`;
}
