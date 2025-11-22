<script setup>
defineOptions({ name: 'PropertyIndexPage' });
import { ref, reactive, onMounted, computed, nextTick } from 'vue';
import { ElMessage } from 'element-plus';
import { Search, Refresh, Filter, Plus, View, Close, Edit } from '@element-plus/icons-vue';
import { listUserSelector } from '@/modules/user/services.js';
import { propertyApi } from '../services';

const loading = ref(false);
const isAdvancedSearch = ref(false);
const tableData = ref([]);
const pagination = reactive({ pageNumber: 1, pageSize: 20, total: 0 });

const searchForm = reactive({
  keyword: '',
  status: null,
  regionId: null,
  minRentPrice: null,
  maxRentPrice: null,
  minArea: null,
  maxArea: null,
  minBedrooms: null,
  maxBedrooms: null,
  minBathrooms: null,
  maxBathrooms: null,
  leaseType: null,
  leaseTerm: null,
  approvedByUser: null,
  isDeleted: null,
});

const dateRanges = reactive({ created: [], approved: [], available: [] });

const createDialogVisible = ref(false);
const createFormRef = ref();
const createForm = reactive({
  regionId: null,
  approvedByUser: null,
  area: null,
  bedrooms: null,
  bathrooms: null,
  maxTenants: null,
  propertyName: '',
  address: '',
  description: '',
  propertyCode: '',
  roomNumber: '',
  rentPrice: null,
  rentDeposit: null,
  propertyFee: null,
  latitude: null,
  longitude: null,
  leaseType: null,
  leaseTerm: null,
});
const createApproverDisplay = ref('');

const editDialogVisible = ref(false);
const editFormRef = ref();
const editForm = reactive({
  propertyId: null,
  regionId: null,
  approvedByUser: null,
  area: null,
  bedrooms: null,
  bathrooms: null,
  maxTenants: null,
  propertyName: '',
  address: '',
  description: '',
  propertyCode: '',
  roomNumber: '',
  rentPrice: null,
  rentDeposit: null,
  propertyFee: null,
  latitude: null,
  longitude: null,
  leaseType: null,
  leaseTerm: null,
  availableDate: null,
});
const editApproverDisplay = ref('');

const detailVisible = ref(false);
const detailData = ref({});
const detailTitle = computed(() => detailData.value?.propertyName || '房源详情');

const statusMap = {
  0: { label: '待审核', type: 'warning' },
  1: { label: '可租', type: 'success' },
  2: { label: '已预约', type: 'info' },
  3: { label: '已出租', type: 'danger' },
  4: { label: '维护中', type: 'warning' },
  5: { label: '停用', type: 'info' },
};
const leaseTypeMap = { 1: '整租', 2: '合租' };
const leaseTermMap = { 0: '月租', 1: '季租', 2: '半年', 3: '年租' };

const sharedFieldRules = {
  approvedByUser: [{ required: true, message: '请选择审核员', trigger: 'change' }],
  area: [{ required: true, message: '请输入房源面积', trigger: 'blur' }],
  bedrooms: [{ required: true, message: '请输入卧室数量', trigger: 'blur' }],
  bathrooms: [{ required: true, message: '请输入卫生间数量', trigger: 'blur' }],
  maxTenants: [{ required: true, message: '请输入最大入住人数', trigger: 'blur' }],
  propertyName: [{ required: true, message: '请输入房源名称', trigger: 'blur' }],
  address: [{ required: true, message: '请输入房源地址', trigger: 'blur' }],
  description: [{ required: true, message: '请输入房源描述', trigger: 'blur' }],
  propertyCode: [{ required: true, message: '请输入房源编码', trigger: 'blur' }],
  roomNumber: [{ required: true, message: '请输入房间编号', trigger: 'blur' }],
  rentPrice: [{ required: true, message: '请输入租金', trigger: 'blur' }],
  rentDeposit: [{ required: true, message: '请输入押金', trigger: 'blur' }],
  propertyFee: [{ required: true, message: '请输入物业费', trigger: 'blur' }],
  latitude: [{ required: true, message: '请输入纬度', trigger: 'blur' }],
  longitude: [{ required: true, message: '请输入经度', trigger: 'blur' }],
  leaseType: [{ required: true, message: '请选择租赁方式', trigger: 'change' }],
  leaseTerm: [{ required: true, message: '请选择租期类型', trigger: 'change' }],
};

const createRules = { ...sharedFieldRules };
const editRules = {
  ...sharedFieldRules,
  availableDate: [
    {
      validator: (_rule, value, cb) => {
        if (!value) return cb();
        const ok = /^\d{4}-\d{2}-\d{2}-\d{2}:\d{2}:\d{2}$/.test(value);
        if (!ok) cb(new Error('时间格式应为 YYYY-MM-DD-HH:MM:SS')); else cb();
      },
      trigger: 'change',
    },
  ],
};

const statusText = (val) => statusMap?.[val]?.label ?? '未知';
const statusType = (val) => statusMap?.[val]?.type ?? 'info';
const leaseTypeText = (val) => leaseTypeMap?.[val] ?? '--';
const leaseTermText = (val) => leaseTermMap?.[val] ?? '--';

const toggleAdvanced = () => (isAdvancedSearch.value = !isAdvancedSearch.value);

// 审核员选择器状态
const selectorState = reactive({
  visible: false,
  loading: false,
  data: [],
  total: 0,
  query: {
    pageNumber: 1,
    pageSize: 10,
    keyword: '',
    searchType: 'UserName',
  },
});
const selectorContext = ref('create');
const selectorSelectedId = ref(null);
const selectorSelectedRow = ref(null);
const approverLabelCache = reactive({});
const searchTypes = [
  { label: '用户名', value: 'UserName' },
  { label: '真实姓名', value: 'RealName' },
  { label: '角色名称', value: 'RoleName' },
];

const toIso = (val) => {
  if (!val) return null;
  const d = typeof val === 'string' ? new Date(val) : val;
  return Number.isNaN(d?.getTime?.()) ? null : d.toISOString();
};

