import http from '@/api/http.js';

export const listUsers = () => http.get('/api/Users');
export const listUsersPaged = (params) => http.get('/api/Users/paged', { params });
export const createUser = (payload) => http.post('/api/Users', payload);
export const setUserStatus = (id, status) => http.post(`/api/Users/${id}/updateUserStatus`, { Status: status });
export const updateUser = (id, payload) => {
  const body = { ...payload };
  // 将前端的 idCard 映射为后端 PatchUserDto 的 IdCrad（拼写差异）
  if (body.idCard !== undefined) {
    body.idCrad = body.idCard;
    delete body.idCard;
  }
  return http.post(`/api/Users/${id}/update`, body);
};