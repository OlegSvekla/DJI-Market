import { screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { kpi, stubFetch } from '@/test/fixtures'
import { renderWithProviders } from '@/test/renderWithProviders'
import { KpiGrid } from './KpiGrid'

describe('KPI-карточки', () => {
  it('показывают скелетоны на первой загрузке', () => {
    const { fetchStub } = stubFetch({ '/analytics/kpi': kpi() })

    vi.stubGlobal('fetch', fetchStub)
    renderWithProviders(<KpiGrid />)

    expect(screen.getAllByTestId('skeleton').length).toBeGreaterThan(0)
  })

  it('выводят показатели и лучшего менеджера', async () => {
    const { fetchStub } = stubFetch({ '/analytics/kpi': kpi() })

    vi.stubGlobal('fetch', fetchStub)
    renderWithProviders(<KpiGrid />)

    expect(await screen.findByText('Алексей Ковалёв')).toBeInTheDocument()
    expect(screen.getByText('Выручка')).toBeInTheDocument()
    expect(screen.getByText('3 отменено · 2 возвращено')).toBeInTheDocument()
  })

  it('рисуют прочерк вместо нуля, когда за период нет продаж', async () => {
    const empty = kpi({
      revenue: { current: 0, previous: 0, changeRate: null },
      grossProfit: { current: 0, previous: 0, changeRate: null },
      margin: { current: null, previous: null, changeRate: null },
      salesCount: { current: 0, previous: 0, changeRate: null },
      averageCheck: { current: null, previous: null, changeRate: null },
      refundedAmount: { current: 0, previous: 0, changeRate: null },
      refundRate: null,
      cancelledCount: 0,
      refundedCount: 0,
      topManager: null,
    })

    const { fetchStub } = stubFetch({ '/analytics/kpi': empty })

    vi.stubGlobal('fetch', fetchStub)
    renderWithProviders(<KpiGrid />)

    expect(await screen.findByText('За период нет оплаченных продаж')).toBeInTheDocument()

    expect(screen.getAllByText('—').length).toBeGreaterThanOrEqual(2)

    expect(screen.getAllByText('нет базы').length).toBeGreaterThan(0)
  })

  it('показывает ошибку с кнопкой повтора, если API не ответил', async () => {
    const { fetchStub } = stubFetch({}, 500)

    vi.stubGlobal('fetch', fetchStub)
    renderWithProviders(<KpiGrid />)

    expect(await screen.findByText('Не удалось загрузить данные')).toBeInTheDocument()
    expect(screen.getByText('Сервис аналитики не отвечает')).toBeInTheDocument()
  })
})
