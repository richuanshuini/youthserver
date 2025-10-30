import http from '@/api/http.js';

/** @typedef {{announceMentId:number, title:string, content:string, type:number, status:number, publishTime?:string, expireTime?:string}} AnnouncementDto */
/** @typedef {{Title:string, Content:string, Type:number, Status?:number, PublishTime?:string, ExpireTime?:string}} InsertAnnouncementDto */

/** @returns {Promise<AnnouncementDto[]>} */
export const listAnnouncements = async () => {
  const data = await http.get('/api/AnnounceMents');
  return Array.isArray(data) ? data : data?.items ?? [];
};

/** @returns {Promise<AnnouncementDto|null>} */
export const getAnnouncement = async (id) => {
  const data = await http.get(`/api/AnnounceMents/${id}`);
  return data ?? null;
};

/**
 * 新增公告
 * @param {InsertAnnouncementDto} payload
 * @returns {Promise<AnnouncementDto>}
 */
export const createAnnouncement = (payload) => {
  return http.post('/api/AnnounceMents', payload);
};

/**
 * 部分更新公告
 * @param {number} id
 * @param {{Title?:string|null, Content?:string|null, Type?:number|null, Status?:number|null, PublishTime?:string|null, ExpireTime?:string|null}} payload
 * @returns {Promise<void>}
 */
export const updateAnnouncement = (id, payload) => {
  return http.post(`/api/AnnounceMents/${id}/update`, payload);
};

/**
 * 软删除公告
 * @param {number} id
 * @returns {Promise<void>}
 */
export const deleteAnnouncement = (id) => {
  return http.post(`/api/AnnounceMents/${id}/delete`);
};

/** 获取回收站公告（仅软删除） */
export const listDeletedAnnouncements = async () => {
  const data = await http.get('/api/AnnounceMents/deleted');
  return Array.isArray(data) ? data : data?.items ?? [];
};

/** 还原公告 */
export const restoreAnnouncement = (id) => {
  return http.post(`/api/AnnounceMents/${id}/restore`);
};

/** 物理删除公告 */
export const hardDeleteAnnouncement = (id) => {
  return http.post(`/api/AnnounceMents/${id}/hard-delete`);
};