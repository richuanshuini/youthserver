import http from '@/api/http.js';

export const listRolePermissions = () =>
  http.get('/api/RolePermission').then((res) => {
    const list = Array.isArray(res) ? res : (res ? [res] : []);
    return list.filter((item) => !(item && item.roleId === 0 && item.permissionId === 0));
  });