import http from '@/api/http.js';

export const listUsers = () => http.get('/api/Users');
export const createUser = (payload) => http.post('/api/Users', payload);