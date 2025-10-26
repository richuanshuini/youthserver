<script setup>
defineOptions({ name: 'UserRoleIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { listUserRoles } from '../services.js';

const loading = ref(false);
const userRoles = ref([]);
const fetchData = async () => {
  loading.value = true;
  try { userRoles.value = await listUserRoles(); } catch { ElMessage.error('获取用户-角色失败'); }
  loading.value = false;
};
onMounted(fetchData);
</script>

<template>
  <el-card>
    <template #header>
      <span>用户-角色映射</span>
    </template>
    <el-table :data="userRoles" v-loading="loading" stripe>
      <el-table-column prop="userId" label="用户ID" width="120" />
      <el-table-column prop="roleId" label="角色ID" width="120" />
    </el-table>
  </el-card>
</template>