const buildQueryPayload = () => {
  const payload = {
    keyword: searchForm.keyword?.trim(),
    status: searchForm.status,
    regionId: searchForm.regionId,
    minRentPrice: searchForm.minRentPrice,
    maxRentPrice: searchForm.maxRentPrice,
    minArea: searchForm.minArea,
    maxArea: searchForm.maxArea,
    minBedrooms: searchForm.minBedrooms,
    maxBedrooms: searchForm.maxBedrooms,
    minBathrooms: searchForm.minBathrooms,
    maxBathrooms: searchForm.maxBathrooms,
    leaseType: searchForm.leaseType,
    leaseTerm: searchForm.leaseTerm,
    approvedByUser: searchForm.approvedByUser,
    isDeleted: searchForm.isDeleted,
  };

  const [createdFrom, createdTo] = dateRanges.created || [];
  if (createdFrom && createdTo) {
    payload.createdFrom = toIso(createdFrom);
    payload.createdTo = toIso(createdTo);
  }
  const [approvedFrom, approvedTo] = dateRanges.approved || [];
  if (approvedFrom && approvedTo) {
    payload.approvedFrom = toIso(approvedFrom);
    payload.approvedTo = toIso(approvedTo);
  }
  const [availableFrom, availableTo] = dateRanges.available || [];
  if (availableFrom && availableTo) {
    payload.availableFrom = toIso(availableFrom);
    payload.availableTo = toIso(availableTo);
  }

  return Object.fromEntries(
    Object.entries(payload).filter(([_, v]) => v !== null && v !== undefined && v !== '' && !Number.isNaN(v))
  );
};

const getErrorMessage = (error, fallback) => {
  if (error?.response?.data?.errors) return Object.values(error.response.data.errors).flat().join('\n');
  if (error?.response?.data?.error) return error.response.data.error;
  if (error?.message) return error.message;
  return fallback;
};

const loadData = async () => {
  loading.value = true;
  try {
    const res = await propertyApi.searchProperties(buildQueryPayload(), pagination.pageNumber, pagination.pageSize);
    tableData.value = res?.items ?? [];
    pagination.total = res?.total ?? 0;
  } catch (error) {
    ElMessage.error(getErrorMessage(error, '获取房源列表失败'));
  } finally {
    loading.value = false;
  }
};

const handleSearch = () => {
  pagination.pageNumber = 1;
  loadData();
};

const resetForm = () => {
  Object.assign(searchForm, {
    keyword: '',
    status: null,
    regionId: null,
    minRentPrice: null,
    maxRentPrice: null,
    minArea: null,
    maxArea: null,
    minBedrooms: null,
    maxBedrooms: null,
    minBathrooms: null,
    maxBathrooms: null,
    leaseType: null,
    leaseTerm: null,
    approvedByUser: null,
    isDeleted: null,
  });
  dateRanges.created = [];
  dateRanges.approved = [];
  dateRanges.available = [];
  pagination.pageNumber = 1;
  loadData();
};

const handlePageChange = (page) => {
  pagination.pageNumber = page;
  loadData();
};
const handleSizeChange = (size) => {
  pagination.pageSize = size;
  pagination.pageNumber = 1;
  loadData();
};

const formatPrice = (val) => (val || val === 0 ? `${val}/月` : '--');
const formatArea = (val) => (val || val === 0 ? `${val}㎡` : '--');
const formatRooms = (row) => `${row?.bedrooms ?? '--'}室${row?.bathrooms ?? '--'}卫`;
const formatDateTime = (val) => {
  if (!val) return '--';
  const d = new Date(val);
  if (Number.isNaN(d.getTime())) return '--';
  const pad = (num) => String(num).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}-${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
};

const formatAvailableDateValue = (val) => {
  if (!val) return null;
  if (typeof val === 'string' && /^\d{4}-\d{2}-\d{2}-\d{2}:\d{2}:\d{2}$/.test(val)) return val;
  const d = new Date(val);
  if (Number.isNaN(d.getTime())) return null;
  const pad = (num) => String(num).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}-${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
};

// 后端需要可解析的时间格式（使用 ISO，避免自定义分隔符导致 .NET 解析失败）
const toBackendDateTime = (val) => {
  if (!val) return null;
  if (typeof val === 'string') {
    const m = val.match(/^(\d{4}-\d{2}-\d{2})-(\d{2}:\d{2}:\d{2})$/);
    if (m) return `${m[1]}T${m[2]}`;
    const d = new Date(val);
    return Number.isNaN(d.getTime()) ? null : d.toISOString();
  }
  if (val instanceof Date && !Number.isNaN(val.getTime())) {
    return val.toISOString();
  }
  return null;
};

const handleCreate = () => {
  if (createFormRef.value) createFormRef.value.resetFields();
  Object.assign(createForm, {
    regionId: null,
    approvedByUser: null,
    area: null,
    bedrooms: null,
    bathrooms: null,
    maxTenants: null,
    propertyName: '',
    address: '',
    description: '',
    propertyCode: '',
    roomNumber: '',
    rentPrice: null,
    rentDeposit: null,
    propertyFee: null,
    latitude: null,
    longitude: null,
    leaseType: null,
    leaseTerm: null,
  });
  createApproverDisplay.value = '';
  createDialogVisible.value = true;
};

const openEdit = async (row) => {
  if (!row) return;
  if (editFormRef.value) {
    editFormRef.value.resetFields();
  }
  Object.assign(editForm, {
    propertyId: row.propertyId ?? null,
    regionId: row.regionId ?? null,
    approvedByUser: row.approvedByUser ?? null,
    area: row.area ?? null,
    bedrooms: row.bedrooms ?? null,
    bathrooms: row.bathrooms ?? null,
    maxTenants: row.maxTenants ?? null,
    propertyName: row.propertyName ?? '',
    address: row.address ?? '',
    description: row.description ?? '',
    propertyCode: row.propertyCode ?? '',
    roomNumber: row.roomNumber ?? '',
    rentPrice: row.rentPrice ?? null,
    rentDeposit: row.rentDeposit ?? null,
    propertyFee: row.propertyFee ?? null,
    latitude: row.latitude ?? null,
    longitude: row.longitude ?? null,
    leaseType: row.leaseType ?? null,
    leaseTerm: row.leaseTerm ?? null,
    availableDate: formatAvailableDateValue(row.availableDate),
  });
  const approverId = row.approvedByUser ?? null;
  if (approverId && approverLabelCache[approverId]) {
    editApproverDisplay.value = approverLabelCache[approverId];
  } else if (approverId) {
    editApproverDisplay.value = row.realName || row.userName || '';
    ensureApproverLabel(approverId, 'edit');
  } else {
    editApproverDisplay.value = '';
  }
  editDialogVisible.value = true;
  await nextTick();
  editFormRef.value?.clearValidate();
};

const submitCreate = () => {
  if (!createFormRef.value) return;
  createFormRef.value.validate(async (valid) => {
    if (!valid) return;
    loading.value = true;
    try {
      await propertyApi.createProperty({ ...createForm });
      ElMessage.success('创建成功');
      createDialogVisible.value = false;
      pagination.pageNumber = 1;
      await loadData();
    } catch (error) {
      ElMessage.error(getErrorMessage(error, '创建房源失败'));
    } finally {
      loading.value = false;
    }
  });
};

