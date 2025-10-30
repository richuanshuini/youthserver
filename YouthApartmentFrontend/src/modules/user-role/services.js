import http from '@/api/http.js';

/** @typedef {{userId:number, roleId:number}} UserRoleDto */

/** @returns {Promise<UserRoleDto[]>} */
export const listUserRoles = async () => {
  const data = await http.get('/api/UserRole');
  return Array.isArray(data) ? data : data?.items ?? [];
};