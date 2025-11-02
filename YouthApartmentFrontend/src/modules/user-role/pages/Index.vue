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
      <div class="card-header">
        <span>用户-角色映射</span>
        <div class="card-actions">
          <el-button type="primary" >新增</el-button>
          <el-button type="danger" disabled>删除</el-button>
        </div>
      </div>
    </template>
    <el-table :data="userRoles" v-loading="loading" stripe>
      <el-table-column prop="userId" label="用户ID" />
      <el-table-column prop="userName" label="姓名" />
      <el-table-column prop="roleId" label="角色ID"  />
      <el-table-column prop="roleName" label="角色" />
      <el-table-column label="操作">
        <template>
          <el-button type="primary" >修改</el-button>
        </template>
      </el-table-column>
    </el-table>
  </el-card>
</template>

<style scoped>
.card-header { display: flex; justify-content: space-between; align-items: center; }
</style>
