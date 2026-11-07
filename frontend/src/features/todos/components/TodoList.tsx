import type { Todo } from '../types'

type TodoListProps = {
  todos: Todo[]
}

const priorityLabels = { Low: 'Låg', Normal: 'Normal', High: 'Hög' } as const

/**
 * Renders the work list in the order the API gives it (most urgent first), including the empty state.
 * Has no idea where the data came from.
 */
export function TodoList({ todos }: TodoListProps) {
  if (todos.length === 0) {
    return <p>Inga uppgifter än. Lägg till din första ovanför.</p>
  }

  return (
    <ul aria-label="Uppgifter">
      {todos.map((todo) => (
        <li key={todo.id}>
          {todo.isDone ? '✅' : '⬜'} {todo.title}
          {todo.dueDate && <span> · senast {todo.dueDate}</span>}
          {todo.priority !== 'Normal' && <span> · {priorityLabels[todo.priority]} prioritet</span>}
          {todo.isOverdue && <strong> · Försenad</strong>}
        </li>
      ))}
    </ul>
  )
}
