const money = new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 0 })
const moneyPrecise = new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 2 })
const compact = new Intl.NumberFormat('ru-RU', { notation: 'compact', maximumFractionDigits: 1 })
const count = new Intl.NumberFormat('ru-RU')

export const DASH = '—'

export function formatMoney(value: number | null | undefined): string {
  return value === null || value === undefined ? DASH : `${money.format(value)} ₽`
}

export function formatMoneyPrecise(value: number | null | undefined): string {
  return value === null || value === undefined ? DASH : `${moneyPrecise.format(value)} ₽`
}

export function formatCompactMoney(value: number | null | undefined): string {
  return value === null || value === undefined ? DASH : `${compact.format(value)} ₽`
}

export function formatCount(value: number | null | undefined): string {
  return value === null || value === undefined ? DASH : count.format(Math.round(value))
}

export function formatPercent(value: number | null | undefined, digits = 1): string {
  if (value === null || value === undefined) return DASH

  return `${(value * 100).toFixed(digits).replace('.', ',')} %`
}

export function formatChange(value: number | null | undefined): string {
  if (value === null || value === undefined) return DASH

  const sign = value > 0 ? '+' : ''

  return `${sign}${(value * 100).toFixed(1).replace('.', ',')} %`
}

export function formatDate(iso: string): string {
  const [year, month, day] = iso.split('-')

  return `${day}.${month}.${year}`
}

export function formatShortDate(iso: string): string {
  const date = new Date(`${iso}T00:00:00`)

  return date.toLocaleDateString('ru-RU', { day: '2-digit', month: 'short' })
}

export function formatMonth(iso: string): string {
  const date = new Date(`${iso}T00:00:00`)

  return date.toLocaleDateString('ru-RU', { month: 'short', year: '2-digit' })
}

export function formatPeriod(from: string, to: string): string {
  return from === to ? formatDate(from) : `${formatDate(from)} — ${formatDate(to)}`
}
