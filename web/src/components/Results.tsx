import type { SimulateResponse } from '../api'

interface ResultsProps {
  result: SimulateResponse | null
  error: string | null
  running: boolean
}

const dps = (value: number) => value.toFixed(1)
const percent = (part: number, total: number) => (total > 0 ? `${((part / total) * 100).toFixed(1)}%` : '—')

export function Results({ result, error, running }: ResultsProps) {
  if (error) {
    return (
      <section className="card results">
        <h2>Results</h2>
        <p className="message error" role="alert">
          {error}
        </p>
      </section>
    )
  }

  if (!result) {
    return (
      <section className="card results">
        <h2>Results</h2>
        <p className="empty">{running ? 'Simulating…' : 'Run a simulation to see results.'}</p>
      </section>
    )
  }

  const swings =
    result.averageHits + result.averageCrits + result.averageGlancings + result.averageDodges + result.averageMisses

  return (
    <section className={`card results${running ? ' stale' : ''}`} aria-busy={running}>
      <h2>Results</h2>

      {result.errors.length > 0 ? null : (
        <>
          <div className="headline">
            <span className="dps">{dps(result.averageDps)}</span>
            <span className="unit">DPS</span>
          </div>
          <p className="range">
            Min {dps(result.minDps)} · Max {dps(result.maxDps)} · {result.iterations} iterations
          </p>

          <dl className="stats">
            <div>
              <dt>Swings per fight</dt>
              <dd>{swings.toFixed(1)}</dd>
            </div>
            <div>
              <dt>Hit</dt>
              <dd>{percent(result.averageHits, swings)}</dd>
            </div>
            <div>
              <dt>Crit</dt>
              <dd>{percent(result.averageCrits, swings)}</dd>
            </div>
            <div>
              <dt>Glancing</dt>
              <dd>{percent(result.averageGlancings, swings)}</dd>
            </div>
            <div>
              <dt>Dodge</dt>
              <dd>{percent(result.averageDodges, swings)}</dd>
            </div>
            <div>
              <dt>Miss</dt>
              <dd>{percent(result.averageMisses, swings)}</dd>
            </div>
          </dl>
        </>
      )}

      {result.errors.map((message) => (
        <p key={message} className="message error">
          {message}
        </p>
      ))}
      {result.warnings.map((message) => (
        <p key={message} className="message warning">
          {message}
        </p>
      ))}
    </section>
  )
}
