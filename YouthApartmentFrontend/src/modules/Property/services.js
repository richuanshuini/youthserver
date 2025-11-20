import http from '@/api/http';

const baseUrl = '/api/Property';

export const propertyApi = {
  /**
   * 多条件分页查询房源
   * @param {object} queryDto 对应后端 PropertyQueryDto
   * @param {number} pageNumber
   * @param {number} pageSize
   */
  searchProperties: (queryDto, pageNumber = 1, pageSize = 10) => {
    return http.post(`${baseUrl}/search`, queryDto, {
      params: { pageNumber, pageSize },
    });
  },

  /** 创建房源 */
  createProperty: (payload) => http.post(baseUrl, payload),

  /** 局部更新房源（审核/修改等） */
  updateProperty: (id, payload) => http.post(`${baseUrl}/${id}/update`, payload),

  /** 删除房源（如后端提供软删接口，可在此实现） */
  deleteProperty: (id) => http.delete(`${baseUrl}/${id}`),
};
