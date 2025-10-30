<script setup>
defineOptions({ name: 'RoleIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { listRoles, updateRole, createRole } from '../services.js';

const headerCellStyle = () => ({ textAlign: 'center' });
const cellStyle = () => ({ textAlign: 'center' });

const loading = ref(false);
/**
 * @typedef {Object} RoleRow
 * @property {number} roleId
 * @property {string} roleName
 * @property {string=} description
 */
const roles = ref([]);
const fetchRoles = async () => {
  loading.value = true;
  try { roles.value = await listRoles(); } catch { ElMessage.error('获取角色失败'); }
  loading.value = false;
};
onMounted(fetchRoles);

// 编辑对话框与表单
const editDialogVisible = ref(false);
const editFormRef = ref();
const editForm = ref({ roleId: 0, roleName: '', description: '' });
const originalEdit = ref({ roleId: 0, roleName: '', description: '' });

const editRules = {
  roleName: [
    {
      validator: (_rule, value, callback) => {
        const v = (value ?? '').trim();
        if (v && v.length > 30) callback(new Error('角色名最长 30 字'));
        else callback();
      },
      trigger: 'blur'
    }
  ],
  description: [
    {
      validator: (_rule, value, callback) => {
        const v = (value ?? '').trim();
        if (v && v.length > 100) callback(new Error('描述最长 100 字'));
        else callback();
      },
      trigger: 'blur'
    }
  ]
};

const openEdit = (row) => {
  const copy = JSON.parse(JSON.stringify(row));
  editForm.value = copy;
  originalEdit.value = JSON.parse(JSON.stringify(row));
  editDialogVisible.value = true;
};

const buildPatch = () => {
  const patch = { RoleName: null, Description: null };
  const newName = (editForm.value.roleName ?? '').trim();
  const oldName = (originalEdit.value.roleName ?? '').trim();
  if (newName !== oldName) {
    patch.RoleName = newName || null;
  }
  const newDesc = (editForm.value.description ?? '').trim();
  const oldDesc = (originalEdit.value.description ?? '').trim();
  if (newDesc !== oldDesc) {
    patch.Description = newDesc || null;
  }
  return patch;
};

const submitEdit = () => {
  if (!editFormRef.value) return;
  editFormRef.value.validate(async (valid) => {
    if (!valid) return;
    const patch = buildPatch();
    try {
      await updateRole(editForm.value.roleId, patch);
      ElMessage.success('修改成功');
      editDialogVisible.value = false;
      await fetchRoles();
    } catch {
      ElMessage.error('修改失败');
    }
  });
};

// 新增对话框与表单
const createDialogVisible = ref(false);
const createFormRef = ref();
const createForm = ref({ roleName: '', description: '' });

const createRules = {
  roleName: [
    { required: true, message: '角色名称不能为空', trigger: 'blur' },
    {
      validator: (_rule, value, callback) => {
        const v = (value ?? '').trim();
        if (v && v.length > 30) callback(new Error('角色名最长 30 字'));
        else callback();
      },
      trigger: 'blur'
    }
  ],
  description: [
    {
      validator: (_rule, value, callback) => {
        const v = (value ?? '').trim();
        if (v && v.length > 100) callback(new Error('描述最长 100 字'));
        else callback();
      },
      trigger: 'blur'
    }
  ]
};

const openCreate = () => {
  createForm.value = { roleName: '', description: '' };
  if (createFormRef.value) {
    createFormRef.value.resetFields();
  }
  createDialogVisible.value = true;
};

const submitCreate = () => {
  if (!createFormRef.value) return;
  createFormRef.value.validate(async (valid) => {
    if (!valid) return;
    const payload = {
      RoleName: (createForm.value.roleName ?? '').trim(),
      Description: (createForm.value.description ?? '').trim() || null,
    };
    try {
      await createRole(payload);
      ElMessage.success('新增成功');
      createDialogVisible.value = false;
      await fetchRoles();
    } catch {
      ElMessage.error('新增失败');
    }
  });
};

</script>

<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>角色管理</span>
        <div class="card-actions">
          <el-button type="primary" @click="openCreate">新增</el-button>
          <el-button type="danger">删除</el-button>
        </div>
      </div>
    </template>


    <el-table :data="roles" class="search-table" v-loading="loading" stripe :header-cell-style="headerCellStyle" :cell-style="cellStyle">
      <el-table-column prop="roleId" label="ID" width="80" />
      <el-table-column prop="roleName" label="角色名" />
      <el-table-column prop="description" label="描述" />
      <el-table-column label="修改">
        <template #default="{ row }">
          <el-button type="primary" @click="openEdit(row)">修改</el-button>
        </template>
      </el-table-column>
    </el-table>
  </el-card>
  <el-dialog v-model="editDialogVisible" title="修改角色" width="480px">
    <el-form :model="editForm" :rules="editRules" ref="editFormRef" label-width="100px">
      <el-form-item label="角色名" prop="roleName">
        <el-input v-model="editForm.roleName" placeholder="最长 30 字" clearable />
      </el-form-item>
      <el-form-item label="描述" prop="description">
        <el-input v-model="editForm.description" placeholder="最长 100 字" type="textarea" :rows="3" clearable />
      </el-form-item>
    </el-form>
    <template #footer>
      <span>
        <el-button @click="editDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitEdit">保存</el-button>
      </span>
    </template>
  </el-dialog>

  <el-dialog v-model="createDialogVisible" title="新增角色" width="480px">
    <el-form :model="createForm" :rules="createRules" ref="createFormRef" label-width="100px">
      <el-form-item label="角色名" prop="roleName">
        <el-input v-model="createForm.roleName" placeholder="最长 30 字" clearable />
      </el-form-item>
      <el-form-item label="描述" prop="description">
        <el-input v-model="createForm.description" placeholder="最长 100 字" type="textarea" :rows="3" clearable />
      </el-form-item>
    </el-form>
    <template #footer>
      <span>
        <el-button @click="createDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitCreate">创建</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<style scoped>
.card-header{display: flex; justify-content: space-between; align-items: center;}
.card-actions{display: flex; align-items: center;}

</style>
