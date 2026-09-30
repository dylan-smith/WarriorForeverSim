import { useLayoutEffect, useRef } from 'react'
import type { EventDetail, SimulationLogEntry } from '../api'
import { eventLabel, percent, spaced } from '../format'

export interface TooltipAnchor {
  x: number
  y: number
}

interface EventTooltipProps {
  entry: SimulationLogEntry
  anchor: TooltipAnchor
}

const OFFSET = 16
const MARGIN = 8

// Groups details by section, keeping the order the sim recorded them in.
function groupBySection(details: EventDetail[]) {
  const sections = new Map<string, EventDetail[]>()
  for (const detail of details) {
    sections.set(detail.section, [...(sections.get(detail.section) ?? []), detail])
  }
  return [...sections]
}

interface RollBarProps {
  label: string
  roll: number
  chance: number
  kind: 'miss' | 'crit'
}

// A 0–100% bar with the "success" region shaded and a marker at the roll: a roll inside
// the shaded region means the attack missed (miss bar) or crit (crit bar).
function RollBar({ label, roll, chance, kind }: RollBarProps) {
  const clamp = (value: number) => Math.min(Math.max(value, 0), 1) * 100
  const landed = roll <= chance

  return (
    <div className="roll">
      <div className="roll-label">
        <span>{label}</span>
        <span className={landed ? `roll-result ${kind}` : 'roll-result'}>
          {percent(roll)} {landed ? '≤' : '>'} {percent(chance)}
        </span>
      </div>
      <div className="roll-bar" aria-hidden="true">
        <div className={`roll-zone ${kind}`} style={{ width: `${clamp(chance)}%` }} />
        <div className="roll-marker" style={{ left: `${clamp(roll)}%` }} />
      </div>
    </div>
  )
}

export function EventTooltip({ entry, anchor }: EventTooltipProps) {
  const ref = useRef<HTMLDivElement>(null)

  // Place next to the anchor, flipping to the other side when it would leave the viewport.
  useLayoutEffect(() => {
    const el = ref.current
    if (!el) {
      return
    }
    const { width, height } = el.getBoundingClientRect()
    let left = anchor.x + OFFSET
    if (left + width > window.innerWidth - MARGIN) {
      left = anchor.x - OFFSET - width
    }
    let top = anchor.y + OFFSET
    if (top + height > window.innerHeight - MARGIN) {
      top = anchor.y - OFFSET - height
    }
    el.style.left = `${Math.max(MARGIN, left)}px`
    el.style.top = `${Math.max(MARGIN, top)}px`
  }, [anchor, entry])

  const rolls =
    entry.missRoll !== null && entry.missChance !== null ? (
      <div className="rolls">
        <RollBar label="Miss roll" roll={entry.missRoll} chance={entry.missChance} kind="miss" />
        {entry.critRoll !== null && entry.critRollChance !== null ? (
          <RollBar label="Crit roll" roll={entry.critRoll} chance={entry.critRollChance} kind="crit" />
        ) : (
          <p className="muted roll-skipped">Crit roll skipped — the attack missed.</p>
        )}
      </div>
    ) : null

  return (
    <div ref={ref} className="event-tooltip" role="tooltip">
      <div className="tooltip-header">
        <span className="tooltip-time">{entry.timestamp.toFixed(2)}s</span>
        <span className="tooltip-event">{eventLabel(entry.event)}</span>
        {entry.damageType ? (
          <span className={`badge badge-${entry.damageType.toLowerCase()}`}>{entry.damageType}</span>
        ) : null}
      </div>
      <p className="tooltip-description">{entry.description}</p>

      {groupBySection(entry.details).map(([section, details]) => (
        <div key={section} className="tooltip-section">
          <h3>{section}</h3>
          {section === 'Attack table' ? rolls : null}
          <dl>
            {details.map((detail) => (
              <div key={detail.label}>
                <dt>{detail.label}</dt>
                <dd>{detail.value}</dd>
              </div>
            ))}
          </dl>
        </div>
      ))}

      <div className="tooltip-section">
        <h3>State after event</h3>
        <dl>
          <div>
            <dt>Total damage</dt>
            <dd>{entry.totalDamage.toFixed(1)}</dd>
          </div>
          <div>
            <dt>Active auras</dt>
            <dd className="chips">
              {entry.activeAuras.length === 0 ? (
                <span className="muted">None</span>
              ) : (
                entry.activeAuras.map((aura) => (
                  <span key={aura} className="chip">
                    {spaced(aura)}
                  </span>
                ))
              )}
            </dd>
          </div>
        </dl>
      </div>
    </div>
  )
}
