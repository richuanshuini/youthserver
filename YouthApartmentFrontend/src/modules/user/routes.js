export default [
  {
    path: 'users',
    name: 'Users',
    component: () => import('./pages/Index.vue'),
    meta: { title: '用户管理', activeMenu: 'user-basic-info' },
  },
];