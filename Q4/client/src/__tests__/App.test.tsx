import { render, screen } from '@testing-library/react';
import { describe, test } from 'vitest';
import App from '../App';

describe('the functionality of the App page', () => {
    test('renders the landing page', () => {
        render(<App />);

        expect(screen.getByText(/Rod String Editor/i)).toBeDefined();
    });

    test('changes page when clicking on element', () => {
        render(<App />);

        screen.getAllByRole('row')[0].click();

        expect(screen.getByText(/Back/i)).toBeDefined();
    });

    test('gets back to regular page after clicking back button', () => {
        render(<App />);

        screen.getAllByRole('row')[0].click();
        screen.getByText(/Back/i).click();

        expect(screen.getByText(/Rod String Editor/i)).toBeDefined();
    })
});