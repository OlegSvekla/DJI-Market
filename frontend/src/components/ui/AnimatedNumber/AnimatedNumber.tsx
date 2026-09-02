import { useEffect, useRef, useState } from 'react'
import { DASH } from '@/lib/format'

interface AnimatedNumberProps {
  value: number | null | undefined
  format: (value: number | null | undefined) => string
  duration?: number
}

function prefersReducedMotion(): boolean {
  return window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false
}

export function AnimatedNumber({ value, format, duration = 700 }: AnimatedNumberProps) {
  const [display, setDisplay] = useState<number | null | undefined>(value)
  const previous = useRef<number | null | undefined>(value)

  useEffect(() => {
    const from = previous.current
    const to = value

    if (to === null || to === undefined || from === null || from === undefined || prefersReducedMotion()) {
      previous.current = to
      setDisplay(to)

      return
    }

    if (from === to) return

    let frame = 0
    const start = performance.now()

    const tick = (now: number) => {
      const progress = Math.min(1, (now - start) / duration)
      const eased = 1 - (1 - progress) ** 3

      setDisplay(from + (to - from) * eased)

      if (progress < 1) {
        frame = requestAnimationFrame(tick)
      } else {
        previous.current = to
      }
    }

    frame = requestAnimationFrame(tick)

    return () => cancelAnimationFrame(frame)
  }, [value, duration])

  if (display === null || display === undefined) {
    return <span>{DASH}</span>
  }

  return <span>{format(display)}</span>
}
