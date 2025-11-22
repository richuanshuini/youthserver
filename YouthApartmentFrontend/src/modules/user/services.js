import http from '@/api/http.js';

export const listUsers = () => http.get('/api/Users');
export const listUsersPaged = (params) => http.get('/api/Users/paged', { params });
export const createUser = (payload) => http.post('/api/Users', payload);
export const setUserStatus = (id, status) => http.post(`/api/Users/${id}/updateUserStatus`, { Status: status });
export const updateUser = (id, payload) => {
  // 直接发送原始 payload，确保包含 idCard
  return http.post(`/api/Users/${id}/update`, payload);
};

// 单条件查询：POST /api/Users/search，传入 UserQueryParams 中的一个字段
export const searchUsers = (payload) => http.post('/api/Users/search', payload);

// 用户选择器：查询可分配为审核员的用户（包含角色信息）
export const listUserSelector = (payload) => http.post('/api/Users/selector', payload);
