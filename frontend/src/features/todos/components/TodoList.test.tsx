import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { TodoList } from './TodoList'

describe('TodoList', () => {
  it('shows a friendly message when there are no todos', () => {
    render(<TodoList todos={[]} />)

    expect(screen.getByText(/inga uppgifter än/i)).toBeInTheDocument()
  })

  it('renders one list item per todo', () => {
    render(
      <TodoList
        todos={[
          { id: 1, title: 'Skriv ett test', isDone: false, createdAt: '2026-11-09T09:00:00Z' },
          { id: 2, title: 'Gör en PR', isDone: true, createdAt: '2026-11-09T10:00:00Z' },
        ]}
      />,
    )

    expect(screen.getAllByRole('listitem')).toHaveLength(2)
  })
})
