<script setup>
defineOptions({ name: 'AnnouncementRecycleBinPage' });
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { listDeletedAnnouncements, restoreAnnouncement, hardDeleteAnnouncement } from '../services.js';

const headerCellStyle = () => ({ textAlign: 'center' });
const cellStyle = () => ({ textAlign: 'center' });

const loading = ref(false);
/** @type {import('../services.js').AnnouncementDto[]} */
const rows = ref([]);
const fetchList = async () => {
  loading.value = true;
  try { rows.value = await listDeletedAnnouncements(); } catch { ElMessage.error('获取回收站公告失败'); }
  loading.value = false;
};
onMounted(fetchList);

// 与公告管理保持一致的枚举展示
const typeOptions = [
  { label: '系统', value: 1 },
  { label: '维护', value: 2 },
  { label: '运营', value: 3 },
];
const statusOptions = [
  { label: '草稿', value: 0 },
  { label: '发布', value: 1 },
  { label: '关闭', value: 2 },
];

const restoreRow = async (row) => {
  try {
    await ElMessageBox.confirm(`确认还原公告: ${row.title}?`, '提示', { type: 'info' });
    await restoreAnnouncement(row.announceMentId);
    ElMessage.success('还原成功');
    await fetchList();
  } catch (e) {
    if (e !== 'cancel') ElMessage.error('还原失败');
  }
};

const hardDeleteRow = async (row) => {
  try {
    await ElMessageBox.confirm(`确认永久删除公告: ${row.title}? 此操作不可恢复。`, '警告', { type: 'warning' });
    await hardDeleteAnnouncement(row.announceMentId);
    ElMessage.success('已永久删除');
    await fetchList();
  } catch (e) {
    if (e !== 'cancel') ElMessage.error('删除失败');
  }
};
</script>

<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>公告回收站</span>
      </div>
    </template>

    <el-table :data="rows" class="search-table" v-loading="loading" stripe :header-cell-style="headerCellStyle" :cell-style="cellStyle">
      <el-table-column prop="announceMentId" label="ID" width="80" />
      <el-table-column prop="title" label="标题" />
      <el-table-column prop="type" label="类型">
        <template #default="{ row }">{{ typeOptions.find(it => it.value === row.type)?.label ?? '未知' }}</template>
      </el-table-column>
      <el-table-column prop="status" label="状态">
        <template #default="{ row }">{{ statusOptions.find(it => it.value === row.status)?.label ?? '未知' }}</template>
      </el-table-column>
      <el-table-column prop="publishTime" label="发布时间" width="180">
        <template #default="{ row }">{{ row.publishTime ? new Date(row.publishTime).toLocaleString() : '-' }}</template>
      </el-table-column>
      <el-table-column prop="expireTime" label="结束时间" width="180">
        <template #default="{ row }">{{ row.expireTime ? new Date(row.expireTime).toLocaleString() : '-' }}</template>
      </el-table-column>
      <el-table-column label="操作" width="240">
        <template #default="{ row }">
          <el-button type="primary" link @click="restoreRow(row)">还原</el-button>
          <el-divider direction="vertical" />
          <el-button type="danger" link @click="hardDeleteRow(row)">永久删除</el-button>
        </template>
      </el-table-column>
    </el-table>
  </el-card>
  
</template>

<style scoped>
.card-header { display: flex; align-items: center; justify-content: space-between; }
</style>