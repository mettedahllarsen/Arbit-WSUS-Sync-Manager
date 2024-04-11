import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: "build",
  },
  server: {
    open: true,
    host: "localhost",
    port: 3001,
  },
})
