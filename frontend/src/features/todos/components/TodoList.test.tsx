import { render, screen, within } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import type { Todo } from '../types'
import { TodoList } from './TodoList'

function aTodo(overrides: Partial<Todo>): Todo {
  return {
    id: 1,
    title: 'Skriv ett test',
    isDone: false,
    createdAt: '2026-11-16T09:00:00Z',
    dueDate: null,
    priority: 'Normal',
    isOverdue: false,
    completedAt: null,
    ...overrides,
  }
}

describe('TodoList', () => {
  it('shows a friendly message when there are no todos', () => {
    render(<TodoList todos={[]} />)

    expect(screen.getByText(/inga uppgifter än/i)).toBeInTheDocument()
  })

  it('renders one list item per todo', () => {
    render(<TodoList todos={[aTodo({ id: 1 }), aTodo({ id: 2, title: 'Gör en PR', isDone: true })]} />)

    expect(screen.getAllByRole('listitem')).toHaveLength(2)
  })

  // UC-1 steg 2: försenade uppgifter ska synas tydligt.
  it('marks overdue todos so nobody misses them', () => {
    render(
      <TodoList
        todos={[
          aTodo({ id: 1, title: 'Skicka offert', dueDate: '2026-11-13', isOverdue: true }),
          aTodo({ id: 2, title: 'Boka möte', dueDate: '2026-11-20' }),
        ]}
      />,
    )

    const [overdue, upcoming] = screen.getAllByRole('listitem')
    expect(within(overdue).getByText(/försenad/i)).toBeInTheDocument()
    expect(within(upcoming).queryByText(/försenad/i)).not.toBeInTheDocument()
  })
})
