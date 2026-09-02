import { texts } from '@/locales/ru'
import { cn } from '@/lib/utils'
import style from './states.module.css'

export function Skeleton({ className }: { className?: string }) {
  return <div data-testid="skeleton" className={cn(style.skeleton, className)} />
}

export function SkeletonRows({ rows = 5, className }: { rows?: number; className?: string }) {
  return (
    <div className={cn(style.rows, className)}>
      {Array.from({ length: rows }).map((_, index) => (
        <div key={index} className={style.row}>
          <Skeleton className={style.rowAvatar} />
          <Skeleton className={style.rowWide} />
          <Skeleton className={style.rowShort} />
          <Skeleton className={style.rowMedium} />
        </div>
      ))}
    </div>
  )
}

interface ErrorStateProps {
  message?: string
  onRetry?: () => void
  className?: string
}

export function ErrorState({ message, onRetry, className }: ErrorStateProps) {
  return (
    <div className={cn(style.error, className)}>
      <p className={style.errorTitle}>{texts.states.errorTitle}</p>
      <p className={style.errorHint}>{message ?? texts.states.errorHint}</p>
      {onRetry ? (
        <button type="button" onClick={onRetry} className={style.retry}>
          {texts.states.retry}
        </button>
      ) : null}
    </div>
  )
}

interface EmptyStateProps {
  title: string
  hint?: string
  className?: string
}

export function EmptyState({ title, hint, className }: EmptyStateProps) {
  return (
    <div className={cn(style.emptyBox, className)}>
      <p className={style.emptyTitle}>{title}</p>
      {hint ? <p className={style.emptyHint}>{hint}</p> : null}
    </div>
  )
}
