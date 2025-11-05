<script setup>
defineOptions({ name: 'UserRoleIndexPage' });
import { ref, onMounted, computed, watch, nextTick } from 'vue';
import { ElMessage } from 'element-plus';
import { listUserRoles, listRoles, listUsersNoRolesPaged, searchUsersNoRolesPaged, assignUserRolesBatch } from '../services.js';

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

// --- Drawer: 批量分配角色 ---
const drawerVisible = ref(false);
const openDrawer = async () => {
  drawerVisible.value = true;
  await ensureRoles();
  await fetchUsersNoRoles();
};

// 左侧角色池：el-transfer
const rolesLoading = ref(false);
const rolesData = ref([]); // [{ key: roleId, label: roleName }]
const selectedRoleIds = ref([]);
const ensureRoles = async () => {
  if (rolesData.value.length > 0) return;
  rolesLoading.value = true;
  try {
    const roles = await listRoles();
    rolesData.value = (roles || []).map(r => ({ key: r.roleId, label: r.roleName }));
  } catch { ElMessage.error('获取角色列表失败'); }
  rolesLoading.value = false;
};

// 右侧用户列表（未分配角色）
const usersNoRoleLoading = ref(false);
const usersNoRole = ref([]);
const usersTotal = ref(0);
const usersPageNumber = ref(1);
const usersPageSize = ref(10);
const usersTableRef = ref();

// 选择持久化：跨分页保留选中用户
const selectedUserIds = ref(new Set());
const toggleRowSelectionSync = async () => {
  await nextTick();
  const table = usersTableRef.value;
  if (!table) return;
  for (const row of usersNoRole.value) {
    table.toggleRowSelection(row, selectedUserIds.value.has(row.userId));
  }
};

// 顶部查询条件 Select + 高级筛选
const searchKey = ref('UserName');
const searchInput = ref('');
const advVisible = ref(false);
const adv = ref({ Email: '', Phone: '', RealName: '', IdCard: '', Gender: '' });

const buildSearchPayload = () => {
  const payload = { PageNumber: usersPageNumber.value, PageSize: usersPageSize.value };
  const key = (searchKey.value || '').trim();
  const val = (searchInput.value || '').trim();
  if (key && val) payload[key] = val;
  Object.entries(adv.value).forEach(([k, v]) => {
    const vv = (v || '').trim();
    if (vv) payload[k] = vv;
  });
  return payload;
};

const fetchUsersNoRoles = async () => {
  usersNoRoleLoading.value = true;
  try {
    const hasAnyCondition = !!searchInput.value || Object.values(adv.value).some(v => (v || '').trim());
    const res = hasAnyCondition
      ? await searchUsersNoRolesPaged(buildSearchPayload())
      : await listUsersNoRolesPaged({ pageNumber: usersPageNumber.value, pageSize: usersPageSize.value });
    usersNoRole.value = res.items || [];
    usersTotal.value = res.total || 0;
    await toggleRowSelectionSync();
  } catch (error) {
    ElMessage.error(getErrorMessage(error, '获取未分配角色的用户失败'));
  } finally {
    usersNoRoleLoading.value = false;
  }
};

const handleUsersSizeChange = (val) => {
  usersPageSize.value = val;
  usersPageNumber.value = 1;
  fetchUsersNoRoles();
};
const handleUsersPageChange = (val) => {
  usersPageNumber.value = val;
  fetchUsersNoRoles();
};

// 表格选择事件：维护 selectedUserIds
const onSelectionChange = (selection) => {
  // Rebuild set from full selection
  const set = new Set(selectedUserIds.value);
  // Ensure any deselected from current page are removed
  const currentIds = new Set(usersNoRole.value.map(r => r.userId));
  for (const id of currentIds) { if (!selection.some(s => s.userId === id)) set.delete(id); }
  // Add newly selected
  for (const s of selection) set.add(s.userId);
  selectedUserIds.value = set;
};

// 点击行切换选择（便捷）
const onRowClick = (row) => {
  const id = row.userId;
  const set = new Set(selectedUserIds.value);
  if (set.has(id)) set.delete(id); else set.add(id);
  selectedUserIds.value = set;
  toggleRowSelectionSync();
};

const getErrorMessage = (error, defaultMessage) => {
  if (error?.response?.data?.errors) {
    const errors = error.response.data.errors;
    const messages = Object.values(errors).flat();
    return messages.join('\n');
  }
  if (error?.response?.data?.error) return error.response.data.error;
  if (error?.response?.data?.title || error?.response?.data?.detail) {
    return [error.response.data.title, error.response.data.detail].filter(Boolean).join('：');
  }
  return defaultMessage;
};

