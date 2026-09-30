import { useState, type FormEvent } from 'react'
import type { SimulateRequest, SimulationOptions } from '../api'
import { spaced } from '../format'

interface SimFormProps {
  options: SimulationOptions
  running: boolean
  onRun: (request: SimulateRequest) => void
}

const BOSS_LEVELS = [60, 61, 62, 63]

export function SimForm({ options, running, onRun }: SimFormProps) {
  const [request, setRequest] = useState<SimulateRequest>(options.defaults)

  const update = <K extends keyof SimulateRequest>(key: K, value: SimulateRequest[K]) =>
    setRequest((current) => ({ ...current, [key]: value }))

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    onRun(request)
  }

  return (
    <form className="card sim-form" onSubmit={handleSubmit}>
      <h2>Settings</h2>

      <label>
        Race
        <select value={request.race} onChange={(e) => update('race', e.target.value)}>
          {options.races.map((race) => (
            <option key={race} value={race}>
              {spaced(race)}
            </option>
          ))}
        </select>
      </label>

      <label>
        Boss level
        <select value={request.bossLevel} onChange={(e) => update('bossLevel', Number(e.target.value))}>
          {BOSS_LEVELS.map((level) => (
            <option key={level} value={level}>
              {level}
            </option>
          ))}
        </select>
      </label>

      <label>
        Boss type
        <select value={request.bossType} onChange={(e) => update('bossType', e.target.value)}>
          {options.bossTypes.map((type) => (
            <option key={type} value={type}>
              {spaced(type)}
            </option>
          ))}
        </select>
      </label>

      <label>
        Fight length (seconds)
        <input
          type="number"
          min={1}
          max={600}
          step={1}
          value={request.fightLength}
          onChange={(e) => update('fightLength', Number(e.target.value))}
        />
      </label>

      <label>
        Iterations
        <input
          type="number"
          min={1}
          max={1000}
          step={1}
          value={request.iterations}
          onChange={(e) => update('iterations', Number(e.target.value))}
        />
      </label>

      <button type="submit" disabled={running}>
        {running ? 'Running…' : 'Run simulation'}
      </button>

      <p className="note">Gear and talents use the default preset for now.</p>
    </form>
  )
}
