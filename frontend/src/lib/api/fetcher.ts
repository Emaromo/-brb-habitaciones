const BASE = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:8080'

export async function serverFetch<T>(
  path: string,
  options: RequestInit = {}
): Promise<T | null> {
  try {
    const res = await fetch(`${BASE}${path}`, {
      cache: 'no-store',
      headers: { 'Content-Type': 'application/json' },
      ...options,
    })
    if (!res.ok) return null
    const json = await res.json()
    return (json.data ?? null) as T
  } catch {
    return null
  }
}