// 执行分配
const assigning = ref(false);
const submitAssign = async () => {
  if (selectedRoleIds.value.length === 0) { ElMessage.warning('请先选择角色'); return; }
  const userIds = Array.from(selectedUserIds.value);
  if (userIds.length === 0) { ElMessage.warning('请先选择用户'); return; }
  assigning.value = true;
  try {
    // 基于后端现有接口：先记录变更前的关系集合，执行分配后再比较差异得到新增数量
    const targetPairs = new Set();
    for (const uid of userIds) {
      for (const rid of selectedRoleIds.value) {
        targetPairs.add(`${uid}-${rid}`);
      }
    }
    const beforeSet = new Set(userRoles.value.map(ur => `${ur.userId}-${ur.roleId}`));

    await assignUserRolesBatch({ userIds, roleIds: selectedRoleIds.value });

    // 重置已选，刷新主列表映射
    selectedUserIds.value = new Set();
    selectedRoleIds.value = [];
    await fetchData();
    const afterSet = new Set(userRoles.value.map(ur => `${ur.userId}-${ur.roleId}`));
    const affectedUsers = new Set();
    for (const key of targetPairs) {
      if (!beforeSet.has(key) && afterSet.has(key)) {
        const uid = Number(key.split('-')[0]);
        affectedUsers.add(uid);
      }
    }
    const affectedCount = affectedUsers.size;
    ElMessage.success(`分配成功：已为 ${affectedCount} 个用户分配角色`);
    await fetchUsersNoRoles();
  } catch (error) {
    ElMessage.error(getErrorMessage(error, '分配失败'));
  } finally {
    assigning.value = false;
  }
};

watch(drawerVisible, (v) => { if (!v) { selectedUserIds.value = new Set(); selectedRoleIds.value = []; } });
</script>

<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>用户-角色映射</span>
        <div class="card-actions">
          <el-button type="primary" @click="openDrawer">新增</el-button>
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

  <!-- 右侧抽屉：批量分配角色 -->
  <el-drawer v-model="drawerVisible" title="批量分配角色" direction="rtl" size="60%">
    <el-row :gutter="16">
      <!-- 左侧：角色池（1） -->
      <el-col :span="8">
        <el-card shadow="never" style="height: 100%">
          <template #header>
            <div>角色池</div>
          </template>
          <el-skeleton v-if="rolesLoading" :rows="5" animated />
          <el-transfer
            v-else
            v-model="selectedRoleIds"
            :data="rolesData"
            filterable
            :titles="['可选角色', '已选角色']"
          />
        </el-card>
      </el-col>

      <!-- 右侧：用户列表（2） -->
      <el-col :span="16">
        <el-card shadow="never" style="height: 100%">
          <template #header>
            <div class="flex-row">
              <el-select v-model="searchKey" placeholder="选择字段" style="width: 160px; margin-right: 8px;">
                <el-option label="用户名" value="UserName" />
                <el-option label="邮箱" value="Email" />
                <el-option label="电话" value="Phone" />
                <el-option label="姓名" value="RealName" />
                <el-option label="身份证号" value="IdCard" />
                <el-option label="性别" value="Gender" />
              </el-select>
              <el-input v-if="searchKey !== 'Gender'" v-model="searchInput" placeholder="请输入查询值" clearable style="width: 220px; margin-right: 8px;" />
              <el-select v-else v-model="adv.Gender" placeholder="选择性别" style="width: 220px; margin-right: 8px;">
                <el-option label="男" value="男" />
                <el-option label="女" value="女" />
              </el-select>
              <el-button type="primary" @click="fetchUsersNoRoles">查询</el-button>
              <el-button text @click="advVisible = !advVisible">高级筛选</el-button>
            </div>
          </template>

          <div v-show="advVisible" class="adv-bar">
            <el-input v-model="adv.Email" placeholder="邮箱" clearable style="width: 180px; margin-right: 8px;" />
            <el-input v-model="adv.Phone" placeholder="电话" clearable style="width: 180px; margin-right: 8px;" />
            <el-input v-model="adv.RealName" placeholder="姓名" clearable style="width: 160px; margin-right: 8px;" />
            <el-input v-model="adv.IdCard" placeholder="身份证号" clearable style="width: 220px; margin-right: 8px;" />
          </div>

          <el-table
            ref="usersTableRef"
            :data="usersNoRole"
            v-loading="usersNoRoleLoading"
            stripe
            row-key="userId"
            :reserve-selection="true"
            @selection-change="onSelectionChange"
            @row-click="onRowClick"
          >
            <el-table-column type="selection" width="50" />
            <el-table-column prop="userId" label="ID" width="90" />
            <el-table-column prop="userName" label="用户名" show-overflow-tooltip />
            <el-table-column prop="email" label="邮箱" show-overflow-tooltip />
            <el-table-column prop="phone" label="电话" show-overflow-tooltip />
            <el-table-column prop="realName" label="姓名" show-overflow-tooltip />
            <el-table-column prop="idCard" label="身份证号码" show-overflow-tooltip />
            <el-table-column prop="gender" label="性别" width="80" />
          </el-table>
          <div class="pagination-bar">
            <el-pagination
              background
              layout="total, sizes, prev, pager, next, jumper"
              :total="usersTotal"
              :page-sizes="[10, 20, 50]"
              :page-size="usersPageSize"
              :current-page="usersPageNumber"
              @size-change="handleUsersSizeChange"
              @current-change="handleUsersPageChange"
            />
          </div>

          <div class="assign-bar">
            <el-button type="primary" :loading="assigning" @click="submitAssign">分配所选角色到所选用户</el-button>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </el-drawer>
</template>

<style scoped>
.card-header { display: flex; justify-content: space-between; align-items: center; }
.flex-row { display: flex; align-items: center; }
.adv-bar { padding: 8px 0; }
.pagination-bar { display: flex; justify-content: flex-end; padding-top: 8px; }
.assign-bar { display: flex; justify-content: flex-end; padding-top: 12px; }
</style>
