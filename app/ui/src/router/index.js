import Vue from 'vue'
import Router from 'vue-router'
import HelloWorld from '@/components/HelloWorld'
import Register from '@/components/Register'
import LogIn from '@/components/LogIn'

Vue.use(Router)

export default new Router({
  routes: [
    // Example: ,{ path: '/', name: 'HelloWorld', component: HelloWorld }
    { path: '/', name: 'HelloWorld', component: HelloWorld },
    { path: '/Register', name: 'Register', component: Register },
    { path: '/LogIn', name: 'LogIn', component: LogIn }
  ]
})