const submitEdit = () => {
  if (!editFormRef.value) return;
  editFormRef.value.validate(async (valid) => {
    if (!valid) return;
    const id = editForm.propertyId ?? null;
    if (id === null || id === undefined) {
      ElMessage.error('缺少房源ID，无法提交修改');
      return;
    }
    const { propertyId, ...rest } = editForm;
    const payload = { ...rest, availableDate: toBackendDateTime(editForm.availableDate) };
    loading.value = true;
    try {
      await propertyApi.updateProperty(id, payload);
      ElMessage.success('修改成功');
      editDialogVisible.value = false;
      await loadData();
    } catch (error) {
      ElMessage.error(getErrorMessage(error, '修改失败'));
    } finally {
      loading.value = false;
    }
  });
};

const handleView = (row) => {
  detailData.value = { ...row };
  detailVisible.value = true;
};

const fetchSelectorData = async () => {
  selectorState.loading = true;
  try {
    const res = await listUserSelector({ ...selectorState.query });
    selectorState.data = res?.items ?? [];
    selectorState.total = res?.total ?? 0;
    refreshSelectedRowAfterFetch();
  } catch (error) {
    ElMessage.error(getErrorMessage(error, '获取审核员列表失败'));
  } finally {
    selectorState.loading = false;
  }
};

const openSelector = (context) => {
  selectorContext.value = context;
  if (context === 'create') {
    selectorSelectedId.value = createForm.approvedByUser ?? null;
  } else {
    selectorSelectedId.value = editForm.approvedByUser ?? null;
  }
  selectorSelectedRow.value = null;
  selectorState.query.keyword = '';
  selectorState.query.searchType = 'UserName';
  selectorState.query.pageNumber = 1;
  fetchSelectorData();
  selectorState.visible = true;
};

const buildApproverLabel = (row) => {
  const name = row.realName || row.userName || `用户${row.userId}`;
  const roles = row.roleNames && row.roleNames.length ? `[${row.roleNames.join(', ')}]` : '';
  return roles ? `${name} ${roles}` : name;
};

const applySelection = (row) => {
  if (!row) return;
  if (selectorContext.value === 'create') {
    createForm.approvedByUser = row.userId;
    createApproverDisplay.value = buildApproverLabel(row);
    createFormRef.value?.clearValidate('approvedByUser');
  } else {
    editForm.approvedByUser = row.userId;
    editApproverDisplay.value = buildApproverLabel(row);
    editFormRef.value?.clearValidate('approvedByUser');
  }
  approverLabelCache[row.userId] = buildApproverLabel(row);
  selectorState.visible = false;
  ElMessage.success('已指定审核员');
};

const handleSelect = (row) => {
  selectorSelectedId.value = row.userId;
  selectorSelectedRow.value = row;
  applySelection(row);
};

const handleRadioChange = (row, val) => {
  selectorSelectedId.value = val ?? row.userId;
  selectorSelectedRow.value = row;
};

const confirmSelector = () => {
  if (!selectorSelectedRow.value) {
    ElMessage.warning('请先选择审核员');
    return;
  }
  applySelection(selectorSelectedRow.value);
};

const handleSelectorPageChange = (page) => {
  selectorState.query.pageNumber = page;
  fetchSelectorData();
};

const handleSelectorSearch = () => {
  selectorState.query.pageNumber = 1;
  fetchSelectorData();
};

const handleSelectorReset = () => {
  selectorState.query.keyword = '';
  selectorState.query.searchType = 'UserName';
  selectorState.query.pageNumber = 1;
  fetchSelectorData();
};

const refreshSelectedRowAfterFetch = () => {
  if (selectorSelectedId.value == null) return;
  const found = selectorState.data.find((r) => r.userId === selectorSelectedId.value);
  if (found) {
    selectorSelectedRow.value = found;
    const label = buildApproverLabel(found);
    approverLabelCache[found.userId] = label;
    if (selectorContext.value === 'edit') {
      editApproverDisplay.value = label;
    } else {
      createApproverDisplay.value = label;
    }
  }
};

const ensureApproverLabel = async (userId, context) => {
  if (!userId) return;
  if (approverLabelCache[userId]) {
    if (context === 'edit') {
      editApproverDisplay.value = approverLabelCache[userId];
    } else {
      createApproverDisplay.value = approverLabelCache[userId];
    }
    return;
  }
  try {
    const res = await listUserSelector({ pageNumber: 1, pageSize: 1, userId });
    const item = res?.items?.[0];
    if (item) {
      const label = buildApproverLabel(item);
      approverLabelCache[userId] = label;
      if (context === 'edit') {
        editApproverDisplay.value = label;
      } else {
        createApproverDisplay.value = label;
      }
    }
  } catch {
    /* 忽略单次回填失败 */
  }
};

onMounted(loadData);
</script>

