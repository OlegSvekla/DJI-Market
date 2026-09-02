import { useTopProducts } from '@/api/queries'
import { Card, CardHeader } from '@/components/ui/Card'
import { QueryState } from '@/components/ui/QueryState'
import { EmptyState, SkeletonRows } from '@/components/ui/States'
import { usePeriod } from '@/widgets/filters/periodContext'
import { formatCount, formatMoney, formatPercent } from '@/lib/format'
import { texts } from '@/locales/ru'
import style from './topProducts.module.css'

const TOP_LIMIT = 6

export function TopProducts() {
  const { params } = usePeriod()
  const query = useTopProducts(params, TOP_LIMIT)

  return (
    <Card className="span-4">
      <CardHeader title={texts.products.title} subtitle={texts.products.subtitle} />

      <div className={style.body}>
        <QueryState
          query={query}
          skeleton={<SkeletonRows rows={TOP_LIMIT} />}
          isEmpty={(products) => products.length === 0}
          empty={<EmptyState title={texts.products.empty} />}
        >
          {(products) => (
            <ol className={style.list}>
              {products.map((product, index) => (
                <li key={product.productId} className={style.item}>
                  <span className={style.rank}>{index + 1}</span>

                  <div className={style.info}>
                    <p className={style.name}>{product.name}</p>
                    <p className={style.meta}>
                      {product.category} · {formatCount(product.quantity)} {texts.products.units} ·{' '}
                      {texts.products.margin} {formatPercent(product.margin, 0)}
                    </p>
                  </div>

                  <span className={style.value}>{formatMoney(product.revenue)}</span>
                </li>
              ))}
            </ol>
          )}
        </QueryState>
      </div>
    </Card>
  )
}
