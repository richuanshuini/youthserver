export default [
  {
    path: 'user-role',
    name: 'UserRole',
    component: () => import('./pages/Index.vue'),
    meta: { title: '用户-角色', activeMenu: 'user-basic-info' },
  },
];