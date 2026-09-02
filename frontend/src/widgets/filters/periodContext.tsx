import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react'
import type { PeriodParams } from '@/api/queries'
import type { PeriodPreset } from '@/api/types'

interface PeriodState {
  params: PeriodParams
  preset: PeriodPreset
  setPreset: (preset: PeriodPreset) => void
  setCustomRange: (from: string, to: string) => void
  custom: { from: string; to: string }
}

const PeriodContext = createContext<PeriodState | null>(null)

function today(): string {
  return new Date().toISOString().slice(0, 10)
}

function monthAgo(): string {
  const date = new Date()

  date.setDate(date.getDate() - 29)

  return date.toISOString().slice(0, 10)
}

export function PeriodProvider({ children }: { children: ReactNode }) {
  const [preset, setPresetState] = useState<PeriodPreset>('Last30Days')
  const [custom, setCustom] = useState({ from: monthAgo(), to: today() })

  const setPreset = useCallback((next: PeriodPreset) => setPresetState(next), [])

  const setCustomRange = useCallback((from: string, to: string) => {
    setCustom({ from, to })
    setPresetState('Custom')
  }, [])

  const params = useMemo<PeriodParams>(
    () => (preset === 'Custom' ? { preset, from: custom.from, to: custom.to } : { preset }),
    [preset, custom.from, custom.to],
  )

  const value = useMemo<PeriodState>(
    () => ({ params, preset, setPreset, setCustomRange, custom }),
    [params, preset, setPreset, setCustomRange, custom],
  )

  return <PeriodContext.Provider value={value}>{children}</PeriodContext.Provider>
}

export function usePeriod(): PeriodState {
  const context = useContext(PeriodContext)

  if (!context) {
    throw new Error('usePeriod используется вне PeriodProvider.')
  }

  return context
}
