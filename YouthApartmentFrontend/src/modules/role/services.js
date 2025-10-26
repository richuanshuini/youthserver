import http from '@/api/http.js';

export const listRoles = () => http.get('/api/Role');
export const getRole = (id) => http.get(`/api/Role/${id}`);