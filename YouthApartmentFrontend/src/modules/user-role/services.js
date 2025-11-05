import http from '@/api/http.js';

// 持久化检测：后端是否支持新的未分配角色用户分页接口（默认禁用，避免首次 404）
let NO_ROLES_PAGED_SUPPORTED = false;

const paginateList = (items = [], pageNumber = 1, pageSize = 10) => {
  const total = items.length;
  const start = Math.max(0, (pageNumber - 1) * pageSize);
  const end = start + pageSize;
  return { items: items.slice(start, end), total, pageNumber, pageSize };
};

const strIncludes = (source, keyword) => {
  const s = String(source ?? '').toLowerCase();
  const k = String(keyword ?? '').toLowerCase();
  return k ? s.includes(k) : true;
};

const applyClientSearch = (items = [], payload = {}) => {
  const { UserName, Email, Phone, RealName, IdCard, Gender } = payload || {};
  return items.filter(u => (
    (!UserName || strIncludes(u?.userName, UserName)) &&
    (!Email || strIncludes(u?.email, Email)) &&
    (!Phone || strIncludes(u?.phone, Phone)) &&
    (!RealName || strIncludes(u?.realName, RealName)) &&
    (!IdCard || strIncludes(u?.idCard, IdCard)) &&
    (!Gender || String(u?.gender ?? '').toLowerCase() === String(Gender).toLowerCase())
  ));
};

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
  if (!NO_ROLES_PAGED_SUPPORTED) {
    const arr = await listUsersNoRolesLegacy();
    const items = Array.isArray(arr) ? arr : [];
    return paginateList(items, pageNumber, pageSize);
  }
  try {
    return await http.get(`/api/Users/NoRoles/paged?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  } catch (e) {
    const status = e?.response?.status;
    if (status === 404) {
      NO_ROLES_PAGED_SUPPORTED = false;
      const arr = await listUsersNoRolesLegacy();
      const items = Array.isArray(arr) ? arr : [];
      return paginateList(items, pageNumber, pageSize);
    }
    throw e;
  }
};

/**
 * 未分配角色用户：模糊查询（多条件）+ 分页
 * @param {{UserName?:string,Email?:string,Phone?:string,RealName?:string,IdCard?:string,Gender?:string,PageNumber?:number,PageSize?:number}} body
 */
export const searchUsersNoRolesPaged = async (body = {}) => {
  const payload = {
    PageNumber: body.PageNumber ?? 1,
    PageSize: body.PageSize ?? 10,
    UserName: body.UserName || undefined,
    Email: body.Email || undefined,
    Phone: body.Phone || undefined,
    RealName: body.RealName || undefined,
    IdCard: body.IdCard || undefined,
    Gender: body.Gender || undefined,
  };
  if (!NO_ROLES_PAGED_SUPPORTED) {
    const arr = await listUsersNoRolesLegacy();
    const items = applyClientSearch(Array.isArray(arr) ? arr : [], payload);
    return paginateList(items, payload.PageNumber, payload.PageSize);
  }
  try {
    return await http.post('/api/Users/search/NoRoles/paged', payload);
  } catch (e) {
    const status = e?.response?.status;
    if (status === 404) {
      NO_ROLES_PAGED_SUPPORTED = false;
      const arr = await listUsersNoRolesLegacy();
      const items = applyClientSearch(Array.isArray(arr) ? arr : [], payload);
      return paginateList(items, payload.PageNumber, payload.PageSize);
    }
    throw e;
  }
};

/** 旧接口：未分配角色用户（不分页） */
export const listUsersNoRolesLegacy = async () => {
  return await http.get('/api/Users/search/NoRoles');
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
      } catch (err) {
        // 重复或无效时后端返回错误，这里忽略逐条失败
      }
    }
  }
  return { created };
};