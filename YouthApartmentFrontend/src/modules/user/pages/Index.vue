<script setup>
defineOptions({ name: 'UserIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { listUsers, createUser, setUserStatus } from '../services.js';

const loading = ref(false);
const users = ref([]);

const dialogVisible = ref(false);
const formRef = ref();
const form = ref({ userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '' });
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
  gender: [{ required: true, message: '性别不能为空', trigger: 'change' }],
  idCard: [
    { required: true, message: '身份证号不能为空', trigger: 'blur' },
    { pattern: /^[1-9]\d{5}(19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$/, message: '请输入有效的身份证号', trigger: 'blur' }
  ],
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

const handleStatusChange = async (row) => {
  try {
    await setUserStatus(row.userId, row.status);
    ElMessage.success('状态更新成功');
  } catch {
    ElMessage.error('状态更新失败');
    row.status = !row.status;
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
      form.value = { userName: '', password: '', email: '', phone: '', realName: '', gender: '', idCard: '' };
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
    </el-table>
  </el-card>

  <el-dialog v-model="dialogVisible" title="新增用户" width="680px">
    <el-form ref="formRef" :model="form" :rules="rules" label-width="90px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="用户名" prop="userName"><el-input v-model="form.userName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="密码" prop="password"><el-input v-model="form.password" type="password" /></el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="邮箱" prop="email"><el-input v-model="form.email" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="电话" prop="phone"><el-input v-model="form.phone" /></el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="姓名" prop="realName"><el-input v-model="form.realName" /></el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="性别" prop="gender">
            <el-select v-model="form.gender" placeholder="请选择" style="width: 100%;">
              <el-option label="男" value="男" />
              <el-option label="女" value="女" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="身份证号" prop="idCard"><el-input v-model="form.idCard" /></el-form-item>
        </el-col>
      </el-row>
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