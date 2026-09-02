import {
  CartesianGrid,
  ResponsiveContainer,
  Scatter,
  ScatterChart,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { useManagerRating } from '@/api/queries'
import { Card, CardHeader } from '@/components/ui/Card'
import { QueryState } from '@/components/ui/QueryState'
import { EmptyState, Skeleton } from '@/components/ui/States'
import { axisTick, chartColors } from '@/lib/chartTheme'
import { formatCompactMoney, formatCount, formatMoney, formatPercent } from '@/lib/format'
import { texts } from '@/locales/ru'
import { usePeriod } from '@/widgets/filters/periodContext'
import style from './managerScatter.module.css'

const BUBBLE_BASE_RADIUS = 5

const BUBBLE_SCALE = 1.7

const PERCENT_FACTOR = 100

interface Point {
  name: string
  color: string
  revenue: number
  margin: number
  salesCount: number
  averageCheck: number | null
}

function ManagerBubble(props: unknown) {
  const { cx, cy, payload } = props as { cx: number; cy: number; payload: Point }
  const radius = BUBBLE_BASE_RADIUS + Math.sqrt(payload.salesCount) * BUBBLE_SCALE

  return (
    <circle
      cx={cx}
      cy={cy}
      r={radius}
      fill={payload.color}
      fillOpacity={0.55}
      stroke={payload.color}
      strokeWidth={1.5}
    />
  )
}

export function ManagerScatter() {
  const { params } = usePeriod()
  const query = useManagerRating(params, 'GrossProfit')

  const points: Point[] = (query.data?.items ?? [])
    .filter((item) => item.salesCount > 0 && item.margin !== null)
    .map((item) => ({
      name: item.name,
      color: item.avatarColor,
      revenue: item.revenue,
      margin: (item.margin ?? 0) * PERCENT_FACTOR,
      salesCount: item.salesCount,
      averageCheck: item.averageCheck,
    }))

  return (
    <Card className="span-8">
      <CardHeader title={texts.scatter.title} subtitle={texts.scatter.subtitle} />

      <div className={style.body}>
        <QueryState
          query={query}
          skeleton={<Skeleton className={style.full} />}
          isEmpty={() => points.length === 0}
          empty={<EmptyState title={texts.scatter.empty} className={style.full} />}
        >
          {() => (
            <ResponsiveContainer width="100%" height="100%">
              <ScatterChart margin={{ top: 8, right: 16, bottom: 16, left: 8 }}>
                <CartesianGrid strokeDasharray="3 3" stroke={chartColors.grid} />

                <XAxis
                  type="number"
                  dataKey="revenue"
                  name={texts.scatter.revenue}
                  tickFormatter={(value) => formatCompactMoney(Number(value))}
                  tick={axisTick}
                  tickLine={false}
                  axisLine={{ stroke: chartColors.axis }}
                  label={{
                    value: texts.scatter.revenue,
                    position: 'insideBottom',
                    offset: -8,
                    fill: chartColors.axisText,
                    fontSize: 11,
                  }}
                />

                <YAxis
                  type="number"
                  dataKey="margin"
                  name={texts.scatter.margin}
                  unit="%"
                  tick={axisTick}
                  tickLine={false}
                  axisLine={false}
                  width={52}
                />

                <Tooltip
                  cursor={{ strokeDasharray: '4 4', stroke: chartColors.cursor }}
                  content={({ active, payload }) => {
                    if (!active || !payload?.length) return null

                    const point = payload[0].payload as Point

                    return (
                      <div className={style.tooltip}>
                        <p className={style.tooltipName}>{point.name}</p>
                        <p className={style.tooltipRow}>
                          {texts.scatter.revenue}: {formatMoney(point.revenue)}
                        </p>
                        <p className={style.tooltipRow}>
                          {texts.scatter.margin}: {formatPercent(point.margin / PERCENT_FACTOR)}
                        </p>
                        <p className={style.tooltipRow}>
                          {texts.scatter.deals}: {formatCount(point.salesCount)} ·{' '}
                          {texts.scatter.averageCheck} {formatMoney(point.averageCheck)}
                        </p>
                      </div>
                    )
                  }}
                />

                <Scatter data={points} animationDuration={500} shape={ManagerBubble} />
              </ScatterChart>
            </ResponsiveContainer>
          )}
        </QueryState>
      </div>
    </Card>
  )
}
