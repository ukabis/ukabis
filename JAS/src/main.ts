import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import App from './App.vue'
import router from './router'

const app = createApp(App)

window.addEventListener("load",function() {
    setTimeout(function(){
      window.scrollTo(0, 1)
    }, 0)
  })

app.use(router)
app.use(createPinia())
app.mount('#app')
