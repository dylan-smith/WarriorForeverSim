import { useLayoutEffect, useRef } from 'react'
import type { DamageType, EventDetail, SimulationLogEntry } from '../api'
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

interface AttackTableBarProps {
  roll: number
  missChance: number
  dodgeChance: number
  glancingChance: number
  critChance: number
  outcome: DamageType
}

// The single-roll attack table as a 0–100% bar: consecutive miss, dodge, glancing, crit and
// hit segments, with a marker at the roll showing which segment it landed in, and a legend in
// the same order and colors.
function AttackTableBar({ roll, missChance, dodgeChance, glancingChance, critChance, outcome }: AttackTableBarProps) {
  const clamp = (value: number) => Math.min(Math.max(value, 0), 1)
  const segments = [
    { kind: 'miss', label: 'Miss', chance: clamp(missChance) },
    { kind: 'dodge', label: 'Dodge', chance: clamp(dodgeChance) },
    { kind: 'glancing', label: 'Glancing', chance: clamp(glancingChance) },
    { kind: 'crit', label: 'Crit', chance: clamp(critChance) },
    { kind: 'hit', label: 'Hit', chance: clamp(1 - missChance - dodgeChance - glancingChance - critChance) },
  ]
  let start = 0

  return (
    <div className="roll">
      <div className="roll-label">
        <span>Attack table roll</span>
        <span className={`roll-result ${outcome.toLowerCase()}`}>
          {percent(roll)} → {outcome}
        </span>
      </div>
      <div className="roll-bar" aria-hidden="true">
        {segments.map((segment) => {
          const left = start
          start += segment.chance
          return (
            <div
              key={segment.kind}
              className={`roll-zone ${segment.kind}`}
              style={{ left: `${left * 100}%`, width: `${segment.chance * 100}%` }}
            />
          )
        })}
        <div className="roll-marker" style={{ left: `${clamp(roll) * 100}%` }} />
      </div>
      <ul className="roll-legend">
        {segments.map((segment) => (
          <li key={segment.kind}>
            <span className={`roll-swatch ${segment.kind}`} aria-hidden="true" />
            {segment.label} {percent(segment.chance)}
          </li>
        ))}
      </ul>
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

  const attackTable =
    entry.attackRoll !== null &&
    entry.missChance !== null &&
    entry.dodgeChance !== null &&
    entry.glancingChance !== null &&
    entry.critChance !== null &&
    entry.damageType !== null ? (
      <div className="tooltip-section">
        <h3>Attack table</h3>
        <AttackTableBar
          roll={entry.attackRoll}
          missChance={entry.missChance}
          dodgeChance={entry.dodgeChance}
          glancingChance={entry.glancingChance}
          critChance={entry.critChance}
          outcome={entry.damageType}
        />
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

      {attackTable}

      {groupBySection(entry.details).map(([section, details]) => (
        <div key={section} className="tooltip-section">
          <h3>{section}</h3>
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
