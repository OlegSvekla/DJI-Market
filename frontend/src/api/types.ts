export type PeriodPreset =
  | 'Today'
  | 'Last7Days'
  | 'Last30Days'
  | 'ThisMonth'
  | 'LastMonth'
  | 'Custom'

export type ManagerSortBy = 'GrossProfit' | 'AverageCheck' | 'Revenue'

export type TimeGranularity = 'Auto' | 'Day' | 'Week' | 'Month'

export type SaleStatus = 'Paid' | 'Cancelled' | 'Refunded'

export interface PeriodRs {
  from: string
  to: string
}

export interface Metric {
  current: number | null
  previous: number | null
  changeRate: number | null
}

export interface TopManager {
  id: number
  name: string
  initials: string
  avatarColor: string
  grossProfit: number
}

export interface Kpi {
  period: PeriodRs
  previousPeriod: PeriodRs
  revenue: Metric
  grossProfit: Metric
  margin: Metric
  salesCount: Metric
  averageCheck: Metric
  refundedAmount: Metric
  refundRate: number | null
  cancelledCount: number
  refundedCount: number
  topManager: TopManager | null
}

export interface ManagerRatingItem {
  position: number
  managerId: number
  name: string
  initials: string
  avatarColor: string
  team: string
  isActive: boolean
  salesCount: number
  revenue: number
  grossProfit: number
  averageCheck: number | null
  margin: number | null
  grossProfitChange: number | null
  averageCheckChange: number | null
  spark: number[]
}

export interface ManagerRating {
  period: PeriodRs
  previousPeriod: PeriodRs
  items: ManagerRatingItem[]
}

export interface TimeSeriesPoint {
  date: string
  revenue: number
  grossProfit: number
  salesCount: number
}

export interface TimeSeries {
  period: PeriodRs
  granularity: TimeGranularity
  points: TimeSeriesPoint[]
}

export interface CategorySlice {
  categoryId: number
  name: string
  revenue: number
  grossProfit: number
  margin: number | null
  share: number
}

export interface TopProduct {
  productId: number
  name: string
  category: string
  quantity: number
  revenue: number
  grossProfit: number
  margin: number | null
}

export interface ManagerBrief {
  id: number
  name: string
  initials: string
  avatarColor: string
}

export interface RecentSale {
  id: number
  number: string
  date: string
  manager: ManagerBrief
  customerCompany: string
  customerName: string
  status: SaleStatus
  itemsCount: number
  itemsPreview: string
  amount: number
  grossProfit: number
}

export interface Paged<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}

export interface FilterOption {
  id: number
  name: string
}

export interface Filters {
  managers: FilterOption[]
  categories: FilterOption[]
  statuses: SaleStatus[]
  firstSaleDate: string | null
  lastSaleDate: string | null
}
