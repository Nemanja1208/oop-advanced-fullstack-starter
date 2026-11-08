import type { Todo } from '../types'

const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5069'

/** The caller's team travels in the X-Team header. Real login arrives in week 4. */
function teamHeaders(team: string): HeadersInit {
  return { 'X-Team': team }
}

async function readJson<T>(response: Response): Promise<T> {
  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`)
  }
  return (await response.json()) as T
}

export async function fetchTodos(team: string): Promise<Todo[]> {
  return readJson<Todo[]>(await fetch(`${apiUrl}/api/todos`, { headers: teamHeaders(team) }))
}

export async function createTodo(team: string, title: string): Promise<Todo> {
  const response = await fetch(`${apiUrl}/api/todos`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...teamHeaders(team) },
    body: JSON.stringify({ title }),
  })
  return readJson<Todo>(response)
}
