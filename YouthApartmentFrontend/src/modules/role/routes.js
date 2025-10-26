export default [
  {
    path: 'roles',
    name: 'Roles',
    component: () => import('./pages/Index.vue'),
    meta: { title: '角色管理', activeMenu: 'user-basic-info' },
  },
];