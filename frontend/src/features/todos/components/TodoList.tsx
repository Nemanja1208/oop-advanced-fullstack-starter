import type { Todo } from '../types'

type TodoListProps = {
  todos: Todo[]
}

/** Renders the list, including the empty state. Has no idea where the data came from. */
export function TodoList({ todos }: TodoListProps) {
  if (todos.length === 0) {
    return <p>Inga uppgifter än. Lägg till din första ovanför.</p>
  }

  return (
    <ul aria-label="Uppgifter">
      {todos.map((todo) => (
        <li key={todo.id}>
          {todo.isDone ? '✅' : '⬜'} {todo.title}
        </li>
      ))}
    </ul>
  )
}
