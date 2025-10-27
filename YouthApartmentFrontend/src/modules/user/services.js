import http from '@/api/http.js';

export const listUsers = () => http.get('/api/Users');
export const createUser = (payload) => http.post('/api/Users', payload);
export const setUserStatus = (id, status) => http.post(`/api/Users/${id}/SetUserStatus`, { status });
export const updateUser = (id, payload) => http.post(`/api/Users/${id}/update`, payload);