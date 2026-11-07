/** Mirrors TodoResponse in the .NET API. If the API changes, this is the first place that should break. */
export type Todo = {
  id: number
  title: string
  isDone: boolean
  createdAt: string
}
