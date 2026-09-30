import { useState } from 'react'
import type { SimulationLogEntry } from '../api'

interface CombatLogProps {
  entries: SimulationLogEntry[]
}

// Event type names arrive as C# class names (e.g. "AutoAttackSwingEvent").
const eventLabel = (name: string) => name.replace(/Event$/, '').replace(/([a-z])([A-Z])/g, '$1 $2')

const percent = (value: number | null) => (value === null ? '' : `${(value * 100).toFixed(1)}%`)

export function CombatLog({ entries }: CombatLogProps) {
  const [damageOnly, setDamageOnly] = useState(false)

  const visible = damageOnly ? entries.filter((entry) => entry.damageType !== null) : entries

  return (
    <section className="card combat-log">
      <div className="combat-log-header">
        <h2>Combat log — first iteration</h2>
        <span className="note">
          {visible.length} of {entries.length} events
        </span>
        <label className="toggle">
          <input type="checkbox" checked={damageOnly} onChange={(e) => setDamageOnly(e.target.checked)} />
          Damage events only
        </label>
      </div>

      {entries.length === 0 ? (
        <p className="empty">No events were recorded.</p>
      ) : (
        <div className="log-scroll">
          <table>
            <thead>
              <tr>
                <th className="num">Time (s)</th>
                <th>Event</th>
                <th>Details</th>
                <th className="num">Damage</th>
                <th className="num">Miss / Crit chance</th>
                <th className="num">Total damage</th>
              </tr>
            </thead>
            <tbody>
              {visible.map((entry, index) => (
                <tr key={index} className={entry.damageType ? `row-${entry.damageType.toLowerCase()}` : 'row-other'}>
                  <td className="num">{entry.timestamp.toFixed(2)}</td>
                  <td>{eventLabel(entry.event)}</td>
                  <td>{entry.description}</td>
                  <td className="num">{entry.damage === null ? '' : entry.damage.toFixed(1)}</td>
                  <td className="num">
                    {entry.damageType === null ? '' : `${percent(entry.missChance)} / ${percent(entry.critChance)}`}
                  </td>
                  <td className="num">{entry.totalDamage.toFixed(0)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}
