export default [
  {
    path: 'permissions',
    name: 'Permissions',
    component: () => import('./pages/Index.vue'),
    meta: { title: '权限管理', activeMenu: 'user-basic-info' },
  },
];