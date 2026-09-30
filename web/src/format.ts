// Event type names arrive as C# class names (e.g. "AutoAttackSwingEvent").
export const eventLabel = (name: string) => name.replace(/Event$/, '').replace(/([a-z])([A-Z])/g, '$1 $2')

// Enum names arrive in PascalCase (e.g. "SwingTimerCooldown"); show them with spaces.
export const spaced = (name: string) => name.replace(/([a-z])([A-Z])/g, '$1 $2')

export const percent = (value: number | null) => (value === null ? '' : `${(value * 100).toFixed(1)}%`)
