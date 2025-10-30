import http from '@/api/http.js';

/** @typedef {{roleId:number, roleName:string, description?:string}} RoleDto */
/** @typedef {{RoleName:string, Description?:string|null}} InsertRoleDto */

/** @returns {Promise<RoleDto[]>} */
export const listRoles = async () => {
  const data = await http.get('/api/Role');
  return Array.isArray(data) ? data : data?.items ?? [];
};

/** @returns {Promise<RoleDto|null>} */
export const getRole = async (id) => {
  const data = await http.get(`/api/Role/${id}`);
  return data ?? null;
};

/**
 * 新增角色
 * @param {InsertRoleDto} payload
 * @returns {Promise<RoleDto>}
 */
export const createRole = (payload) => {
  return http.post('/api/Role', payload);
};

/**
 * 部分更新角色信息
 * @param {number} id
 * @param {{RoleName: (string|null), Description: (string|null)}} payload
 * @returns {Promise<void>}
 */
export const updateRole = (id, payload) => {
  return http.post(`/api/Role/${id}/update`, payload);
};