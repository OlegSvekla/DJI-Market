import { useState } from 'react'
import {
  Area,
  AreaChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import { useTimeSeries } from '@/api/queries'
import { Card, CardHeader } from '@/components/ui/Card'
import { QueryState } from '@/components/ui/QueryState'
import { EmptyState, Skeleton } from '@/components/ui/States'
import { axisTick, chartColors, tooltipStyle } from '@/lib/chartTheme'
import {
  formatCompactMoney,
  formatCount,
  formatMoney,
  formatMonth,
  formatShortDate,
} from '@/lib/format'
import { cn } from '@/lib/utils'
import { texts } from '@/locales/ru'
import { usePeriod } from '@/widgets/filters/periodContext'
import style from './dynamicsChart.module.css'

type SeriesKey = 'revenue' | 'grossProfit' | 'salesCount'

const SERIES: Array<{ key: SeriesKey; label: string; color: string }> = [
  { key: 'revenue', label: texts.dynamics.revenue, color: chartColors.revenue },
  { key: 'grossProfit', label: texts.dynamics.grossProfit, color: chartColors.grossProfit },
  { key: 'salesCount', label: texts.dynamics.salesCount, color: chartColors.salesCount },
]

const GRADIENT_ID = 'seriesFill'

export function DynamicsChart() {
  const { params } = usePeriod()
  const [series, setSeries] = useState<SeriesKey>('revenue')
  const query = useTimeSeries(params)

  const active = SERIES.find((item) => item.key === series)!
  const isMonthly = query.data?.granularity === 'Month'
  const formatAxisDate = (value: string) =>
    isMonthly ? formatMonth(value) : formatShortDate(value)

  return (
    <Card className="span-8">
      <CardHeader
        title={texts.dynamics.title}
        subtitle={
          query.data
            ? `${texts.dynamics.stepPrefix}: ${texts.dynamics.steps[query.data.granularity]}`
            : undefined
        }
        action={
          <div className={style.series}>
            {SERIES.map((item) => (
              <button
                key={item.key}
                type="button"
                onClick={() => setSeries(item.key)}
                className={cn(style.seriesButton, series === item.key && style.seriesActive)}
              >
                {item.label}
              </button>
            ))}
          </div>
        }
      />

      <div className={style.body}>
        <QueryState
          query={query}
          skeleton={<Skeleton className={style.full} />}
          isEmpty={(data) => !data.points.some((point) => point.salesCount > 0)}
          empty={
            <EmptyState
              title={texts.dynamics.empty}
              hint={texts.dynamics.emptyHint}
              className={style.full}
            />
          }
        >
          {(data) => (
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={data.points} margin={{ top: 4, right: 8, bottom: 0, left: 8 }}>
                <defs>
                  <linearGradient id={GRADIENT_ID} x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor={active.color} stopOpacity={0.22} />
                    <stop offset="100%" stopColor={active.color} stopOpacity={0.02} />
                  </linearGradient>
                </defs>

                <CartesianGrid strokeDasharray="3 3" stroke={chartColors.grid} vertical={false} />

                <XAxis
                  dataKey="date"
                  tickFormatter={formatAxisDate}
                  tick={axisTick}
                  tickLine={false}
                  axisLine={{ stroke: chartColors.axis }}
                  minTickGap={24}
                />

                <YAxis
                  tickFormatter={(value: number) =>
                    series === 'salesCount' ? formatCount(value) : formatCompactMoney(value)
                  }
                  tick={axisTick}
                  tickLine={false}
                  axisLine={false}
                  width={72}
                />

                <Tooltip
                  cursor={{ stroke: chartColors.cursor, strokeDasharray: '4 4' }}
                  contentStyle={tooltipStyle}
                  labelFormatter={(value) => formatAxisDate(String(value))}
                  formatter={(value) => [
                    series === 'salesCount'
                      ? formatCount(Number(value))
                      : formatMoney(Number(value)),
                    active.label,
                  ]}
                />

                <Area
                  type="monotone"
                  dataKey={series}
                  stroke={active.color}
                  strokeWidth={2}
                  fill={`url(#${GRADIENT_ID})`}
                  animationDuration={450}
                  dot={false}
                  activeDot={{ r: 4, strokeWidth: 2, stroke: '#fff' }}
                />
              </AreaChart>
            </ResponsiveContainer>
          )}
        </QueryState>
      </div>
    </Card>
  )
}
