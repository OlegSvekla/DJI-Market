import { ArrowDownRight, ArrowUpRight, Minus } from 'lucide-react'
import { texts } from '@/locales/ru'
import { formatChange } from '@/lib/format'
import { cn } from '@/lib/utils'
import style from './delta.module.css'

const FLAT_THRESHOLD = 0.0005

interface DeltaProps {
  value: number | null | undefined
  inverted?: boolean
  className?: string
}

export function Delta({ value, inverted = false, className }: DeltaProps) {
  if (value === null || value === undefined) {
    return (
      <span title={texts.delta.noBaseTitle} className={cn(style.empty, className)}>
        <Minus className={style.icon} />
        {texts.delta.noBase}
      </span>
    )
  }

  const isFlat = Math.abs(value) < FLAT_THRESHOLD
  const isGood = inverted ? value < 0 : value > 0
  const Icon = value > 0 ? ArrowUpRight : ArrowDownRight

  return (
    <span
      className={cn(
        style.main,
        isFlat ? style.flat : isGood ? style.good : style.bad,
        className,
      )}
    >
      {isFlat ? <Minus className={style.icon} /> : <Icon className={style.icon} />}
      {formatChange(value)}
    </span>
  )
}
