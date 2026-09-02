export const chartColors = {
  revenue: '#2563eb',
  grossProfit: '#059669',
  salesCount: '#b45309',
  negative: '#dc2626',
  grid: '#eef1f5',
  axis: '#e2e8f0',
  axisText: '#94a3b8',
  cursor: '#cbd5e1',
} as const

export const categoryPalette = [
  '#2563eb',
  '#7c3aed',
  '#0891b2',
  '#059669',
  '#d97706',
  '#db2777',
] as const

export const axisTick = { fill: chartColors.axisText, fontSize: 11 } as const

export const tooltipStyle = {
  borderRadius: 10,
  border: `1px solid ${chartColors.axis}`,
  boxShadow: '0 8px 24px rgba(15,23,42,0.08)',
  fontSize: 12,
} as const