<template>
  <div class="property-page">
    <el-card class="search-card" shadow="hover">
      <template #header>
        <div class="card-header">
          <span>房源筛选</span>
        </div>
      </template>

      <el-form :inline="true" :model="searchForm" label-width="80px" class="search-form">
        <el-form-item label="关键字">
          <el-input
            v-model="searchForm.keyword"
            clearable
            placeholder="房源名 / 地址 / 编码"
            style="min-width: 260px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="状态" class="form-item-small">
          <el-select v-model="searchForm.status" clearable placeholder="全部" style="width: 140px">
            <el-option v-for="(meta, key) in statusMap" :key="key" :label="meta.label" :value="Number(key)" />
          </el-select>
        </el-form-item>
        <el-form-item label="区域" class="form-item-small">
          <el-input v-model="searchForm.regionId" clearable placeholder="区域ID" style="width: 140px" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" :icon="Search" @click="handleSearch">查询</el-button>
          <el-button :icon="Refresh" @click="resetForm">重置</el-button>
          <el-button type="text" :icon="Filter" @click="toggleAdvanced">
            {{ isAdvancedSearch ? '收起高级筛选' : '高级筛选' }}
          </el-button>
        </el-form-item>
      </el-form>

      <div v-show="isAdvancedSearch" class="advanced-area">
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="租金范围">
              <el-space>
                <el-input-number v-model="searchForm.minRentPrice" :controls="false" placeholder="最小" />
                <span class="range-divider">-</span>
                <el-input-number v-model="searchForm.maxRentPrice" :controls="false" placeholder="最大" />
              </el-space>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="面积范围">
              <el-space>
                <el-input-number v-model="searchForm.minArea" :controls="false" placeholder="最小" />
                <span class="range-divider">-</span>
                <el-input-number v-model="searchForm.maxArea" :controls="false" placeholder="最大" />
              </el-space>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="租赁方式">
              <el-select v-model="searchForm.leaseType" clearable placeholder="全部" style="width: 140px">
                <el-option label="整租" :value="1" />
                <el-option label="合租" :value="2" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="租期类型">
              <el-select v-model="searchForm.leaseTerm" clearable placeholder="全部" style="width: 140px">
                <el-option label="月租" :value="0" />
                <el-option label="季租" :value="1" />
                <el-option label="半年" :value="2" />
                <el-option label="年租" :value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="创建时间">
              <el-date-picker
                v-model="dateRanges.created"
                type="daterange"
                unlink-panels
                start-placeholder="开始日期"
                end-placeholder="结束日期"
                style="width: 100%;"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="审核时间">
              <el-date-picker
                v-model="dateRanges.approved"
                type="daterange"
                unlink-panels
                start-placeholder="开始日期"
                end-placeholder="结束日期"
                style="width: 100%;"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="卧室数量">
              <el-space>
                <el-input-number v-model="searchForm.minBedrooms" :controls="false" placeholder="最小" />
                <span class="range-divider">-</span>
                <el-input-number v-model="searchForm.maxBedrooms" :controls="false" placeholder="最大" />
              </el-space>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="卫生间数量">
              <el-space>
                <el-input-number v-model="searchForm.minBathrooms" :controls="false" placeholder="最小" />
                <span class="range-divider">-</span>
                <el-input-number v-model="searchForm.maxBathrooms" :controls="false" placeholder="最大" />
              </el-space>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="可入住时间">
              <el-date-picker
                v-model="dateRanges.available"
                type="daterange"
                unlink-panels
                start-placeholder="开始日期"
                end-placeholder="结束日期"
                style="width: 100%;"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="是否包含已删除">
              <el-select v-model="searchForm.isDeleted" clearable placeholder="未选择" style="width: 140px">
                <el-option label="未删除" :value="false" />
                <el-option label="已删除" :value="true" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="审核员ID">
              <el-input v-model="searchForm.approvedByUser" clearable placeholder="输入用户ID" />
            </el-form-item>
          </el-col>
        </el-row>
      </div>
    </el-card>

    <el-card class="table-card" shadow="hover">
      <template #header>
        <div class="card-header">
          <span>房源列表</span>
          <div class="card-actions">
            <el-button type="primary" :icon="Plus" @click="handleCreate">新增房源</el-button>
          </div>
        </div>
      </template>


        <el-table :data="tableData" v-loading="loading" border stripe>
        <el-table-column prop="propertyId" label="房源ID" width="110" />
        <el-table-column prop="propertyName" label="房源名称" min-width="160" show-overflow-tooltip />
        <el-table-column prop="propertyCode" label="编码" min-width="140" show-overflow-tooltip />
        <el-table-column prop="regionId" label="区域" width="100" />
        <el-table-column prop="address" label="地址" min-width="200" show-overflow-tooltip />
        <el-table-column label="租金" width="120">
          <template #default="{ row }">
            {{ formatPrice(row.rentPrice) }}
          </template>
        </el-table-column>
        <el-table-column label="面积" width="120">
          <template #default="{ row }">
            {{ formatArea(row.area) }}
          </template>
        </el-table-column>
        <el-table-column label="户型" width="140">
          <template #default="{ row }">
            {{ formatRooms(row) }}
          </template>
        </el-table-column>
        <el-table-column label="租赁方式" width="120">
          <template #default="{ row }">
            {{ leaseTypeText(row.leaseType) }}
          </template>
        </el-table-column>
        <el-table-column label="租期" width="120">
          <template #default="{ row }">
            {{ leaseTermText(row.leaseTerm) }}
          </template>
        </el-table-column>
        <el-table-column prop="availableDate" label="可入住" min-width="160" show-overflow-tooltip>
          <template #default="{ row }">
            {{ formatDateTime(row.availableDate) }}
          </template>
        </el-table-column>
        <el-table-column label="创建于" min-width="170" show-overflow-tooltip>
          <template #default="{ row }">
            {{ formatDateTime(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="更新于" min-width="170" show-overflow-tooltip>
          <template #default="{ row }">
            {{ formatDateTime(row.updatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="审核于" min-width="170" show-overflow-tooltip>
          <template #default="{ row }">
            {{ formatDateTime(row.approvedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="状态" width="120">
          <template #default="{ row }">
            <el-tag :type="statusType(row.status)" effect="plain">
              {{ statusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" fixed="right" width="180">
          <template #default="{ row }">
            <el-space size="small">
              <el-button type="primary" link :icon="View" @click="handleView(row)">查看详情</el-button>
              <el-button type="primary" link :icon="Edit" @click="openEdit(row)">修改</el-button>
            </el-space>
          </template>
        </el-table-column>
        </el-table>

      <div class="pagination">
        <el-pagination
          v-model:current-page="pagination.pageNumber"
          v-model:page-size="pagination.pageSize"
          :total="pagination.total"
          background
          layout="total, sizes, prev, pager, next, jumper"
          :page-sizes="[10, 20, 50]"
          @current-change="handlePageChange"
          @size-change="handleSizeChange"
        />
      </div>
    </el-card>

    <el-drawer v-model="detailVisible" :with-header="false" size="46%" destroy-on-close class="detail-drawer">
      <div class="detail-shell">
        <div class="detail-hero">
          <div>
            <div class="hero-eyebrow">ID {{ detailData.propertyId ?? '--' }}</div>
            <div class="hero-title">{{ detailData.propertyName || '未命名房源' }}</div>
            <div class="hero-sub">
              <el-tag size="small" :type="statusType(detailData.status)" effect="dark">
                {{ statusText(detailData.status) }}
              </el-tag>
              <span class="hero-code">编码：{{ detailData.propertyCode || '--' }}</span>
              <span class="hero-code">房间：{{ detailData.roomNumber || '--' }}</span>
            </div>
          </div>
          <div class="hero-stats">
            <div class="stat">
              <div class="stat-value">{{ formatArea(detailData.area) }}</div>
              <div class="stat-label">面积</div>
            </div>
            <div class="stat">
              <div class="stat-value">{{ formatRooms(detailData) }}</div>
              <div class="stat-label">户型</div>
            </div>
            <div class="stat">
              <div class="stat-value">{{ leaseTypeText(detailData.leaseType) }}</div>
              <div class="stat-label">租赁方式</div>
            </div>
          </div>
        </div>

        <div class="detail-main">
          <el-card shadow="never" class="detail-card">
            <div class="card-title">价格 · 租期</div>
            <div class="meta-grid">
              <div class="meta-item">
                <div class="meta-label">租金</div>
                <div class="meta-value">{{ formatPrice(detailData.rentPrice) }}</div>
              </div>
              <div class="meta-item">
                <div class="meta-label">押金</div>
                <div class="meta-value">{{ detailData.rentDeposit ?? '--' }}</div>
              </div>
              <div class="meta-item">
                <div class="meta-label">物业费</div>
                <div class="meta-value">{{ detailData.propertyFee ?? '--' }}</div>
              </div>
              <div class="meta-item">
                <div class="meta-label">租期类型</div>
                <div class="meta-value">{{ leaseTermText(detailData.leaseTerm) }}</div>
              </div>
              <div class="meta-item">
                <div class="meta-label">可入住时间</div>
                <div class="meta-value">{{ formatDateTime(detailData.availableDate) }}</div>
              </div>
            </div>
          </el-card>

          <el-card shadow="never" class="detail-card">
            <div class="card-title">位置 · 审核</div>
            <div class="meta-grid">
              <div class="meta-item meta-span-2">
                <div class="meta-label">地址</div>
                <div class="meta-value">{{ detailData.address || '--' }}</div>
              </div>
              <div class="meta-item">
                <div class="meta-label">区域 ID</div>
                <div class="meta-value">{{ detailData.regionId ?? '--' }}</div>
              </div>
              <div class="meta-item">
                <div class="meta-label">审核员</div>
                <div class="meta-value">{{ detailData.realName || '--' }}</div>
              </div>
              <div class="meta-item">
                <div class="meta-label">经纬度</div>
                <div class="meta-value">
                  {{ detailData.latitude ?? '--' }}, {{ detailData.longitude ?? '--' }}
                </div>
              </div>
            </div>
          </el-card>

          <el-card shadow="never" class="detail-card">
            <div class="card-title">描述</div>
            <div class="description-block">
              <p>{{ detailData.description || '暂无描述' }}</p>
            </div>
          </el-card>

          <el-card shadow="never" class="detail-card compact">
            <div class="card-title">时间线</div>
            <div class="timeline">
              <div class="timeline-item">
                <span class="timeline-label">创建</span>
                <span class="timeline-value">{{ formatDateTime(detailData.createdAt) }}</span>
              </div>
              <div class="timeline-item">
                <span class="timeline-label">更新</span>
                <span class="timeline-value">{{ formatDateTime(detailData.updatedAt) }}</span>
              </div>
              <div class="timeline-item">
                <span class="timeline-label">审核</span>
                <span class="timeline-value">{{ formatDateTime(detailData.approvedAt) }}</span>
              </div>
            </div>
          </el-card>
        </div>
      </div>
    </el-drawer>

    <el-drawer
      v-model="createDialogVisible"
      direction="rtl"
      size="68%"
      :with-header="false"
      destroy-on-close
      class="create-drawer"
    >
      <div class="drawer-header">
        <div>
          <h3>新增房源</h3>
          <p>完整填写基础、户型、费用与坐标信息，便于后续审核与筛选。</p>
        </div>
        <el-button circle plain size="large" :icon="Close" @click="createDialogVisible = false" />
      </div>
      <div class="drawer-body">
        <el-form ref="createFormRef" :model="createForm" :rules="createRules" label-width="110px" class="create-form">
          <div class="form-section">
            <div class="section-title">基础信息</div>
            <div class="field-grid">
              <el-form-item class="grid-item" label="房源名称" prop="propertyName">
                <el-input v-model="createForm.propertyName" placeholder="请输入房源名称" />
              </el-form-item>
              <el-form-item class="grid-item" label="房源编码" prop="propertyCode">
                <el-input v-model="createForm.propertyCode" placeholder="例如 GZ-TNH-0502" />
              </el-form-item>
              <el-form-item class="grid-item" label="房间编号" prop="roomNumber">
                <el-input v-model="createForm.roomNumber" placeholder="如 502-A" />
              </el-form-item>
              <el-form-item class="grid-item" label="区域ID" prop="regionId">
                <el-input-number v-model="createForm.regionId" :controls="false" placeholder="可选填" style="width: 100%;" />
              </el-form-item>
              <el-form-item class="grid-item grid-item--full" label="地址" prop="address">
                <el-input v-model="createForm.address" placeholder="请输入具体地址" />
              </el-form-item>
              <el-form-item class="grid-item grid-item--full" label="描述" prop="description">
                <el-input
                  v-model="createForm.description"
                  type="textarea"
                  :rows="2"
                  placeholder="可补充亮点、装修、周边配套"
                />
              </el-form-item>
            </div>
          </div>

          <div class="form-section">
            <div class="section-title">户型与面积</div>
            <div class="field-grid">
              <el-form-item class="grid-item" label="面积" prop="area">
                <div class="input-with-unit">
                  <el-input-number v-model="createForm.area" :controls="false" placeholder="面积" style="width: 100%;" />
                  <span class="unit">㎡</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="卧室数量" prop="bedrooms">
                <div class="input-with-unit">
                  <el-input-number v-model="createForm.bedrooms" :controls="false" placeholder="卧室" style="width: 100%;" />
                  <span class="unit">间</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="卫生间" prop="bathrooms">
                <div class="input-with-unit">
                  <el-input-number v-model="createForm.bathrooms" :controls="false" placeholder="卫生间" style="width: 100%;" />
                  <span class="unit">间</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="最大入住" prop="maxTenants">
                <div class="input-with-unit">
                  <el-input-number
                    v-model="createForm.maxTenants"
                    :controls="false"
                    placeholder="人数"
                    style="width: 100%;"
                  />
                  <span class="unit">人</span>
                </div>
              </el-form-item>
            </div>
          </div>

          <div class="form-section">
            <div class="section-title">价格与费用</div>
            <div class="field-grid">
              <el-form-item class="grid-item" label="租金" prop="rentPrice">
                <div class="input-with-unit">
                  <el-input-number
                    v-model="createForm.rentPrice"
                    :controls="false"
                    :precision="2"
                    :step="100"
                    placeholder="租金"
                    style="width: 100%;"
                  />
                  <span class="unit">元/月</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="押金" prop="rentDeposit">
                <div class="input-with-unit">
                  <el-input-number
                    v-model="createForm.rentDeposit"
                    :controls="false"
                    :precision="2"
                    :step="100"
                    placeholder="押金"
                    style="width: 100%;"
                  />
                  <span class="unit">元</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="物业费" prop="propertyFee">
                <div class="input-with-unit">
                  <el-input-number
                    v-model="createForm.propertyFee"
                    :controls="false"
                    :precision="2"
                    :step="10"
                    placeholder="物业费"
                    style="width: 100%;"
                  />
                  <span class="unit">元</span>
                </div>
              </el-form-item>
            </div>
          </div>

          <div class="form-section">
            <div class="section-title">坐标与租赁</div>
            <div class="field-grid">
              <el-form-item class="grid-item" label="纬度" prop="latitude">
                <el-input-number
                  v-model="createForm.latitude"
                  :controls="false"
                  :precision="6"
                  placeholder="纬度 -90 ~ 90"
                  style="width: 100%;"
                />
              </el-form-item>
              <el-form-item class="grid-item" label="经度" prop="longitude">
                <el-input-number
                  v-model="createForm.longitude"
                  :controls="false"
                  :precision="6"
                  placeholder="经度 -180 ~ 180"
                  style="width: 100%;"
                />
              </el-form-item>
              <el-form-item class="grid-item" label="租赁方式" prop="leaseType">
                <el-select v-model="createForm.leaseType" placeholder="请选择" style="width: 100%;">
                  <el-option label="整租" :value="1" />
                  <el-option label="合租" :value="2" />
                </el-select>
              </el-form-item>
              <el-form-item class="grid-item" label="租期类型" prop="leaseTerm">
                <el-select v-model="createForm.leaseTerm" placeholder="请选择" style="width: 100%;">
                  <el-option label="月租" :value="0" />
                  <el-option label="季租" :value="1" />
                  <el-option label="半年" :value="2" />
                  <el-option label="年租" :value="3" />
                </el-select>
              </el-form-item>
              <el-form-item class="grid-item" label="审核员" prop="approvedByUser">
                <div class="selector-trigger" @click="openSelector('create')">
                  <el-input
                    v-model="createApproverDisplay"
                    readonly
                    placeholder="请指派审核员（必填）"
                    :suffix-icon="Search"
                  />
                  <div class="click-mask"></div>
                </div>
                <input type="hidden" :value="createForm.approvedByUser">
              </el-form-item>
            </div>
          </div>
        </el-form>
      </div>
      <div class="drawer-footer">
        <div class="footer-tip">
          提示：提交后自动进入待审核，可在房源列表中查看状态。
        </div>
        <div class="footer-actions">
          <el-button class="plain-btn" @click="createDialogVisible = false">取消</el-button>
          <el-button type="primary" class="gradient-btn" @click="submitCreate">确认创建</el-button>
        </div>
      </div>
    </el-drawer>

    <el-drawer
      v-model="editDialogVisible"
      direction="rtl"
      size="68%"
      :with-header="false"
      destroy-on-close
      class="create-drawer"
    >
      <div class="drawer-header">
        <div>
          <h3>修改房源</h3>
          <p>更新房源基础信息（不包含审核流程）。</p>
        </div>
        <el-button circle plain size="large" :icon="Close" @click="editDialogVisible = false" />
      </div>
      <div class="drawer-body">
        <el-form ref="editFormRef" :model="editForm" :rules="editRules" label-width="110px" class="create-form">
          <div class="form-section">
            <div class="section-title">基础信息</div>
            <div class="field-grid">
              <el-form-item class="grid-item" label="房源名称" prop="propertyName">
                <el-input v-model="editForm.propertyName" placeholder="请输入房源名称" />
              </el-form-item>
              <el-form-item class="grid-item" label="房源编码" prop="propertyCode">
                <el-input v-model="editForm.propertyCode" placeholder="例如 GZ-TNH-0502" />
              </el-form-item>
              <el-form-item class="grid-item" label="房间编号" prop="roomNumber">
                <el-input v-model="editForm.roomNumber" placeholder="如 502-A" />
              </el-form-item>
              <el-form-item class="grid-item" label="区域ID" prop="regionId">
                <el-input-number v-model="editForm.regionId" :controls="false" placeholder="可选填" style="width: 100%;" />
              </el-form-item>
              <el-form-item class="grid-item grid-item--full" label="地址" prop="address">
                <el-input v-model="editForm.address" placeholder="请输入具体地址" />
              </el-form-item>
              <el-form-item class="grid-item grid-item--full" label="描述" prop="description">
                <el-input
                  v-model="editForm.description"
                  type="textarea"
                  :rows="2"
                  placeholder="可补充亮点、装修、周边配套"
                />
              </el-form-item>
            </div>
          </div>

          <div class="form-section">
            <div class="section-title">户型与面积</div>
            <div class="field-grid">
              <el-form-item class="grid-item" label="面积" prop="area">
                <div class="input-with-unit">
                  <el-input-number v-model="editForm.area" :controls="false" placeholder="面积" style="width: 100%;" />
                  <span class="unit">㎡</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="卧室数量" prop="bedrooms">
                <div class="input-with-unit">
                  <el-input-number v-model="editForm.bedrooms" :controls="false" placeholder="卧室" style="width: 100%;" />
                  <span class="unit">间</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="卫生间" prop="bathrooms">
                <div class="input-with-unit">
                  <el-input-number v-model="editForm.bathrooms" :controls="false" placeholder="卫生间" style="width: 100%;" />
                  <span class="unit">间</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="最大入住" prop="maxTenants">
                <div class="input-with-unit">
                  <el-input-number
                    v-model="editForm.maxTenants"
                    :controls="false"
                    placeholder="人数"
                    style="width: 100%;"
                  />
                  <span class="unit">人</span>
                </div>
              </el-form-item>
            </div>
          </div>

          <div class="form-section">
            <div class="section-title">价格与费用</div>
            <div class="field-grid">
              <el-form-item class="grid-item" label="租金" prop="rentPrice">
                <div class="input-with-unit">
                  <el-input-number
                    v-model="editForm.rentPrice"
                    :controls="false"
                    :precision="2"
                    :step="100"
                    placeholder="租金"
                    style="width: 100%;"
                  />
                  <span class="unit">元/月</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="押金" prop="rentDeposit">
                <div class="input-with-unit">
                  <el-input-number
                    v-model="editForm.rentDeposit"
                    :controls="false"
                    :precision="2"
                    :step="100"
                    placeholder="押金"
                    style="width: 100%;"
                  />
                  <span class="unit">元</span>
                </div>
              </el-form-item>
              <el-form-item class="grid-item" label="物业费" prop="propertyFee">
                <div class="input-with-unit">
                  <el-input-number
                    v-model="editForm.propertyFee"
                    :controls="false"
                    :precision="2"
                    :step="10"
                    placeholder="物业费"
                    style="width: 100%;"
                  />
                  <span class="unit">元</span>
                </div>
              </el-form-item>
            </div>
          </div>

          <div class="form-section">
            <div class="section-title">坐标与租赁</div>
            <div class="field-grid">
              <el-form-item class="grid-item" label="纬度" prop="latitude">
                <el-input-number
                  v-model="editForm.latitude"
                  :controls="false"
                  :precision="6"
                  placeholder="纬度 -90 ~ 90"
                  style="width: 100%;"
                />
              </el-form-item>
              <el-form-item class="grid-item" label="经度" prop="longitude">
                <el-input-number
                  v-model="editForm.longitude"
                  :controls="false"
                  :precision="6"
                  placeholder="经度 -180 ~ 180"
                  style="width: 100%;"
                />
              </el-form-item>
              <el-form-item class="grid-item" label="租赁方式" prop="leaseType">
                <el-select v-model="editForm.leaseType" placeholder="请选择" style="width: 100%;">
                  <el-option label="整租" :value="1" />
                  <el-option label="合租" :value="2" />
                </el-select>
              </el-form-item>
              <el-form-item class="grid-item" label="租期类型" prop="leaseTerm">
                <el-select v-model="editForm.leaseTerm" placeholder="请选择" style="width: 100%;">
                  <el-option label="月租" :value="0" />
                  <el-option label="季租" :value="1" />
                  <el-option label="半年" :value="2" />
                  <el-option label="年租" :value="3" />
                </el-select>
              </el-form-item>
              <el-form-item class="grid-item" label="审核员" prop="approvedByUser">
                <div class="selector-trigger" @click="openSelector('edit')">
                  <el-input
                    v-model="editApproverDisplay"
                    readonly
                    placeholder="请指派审核员（必填）"
                    :suffix-icon="Search"
                  />
                  <div class="click-mask"></div>
                </div>
                <input type="hidden" :value="editForm.approvedByUser">
              </el-form-item>
              <el-form-item class="grid-item" label="可入住时间" prop="availableDate">
                <el-date-picker
                  v-model="editForm.availableDate"
                  type="datetime"
                  placeholder="选择日期时间"
                  format="YYYY-MM-DD HH:mm:ss"
                  value-format="YYYY-MM-DD-HH:mm:ss"
                  style="width: 100%;"
                />
              </el-form-item>
            </div>
          </div>
        </el-form>
      </div>
      <div class="drawer-footer">
        <div class="footer-tip">
          提示：仅修改表单内容，不会触发审核状态变更。
        </div>
        <div class="footer-actions">
          <el-button class="plain-btn" @click="editDialogVisible = false">取消</el-button>
          <el-button type="primary" class="gradient-btn" @click="submitEdit">确认保存</el-button>
        </div>
      </div>
    </el-drawer>

    <el-dialog
      v-model="selectorState.visible"
      title="选择审核员"
      width="720px"
      append-to-body
      align-center
      class="selector-dialog"
    >
      <div class="selector-body">
        <div class="selector-filter">
          <el-select v-model="selectorState.query.searchType" class="selector-select">
            <el-option v-for="t in searchTypes" :key="t.value" :label="t.label" :value="t.value" />
          </el-select>
          <el-input
            v-model="selectorState.query.keyword"
            class="selector-input"
            placeholder="输入关键词搜索..."
            clearable
            @keyup.enter="handleSelectorSearch"
          >
            <template #append>
              <el-button :icon="Search" @click="handleSelectorSearch" />
            </template>
          </el-input>
          <el-button :icon="Refresh" @click="handleSelectorReset">重置</el-button>
        </div>

        <div class="selector-table">
          <el-table :data="selectorState.data" v-loading="selectorState.loading" border stripe height="360px" size="small">
            <el-table-column label="" width="60" align="center">
              <template #default="{ row }">
                <el-radio v-model="selectorSelectedId" :label="row.userId" @change="(val) => handleRadioChange(row, val)" />
              </template>
            </el-table-column>
            <el-table-column label="用户" min-width="180">
              <template #default="{ row }">
                <div class="user-cell-name">{{ row.realName }}</div>
                <div class="user-cell-username">@{{ row.userName }}</div>
                <div class="user-cell-phone" v-if="row.phone">{{ row.phone }}</div>
              </template>
            </el-table-column>
            <el-table-column label="角色" min-width="220">
              <template #default="{ row }">
                <el-space wrap :size="4">
                  <el-tag
                    v-for="role in row.roleNames"
                    :key="role"
                    size="small"
                    type="info"
                    effect="plain"
                  >
                    {{ role }}
                  </el-tag>
                </el-space>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="90" align="center" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" link @click="handleSelect(row)">选择</el-button>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <div class="selector-footer">
          <el-pagination
            v-model:current-page="selectorState.query.pageNumber"
            :page-size="selectorState.query.pageSize"
            :total="selectorState.total"
            layout="total, prev, pager, next"
            small
            background
            @current-change="handleSelectorPageChange"
          />
          <div class="selector-actions">
            <el-button @click="selectorState.visible = false">取消</el-button>
            <el-button type="primary" @click="confirmSelector">确认</el-button>
          </div>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<style scoped>
.property-page {
  display: flex;
  flex-direction: column;
  gap: 16px;
  --accent: #1f7bfd;
  --card-soft: #f8fbff;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.card-actions {
  display: flex;
  gap: 8px;
}
.search-form {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}
.advanced-area {
  margin-top: 8px;
  padding-top: 10px;
  border-top: 1px dashed #e5e6eb;
}
.form-item-small {
  margin-right: 0;
}
.range-divider {
  color: #999;
  padding: 0 4px;
}
.table-card {
  min-height: 400px;
}

/* ✅ 新增/保留这些样式 */
.table-card {
  min-height: 400px;
  /* 确保卡片本身不会限制表格的高度伸展，或者按需设置 flex */
  display: flex;
  flex-direction: column;
}


:deep(.el-table__body-wrapper::-webkit-scrollbar-thumb) {
  background-color: #dcdfe6;
  border-radius: 5px;
}

.pagination {
  margin-top: 12px;
  display: flex;
  justify-content: flex-end;
}
.create-drawer :deep(.el-drawer__body) {
  display: flex;
  flex-direction: column;
  height: 100%;
  padding: 0;
  background: #f4f6fb;
}
.drawer-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 28px 12px;
  border-bottom: 1px solid #e5e8f0;
}
.drawer-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #1f2d3d;
}
.drawer-header p {
  margin: 6px 0 0;
  color: #6b7280;
  font-size: 13px;
}
.drawer-body {
  flex: 1;
  overflow-y: auto;
  padding: 18px 32px 90px;
}
.drawer-footer {
  position: sticky;
  bottom: 0;
  width: 100%;
  background: #fff;
  border-top: 1px solid #e5e8f0;
  padding: 16px 28px;
  box-shadow: 0 -4px 14px rgba(17, 24, 39, 0.08);
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.footer-tip {
  font-size: 12px;
  color: #8a8f9d;
}
.footer-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
.selector-dialog :deep(.el-dialog__body) {
  padding: 10px 12px 14px;
}
.selector-body {
  background: linear-gradient(135deg, #f9fbff 0%, #f3f6fb 100%);
  border: 1px solid #e5edff;
  border-radius: 12px;
  padding: 12px;
  box-shadow: 0 6px 20px rgba(31, 123, 253, 0.08);
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.selector-trigger {
  position: relative;
  width: 100%;
  cursor: pointer;
}
.selector-trigger :deep(.el-input__wrapper) {
  cursor: pointer;
}
.click-mask {
  position: absolute;
  inset: 0;
  z-index: 1;
  cursor: pointer;
}
.selector-filter {
  display: flex;
  gap: 8px;
  align-items: center;
  margin-bottom: 10px;
  background: #fff;
  border: 1px solid #e8ecf5;
  border-radius: 10px;
  padding: 10px;
  flex-wrap: nowrap;
}
.selector-filter :deep(.el-input__wrapper) {
  box-shadow: none;
  border-radius: 8px;
  border: 1px solid #dcdfe6;
}
.selector-filter :deep(.el-input-group__append) {
  padding: 0;
  background: #f5f7fa;
  border-left: 1px solid #dcdfe6;
  border-top-left-radius: 8px;
  border-bottom-left-radius: 8px;
  border-top-right-radius: 8px;
  border-bottom-right-radius: 8px;
}
.selector-filter :deep(.el-input-group__append .el-button) {
  border: none;
  box-shadow: none;
  background: transparent;
  height: 100%;
  margin: 0;
  border-top-left-radius: 8px;
  border-bottom-left-radius: 8px;
  border-top-right-radius: 8px;
  border-bottom-right-radius: 8px;
}
.selector-select {
  flex: 0 0 130px;
}
.selector-input {
  flex: 1;
  min-width: 200px;
}
.selector-table {
  border: 1px solid #e5e8f0;
  border-radius: 10px;
  overflow: hidden;
  background: #fff;
}
.selector-dialog :deep(.el-table th) {
  background: #f7f9fc;
  color: #1f2d3d;
  font-weight: 600;
}
.selector-dialog :deep(.el-table__body-wrapper) {
  background: #fff;
}
.selector-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 12px;
  gap: 10px;
}
.selector-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}
.user-cell-name {
  font-weight: 600;
}
.user-cell-username {
  font-size: 12px;
  color: #666;
}
.user-cell-phone {
  font-size: 12px;
  color: #999;
}
.create-form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}
.create-form :deep(.el-form-item) {
  margin-bottom: 24px;
}
.form-section {
  background: #fff;
  border: 1px solid #e5edff;
  border-radius: 12px;
  padding: 14px 16px;
  box-shadow: 0 6px 18px rgba(31, 123, 253, 0.08);
}
.section-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  color: #1f2d3d;
  margin-bottom: 10px;
}
.section-title::before {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--accent);
  box-shadow: 0 0 0 6px rgba(31, 123, 253, 0.14);
}
.input-with-unit {
  display: flex;
  align-items: center;
  gap: 4px;
  width: 100%;
  padding: 4px 8px;
  border: 1px solid #e5e8f0;
  border-radius: 10px;
  background: #fff;
}
.input-with-unit :deep(.el-input-number) {
  flex: 1;
  min-width: 0;
}
.input-with-unit :deep(.el-input-number__decrease),
.input-with-unit :deep(.el-input-number__increase) {
  display: none;
}
.unit {
  font-size: 12px;
  color: #8a8f9d;
  padding-left: 4px;
  white-space: nowrap;
}
.unit::before {
  content: '/';
  margin-right: 2px;
  color: #c0c4d6;
}
.field-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 12px 20px;
}
.field-grid :deep(.el-form-item) {
  margin-bottom: 10px;
}
.field-grid :deep(.el-form-item__content) {
  display: flex;
  flex-direction: column;
  align-items: stretch;
  gap: 10px;
}
.grid-item--full {
  grid-column: 1 / -1;
}
.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
.plain-btn {
  color: #606266;
  border-color: #e4e7ed;
}
.gradient-btn {
  background-image: linear-gradient(90deg, #1f7bfd 0%, #3b9dff 100%);
  border: none;
  box-shadow: 0 8px 20px rgba(31, 123, 253, 0.25);
}
.gradient-btn:hover {
  filter: brightness(1.05);
}
.create-drawer :deep(.el-form-item__error) {
  margin-top: 4px;
  padding-left: 0;
  font-size: 12px;
  color: #f56c6c;
  line-height: 1.4;
}
.detail-drawer :deep(.el-drawer__body) {
  padding: 0;
  background: linear-gradient(180deg, #f4f7ff 0%, #ffffff 26%);
}
.detail-shell {
  padding: 18px 18px 24px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.detail-hero {
  background: linear-gradient(135deg, #1f7bfd 0%, #4bc8f2 100%);
  color: #fff;
  border-radius: 14px;
  padding: 18px 20px;
  display: flex;
  justify-content: space-between;
  gap: 12px;
  box-shadow: 0 10px 30px rgba(31, 123, 253, 0.25);
}
.hero-eyebrow {
  opacity: 0.92;
  font-size: 13px;
  letter-spacing: 0.3px;
}
.hero-title {
  font-size: 22px;
  font-weight: 700;
  margin: 6px 0 4px;
}
.hero-sub {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
}
.hero-code {
  font-size: 13px;
  opacity: 0.9;
}
.hero-stats {
  display: flex;
  align-items: center;
  gap: 12px;
}
.stat {
  background: rgba(255, 255, 255, 0.18);
  border-radius: 12px;
  padding: 12px 14px;
  min-width: 90px;
  text-align: right;
}
.stat-value {
  font-size: 16px;
  font-weight: 700;
}
.stat-label {
  font-size: 12px;
  opacity: 0.9;
}
.detail-main {
  display: grid;
  grid-template-columns: 1fr;
  gap: 12px;
}
.detail-card {
  border-radius: 12px;
}
.detail-card :deep(.el-card__body) {
  padding: 14px 16px;
}
.card-title {
  font-weight: 700;
  margin-bottom: 10px;
  color: #1f2d3d;
}
.meta-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 12px 16px;
}
.meta-item {
  padding: 10px 12px;
  border: 1px dashed #e5e8f0;
  border-radius: 10px;
  background: #f9fbff;
}
.meta-item.meta-span-2 {
  grid-column: span 2;
}
.meta-label {
  font-size: 12px;
  color: #6b7280;
  margin-bottom: 4px;
}
.meta-value {
  font-weight: 600;
  color: #1f2937;
  word-break: break-word;
}
.description-block {
  background: #f9fafb;
  border-radius: 10px;
  padding: 10px 12px;
  color: #374151;
  line-height: 1.5;
}
.timeline {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.timeline-item {
  display: flex;
  justify-content: space-between;
  padding: 10px 12px;
  border: 1px solid #edf1f7;
  border-radius: 10px;
  background: #fbfcff;
}
.timeline-label {
  color: #6b7280;
}
.timeline-value {
  font-weight: 600;
  color: #1f2937;
}
.detail-card.compact .timeline {
  gap: 6px;
}

@media (max-width: 992px) {
  .create-drawer :deep(.el-col-12),
  .create-drawer :deep(.el-col-8) {
    max-width: 100%;
    flex: 0 0 100%;
  }
  .create-drawer :deep(.el-form-item__label) {
    white-space: nowrap;
  }
  .detail-hero {
    flex-direction: column;
    align-items: flex-start;
  }
  .hero-stats {
    width: 100%;
    justify-content: space-between;
  }
}

</style>
