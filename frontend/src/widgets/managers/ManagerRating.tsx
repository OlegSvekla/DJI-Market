import { useState } from 'react'
import { motion } from 'motion/react'
import { useManagerRating } from '@/api/queries'
import type { ManagerSortBy } from '@/api/types'
import { Avatar } from '@/components/ui/Avatar'
import { Card, CardHeader } from '@/components/ui/Card'
import { Delta } from '@/components/ui/Delta'
import { Sparkline } from '@/components/ui/Sparkline'
import { EmptyState, ErrorState, SkeletonRows } from '@/components/ui/States'
import { chartColors } from '@/lib/chartTheme'
import { formatCount, formatMoney, formatPercent } from '@/lib/format'
import { cn } from '@/lib/utils'
import { texts } from '@/locales/ru'
import { usePeriod } from '@/widgets/filters/periodContext'
import style from './managerRating.module.css'

const ROW_TRANSITION = { type: 'spring', stiffness: 380, damping: 34 } as const

const SKELETON_ROWS = 8

const NO_POSITION = '—'

const MODES: Array<{ value: ManagerSortBy; label: string }> = [
  { value: 'GrossProfit', label: texts.rating.byProfit },
  { value: 'AverageCheck', label: texts.rating.byAverageCheck },
  { value: 'Revenue', label: texts.rating.byRevenue },
]

export function ManagerRating() {
  const { params } = usePeriod()
  const [sortBy, setSortBy] = useState<ManagerSortBy>('GrossProfit')
  const { data, isPending, isError, error, refetch, isFetching } = useManagerRating(params, sortBy)

  const rows = data?.items ?? []
  const withSales = rows.filter((row) => row.salesCount > 0)

  return (
    <Card className="span-8">
      <CardHeader
        title={texts.rating.title}
        subtitle={texts.rating.subtitle}
        action={
          <div className={style.modes}>
            {MODES.map((mode) => (
              <button
                key={mode.value}
                type="button"
                onClick={() => setSortBy(mode.value)}
                className={cn(style.mode, sortBy === mode.value && style.modeActive)}
              >
                {mode.label}
              </button>
            ))}
          </div>
        }
      />

      <div className={style.body}>
        {isError ? (
          <ErrorState message={(error as Error)?.message} onRetry={() => void refetch()} />
        ) : isPending ? (
          <SkeletonRows rows={SKELETON_ROWS} />
        ) : withSales.length === 0 ? (
          <EmptyState title={texts.rating.empty} hint={texts.rating.emptyHint} />
        ) : (
          <table className={style.table}>
            <thead>
              <tr className={style.headRow}>
                <th>{texts.rating.columns.position}</th>
                <th>{texts.rating.columns.manager}</th>
                <th className={style.alignRight}>{texts.rating.columns.salesCount}</th>
                <th className={style.alignRight}>{texts.rating.columns.revenue}</th>
                <th className={style.alignRight}>{texts.rating.columns.grossProfit}</th>
                <th className={style.alignRight}>{texts.rating.columns.averageCheck}</th>
                <th className={style.alignRight}>{texts.rating.columns.margin}</th>
                <th className={style.alignCenter}>{texts.rating.columns.trend}</th>
                <th className={style.alignRight}>{texts.rating.columns.change}</th>
              </tr>
            </thead>
            <tbody className={cn(isFetching && style.fetching)}>
              {rows.map((row) => (
                <motion.tr
                  key={row.managerId}
                  layout
                  transition={ROW_TRANSITION}
                  className={cn(style.row, row.salesCount === 0 && style.idle)}
                >
                  <td className={style.position}>
                    {row.salesCount === 0 ? NO_POSITION : row.position}
                  </td>
                  <td>
                    <div className={style.manager}>
                      <Avatar initials={row.initials} color={row.avatarColor} size="sm" />
                      <div className={style.managerText}>
                        <p className={style.name}>
                          {row.name}
                          {!row.isActive ? (
                            <span className={style.badge}>{texts.rating.inactive}</span>
                          ) : null}
                        </p>
                        <p className={style.team}>{row.team}</p>
                      </div>
                    </div>
                  </td>
                  <td className={style.number}>{formatCount(row.salesCount)}</td>
                  <td className={style.number}>{formatMoney(row.revenue)}</td>
                  <td className={style.numberStrong}>{formatMoney(row.grossProfit)}</td>
                  <td className={style.number}>{formatMoney(row.averageCheck)}</td>
                  <td className={style.number}>{formatPercent(row.margin)}</td>
                  <td>
                    <div className={style.trend}>
                      <Sparkline
                        points={row.spark}
                        color={row.grossProfit >= 0 ? chartColors.grossProfit : chartColors.negative}
                      />
                    </div>
                  </td>
                  <td className={style.alignRight}>
                    <Delta
                      value={
                        sortBy === 'AverageCheck' ? row.averageCheckChange : row.grossProfitChange
                      }
                    />
                  </td>
                </motion.tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </Card>
  )
}
