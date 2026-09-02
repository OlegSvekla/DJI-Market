import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { apiGet } from './client'
import type {
  CategorySlice,
  Filters,
  Kpi,
  ManagerRating,
  ManagerSortBy,
  Paged,
  PeriodPreset,
  RecentSale,
  SaleStatus,
  TimeSeries,
  TopProduct,
} from './types'

export interface PeriodParams {
  preset: PeriodPreset
  from?: string
  to?: string
}

const keys = {
  kpi: (period: PeriodParams) => ['kpi', period] as const,
  managers: (period: PeriodParams, sortBy: ManagerSortBy) => ['managers', period, sortBy] as const,
  timeseries: (period: PeriodParams) => ['timeseries', period] as const,
  categories: (period: PeriodParams) => ['categories', period] as const,
  topProducts: (period: PeriodParams, limit: number) => ['top-products', period, limit] as const,
  recentSales: (period: PeriodParams, page: number, pageSize: number, status?: SaleStatus) =>
    ['recent-sales', period, page, pageSize, status ?? null] as const,
  filters: () => ['filters'] as const,
}

export function useKpi(period: PeriodParams) {
  return useQuery({
    queryKey: keys.kpi(period),
    queryFn: () => apiGet<Kpi>('/analytics/kpi', { ...period }),
    placeholderData: keepPreviousData,
  })
}

export function useManagerRating(period: PeriodParams, sortBy: ManagerSortBy) {
  return useQuery({
    queryKey: keys.managers(period, sortBy),
    queryFn: () => apiGet<ManagerRating>('/analytics/managers', { ...period, sortBy }),
    placeholderData: keepPreviousData,
  })
}

export function useTimeSeries(period: PeriodParams) {
  return useQuery({
    queryKey: keys.timeseries(period),
    queryFn: () => apiGet<TimeSeries>('/analytics/timeseries', { ...period, granularity: 'Auto' }),
    placeholderData: keepPreviousData,
  })
}

export function useCategories(period: PeriodParams) {
  return useQuery({
    queryKey: keys.categories(period),
    queryFn: () => apiGet<CategorySlice[]>('/analytics/categories', { ...period }),
    placeholderData: keepPreviousData,
  })
}

export function useTopProducts(period: PeriodParams, limit = 6) {
  return useQuery({
    queryKey: keys.topProducts(period, limit),
    queryFn: () => apiGet<TopProduct[]>('/analytics/top-products', { ...period, limit }),
    placeholderData: keepPreviousData,
  })
}

export function useRecentSales(
  period: PeriodParams,
  page: number,
  pageSize: number,
  status?: SaleStatus,
) {
  return useQuery({
    queryKey: keys.recentSales(period, page, pageSize, status),
    queryFn: () =>
      apiGet<Paged<RecentSale>>('/sales/recent', { ...period, page, pageSize, status }),
    placeholderData: keepPreviousData,
  })
}

export function useFilters() {
  return useQuery({
    queryKey: keys.filters(),
    queryFn: () => apiGet<Filters>('/meta/filters'),
    staleTime: Infinity,
  })
}
