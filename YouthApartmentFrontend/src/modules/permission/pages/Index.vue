<script setup>
defineOptions({ name: 'PermissionIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { listPermissions } from '../services.js';

const loading = ref(false);
const permissions = ref([]);
const fetchPermissions = async () => {
  loading.value = true;
  try { permissions.value = await listPermissions(); } catch { ElMessage.error('获取权限失败'); }
  loading.value = false;
};
onMounted(fetchPermissions);
</script>

<template>
  <el-card>
    <template #header>
      <span>权限管理</span>
    </template>
    <el-table :data="permissions" v-loading="loading" stripe>
      <el-table-column prop="permissionId" label="ID" width="80" />
      <el-table-column prop="permissionName" label="权限名" />
      <el-table-column prop="module" label="模块" />
      <el-table-column prop="description" label="描述" />
    </el-table>
  </el-card>
</template>