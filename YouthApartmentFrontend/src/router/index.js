import { createRouter, createWebHistory } from 'vue-router';
import AdminLayout from '../layout/AdminLayout.vue';
import { loadModuleRoutes } from './modules-loader.js';

const baseRoutes = [
  { path: '/', redirect: '/admin' },
  { path: '/home', redirect: '/admin/home' },
  { path: '/403', name: 'Forbidden', component: () => import('../components/common/Forbidden.vue') },
  { path: '/:pathMatch(.*)*', name: 'NotFound', component: () => import('../components/common/NotFound.vue') },
];

const router = createRouter({
  history: createWebHistory(),
  routes: [
    ...baseRoutes,
    {
      path: '/admin',
      component: AdminLayout,
      children: [
        { path: '', redirect: 'home' },
        ...loadModuleRoutes(),
      ],
    },
  ],
});

export default router;