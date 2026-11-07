import type { Todo } from '../types'

const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5069'

async function readJson<T>(response: Response): Promise<T> {
  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`)
  }
  return (await response.json()) as T
}

export async function fetchTodos(): Promise<Todo[]> {
  return readJson<Todo[]>(await fetch(`${apiUrl}/api/todos`))
}

export async function createTodo(title: string): Promise<Todo> {
  const response = await fetch(`${apiUrl}/api/todos`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title }),
  })
  return readJson<Todo>(response)
}
