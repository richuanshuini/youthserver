import http from '@/api/http.js';

export const listPermissions = () => http.get('/api/Permission');
export const getPermission = (id) => http.get(`/api/Permission/${id}`);