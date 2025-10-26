<script setup>
defineOptions({ name: 'UserIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { listUsers, createUser } from '../services.js';

const loading = ref(false);
const users = ref([]);

const dialogVisible = ref(false);
const formRef = ref();
const form = ref({ userName: '', password: '', email: '', phone: '', realName: '', gender: '' });
const rules = {
  userName: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '邮箱格式不正确', trigger: 'blur' },
  ],
  phone: [{ required: true, message: '请输入电话', trigger: 'blur' }],
  realName: [{ required: true, message: '请输入真实姓名', trigger: 'blur' }],
  gender: [{ required: true, message: '请选择性别', trigger: 'change' }],
};

const fetchUsers = async () => {
  loading.value = true;
  try {
    users.value = await listUsers();
  } catch {
    ElMessage.error('获取用户列表失败');
  } finally {
    loading.value = false;
  }
};

const openCreate = () => { dialogVisible.value = true; };
const submitCreate = () => {
  formRef.value.validate(async (valid) => {
    if (!valid) return;
    try {
      await createUser({ ...form.value });
      ElMessage.success('创建成功');
      dialogVisible.value = false;
      form.value = { userName: '', password: '', email: '', phone: '', realName: '', gender: '' };
      fetchUsers();
    } catch {
      ElMessage.error('创建失败');
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
      <el-table-column prop="email" label="邮箱" />
      <el-table-column prop="phone" label="电话" />
      <el-table-column prop="realName" label="姓名" />
      <el-table-column prop="gender" label="性别" />
      <el-table-column prop="status" label="状态">
        <template #default="{ row }">
          <el-tag :type="row.status ? 'success' : 'info'">{{ row.status ? '启用' : '禁用' }}</el-tag>
        </template>
      </el-table-column>
    </el-table>
  </el-card>

  <el-dialog v-model="dialogVisible" title="新增用户" width="560px">
    <el-form ref="formRef" :model="form" :rules="rules" label-width="90px">
      <el-form-item label="用户名" prop="userName"><el-input v-model="form.userName" /></el-form-item>
      <el-form-item label="密码" prop="password"><el-input v-model="form.password" type="password" /></el-form-item>
      <el-form-item label="邮箱" prop="email"><el-input v-model="form.email" /></el-form-item>
      <el-form-item label="电话" prop="phone"><el-input v-model="form.phone" /></el-form-item>
      <el-form-item label="姓名" prop="realName"><el-input v-model="form.realName" /></el-form-item>
      <el-form-item label="性别" prop="gender">
        <el-select v-model="form.gender" placeholder="请选择">
          <el-option label="男" value="男" />
          <el-option label="女" value="女" />
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible = false">取消</el-button>
      <el-button type="primary" @click="submitCreate">提交</el-button>
    </template>
  </el-dialog>
</template>

<style scoped>
.card-header { display: flex; justify-content: space-between; align-items: center; }
</style>