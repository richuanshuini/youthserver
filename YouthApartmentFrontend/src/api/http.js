import axios from 'axios';

const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5160',
  timeout: 15000,
});

http.interceptors.response.use(
  (res) => {
    const data = res.data;
    // 若后端用 200 返回 { error: '...' }，统一视为错误并 reject
    if (data && typeof data === 'object' && 'error' in data && data.error) {
      return Promise.reject({
        isApiError: true,
        message: data.error,
        // 模拟 axios 错误结构，便于组件用同一解析逻辑
        response: { data, status: res.status, headers: res.headers },
      });
    }
    return data;
  },
  (error) => Promise.reject(error)
);

export default http;