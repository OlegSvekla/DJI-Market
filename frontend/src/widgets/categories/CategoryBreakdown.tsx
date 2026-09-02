import type { CSSProperties } from 'react'
import { motion } from 'motion/react'
import { useCategories } from '@/api/queries'
import { Card, CardHeader } from '@/components/ui/Card'
import { QueryState } from '@/components/ui/QueryState'
import { EmptyState, SkeletonRows } from '@/components/ui/States'
import { usePeriod } from '@/widgets/filters/periodContext'
import { categoryPalette } from '@/lib/chartTheme'
import { formatMoney, formatPercent } from '@/lib/format'
import { texts } from '@/locales/ru'
import style from './categoryBreakdown.module.css'

const BAR_ANIMATION = { duration: 0.5, ease: 'easeOut' } as const

export function CategoryBreakdown() {
  const { params } = usePeriod()
  const query = useCategories(params)

  return (
    <Card className="span-4">
      <CardHeader title={texts.categories.title} subtitle={texts.categories.subtitle} />

      <div className={style.body}>
        <QueryState
          query={query}
          skeleton={<SkeletonRows rows={6} />}
          isEmpty={(slices) => slices.length === 0}
          empty={<EmptyState title={texts.categories.empty} hint={texts.categories.emptyHint} />}
        >
          {(slices) => (
            <ul className={style.list}>
              {slices.map((slice, index) => (
                <li key={slice.categoryId}>
                  <div className={style.head}>
                    <span className={style.name}>{slice.name}</span>
                    <span className={style.value}>{formatMoney(slice.revenue)}</span>
                  </div>

                  <div className={style.track}>
                    <motion.div
                      className={style.bar}
                      style={
                        {
                          '--slice-color': categoryPalette[index % categoryPalette.length],
                        } as CSSProperties
                      }
                      initial={{ width: 0 }}
                      animate={{ width: `${Math.max(slice.share * 100, 1)}%` }}
                      transition={{ ...BAR_ANIMATION, delay: index * 0.05 }}
                    />
                  </div>

                  <div className={style.meta}>
                    <span>
                      {formatPercent(slice.share, 1)} {texts.categories.shareOfRevenue}
                    </span>
                    <span>
                      {texts.categories.margin} {formatPercent(slice.margin, 1)}
                    </span>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </QueryState>
      </div>
    </Card>
  )
}
