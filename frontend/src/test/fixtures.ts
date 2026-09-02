import type { Kpi, ManagerRating, ManagerRatingItem } from '@/api/types'

export function managerItem(overrides: Partial<ManagerRatingItem> = {}): ManagerRatingItem {
  return {
    position: 1,
    managerId: 1,
    name: 'Алексей Ковалёв',
    initials: 'АК',
    avatarColor: '#2563eb',
    team: 'Корпоративные продажи',
    isActive: true,
    salesCount: 10,
    revenue: 1_000_000,
    grossProfit: 200_000,
    averageCheck: 100_000,
    margin: 0.2,
    grossProfitChange: 0.1,
    averageCheckChange: -0.05,
    spark: [1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 2],
    ...overrides,
  }
}

export function managerRating(items: ManagerRatingItem[]): ManagerRating {
  return {
    period: { from: '2026-03-01', to: '2026-03-31' },
    previousPeriod: { from: '2026-02-01', to: '2026-02-28' },
    items,
  }
}

export function kpi(overrides: Partial<Kpi> = {}): Kpi {
  return {
    period: { from: '2026-03-01', to: '2026-03-31' },
    previousPeriod: { from: '2026-02-01', to: '2026-02-28' },
    revenue: { current: 1_000_000, previous: 800_000, changeRate: 0.25 },
    grossProfit: { current: 200_000, previous: 180_000, changeRate: 0.111 },
    margin: { current: 0.2, previous: 0.225, changeRate: -0.111 },
    salesCount: { current: 42, previous: 40, changeRate: 0.05 },
    averageCheck: { current: 23_809, previous: 20_000, changeRate: 0.19 },
    refundedAmount: { current: 50_000, previous: 30_000, changeRate: 0.666 },
    refundRate: 0.047,
    cancelledCount: 3,
    refundedCount: 2,
    topManager: {
      id: 1,
      name: 'Алексей Ковалёв',
      initials: 'АК',
      avatarColor: '#2563eb',
      grossProfit: 120_000,
    },
    ...overrides,
  }
}

export function stubFetch(routes: Record<string, unknown>, status = 200) {
  const calls: string[] = []

  const fetchStub = async (input: RequestInfo | URL) => {
    const url = String(input)

    calls.push(url)

    const match = Object.keys(routes).find((key) => url.includes(key))

    if (status >= 500) {
      return new Response('<html>502 Bad Gateway</html>', {
        status,
        headers: { 'Content-Type': 'text/html' },
      })
    }

    if (!match) {
      return new Response(JSON.stringify({ title: 'Не найдено', detail: 'Нет такого маршрута' }), {
        status: 404,
        headers: { 'Content-Type': 'application/json' },
      })
    }

    return new Response(JSON.stringify(routes[match]), {
      status: 200,
      headers: { 'Content-Type': 'application/json' },
    })
  }

  return { calls, fetchStub: fetchStub as unknown as typeof fetch }
}
