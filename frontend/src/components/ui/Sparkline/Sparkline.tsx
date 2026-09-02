interface SparklineProps {
  points: number[]
  color: string
  width?: number
  height?: number
}

export function Sparkline({ points, color, width = 76, height = 24 }: SparklineProps) {
  if (points.length < 2 || points.every((point) => point === 0)) {
    return <span className="inline-block h-6 w-[76px]" aria-hidden />
  }

  const max = Math.max(...points)
  const min = Math.min(...points, 0)
  const span = max - min || 1
  const step = width / (points.length - 1)

  const line = points
    .map((point, index) => {
      const x = index * step
      const y = height - ((point - min) / span) * (height - 4) - 2

      return `${index === 0 ? 'M' : 'L'}${x.toFixed(1)},${y.toFixed(1)}`
    })
    .join(' ')

  const area = `${line} L${width},${height} L0,${height} Z`

  return (
    <svg width={width} height={height} viewBox={`0 0 ${width} ${height}`} aria-hidden>
      <path d={area} fill={color} fillOpacity={0.12} />
      <path d={line} fill="none" stroke={color} strokeWidth={1.5} strokeLinejoin="round" />
    </svg>
  )
}
