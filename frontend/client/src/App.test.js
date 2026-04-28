import { render, screen } from '@testing-library/react';
import App from './App';

test('renders sentiment analyzer heading', () => {
  render(<App />);
  const headingElement = screen.getByText(/project 1: sentiment analyzer/i);
  expect(headingElement).toBeInTheDocument();
});
