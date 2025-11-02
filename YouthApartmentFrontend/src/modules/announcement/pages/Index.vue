<script setup>
defineOptions({ name: 'AnnouncementIndexPage' });
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import Editor from '@/components/common/Editor.vue';
import { listAnnouncements, createAnnouncement, updateAnnouncement, deleteAnnouncement } from '../services.js';

const headerCellStyle = () => ({ textAlign: 'center' });
const cellStyle = () => ({ textAlign: 'center' });

const loading = ref(false);
/** @type {import('../services.js').AnnouncementDto[]} */
const rows = ref([]);
/** 选择的公告行 */
/** @type {import('../services.js').AnnouncementDto[]} */
const selectedRows = ref([]);
const handleSelectionChange = (selection) => { selectedRows.value = selection; };
const fetchList = async () => {
  loading.value = true;
  try { rows.value = await listAnnouncements(); } catch { ElMessage.error('获取公告失败'); }
  loading.value = false;
};
onMounted(fetchList);

// 枚举选项（与后端保持一致）
const typeOptions = [
  { label: '系统', value: 1 },
  { label: '维护', value: 2 },
  { label: '运营', value: 3 },
];
const statusOptions = [
  { label: '草稿', value: 0 },
  { label: '发布', value: 1 },
  { label: '关闭', value: 2 },
];

// 新增
const createDialogVisible = ref(false);
const createFormRef = ref();
const createForm = ref({ title: '', type: 1, status: 0, content: '', publishTime: null, expireTime: null });
const createRules = {
  title: [
    { required: true, message: '标题不能为空', trigger: 'blur' },
    {
      validator: (_r, v, cb) => {
        const s = (v ?? '').trim();
        if (s && s.length > 255) cb(new Error('标题最长 255 字')); else cb();
      }, trigger: 'blur'
    }
  ],
  content: [ { required: true, message: '内容不能为空', trigger: 'change' } ],
  publishTime: [ { required: true, message: '发布时间不能为空', trigger: 'change' } ],
  expireTime: [
    { required: true, message: '结束时间不能为空', trigger: 'change' },
    {
      validator: (_r, v, cb) => {
        if (v && createForm.value.publishTime && new Date(v) < new Date(createForm.value.publishTime)) {
          cb(new Error('结束时间不能早于发布时间'));
        } else {
          cb();
        }
      }, trigger: 'change'
    }
  ],
};
const openCreate = () => {
  createForm.value = { title: '', type: 1, status: 0, content: '', publishTime: null, expireTime: null };
  createDialogVisible.value = true;
};

const durationText = (start, end) => {
  if (!start || !end) return '-';
  const diff = new Date(end) - new Date(start);
  if (diff < 0) return '时间范围无效';
  const days = Math.floor(diff / (1000 * 60 * 60 * 24));
  const hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
  const minutes = Math.floor((diff / (1000 * 60)) % 60);
  return `${days}天 ${hours}小时 ${minutes}分钟`;
};
const submitCreate = () => {
  if (!createFormRef.value) return;
  createFormRef.value.validate(async (valid) => {
    if (!valid) return;
    const payload = {
      Title: (createForm.value.title ?? '').trim(),
      Content: createForm.value.content ?? '',
      Type: createForm.value.type,
      Status: createForm.value.status,
      PublishTime: createForm.value.publishTime,
      ExpireTime: createForm.value.expireTime,
    };
    try {
      await createAnnouncement(payload);
      ElMessage.success('新增成功');
      createDialogVisible.value = false;
      await fetchList();
    } catch { ElMessage.error('新增失败'); }
  });
};

// 编辑
const editDialogVisible = ref(false);
const editFormRef = ref();
const editForm = ref({ announceMentId: 0, title: '', type: 1, status: 0, content: '', publishTime: null, expireTime: null });
const originalEdit = ref({ title: '', type: 1, status: 0, content: '', publishTime: null, expireTime: null });
const editRules = {
  title: [
    {
      validator: (_r, v, cb) => {
        const s = (v ?? '').trim();
        if (s && s.length > 255) cb(new Error('标题最长 255 字')); else cb();
      }, trigger: 'blur'
    }
  ],
  expireTime: [
    {
      validator: (_r, v, cb) => {
        if (v && editForm.value.publishTime && new Date(v) < new Date(editForm.value.publishTime)) {
          cb(new Error('结束时间不能早于发布时间'));
        } else {
          cb();
        }
      }, trigger: 'change'
    }
  ]
};
const openEdit = (row) => {
  const copy = JSON.parse(JSON.stringify(row));
  editForm.value = {
    announceMentId: copy.announceMentId,
    title: copy.title,
    type: copy.type,
    status: copy.status,
    content: copy.content,
    publishTime: copy.publishTime,
    expireTime: copy.expireTime,
  };
  originalEdit.value = {
    title: copy.title,
    type: copy.type,
    status: copy.status,
    content: copy.content,
    publishTime: copy.publishTime,
    expireTime: copy.expireTime,
  };
  editDialogVisible.value = true;
};
const buildPatch = () => {
  const patch = { Title: null, Content: null, Type: null, Status: null, PublishTime: null, ExpireTime: null };
  const tNew = (editForm.value.title ?? '').trim();
  const tOld = (originalEdit.value.title ?? '').trim();
  if (tNew !== tOld) patch.Title = tNew || null;
  if ((editForm.value.content ?? '') !== (originalEdit.value.content ?? '')) patch.Content = editForm.value.content ?? null;
  if (editForm.value.type !== originalEdit.value.type) patch.Type = editForm.value.type ?? null;
  if (editForm.value.status !== originalEdit.value.status) patch.Status = editForm.value.status ?? null;
  if (editForm.value.publishTime !== originalEdit.value.publishTime) patch.PublishTime = editForm.value.publishTime ?? null;
  if (editForm.value.expireTime !== originalEdit.value.expireTime) patch.ExpireTime = editForm.value.expireTime ?? null;
  return patch;
};
const submitEdit = () => {
  if (!editFormRef.value) return;
  editFormRef.value.validate(async (valid) => {
    if (!valid) return;
    const patch = buildPatch();
    try {
      await updateAnnouncement(editForm.value.announceMentId, patch);
      ElMessage.success('修改成功');
      editDialogVisible.value = false;
      await fetchList();
    } catch {
      ElMessage.error('修改失败');
    }
  });
};

