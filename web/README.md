# Warrior Forever Sim — Web UI

React + TypeScript (Vite) front end for the simulator. It talks to `src/WarriorForeverSim.Api`.

## Development

```
# terminal 1 (from src/)
dotnet run --project WarriorForeverSim.Api

# terminal 2 (from web/)
npm install
npm run dev
```

Vite proxies `/api` to the API at `http://localhost:5058`.

## Production build

`npm run build` writes the bundle to `src/WarriorForeverSim.Api/wwwroot`, so running the API alone serves the UI.
