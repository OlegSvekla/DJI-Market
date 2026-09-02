import { useEffect, useRef, useState } from 'react'
import { CalendarRange } from 'lucide-react'
import type { PeriodPreset } from '@/api/types'
import { cn } from '@/lib/utils'
import { texts } from '@/locales/ru'
import { usePeriod } from './periodContext'
import style from './periodPicker.module.css'

const PRESETS: Array<{ value: PeriodPreset; label: string }> = [
  { value: 'Today', label: texts.period.today },
  { value: 'Last7Days', label: texts.period.last7Days },
  { value: 'Last30Days', label: texts.period.last30Days },
  { value: 'ThisMonth', label: texts.period.thisMonth },
  { value: 'LastMonth', label: texts.period.lastMonth },
]

interface PeriodPickerProps {
  minDate?: string
  maxDate?: string
}

export function PeriodPicker({ minDate, maxDate }: PeriodPickerProps) {
  const { preset, setPreset, setCustomRange, custom } = usePeriod()
  const [open, setOpen] = useState(false)
  const [draft, setDraft] = useState(custom)
  const [error, setError] = useState<string | null>(null)
  const popover = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!open) return

    function onClickOutside(event: MouseEvent) {
      if (!popover.current?.contains(event.target as Node)) setOpen(false)
    }

    document.addEventListener('mousedown', onClickOutside)

    return () => document.removeEventListener('mousedown', onClickOutside)
  }, [open])

  function apply() {
    if (draft.from > draft.to) {
      setError(texts.period.invalidRange)

      return
    }

    setError(null)
    setCustomRange(draft.from, draft.to)
    setOpen(false)
  }

  return (
    <div className={style.main}>
      <div className={style.presets}>
        {PRESETS.map((item) => (
          <button
            key={item.value}
            type="button"
            onClick={() => setPreset(item.value)}
            className={cn(style.preset, preset === item.value && style.presetActive)}
          >
            {item.label}
          </button>
        ))}
      </div>

      <div className={style.customWrap} ref={popover}>
        <button
          type="button"
          onClick={() => setOpen((value) => !value)}
          className={cn(style.customButton, preset === 'Custom' && style.customActive)}
        >
          <CalendarRange className={style.calendarIcon} />
          {preset === 'Custom' ? `${custom.from} — ${custom.to}` : texts.period.custom}
        </button>

        {open ? (
          <div className={style.popover}>
            <p className={style.popoverTitle}>{texts.period.customTitle}</p>

            <label className={cn(style.field, style.fieldSpaced)}>
              {texts.period.from}
              <input
                type="date"
                value={draft.from}
                min={minDate}
                max={maxDate}
                onChange={(event) => setDraft((value) => ({ ...value, from: event.target.value }))}
                className={style.input}
              />
            </label>

            <label className={style.field}>
              {texts.period.to}
              <input
                type="date"
                value={draft.to}
                min={minDate}
                max={maxDate}
                onChange={(event) => setDraft((value) => ({ ...value, to: event.target.value }))}
                className={style.input}
              />
            </label>

            {error ? <p className={style.error}>{error}</p> : null}

            <button type="button" onClick={apply} className={style.apply}>
              {texts.period.apply}
            </button>
          </div>
        ) : null}
      </div>
    </div>
  )
}
