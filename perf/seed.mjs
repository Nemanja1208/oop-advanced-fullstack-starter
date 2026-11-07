// Fills the API with todos so the load test measures a realistic list, not an empty one.
// Usage: node perf/seed.mjs [count] [apiUrl]
const count = Number(process.argv[2] ?? 1000)
const apiUrl = process.argv[3] ?? 'http://localhost:5069'
const priorities = ['Low', 'Normal', 'High']

const today = new Date()
for (let i = 1; i <= count; i++) {
  const due = new Date(today)
  due.setDate(today.getDate() + (i % 30))
  const body = {
    title: `Uppgift ${i}`,
    dueDate: i % 5 === 0 ? null : due.toISOString().slice(0, 10),
    priority: priorities[i % 3],
  }
  const response = await fetch(`${apiUrl}/api/todos`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  })
  if (!response.ok) {
    console.error(`Todo ${i} failed with status ${response.status}`)
    process.exit(1)
  }
}
console.log(`Created ${count} todos at ${apiUrl}`)