// 批量删除（选择项）
const removeSelected = async () => {
  if (!selectedRows.value || selectedRows.value.length === 0) return;
  try {
    await ElMessageBox.confirm(`确认删除选中的 ${selectedRows.value.length} 条公告？`, '提示', { type: 'warning' });
    const ids = selectedRows.value.map(r => r.announceMentId);
    await Promise.all(ids.map(id => deleteAnnouncement(id)));
    ElMessage.success('删除成功');
    selectedRows.value = [];
    await fetchList();
  } catch (e) {
    if (e !== 'cancel') ElMessage.error('删除失败');
  }
};
</script>

<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>公告管理</span>
        <div class="card-actions">
          <el-button type="primary" @click="openCreate">新增</el-button>
          <el-button type="danger" :disabled="selectedRows.length === 0" @click="removeSelected">删除</el-button>
        </div>
      </div>
    </template>

    <el-table :data="rows" class="search-table" v-loading="loading" stripe :header-cell-style="headerCellStyle" :cell-style="cellStyle" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="announceMentId" label="ID" width="80" />
      <el-table-column prop="title" label="标题" />
      <el-table-column prop="type" label="类型">
        <template #default="{ row }">{{ typeOptions.find(it => it.value === row.type)?.label ?? '未知' }}</template>
      </el-table-column>
      <el-table-column prop="status" label="状态">
        <template #default="{ row }">{{ statusOptions.find(it => it.value === row.status)?.label ?? '未知' }}</template>
      </el-table-column>
      <el-table-column prop="publishTime" label="发布时间" />
      <el-table-column prop="expireTime" label="结束时间" />
      <el-table-column label="操作" width="220">
        <template #default="{ row }">
          <el-button type="primary" @click="openEdit(row)">修改</el-button>

        </template>
      </el-table-column>
    </el-table>
  </el-card>

  <!-- 新增 -->
  <el-dialog v-model="createDialogVisible" title="新增公告" width="95%" custom-class="fullscreen-dialog">
    <el-form :model="createForm" :rules="createRules" ref="createFormRef" label-width="100px">
      <el-form-item label="标题" prop="title">
        <el-input v-model="createForm.title" placeholder="最长 255 字" clearable />
      </el-form-item>
      <el-form-item label="类型">
        <el-select v-model="createForm.type" style="width: 240px">
          <el-option v-for="it in typeOptions" :key="it.value" :label="it.label" :value="it.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="createForm.status" style="width: 240px">
          <el-option v-for="st in statusOptions" :key="st.value" :label="st.label" :value="st.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="发布时间" prop="publishTime">
        <el-date-picker v-model="createForm.publishTime" type="datetime" placeholder="选择发布时间" format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DDTHH:mm:ss" />
      </el-form-item>
      <el-form-item label="结束时间" prop="expireTime">
        <el-date-picker v-model="createForm.expireTime" type="datetime" placeholder="选择结束时间" format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DDTHH:mm:ss" />
      </el-form-item>
      <el-form-item label="持续时间">
        <el-input :value="durationText(createForm.publishTime, createForm.expireTime)" disabled />
      </el-form-item>
      <el-form-item label="内容" prop="content">
        <Editor v-model="createForm.content" />
      </el-form-item>
    </el-form>
    <template #footer>
      <span>
        <el-button @click="createDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitCreate">创建</el-button>
      </span>
    </template>
  </el-dialog>

  <!-- 编辑 -->
  <el-dialog v-model="editDialogVisible" title="修改公告" width="95%" custom-class="fullscreen-dialog">
    <el-form :model="editForm" :rules="editRules" ref="editFormRef" label-width="100px">
      <el-form-item label="标题" prop="title">
        <el-input v-model="editForm.title" placeholder="最长 255 字" clearable />
      </el-form-item>
      <el-form-item label="类型">
        <el-select v-model="editForm.type" style="width: 240px">
          <el-option v-for="it in typeOptions" :key="it.value" :label="it.label" :value="it.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="editForm.status" style="width: 240px">
          <el-option v-for="st in statusOptions" :key="st.value" :label="st.label" :value="st.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="发布时间" prop="publishTime">
        <el-date-picker v-model="editForm.publishTime" type="datetime" placeholder="选择发布时间" format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DDTHH:mm:ss" />
      </el-form-item>
      <el-form-item label="结束时间" prop="expireTime">
        <el-date-picker v-model="editForm.expireTime" type="datetime" placeholder="选择结束时间" format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DDTHH:mm:ss" />
      </el-form-item>
      <el-form-item label="持续时间">
        <el-input :value="durationText(editForm.publishTime, editForm.expireTime)" disabled />
      </el-form-item>
      <el-form-item label="内容">
        <Editor v-model="editForm.content" />
      </el-form-item>
    </el-form>
    <template #footer>
      <span>
        <el-button @click="editDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitEdit">保存</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<style scoped>
.card-header{display: flex; justify-content: space-between; align-items: center;}
.card-actions{display: flex; align-items: center;}

.fullscreen-dialog {
  width: 100% !important;
  height: 100% !important;
  margin: 0 !important;
  max-width: none !important;
}
</style>
