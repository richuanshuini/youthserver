export default [
  {
    path: 'role-permission',
    name: 'RolePermission',
    component: () => import('./pages/Index.vue'),
    meta: { title: '角色-权限', activeMenu: 'user-basic-info' },
  },
];