import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import Workspace from '../views/Workspace.vue'
import Organization from '../views/Organization.vue'
import User from '../views/User.vue'

const routes = [
  {
    path: '/login',
    component: Login,
  },
  {
    path: '/register',
    component: Register,
  },
  {
    path: '/workspace',
    component: Workspace,
    
    meta: { requiresAuth: true },
  },
  {
    path: '/organization/:orgCode',
    component: Organization,
    props: true,
    meta: { requiresAuth: true },
  },
  {
    path: '/user/:email',
    component: User,
    props: true,
    meta: { requiresAuth: true },
  },
  {
    path: '/',
    redirect: '/login',
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth && !localStorage.getItem('token')) {
    next('/login')
  } else {
    next()
  }
})

export default router
