import { motion } from 'motion/react'
import {
  Banknote,
  Coins,
  Percent,
  Receipt,
  ShoppingCart,
  Trophy,
  type LucideIcon,
} from 'lucide-react'
import { useKpi } from '@/api/queries'
import type { Kpi, Metric } from '@/api/types'
import { AnimatedNumber } from '@/components/ui/AnimatedNumber'
import { Avatar } from '@/components/ui/Avatar'
import { Delta } from '@/components/ui/Delta'
import { ErrorState, Skeleton } from '@/components/ui/States'
import { usePeriod } from '@/widgets/filters/periodContext'
import { formatCount, formatMoney, formatPercent, formatPeriod } from '@/lib/format'
import { texts } from '@/locales/ru'
import { cn } from '@/lib/utils'
import style from './kpiGrid.module.css'

const CARD_ANIMATION = { duration: 0.25, ease: 'easeOut' } as const

const CARD_DELAY_STEP = 0.04

interface CardSpec {
  key: string
  label: string
  icon: LucideIcon
  metric: (kpi: Kpi) => Metric
  format: (value: number | null | undefined) => string
  hint?: (kpi: Kpi) => string | null
}

const CARDS: CardSpec[] = [
  {
    key: 'revenue',
    label: texts.kpi.revenue,
    icon: Banknote,
    metric: (kpi) => kpi.revenue,
    format: formatMoney,
  },
  {
    key: 'grossProfit',
    label: texts.kpi.grossProfit,
    icon: Coins,
    metric: (kpi) => kpi.grossProfit,
    format: formatMoney,
  },
  {
    key: 'margin',
    label: texts.kpi.margin,
    icon: Percent,
    metric: (kpi) => kpi.margin,
    format: (value) => formatPercent(value),
  },
  {
    key: 'salesCount',
    label: texts.kpi.salesCount,
    icon: ShoppingCart,
    metric: (kpi) => kpi.salesCount,
    format: formatCount,
    hint: (kpi) =>
      kpi.cancelledCount + kpi.refundedCount > 0
        ? texts.kpi.cancelledAndRefunded(kpi.cancelledCount, kpi.refundedCount)
        : null,
  },
  {
    key: 'averageCheck',
    label: texts.kpi.averageCheck,
    icon: Receipt,
    metric: (kpi) => kpi.averageCheck,
    format: formatMoney,
  },
]

export function KpiGrid() {
  const { params } = usePeriod()
  const { data, isPending, isError, error, refetch, isFetching } = useKpi(params)

  if (isError) {
    return <ErrorState message={(error as Error)?.message} onRetry={() => void refetch()} />
  }

  return (
    <div className={style.grid}>
      {CARDS.map((card, index) => (
        <motion.article
          key={card.key}
          initial={{ opacity: 0, y: 8 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ ...CARD_ANIMATION, delay: index * CARD_DELAY_STEP }}
          className={cn(style.card, isFetching && style.fetching)}
        >
          <div className={style.cardHead}>
            <span className={style.label}>{card.label}</span>
            <card.icon className={style.icon} />
          </div>

          {isPending || !data ? (
            <>
              <Skeleton className={style.skeletonValue} />
              <Skeleton className={style.skeletonDelta} />
            </>
          ) : (
            <>
              <p className={style.value}>
                <AnimatedNumber value={card.metric(data).current} format={card.format} />
              </p>
              <div className={style.delta}>
                <Delta value={card.metric(data).changeRate} />
              </div>
              {card.hint?.(data) ? <p className={style.hint}>{card.hint(data)}</p> : null}
            </>
          )}
        </motion.article>
      ))}

      <motion.article
        initial={{ opacity: 0, y: 8 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ ...CARD_ANIMATION, delay: CARDS.length * CARD_DELAY_STEP }}
        className={style.top}
      >
        <div className={style.cardHead}>
          <span className={style.topLabel}>{texts.kpi.topManager}</span>
          <Trophy className={style.topIcon} />
        </div>

        {isPending || !data ? (
          <Skeleton className={style.topSkeleton} />
        ) : data.topManager ? (
          <div className={style.topBody}>
            <Avatar
              initials={data.topManager.initials}
              color={data.topManager.avatarColor}
              size="sm"
            />
            <div>
              <p className={style.topName}>{data.topManager.name}</p>
              <p className={style.topProfit}>
                {formatMoney(data.topManager.grossProfit)} {texts.kpi.topManagerProfit}
              </p>
            </div>
          </div>
        ) : (
          <p className={style.topEmpty}>{texts.kpi.noPaidSales}</p>
        )}

        {data ? (
          <p className={style.topPeriod}>{formatPeriod(data.period.from, data.period.to)}</p>
        ) : null}
      </motion.article>
    </div>
  )
}
