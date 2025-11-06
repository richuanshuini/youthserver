import http from '@/api/http.js';

/** @typedef {{userId:number, roleId:number}} UserRoleDto */
/** @returns {Promise<UserRoleDto[]>} */
export const listUserRoles = async () => {
  const data = await http.get('/api/UserRole');
  return Array.isArray(data) ? data : data?.items ?? [];
};

/** @typedef {{roleId:number, roleName:string, description?:string}} RoleDto */
/** @returns {Promise<RoleDto[]>} */
export const listRoles = async () => {
  const data = await http.get('/api/Role');
  return Array.isArray(data) ? data : data?.items ?? [];
};

/**
 * 未分配角色用户：分页查询（无筛选）
 * @param {{pageNumber?:number,pageSize?:number}} params
 */
export const listUsersNoRolesPaged = async (params = {}) => {
  const { pageNumber = 1, pageSize = 10 } = params;
  const res = await http.get('/api/Users/NoRoles/paged', {
    params: { pageNumber, pageSize },
  });
  return res ?? { items: [], total: 0, pageNumber, pageSize };
};

/**
 * 批量分配用户角色
 * @param {{userIds:number[], roleIds:number[]}} body
 */
export const assignUserRolesBatch = async (body) => {
  const { userIds = [], roleIds = [] } = body || {};
  let created = 0;
  for (const uid of userIds) {
    for (const rid of roleIds) {
      try {
        await http.post('/api/UserRole', { UserId: uid, RoleId: rid });
        created++;
      } catch {
        // 重复或无效时后端返回错误，这里忽略逐条失败
      }
    }
  }
  return { created };
};
