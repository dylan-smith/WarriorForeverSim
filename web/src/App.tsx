import { useEffect, useState } from 'react'
import { getOptions, simulate, type SimulateRequest, type SimulateResponse, type SimulationOptions } from './api'
import { CombatLog } from './components/CombatLog'
import { Results } from './components/Results'
import { SimForm } from './components/SimForm'

function App() {
  const [options, setOptions] = useState<SimulationOptions | null>(null)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [result, setResult] = useState<SimulateResponse | null>(null)
  const [runError, setRunError] = useState<string | null>(null)
  const [running, setRunning] = useState(false)

  useEffect(() => {
    getOptions()
      .then(setOptions)
      .catch((error: Error) => setLoadError(error.message))
  }, [])

  const handleRun = async (request: SimulateRequest) => {
    setRunning(true)
    setRunError(null)
    try {
      setResult(await simulate(request))
    } catch (error) {
      setRunError(error instanceof Error ? error.message : String(error))
    } finally {
      setRunning(false)
    }
  }

  return (
    <main>
      <header>
        <h1>Warrior Forever Sim</h1>
        <p>Combat simulator for warriors in World of Warcraft Forever.</p>
      </header>

      {loadError ? (
        <p className="message error" role="alert">
          Could not reach the simulator API: {loadError}
        </p>
      ) : !options ? (
        <p className="empty">Loading…</p>
      ) : (
        <>
          <div className="layout">
            <SimForm options={options} running={running} onRun={handleRun} />
            <Results result={result} error={runError} running={running} />
          </div>
          {result && result.errors.length === 0 && !runError ? <CombatLog entries={result.firstRunLog} /> : null}
        </>
      )}
    </main>
  )
}

export default App
