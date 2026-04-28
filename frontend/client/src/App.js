import './App.css';
import { useMemo, useState } from 'react';

const defaultApiBase = 'https://localhost:7048';

function App() {
  const [text, setText] = useState('');
  const [result, setResult] = useState(null);
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const apiBase = useMemo(
    () => process.env.REACT_APP_API_BASE_URL || defaultApiBase,
    []
  );

  const handleAnalyze = async (event) => {
    event.preventDefault();
    setError('');
    setResult(null);

    if (!text.trim()) {
      setError('Please enter some text to analyze.');
      return;
    }

    setIsLoading(true);

    try {
      const response = await fetch(`${apiBase}/api/Sentiment`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ text })
      });

      if (!response.ok) {
        const message = await response.text();
        throw new Error(message || 'Sentiment request failed.');
      }

      const data = await response.json();
      setResult(data);
    } catch (requestError) {
      if (requestError instanceof TypeError) {
        setError(
          `Cannot reach API at ${apiBase}. Make sure the .NET API is running.`
        );
      } else {
        setError(requestError.message || 'Something went wrong while analyzing sentiment.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  const scorePercent = (value) => `${Math.round((value || 0) * 100)}%`;

  return (
    <main className="app-shell">
      <section className="card">
        <h1>Project 1: Sentiment Analyzer</h1>
        <p className="subtitle">Input text and get sentiment using your .NET API + Azure AI Language.</p>

        <form onSubmit={handleAnalyze}>
          <label htmlFor="sentiment-text">Text to analyze</label>
          <textarea
            id="sentiment-text"
            value={text}
            onChange={(event) => setText(event.target.value)}
            placeholder="Example: I love building AI-powered apps with .NET and React."
            rows={6}
          />

          <button type="submit" disabled={isLoading}>
            {isLoading ? 'Analyzing...' : 'Analyze sentiment'}
          </button>
        </form>

        {error && <p className="error">{error}</p>}

        {result && (
          <section className="result" aria-live="polite">
            <h2>Result</h2>
            <p>
              Overall sentiment: <strong>{result.sentiment}</strong>
            </p>
            <ul>
              <li>Positive: {scorePercent(result?.scores?.positive)}</li>
              <li>Neutral: {scorePercent(result?.scores?.neutral)}</li>
              <li>Negative: {scorePercent(result?.scores?.negative)}</li>
            </ul>
          </section>
        )}
      </section>
    </main>
  );
}

export default App;
