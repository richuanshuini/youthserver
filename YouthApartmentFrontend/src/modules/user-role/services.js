import http from '@/api/http.js';

export const listUserRoles = () => http.get('/api/UserRole');