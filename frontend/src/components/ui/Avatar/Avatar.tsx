import type { CSSProperties } from 'react'
import { cn } from '@/lib/utils'
import style from './avatar.module.css'

interface AvatarProps {
  initials: string
  color: string
  size?: 'sm' | 'md'
  className?: string
}

export function Avatar({ initials, color, size = 'md', className }: AvatarProps) {
  return (
    <span
      className={cn(style.main, style[size], className)}
      style={{ '--avatar-color': color } as CSSProperties}
      aria-hidden
    >
      {initials}
    </span>
  )
}
