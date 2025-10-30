export default [
  {
    path: 'announcements',
    name: 'AnnouncementIndex',
    meta: { title: '公告管理', activeMenu: '/admin/announcements' },
    component: () => import('./pages/Index.vue'),
  },
];