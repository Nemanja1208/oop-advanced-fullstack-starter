import { useEffect, useState, type FormEvent } from 'react'
import { createTodo, fetchTodos } from './api/todosApi'
import { TodoList } from './components/TodoList'
import { defaultTeam, teams } from './teams'
import type { Todo } from './types'

type LoadState =
  { status: 'loading' } | { status: 'error'; message: string } | { status: 'ready'; todos: Todo[] }

/** Owns the data for the todos feature: loading, error and ready states, plus the create form. */
export function TodosPage() {
  const [state, setState] = useState<LoadState>({ status: 'loading' })
  const [title, setTitle] = useState('')
  const [team, setTeam] = useState<string>(defaultTeam)

  useEffect(() => {
    fetchTodos(team)
      .then((todos) => setState({ status: 'ready', todos }))
      .catch(() => setState({ status: 'error', message: 'Kunde inte hämta uppgifterna. Körs API:t?' }))
  }, [team])

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    if (title.trim() === '' || state.status !== 'ready') {
      return
    }
    const created = await createTodo(team, title)
    setState({ status: 'ready', todos: [created, ...state.todos] })
    setTitle('')
  }

  return (
    <main>
      <h1>Uppgifter</h1>
      <label htmlFor="team">Team</label>
      <select id="team" value={team} onChange={(event) => setTeam(event.target.value)}>
        {teams.map((t) => (
          <option key={t.slug} value={t.slug}>
            {t.name}
          </option>
        ))}
      </select>
      <form onSubmit={handleSubmit}>
        <label htmlFor="title">Ny uppgift</label>
        <input id="title" value={title} onChange={(event) => setTitle(event.target.value)} />
        <button type="submit">Lägg till</button>
      </form>
      {state.status === 'loading' && <p>Laddar...</p>}
      {state.status === 'error' && <p role="alert">{state.message}</p>}
      {state.status === 'ready' && <TodoList todos={state.todos} />}
    </main>
  )
}
