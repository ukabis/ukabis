import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  // server: {
  //   proxy: {
  //     "^/api": {
  //       target: "https://api.wagri-dev.net",
  //       changeOrigin: true,
  //       rewrite: (path) => path.replace("/api", "/")
  //     }
  //   }
  // },
  // server: {
  //   proxy: {
  //     "https://api.wagri-dev.net": {
  //       target: "http://localhost:5174",
  //       changeOrigin: true,
  //     }
  //   }
  // },
  // server: {
  //   proxy: {
  //     "https://api.wagri-dev.net": {
  //       target: window.location.origin,
  //       changeOrigin: true,
  //     }
  //   }
  // },
  resolve: {
    alias: {
      "@/": `${__dirname}/src/`
    }
  },
  build: { cssCodeSplit: true },
  base: "/",
})
