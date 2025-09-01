import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@todo-app/ui': path.resolve(__dirname, '../../packages/ui/src'),
      '@todo-app/hooks': path.resolve(__dirname, '../../packages/hooks/src'),
      '@todo-app/utils': path.resolve(__dirname, '../../utils/src'),
    },
  },
  server: {
    port: 3000,
    host: true,
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
  },
}) 