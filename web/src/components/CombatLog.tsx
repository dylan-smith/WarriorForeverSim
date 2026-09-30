import { useState, type FocusEvent, type KeyboardEvent, type MouseEvent } from 'react'
import type { SimulationLogEntry } from '../api'
import { eventLabel, percent } from '../format'
import { EventTooltip, type TooltipAnchor } from './EventTooltip'

interface CombatLogProps {
  entries: SimulationLogEntry[]
}

interface Hovered {
  index: number
  anchor: TooltipAnchor
}

export function CombatLog({ entries }: CombatLogProps) {
  const [damageOnly, setDamageOnly] = useState(false)
  const [hovered, setHovered] = useState<Hovered | null>(null)

  const visible = damageOnly ? entries.filter((entry) => entry.damageType !== null) : entries
  const hoveredEntry = hovered ? visible[hovered.index] : undefined

  const showAtPointer = (index: number) => (event: MouseEvent) =>
    setHovered({ index, anchor: { x: event.clientX, y: event.clientY } })

  // Keyboard focus has no pointer position, so anchor to the row's lower-left area.
  const showAtRow = (index: number) => (event: FocusEvent<HTMLTableRowElement>) => {
    const rect = event.currentTarget.getBoundingClientRect()
    setHovered({ index, anchor: { x: rect.left + 24, y: rect.bottom - 8 } })
  }

  const hide = () => setHovered(null)

  const hideOnEscape = (event: KeyboardEvent) => {
    if (event.key === 'Escape') {
      hide()
    }
  }

  const toggleDamageOnly = (checked: boolean) => {
    hide()
    setDamageOnly(checked)
  }

  return (
    <section className="card combat-log">
      <div className="combat-log-header">
        <h2>Combat log — first iteration</h2>
        <span className="note">
          {visible.length} of {entries.length} events · hover a row for details
        </span>
        <label className="toggle">
          <input type="checkbox" checked={damageOnly} onChange={(e) => toggleDamageOnly(e.target.checked)} />
          Damage events only
        </label>
      </div>

      {entries.length === 0 ? (
        <p className="empty">No events were recorded.</p>
      ) : (
        <div className="log-scroll" onScroll={hide}>
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
            <tbody onKeyDown={hideOnEscape}>
              {visible.map((entry, index) => {
                const kind = entry.damageType ? `row-${entry.damageType.toLowerCase()}` : 'row-other'
                return (
                  <tr
                    key={index}
                    className={hovered?.index === index ? `${kind} row-active` : kind}
                    tabIndex={0}
                    onMouseEnter={showAtPointer(index)}
                    onMouseMove={showAtPointer(index)}
                    onMouseLeave={hide}
                    onFocus={showAtRow(index)}
                    onBlur={hide}
                  >
                    <td className="num">{entry.timestamp.toFixed(2)}</td>
                    <td>{eventLabel(entry.event)}</td>
                    <td>{entry.description}</td>
                    <td className="num">{entry.damage === null ? '' : entry.damage.toFixed(1)}</td>
                    <td className="num">
                      {entry.damageType === null ? '' : `${percent(entry.missChance)} / ${percent(entry.critChance)}`}
                    </td>
                    <td className="num">{entry.totalDamage.toFixed(0)}</td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      )}

      {hovered && hoveredEntry ? <EventTooltip entry={hoveredEntry} anchor={hovered.anchor} /> : null}
    </section>
  )
}
