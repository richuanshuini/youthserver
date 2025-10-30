export default [
  {
    path: 'announcements',
    name: 'AnnouncementIndex',
    meta: { title: '公告管理', activeMenu: '/admin/announcements' },
    component: () => import('./pages/Index.vue'),
  },
  {
    path: 'announcements/recycle-bin',
    name: 'AnnouncementRecycleBin',
    meta: { title: '公告回收站', activeMenu: '/admin/announcements/recycle-bin' },
    component: () => import('./pages/RecycleBin.vue'),
  },
];