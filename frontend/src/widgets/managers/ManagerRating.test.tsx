import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { managerItem, managerRating, stubFetch } from '@/test/fixtures'
import { renderWithProviders } from '@/test/renderWithProviders'
import { ManagerRating } from './ManagerRating'

const byProfit = managerRating([
  managerItem({ position: 1, managerId: 1, name: 'Алексей Ковалёв', grossProfit: 200_000 }),
  managerItem({ position: 2, managerId: 2, name: 'Мария Смирнова', grossProfit: 90_000 }),
])

const byAverageCheck = managerRating([
  managerItem({ position: 1, managerId: 2, name: 'Мария Смирнова', averageCheck: 400_000 }),
  managerItem({ position: 2, managerId: 1, name: 'Алексей Ковалёв', averageCheck: 100_000 }),
])

describe('Рейтинг менеджеров', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
  })

  it('показывает скелетон, пока данные не пришли', () => {
    const { fetchStub } = stubFetch({ '/analytics/managers': byProfit })

    vi.stubGlobal('fetch', fetchStub)
    renderWithProviders(<ManagerRating />)

    expect(screen.getAllByTestId('skeleton').length).toBeGreaterThan(0)
  })

  it('выводит строки рейтинга с метриками', async () => {
    const { fetchStub } = stubFetch({ '/analytics/managers': byProfit })

    vi.stubGlobal('fetch', fetchStub)
    renderWithProviders(<ManagerRating />)

    expect(await screen.findByText('Алексей Ковалёв')).toBeInTheDocument()
    expect(screen.getByText('Мария Смирнова')).toBeInTheDocument()
  })

  it('переключение режима запрашивает другую сортировку и перестраивает список', async () => {
    const user = userEvent.setup()
    const calls: string[] = []

    vi.stubGlobal('fetch', (async (input: RequestInfo | URL) => {
      const url = String(input)

      calls.push(url)

      const body = url.includes('sortBy=AverageCheck') ? byAverageCheck : byProfit

      return new Response(JSON.stringify(body), {
        status: 200,
        headers: { 'Content-Type': 'application/json' },
      })
    }) as unknown as typeof fetch)

    renderWithProviders(<ManagerRating />)

    await screen.findByText('Алексей Ковалёв')
    expect(calls.some((url) => url.includes('sortBy=GrossProfit'))).toBe(true)

    await user.click(screen.getByRole('button', { name: 'По среднему чеку' }))

    await waitFor(() => expect(calls.some((url) => url.includes('sortBy=AverageCheck'))).toBe(true))

    await waitFor(() => {
      const rows = screen.getAllByRole('row')

      expect(rows[1]).toHaveTextContent('Мария Смирнова')
    })
  })

  it('показывает ошибку и кнопку повтора, когда API недоступен', async () => {
    const { fetchStub } = stubFetch({}, 500)

    vi.stubGlobal('fetch', fetchStub)
    renderWithProviders(<ManagerRating />)

    expect(await screen.findByText('Не удалось загрузить данные')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Повторить' })).toBeInTheDocument()
  })

  it('показывает пустое состояние, если за период нет продаж', async () => {
    const empty = managerRating([managerItem({ salesCount: 0, revenue: 0, grossProfit: 0 })])
    const { fetchStub } = stubFetch({ '/analytics/managers': empty })

    vi.stubGlobal('fetch', fetchStub)
    renderWithProviders(<ManagerRating />)

    expect(await screen.findByText('За период нет продаж')).toBeInTheDocument()
  })
})
