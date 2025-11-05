<script setup>
defineOptions({ name: 'UserRoleIndexPage' });
import { ref, onMounted, computed } from 'vue';
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

// 将平铺的用户-角色映射，按用户分组并聚合角色，便于在表格中清晰展示多个角色
const groupedRows = computed(() => {
  const map = new Map();
  for (const ur of userRoles.value) {
    const uid = ur.userId;
    let group = map.get(uid);
    if (!group) {
      group = { userId: uid, userName: ur.userName, roles: [] };
      map.set(uid, group);
    }
    if (ur.roleId != null) {
      group.roles.push({ roleId: ur.roleId, roleName: ur.roleName });
    }
  }
  return Array.from(map.values()).sort((a, b) => a.userId - b.userId);
});
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
    <el-table :data="groupedRows" v-loading="loading" stripe>
      <el-table-column prop="userId" label="用户ID" />
      <el-table-column prop="userName" label="姓名" />
      <el-table-column label="角色">
        <template #default="{ row }">
          <el-space wrap>
            <el-tag
              v-for="role in row.roles"
              :key="role.roleId"
              type="info"
              effect="plain"
              size="small"
            >
              {{ role.roleName }}
            </el-tag>
          </el-space>
        </template>
      </el-table-column>
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
