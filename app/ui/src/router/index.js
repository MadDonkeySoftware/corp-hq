import Vue from 'vue'
import Router from 'vue-router'
import Main from '@/pages/Main'
import Register from '@/pages/Register'
import LogIn from '@/pages/LogIn'
import Dashboard from '@/pages/Dashboard'
import MiningOperations from '@/pages/MiningOperations'

Vue.use(Router)

export default new Router({
  routes: [
    { path: '/', name: 'Main', component: Main },
    { path: '/Register', name: 'Register', component: Register },
    { path: '/LogIn', name: 'LogIn', component: LogIn },
    { path: '/Dashboard', name: 'Dashboard', component: Dashboard },
    { path: '/MiningOperations', name: 'MiningOperations', component: MiningOperations }
  ]
})
