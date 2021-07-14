import { defineConfig } from 'vite'
import sourcemaps from 'rollup-plugin-sourcemaps'

export default defineConfig({
  root: "src",
  plugins: [sourcemaps()],
  build: {
    outDir: "../public",
    emptyOutDir: true,
    sourcemap: "inline"
  }
});
