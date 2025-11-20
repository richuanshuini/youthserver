export default [
  {
    path: 'property', // => /admin/property
    name: 'PropertyIndex',
    meta: { title: '房源管理', activeMenu: '/admin/property' },
    component: () => import('./pages/Index.vue'),
  },
];
