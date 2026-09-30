import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Forward API calls to WarriorForeverSim.Api (see its launchSettings.json).
    proxy: {
      '/api': 'http://localhost:5058',
    },
  },
  build: {
    // The API serves the built UI from its wwwroot folder.
    outDir: '../src/WarriorForeverSim.Api/wwwroot',
    emptyOutDir: true,
  },
})
