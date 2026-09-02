import type { ReactNode } from 'react'
import { ErrorState } from '../States'

interface QueryLike<T> {
  data: T | undefined
  isPending: boolean
  isError: boolean
  error: unknown
  refetch: () => unknown
}

interface QueryStateProps<T> {
  query: QueryLike<T>
  skeleton: ReactNode
  isEmpty?: (data: T) => boolean
  empty?: ReactNode
  className?: string
  children: (data: T) => ReactNode
}

export function QueryState<T>({
  query,
  skeleton,
  isEmpty,
  empty,
  className,
  children,
}: QueryStateProps<T>) {
  if (query.isError) {
    return (
      <ErrorState
        message={(query.error as Error)?.message}
        onRetry={() => void query.refetch()}
        className={className}
      />
    )
  }

  if (query.isPending || query.data === undefined) {
    return <>{skeleton}</>
  }

  if (empty && isEmpty?.(query.data)) {
    return <>{empty}</>
  }

  return <>{children(query.data)}</>
}
