<script setup>
defineOptions({ name: 'RolePermissionIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { listRolePermissions } from '../services.js';


const loading = ref(false);
const rolePermissions = ref([]);
const fetchData = async () => {
  loading.value = true;
  try { rolePermissions.value = await listRolePermissions(); } catch { ElMessage.error('获取角色-权限失败'); }
  loading.value = false;
};
onMounted(fetchData);
</script>

<template>
  <el-card>
    
    <template #header>
      <span>角色-权限映射</span>
    </template>
    <el-table :data="rolePermissions" v-loading="loading" stripe>
      <el-table-column prop="roleId" label="角色ID" width="120" />
      <el-table-column prop="permissionId" label="权限ID" width="120" />
    </el-table>
  </el-card>
</template>