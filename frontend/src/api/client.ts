export class ApiError extends Error {
  status: number
  detail?: string

  constructor(message: string, status: number, detail?: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.detail = detail
  }
}

export async function apiGet<T>(path: string, params?: Record<string, unknown>): Promise<T> {
  const query = new URLSearchParams()

  for (const [key, value] of Object.entries(params ?? {})) {
    if (value !== undefined && value !== null && value !== '') query.set(key, String(value))
  }

  const url = `/api${path}${query.size ? `?${query}` : ''}`
  const response = await fetch(url, { headers: { Accept: 'application/json' } })

  if (!response.ok) {
    const problem = await response.json().catch(() => null)
    const fallback =
      response.status >= 500
        ? 'Сервис аналитики не отвечает'
        : `Запрос отклонён (${response.status})`

    throw new ApiError(problem?.detail ?? problem?.title ?? fallback, response.status)
  }

  return response.json() as Promise<T>
}
