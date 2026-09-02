import { motion } from 'motion/react'
import { useKpi } from '@/api/queries'
import { Card, CardHeader } from '@/components/ui/Card'
import { Delta } from '@/components/ui/Delta'
import { ErrorState, Skeleton } from '@/components/ui/States'
import { usePeriod } from '@/widgets/filters/periodContext'
import { formatCount, formatMoney, formatPercent } from '@/lib/format'
import { texts } from '@/locales/ru'
import style from './dealQuality.module.css'

const BAR_ANIMATION = { duration: 0.5, ease: 'easeOut' } as const

export function DealQuality() {
  const { params } = usePeriod()
  const { data, isPending, isError, error, refetch } = useKpi(params)

  const paid = data?.salesCount.current ?? 0
  const total = paid + (data?.cancelledCount ?? 0) + (data?.refundedCount ?? 0)
  const successShare = total === 0 ? 0 : paid / total

  return (
    <Card className="span-4">
      <CardHeader title={texts.quality.title} subtitle={texts.quality.subtitle} />

      <div className={style.body}>
        {isError ? (
          <ErrorState message={(error as Error)?.message} onRetry={() => void refetch()} />
        ) : isPending || !data ? (
          <div className={style.skeletons}>
            <Skeleton className={style.skeletonWide} />
            <Skeleton className={style.skeletonLine} />
            <Skeleton className={style.skeletonBlock} />
          </div>
        ) : (
          <>
            <div>
              <div className={style.headline}>
                <span className={style.share}>{formatPercent(successShare, 1)}</span>
                <span className={style.caption}>{texts.quality.successShare}</span>
              </div>

              <div className={style.track}>
                <motion.div
                  className={style.paidBar}
                  initial={{ width: 0 }}
                  animate={{ width: `${successShare * 100}%` }}
                  transition={BAR_ANIMATION}
                />
                <motion.div
                  className={style.refundedBar}
                  initial={{ width: 0 }}
                  animate={{
                    width: total === 0 ? '0%' : `${(data.refundedCount / total) * 100}%`,
                  }}
                  transition={{ ...BAR_ANIMATION, delay: 0.1 }}
                />
              </div>

              <div className={style.legend}>
                <span className={style.legendItem}>
                  <span className={`${style.dot} ${style.dotPaid}`} /> {texts.quality.paid}{' '}
                  {formatCount(paid)}
                </span>
                <span className={style.legendItem}>
                  <span className={`${style.dot} ${style.dotRefunded}`} /> {texts.quality.refunded}{' '}
                  {formatCount(data.refundedCount)}
                </span>
                <span className={style.legendItem}>
                  <span className={`${style.dot} ${style.dotCancelled}`} />{' '}
                  {texts.quality.cancelled} {formatCount(data.cancelledCount)}
                </span>
              </div>
            </div>

            <dl className={style.rows}>
              <div className={style.row}>
                <dt className={style.rowLabel}>{texts.quality.refundedAmount}</dt>
                <dd className={style.rowValue}>
                  {formatMoney(data.refundedAmount.current)}
                  <Delta value={data.refundedAmount.changeRate} inverted />
                </dd>
              </div>

              <div className={style.row}>
                <dt className={style.rowLabel}>{texts.quality.refundShare}</dt>
                <dd className={style.rowValue}>{formatPercent(data.refundRate, 1)}</dd>
              </div>
            </dl>
          </>
        )}
      </div>
    </Card>
  )
}
