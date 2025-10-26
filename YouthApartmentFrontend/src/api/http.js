import axios from 'axios';

const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5160',
  timeout: 15000,
});

http.interceptors.response.use(
  (res) => res.data,
  (error) => Promise.reject(error)
);

export default http;