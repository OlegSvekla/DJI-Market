import { useEffect, useState } from 'react'
import { ChevronLeft, ChevronRight } from 'lucide-react'
import { useRecentSales } from '@/api/queries'
import type { SaleStatus } from '@/api/types'
import { Avatar } from '@/components/ui/Avatar'
import { Card, CardHeader } from '@/components/ui/Card'
import { EmptyState, ErrorState, SkeletonRows } from '@/components/ui/States'
import { formatDate, formatMoney } from '@/lib/format'
import { cn } from '@/lib/utils'
import { texts } from '@/locales/ru'
import { usePeriod } from '@/widgets/filters/periodContext'
import style from './recentSales.module.css'

const PAGE_SIZE = 8

const NO_PROFIT = '—'

const STATUS_CLASS: Record<SaleStatus, string> = {
  Paid: style.statusPaid,
  Cancelled: style.statusCancelled,
  Refunded: style.statusRefunded,
}

const FILTERS: Array<{ value: SaleStatus | 'all'; label: string }> = [
  { value: 'all', label: texts.sales.filters.all },
  { value: 'Paid', label: texts.sales.filters.paid },
  { value: 'Refunded', label: texts.sales.filters.refunded },
  { value: 'Cancelled', label: texts.sales.filters.cancelled },
]

export function RecentSales() {
  const { params } = usePeriod()
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState<SaleStatus | 'all'>('all')

  useEffect(() => setPage(1), [params, status])

  const { data, isPending, isError, error, refetch, isFetching } = useRecentSales(
    params,
    page,
    PAGE_SIZE,
    status === 'all' ? undefined : status,
  )

  const totalPages = data ? Math.max(1, Math.ceil(data.total / data.pageSize)) : 1

  return (
    <Card className="span-12">
      <CardHeader
        title={texts.sales.title}
        subtitle={data ? texts.sales.dealsInPeriod(data.total) : undefined}
        action={
          <div className={style.filters}>
            {FILTERS.map((filter) => (
              <button
                key={filter.value}
                type="button"
                onClick={() => setStatus(filter.value)}
                className={cn(style.filter, status === filter.value && style.filterActive)}
              >
                {filter.label}
              </button>
            ))}
          </div>
        }
      />

      <div className={style.body}>
        {isError ? (
          <ErrorState message={(error as Error)?.message} onRetry={() => void refetch()} />
        ) : isPending ? (
          <SkeletonRows rows={PAGE_SIZE} />
        ) : data && data.items.length === 0 ? (
          <EmptyState title={texts.sales.empty} hint={texts.sales.emptyHint} />
        ) : (
          <table className={style.table}>
            <thead>
              <tr className={style.headRow}>
                <th>{texts.sales.columns.date}</th>
                <th>{texts.sales.columns.number}</th>
                <th>{texts.sales.columns.manager}</th>
                <th>{texts.sales.columns.customer}</th>
                <th>{texts.sales.columns.items}</th>
                <th>{texts.sales.columns.status}</th>
                <th className={style.alignRight}>{texts.sales.columns.amount}</th>
                <th className={style.alignRight}>{texts.sales.columns.grossProfit}</th>
              </tr>
            </thead>
            <tbody className={cn(isFetching && style.fetching)}>
              {data?.items.map((sale) => (
                <tr key={sale.id} className={style.row}>
                  <td className={style.date}>{formatDate(sale.date)}</td>
                  <td className={style.number}>{sale.number}</td>
                  <td>
                    <div className={style.manager}>
                      <Avatar
                        initials={sale.manager.initials}
                        color={sale.manager.avatarColor}
                        size="sm"
                      />
                      <span className={style.managerName}>{sale.manager.name}</span>
                    </div>
                  </td>
                  <td>
                    <p className={style.company}>{sale.customerCompany}</p>
                    <p className={style.contact}>{sale.customerName}</p>
                  </td>
                  <td className={style.itemsCell}>
                    <p className={style.items}>{sale.itemsPreview}</p>
                    {sale.itemsCount > 1 ? (
                      <p className={style.itemsMore}>{texts.sales.moreItems(sale.itemsCount - 1)}</p>
                    ) : null}
                  </td>
                  <td>
                    <span className={cn(style.status, STATUS_CLASS[sale.status])}>
                      {texts.sales.statuses[sale.status]}
                    </span>
                  </td>
                  <td className={sale.status === 'Paid' ? style.amount : style.amountVoid}>
                    {formatMoney(sale.amount)}
                  </td>
                  <td className={sale.status === 'Paid' ? style.profit : style.profitVoid}>
                    {sale.status === 'Paid' ? formatMoney(sale.grossProfit) : NO_PROFIT}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {data && data.total > PAGE_SIZE ? (
        <footer className={style.footer}>
          <span className={style.pageLabel}>{texts.sales.page(data.page, totalPages)}</span>
          <div className={style.pager}>
            <button
              type="button"
              disabled={page <= 1}
              onClick={() => setPage((value) => Math.max(1, value - 1))}
              className={style.pagerButton}
            >
              <ChevronLeft className={style.pagerIcon} />
            </button>
            <button
              type="button"
              disabled={page >= totalPages}
              onClick={() => setPage((value) => value + 1)}
              className={style.pagerButton}
            >
              <ChevronRight className={style.pagerIcon} />
            </button>
          </div>
        </footer>
      ) : null}
    </Card>
  )
}
