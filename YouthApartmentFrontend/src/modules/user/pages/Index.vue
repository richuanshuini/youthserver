<script setup>
defineOptions({ name: 'UserIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { listUsersPaged, createUser, setUserStatus, updateUser } from '../services.js';

const loading = ref(false);
const users = ref([]);
const currentPage = ref(1);
const pageSize = ref(10);
const total = ref(0);

// --- Create User Dialog ---
const createDialogVisible = ref(false);
const createFormRef = ref();
const createForm = ref({ userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '' });

// --- Edit User Dialog ---
const editDialogVisible = ref(false);
const editFormRef = ref();
const editForm = ref({ userId: null, userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '' });

const rules = {
  userName: [
    { required: true, message: '用户名不能为空', trigger: 'blur' },
    { min: 3, max: 30, message: '用户名长度必须在3到30个字符之间', trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9]+$/, message: '用户名只能包含字母和数字', trigger: 'blur' },
  ],
  password: [
    { required: true, message: '密码不能为空', trigger: 'blur' },
    { min: 6, max: 20, message: '密码的长度必须在6到20个字符之间', trigger: 'blur' },
    { pattern: /^[a-zA-Z0-9]+$/, message: '密码只能包含字母和数字', trigger: 'blur' },
  ],
  email: [
    { required: true, message: '邮箱不能为空', trigger: 'blur' },
    { type: 'email', message: '邮箱格式不正确', trigger: 'blur' },
  ],
  phone: [
    { required: true, message: '电话号码不能为空', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入有效的11位中国大陆手机号码', trigger: 'blur' },
  ],
  realName: [{ required: true, message: '真实姓名不能为空', trigger: 'blur' }],
  gender: [
      { required: true, message: '性别不能为空', trigger: 'change' },
      { pattern: /^(男|女)$/, message: '性别必须为 \'男\' 或 \'女\'', trigger: 'change' }
  ],
  idCard: [
    { required: true, message: '身份证号不能为空', trigger: 'blur' },
    { pattern: /^[1-9]\d{5}(19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$/, message: '请输入有效的身份证号', trigger: 'blur' }
  ],
};

const editRules = {
    ...rules,
    password: [ // Override password rule for editing
        { min: 6, max: 20, message: '密码的长度必须在6到20个字符之间', trigger: 'blur' },
        { pattern: /^[a-zA-Z0-9]+$/, message: '密码只能包含字母和数字', trigger: 'blur' },
    ]
};

const fetchUsers = async () => {
  loading.value = true;
  try {
    const res = await listUsersPaged({ pageNumber: currentPage.value, pageSize: pageSize.value });
    users.value = res.items;
    total.value = res.total;
  } catch (error) {
    ElMessage.error(getErrorMessage(error, '获取用户列表失败'));
  } finally {
    loading.value = false;
  }
};

const handleSizeChange = (val) => {
  pageSize.value = val;
  currentPage.value = 1;
  fetchUsers();
};

const handleCurrentChange = (val) => {
  currentPage.value = val;
  fetchUsers();
};

const handleStatusChange = async (row) => {
  try {
    await setUserStatus(row.userId, row.status);
    ElMessage.success('状态更新成功');
  } catch (error) {
    ElMessage.error(getErrorMessage(error, '状态更新失败'));
    row.status = !row.status;
  }
};

// --- Create User Logic ---
const openCreate = () => {
  createDialogVisible.value = true;
  // Reset form if needed
  if (createFormRef.value) {
    createFormRef.value.resetFields();
  }
  createForm.value = { userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '' };
};

const getErrorMessage = (error, defaultMessage) => {
  // ValidationProblemDetails
  if (error?.response?.data?.errors) {
    const errors = error.response.data.errors;
    const messages = Object.values(errors).flat();
    return messages.join('\n');
  }
  // 简单错误字符串
  if (error?.response?.data?.error) {
    return error.response.data.error;
  }
  // problem+json 兼容（可选）
  if (error?.response?.data?.title || error?.response?.data?.detail) {
    return [error.response.data.title, error.response.data.detail].filter(Boolean).join('：');
  }
  return defaultMessage;
};

const submitCreate = () => {
  createFormRef.value.validate(async (valid) => {
    if (!valid) return;
    try {
      await createUser({ ...createForm.value });
      ElMessage.success('创建成功');
      createDialogVisible.value = false;
      fetchUsers();
    } catch (error) {
      ElMessage.error(getErrorMessage(error, '创建失败'));
    }
  });
};

// --- Edit User Logic ---
const openEdit = (row) => {
  if (editFormRef.value) {
    editFormRef.value.resetFields();
  }
  // 将行数据复制到编辑表单，包括密码字段
  editForm.value = { ...row };
  editDialogVisible.value = true;
};

const submitEdit = () => {
  editFormRef.value.validate(async (valid) => {
    if (!valid) return;
    try {
      await updateUser(editForm.value.userId, editForm.value);
      ElMessage.success('修改成功');
      editDialogVisible.value = false;
      fetchUsers();
    } catch (error) {
      ElMessage.error(getErrorMessage(error, '修改失败'));
    }
  });
};


onMounted(fetchUsers);
</script>

<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>用户管理</span>
        <el-button type="primary" @click="openCreate">新增用户</el-button>
      </div>
    </template>

  <el-table :data="users" v-loading="loading" stripe>
      <el-table-column prop="userId" label="ID" width="80" />
      <el-table-column prop="userName" label="用户名" />
      <el-table-column prop="password" label="密码" />
      <el-table-column prop="email" label="邮箱" />
      <el-table-column prop="phone" label="电话" />
      <el-table-column prop="realName" label="姓名" />
      <el-table-column prop="idCard" label="身份证号" />
      <el-table-column prop="gender" label="性别" />
      <el-table-column prop="status" label="状态">
        <template #default="{ row }">
          <el-switch
            v-model="row.status"
            @change="() => handleStatusChange(row)"
            active-color="#13ce66"
            inactive-color="#ff4949"
          />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="120">
        <template #default="{ row }">
          <el-button type="primary" plain @click="openEdit(row)">修改</el-button>
        </template>
      </el-table-column>
  </el-table>
  <div class="pagination">
    <el-pagination
      v-model:current-page="currentPage"
      v-model:page-size="pageSize"
      :page-sizes="[10, 20, 50, 100]"
      :background="true"
      layout="total, sizes, prev, pager, next, jumper"
      :total="total"
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
    />
  </div>
  </el-card>

  <!-- Create User Dialog -->
  <el-dialog v-model="createDialogVisible" title="新增用户" width="680px">
    <el-form ref="createFormRef" :model="createForm" :rules="rules" label-width="90px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="用户名" prop="userName"><el-input v-model="createForm.userName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="密码" prop="password"><el-input v-model="createForm.password" type="text" /></el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="邮箱" prop="email"><el-input v-model="createForm.email" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="电话" prop="phone"><el-input v-model="createForm.phone" /></el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="姓名" prop="realName"><el-input v-model="createForm.realName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="性别" prop="gender">
            <el-select v-model="createForm.gender" placeholder="请选择" style="width: 100%;">
              <el-option label="男" value="男" />
              <el-option label="女" value="女" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="身份证号" prop="idCard"><el-input v-model="createForm.idCard" /></el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <el-button @click="createDialogVisible = false">取消</el-button>
      <el-button type="primary" @click="submitCreate">提交</el-button>
    </template>
  </el-dialog>

  <!-- Edit User Dialog -->
  <el-dialog v-model="editDialogVisible" title="修改用户" width="680px">
    <el-form ref="editFormRef" :model="editForm" :rules="editRules" label-width="90px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="用户名" prop="userName"><el-input v-model="editForm.userName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="密码" prop="password">
            <el-input v-model="editForm.password" type="text" placeholder="留空则不修改密码" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="邮箱" prop="email"><el-input v-model="editForm.email" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="电话" prop="phone"><el-input v-model="editForm.phone" /></el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="姓名" prop="realName"><el-input v-model="editForm.realName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="性别" prop="gender">
            <el-select v-model="editForm.gender" placeholder="请选择" style="width: 100%;">
              <el-option label="男" value="男" />
              <el-option label="女" value="女" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="身份证号" prop="idCard"><el-input v-model="editForm.idCard" /></el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <el-button @click="editDialogVisible = false">取消</el-button>
      <el-button type="primary" @click="submitEdit">提交</el-button>
    </template>
  </el-dialog>
</template>

<style scoped>
.card-header { display: flex; justify-content: space-between; align-items: center; }
.pagination { margin-top: 16px; display: flex; justify-content: flex-end; }
</style>