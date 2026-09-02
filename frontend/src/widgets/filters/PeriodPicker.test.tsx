import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { kpi, stubFetch } from '@/test/fixtures'
import { renderWithProviders } from '@/test/renderWithProviders'
import { KpiGrid } from '@/widgets/kpi/KpiGrid'
import { PeriodPicker } from './PeriodPicker'
import { usePeriod } from './periodContext'

function PeriodProbe() {
  const { params } = usePeriod()

  return <output data-testid="period">{JSON.stringify(params)}</output>
}

describe('Выбор периода', () => {
  it('по умолчанию открывается на 30 днях', () => {
    renderWithProviders(
      <>
        <PeriodPicker />
        <PeriodProbe />
      </>,
    )

    expect(screen.getByTestId('period')).toHaveTextContent('"preset":"Last30Days"')
  })

  it('клик по пресету меняет период', async () => {
    const user = userEvent.setup()

    renderWithProviders(
      <>
        <PeriodPicker />
        <PeriodProbe />
      </>,
    )

    await user.click(screen.getByRole('button', { name: '7 дней' }))

    expect(screen.getByTestId('period')).toHaveTextContent('"preset":"Last7Days"')
  })

  it('смена периода перезапрашивает данные дашборда', async () => {
    const user = userEvent.setup()
    const { calls, fetchStub } = stubFetch({ '/analytics/kpi': kpi() })

    vi.stubGlobal('fetch', fetchStub)

    renderWithProviders(
      <>
        <PeriodPicker />
        <KpiGrid />
      </>,
    )

    await waitFor(() => expect(calls.some((url) => url.includes('preset=Last30Days'))).toBe(true))

    await user.click(screen.getByRole('button', { name: 'Прошлый месяц' }))

    await waitFor(() => expect(calls.some((url) => url.includes('preset=LastMonth'))).toBe(true))
  })

  it('не применяет диапазон, если дата начала позже даты окончания', async () => {
    const user = userEvent.setup()

    renderWithProviders(
      <>
        <PeriodPicker />
        <PeriodProbe />
      </>,
    )

    await user.click(screen.getByRole('button', { name: /Свой период/ }))

    const [from, to] = screen.getAllByLabelText(/^(С|По)$/)

    await user.clear(from)
    await user.type(from, '2026-03-31')
    await user.clear(to)
    await user.type(to, '2026-03-01')
    await user.click(screen.getByRole('button', { name: 'Применить' }))

    expect(screen.getByText('Дата начала позже даты окончания')).toBeInTheDocument()
    expect(screen.getByTestId('period')).toHaveTextContent('"preset":"Last30Days"')
  })

  it('применяет корректный произвольный диапазон', async () => {
    const user = userEvent.setup()

    renderWithProviders(
      <>
        <PeriodPicker />
        <PeriodProbe />
      </>,
    )

    await user.click(screen.getByRole('button', { name: /Свой период/ }))

    const [from, to] = screen.getAllByLabelText(/^(С|По)$/)

    await user.clear(from)
    await user.type(from, '2026-01-01')
    await user.clear(to)
    await user.type(to, '2026-01-31')
    await user.click(screen.getByRole('button', { name: 'Применить' }))

    const period = screen.getByTestId('period')

    expect(period).toHaveTextContent('"preset":"Custom"')
    expect(period).toHaveTextContent('"from":"2026-01-01"')
    expect(period).toHaveTextContent('"to":"2026-01-31"')
  })
})
