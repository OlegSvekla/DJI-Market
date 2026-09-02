import { useFilters } from '@/api/queries'
import { formatPeriod } from '@/lib/format'
import { texts } from '@/locales/ru'
import { CategoryBreakdown } from '@/widgets/categories/CategoryBreakdown'
import { PeriodPicker } from '@/widgets/filters/PeriodPicker'
import { usePeriod } from '@/widgets/filters/periodContext'
import { DealQuality } from '@/widgets/kpi/DealQuality'
import { KpiGrid } from '@/widgets/kpi/KpiGrid'
import { ManagerRating } from '@/widgets/managers/ManagerRating'
import { ManagerScatter } from '@/widgets/managers/ManagerScatter'
import { TopProducts } from '@/widgets/products/TopProducts'
import { RecentSales } from '@/widgets/sales/RecentSales'
import { DynamicsChart } from '@/widgets/timeseries/DynamicsChart'
import style from './app.module.css'

export default function App() {
  const { data: filters } = useFilters()
  const { preset, custom } = usePeriod()

  return (
    <div className={style.page}>
      <header className={style.header}>
        <div className={style.headerInner}>
          <div>
            <div className={style.brand}>
              <span className={style.logo}>{texts.app.brand}</span>
              <h1 className={style.title}>{texts.app.title}</h1>
            </div>
            <p className={style.subtitle}>
              {preset === 'Custom' ? formatPeriod(custom.from, custom.to) : texts.app.subtitle}
            </p>
          </div>

          <PeriodPicker
            minDate={filters?.firstSaleDate ?? undefined}
            maxDate={filters?.lastSaleDate ?? undefined}
          />
        </div>
      </header>

      <main className={style.main}>
        <KpiGrid />

        <div className="grid-12">
          <DynamicsChart />
          <CategoryBreakdown />
        </div>

        <div className="grid-12">
          <ManagerRating />
          <TopProducts />
        </div>

        <div className="grid-12">
          <ManagerScatter />
          <DealQuality />
        </div>

        <div className="grid-12">
          <RecentSales />
        </div>

        <footer className={style.footnote}>{texts.app.footnote}</footer>
      </main>
    </div>
  )
}
