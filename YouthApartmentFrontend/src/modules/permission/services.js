import http from '@/api/http.js';

/** @typedef {{permissionId:number, permissionName:string, module?:string, description?:string}} PermissionDto */

/** @returns {Promise<PermissionDto[]>} */
export const listPermissions = async () => {
  const data = await http.get('/api/Permission');
  return Array.isArray(data) ? data : data?.items ?? [];
};

/** @returns {Promise<PermissionDto|null>} */
export const getPermission = async (id) => {
  const data = await http.get(`/api/Permission/${id}`);
  return data ?? null;
};