import http from '@/api/http.js';

//获取房源数据
export const listProperty=async ()=>{
  return await http.get('api/Property');
}
