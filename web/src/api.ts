export interface SimulateRequest {
  race: string
  bossLevel: number
  bossType: string
  fightLength: number
  iterations: number
}

export interface SimulateResponse {
  iterations: number
  averageDps: number
  minDps: number
  maxDps: number
  averageHits: number
  averageCrits: number
  averageMisses: number
  averageDodges: number
  averageGlancings: number
  warnings: string[]
  errors: string[]
  firstRunLog: SimulationLogEntry[]
}

export type DamageType = 'Hit' | 'Crit' | 'Glancing' | 'Dodge' | 'Miss'

export interface SimulationLogEntry {
  timestamp: number
  event: string
  description: string
  damage: number | null
  damageType: DamageType | null
  missChance: number | null
  dodgeChance: number | null
  glancingChance: number | null
  critChance: number | null
  attackRoll: number | null
  totalDamage: number
  details: EventDetail[]
  activeAuras: string[]
}

export interface EventDetail {
  section: string
  label: string
  value: string
}

export interface SimulationOptions {
  races: string[]
  bossTypes: string[]
  defaults: SimulateRequest
}

interface ValidationProblem {
  title?: string
  errors?: Record<string, string[]>
}

async function readError(response: Response): Promise<string> {
  try {
    const problem = (await response.json()) as ValidationProblem
    const messages = Object.values(problem.errors ?? {}).flat()
    if (messages.length > 0) {
      return messages.join(' ')
    }
    if (problem.title) {
      return problem.title
    }
  } catch {
    // Not a JSON problem body; fall through to the status text.
  }
  return `Request failed (${response.status} ${response.statusText})`
}

export async function getOptions(): Promise<SimulationOptions> {
  const response = await fetch('/api/options')
  if (!response.ok) {
    throw new Error(await readError(response))
  }
  return (await response.json()) as SimulationOptions
}

export async function simulate(request: SimulateRequest): Promise<SimulateResponse> {
  const response = await fetch('/api/simulate', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })
  if (!response.ok) {
    throw new Error(await readError(response))
  }
  return (await response.json()) as SimulateResponse
}
