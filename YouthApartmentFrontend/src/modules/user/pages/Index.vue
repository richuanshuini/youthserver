<script setup>
defineOptions({ name: 'UserIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { UserFilled } from '@element-plus/icons-vue';
import { listUsersPaged, createUser, setUserStatus, updateUser, searchUsers } from '../services.js';

/**
 * @typedef {object} UserRow
 * @property {number} userId
 * @property {string} userName
 * @property {string} password
 * @property {string} email
 * @property {string} phone
 * @property {string} realName
 * @property {string} idCard
 * @property {string} gender
 * @property {string} userAvatarUrl
 * @property {boolean} status
 */

// 解析头像地址：
// - data: URI 直接使用
// - http(s) 绝对地址直接使用
// - 以 / 开头的后端相对路径，拼接后端 baseURL
const apiBase = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5160';

const resolveAvatarUrl = (u) => {
  if (!u || typeof u !== 'string') return '';
  const val = u.trim();
  if (!val) return '';

  // 绝对地址或 data URI
  if (val.startsWith('data:') || val.startsWith('http')) return val;

  // 以 / 开头的后端相对路径
  if (val.startsWith('/')) return apiBase + val;

  // 仅当看起来像相对资源路径（包含 / 或 .）时才拼接
  if (/[/\\.]/.test(val)) {
    const rel = val.replace(/^\/+/, '');
    return `${apiBase}/${rel}`;
  }

  // 像 "string" 这类占位值：不返回 URL，避免触发网络请求
  return '';
};

const loading = ref(false);
/** @type {import('vue').Ref<UserRow[]>} */
const users = ref([]);
const currentPage = ref(1);
const pageSize = ref(10);
const total = ref(0);

const headerCellStyle = () => ({ textAlign: 'center' });
const cellStyle = () => ({ textAlign: 'center' });

// --- Search Module ---
const searchKey = ref('UserName'); // 默认使用“用户名”以便初始展示输入框
const searchText = ref(''); // for string fields
const searchGender = ref('');
const searchStatus = ref('all'); // all | enabled | disabled

// --- Create User Dialog ---
const createDialogVisible = ref(false);
const createFormRef = ref();
const createForm = ref({ userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '', userAvatarUrl: '' });

// --- Edit User Dialog ---
const editDialogVisible = ref(false);
const editFormRef = ref();
const editForm = ref({ userId: null, userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '', userAvatarUrl: '' });

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
  createForm.value = { userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '', userAvatarUrl: '' };
};

const openDelete=()=>{

}

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

// 处理新增用户头像选择（不自动上传，转为 base64 存到表单中）
const handleCreateAvatarChange = (uploadFile) => {
  const file = uploadFile.raw;
  if (!file) return;
  const isImage = file.type.startsWith('image/');
  const isLt2M = file.size / 1024 / 1024 < 2;
  if (!isImage) { ElMessage.error('请上传图片文件'); return; }
  if (!isLt2M) { ElMessage.error('图片大小不能超过2MB'); return; }
  const reader = new FileReader();
  reader.onload = () => { createForm.value.userAvatarUrl = reader.result; };
  reader.readAsDataURL(file);
};

// 处理编辑用户头像选择（不自动上传，转为 base64 存到表单中）
const handleEditAvatarChange = (uploadFile) => {
  const file = uploadFile.raw;
  if (!file) return;
  const isImage = file.type.startsWith('image/');
  const isLt2M = file.size / 1024 / 1024 < 2;
  if (!isImage) { ElMessage.error('请上传图片文件'); return; }
  if (!isLt2M) { ElMessage.error('图片大小不能超过2MB'); return; }
  const reader = new FileReader();
  reader.onload = () => { editForm.value.userAvatarUrl = reader.result; };
  reader.readAsDataURL(file);
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

// 应用搜索（单条件）
const applySearch = async () => {
  // 若未选择条件，或必要值为空，则回到分页列表
  if (!searchKey.value) {
    currentPage.value = 1;
    await fetchUsers();
    return;
  }

  let payload = {};
  if (searchKey.value === 'Status') {
    if (searchStatus.value === 'enabled') payload = { Status: true };
    else if (searchStatus.value === 'disabled') payload = { Status: false };
    else {
      await fetchUsers();
      return;
    }
  } else if (searchKey.value === 'Gender') {
    if (!searchGender.value) {
      await fetchUsers();
      return;
    }
    payload = { Gender: searchGender.value };
  } else {
    const text = (searchText.value || '').trim();
    if (!text) {
      await fetchUsers();
      return;
    }
    // 其它字符串字段
    payload = { [searchKey.value]: text };
  }

  try {
    loading.value = true;
    const res = await searchUsers(payload);
    users.value = Array.isArray(res) ? res : [];
    total.value = users.value.length;
    currentPage.value = 1; // 搜索后回到第一页
  } catch (error) {
    ElMessage.error(getErrorMessage(error, '查询失败'));
  } finally {
    loading.value = false;
  }
};

// 重置搜索
const resetSearch = async () => {
  searchText.value = '';
  searchGender.value = '';
  searchStatus.value = 'all';
  currentPage.value = 1;
  await fetchUsers();
};


onMounted(fetchUsers);
</script>

<template>
  <!-- 搜索模块（独立 div，置于用户显示表单上方） -->
  <el-card class="box-card">
    <div class="search-bar">
      <el-form class="search-form" label-position="top" size="large">
        <el-row :gutter="10">
          <el-col :span="5">
            <el-form-item>
              <el-select v-model="searchKey" placeholder="请选择" style="width: 100%;" filterable>
                <el-option label="用户名" value="UserName" />
                <el-option label="姓名" value="RealName" />
                <el-option label="邮箱" value="Email" />
                <el-option label="电话" value="Phone" />
                <el-option label="性别" value="Gender" />
                <el-option label="用户状态" value="Status" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8" v-if="searchKey && searchKey !== 'Status' && searchKey !== 'Gender'">
            <el-form-item>
              <el-input v-model="searchText" placeholder="请输入查询条件" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8" v-if="searchKey === 'Gender'">
            <el-form-item>
              <el-select v-model="searchGender" placeholder="请选择" style="width: 100%;" filterable>
                <el-option label="男" value="男" />
                <el-option label="女" value="女" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8" v-if="searchKey === 'Status'">
            <el-form-item>
              <el-select v-model="searchStatus" placeholder="请选择" style="width: 100%;">
                <el-option label="全部" value="all" />
                <el-option label="启用" value="enabled" />
                <el-option label="禁用" value="disabled" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8" class="search-actions">
            <el-form-item label=" ">
              <el-button type="primary" @click="applySearch">查询</el-button>
              <el-button @click="resetSearch">重置</el-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
  </el-card>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>用户管理</span>
        <div class="card-actions">
          <el-button type="primary" @click="openCreate">新增</el-button>
          <el-button type="danger" @click="openDelete" disabled>删除</el-button>
        </div>
      </div>
    </template>

  <el-table :data="users" v-loading="loading" stripe :header-cell-style="headerCellStyle" :cell-style="cellStyle">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="userId" label="ID" width="80" />
      <el-table-column prop="userName" label="用户名" />
      <el-table-column prop="password" label="密码" />
      <el-table-column prop="email" label="邮箱" />
      <el-table-column prop="phone" label="电话" />
      <el-table-column prop="realName" label="姓名" />
      <el-table-column prop="idCard" label="身份证号" />
      <el-table-column prop="gender" label="性别" />
      <el-table-column prop="userAvatarUrl" label="头像" width="120" align="center" header-align="center" class-name="avatar-col">
        <template #default="{ row }">
          <el-avatar :size="64" :src="resolveAvatarUrl(row.userAvatarUrl)">
            <el-icon><UserFilled /></el-icon>
          </el-avatar>
        </template>
      </el-table-column>
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
    <el-form
      class="optimized-form"
      ref="createFormRef"
      :model="createForm"
      :rules="rules"
      label-position="top"
      size="large"
      style="max-width: 600px; margin: 0 auto;"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="用户名" prop="userName"><el-input v-model="createForm.userName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="密码" prop="password"><el-input v-model="createForm.password" type="password" show-password clearable /></el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="头像" class="avatar-form-item">
            <el-upload
              class="avatar-uploader"
              action="#"
              :show-file-list="false"
              :auto-upload="false"
              :on-change="handleCreateAvatarChange"
            >
              <el-avatar :size="64" :src="resolveAvatarUrl(createForm.userAvatarUrl)">
                <el-icon><UserFilled /></el-icon>
              </el-avatar>
            </el-upload>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="邮箱" prop="email"><el-input v-model="createForm.email" clearable /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="电话" prop="phone"><el-input v-model="createForm.phone" clearable /></el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="姓名" prop="realName"><el-input v-model="createForm.realName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="性别" prop="gender">
            <el-select v-model="createForm.gender" placeholder="请选择" style="width: 100%;" filterable>
              <el-option label="男" value="男" />
              <el-option label="女" value="女" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="身份证号" prop="idCard"><el-input v-model="createForm.idCard" clearable /></el-form-item>
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
    <el-form
      class="optimized-form"
      ref="editFormRef"
      :model="editForm"
      :rules="editRules"
      label-position="top"
      size="large"
      style="max-width: 600px; margin: 0 auto;"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="用户名" prop="userName"><el-input v-model="editForm.userName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="密码" prop="password">
            <el-input v-model="editForm.password" type="password" show-password placeholder="留空则不修改密码" clearable />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="头像" class="avatar-form-item">
            <el-upload
              class="avatar-uploader"
              action="#"
              :show-file-list="false"
              :auto-upload="false"
              :on-change="handleEditAvatarChange"
            >
              <el-avatar :size="64" :src="resolveAvatarUrl(editForm.userAvatarUrl)">
                <el-icon><UserFilled /></el-icon>
              </el-avatar>
            </el-upload>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="邮箱" prop="email"><el-input v-model="editForm.email" clearable /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="电话" prop="phone"><el-input v-model="editForm.phone" clearable /></el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="姓名" prop="realName"><el-input v-model="editForm.realName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="性别" prop="gender">
            <el-select v-model="editForm.gender" placeholder="请选择" style="width: 100%;" filterable>
              <el-option label="男" value="男" />
              <el-option label="女" value="女" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="身份证号" prop="idCard"><el-input v-model="editForm.idCard" clearable /></el-form-item>
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
.card-actions { display: flex; align-items: center;}
.search-form { max-width: 600px; }
.search-form{ min-height: 40px; }
.search-form{ min-height: 40px; }
.search-actions { display: flex;justify-content:center; height: 40px;align-items: center; margin-top:6px}
.pagination { margin-top: 16px; display: flex; justify-content: flex-end; }
/* 头像列表列居中对齐 */
:deep(.avatar-col) { display: flex; align-items: center; justify-content: center; }
/* 新增用户对话框头像项内容居中 */
:deep(.avatar-form-item ) { display: flex; align-items: center; }
.avatar-uploader { display: inline-block; }
/* 表单优化：顶部标签、间距与可读性 */
.optimized-form { margin-bottom: 16px; }
.optimized-form { font-weight: 600; }
.optimized-form { min-height: 40px; }
.optimized-form { min-height: 40px; }
.box-card{margin-bottom: 16px; height: 80px}
</style>
