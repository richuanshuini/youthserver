<script setup>
defineOptions({ name: 'RoleIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { listRoles } from '../services.js';

const loading = ref(false);
const roles = ref([]);
const fetchRoles = async () => {
  loading.value = true;
  try { roles.value = await listRoles(); } catch { ElMessage.error('获取角色失败'); }
  loading.value = false;
};
onMounted(fetchRoles);
</script>

<template>
  <el-card>
    <template #header>
      <span>角色管理</span>
    </template>
    <el-table :data="roles" v-loading="loading" stripe>
      <el-table-column prop="roleId" label="ID" width="80" />
      <el-table-column prop="roleName" label="角色名" />
      <el-table-column prop="description" label="描述" />
    </el-table>
  </el-card>
</template>