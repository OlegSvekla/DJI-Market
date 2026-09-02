import type { ReactNode } from 'react'
import { cn } from '@/lib/utils'
import style from './card.module.css'

interface CardProps {
  children: ReactNode
  className?: string
}

export function Card({ children, className }: CardProps) {
  return <section className={cn(style.main, className)}>{children}</section>
}

interface CardHeaderProps {
  title: string
  subtitle?: string
  action?: ReactNode
}

export function CardHeader({ title, subtitle, action }: CardHeaderProps) {
  return (
    <header className={style.header}>
      <div className={style.titles}>
        <h2 className={style.title}>{title}</h2>
        {subtitle ? <p className={style.subtitle}>{subtitle}</p> : null}
      </div>
      {action ? <div className={style.action}>{action}</div> : null}
    </header>
  )
}

export function CardBody({ children, className }: CardProps) {
  return <div className={cn(style.body, className)}>{children}</div>
}
