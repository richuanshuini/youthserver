<script setup>
defineOptions({ name: 'PropertyIndexPage' });
import { ref, reactive, onMounted, computed } from 'vue';
import { ElMessage } from 'element-plus';
import { Search, Refresh, Filter, Plus, View } from '@element-plus/icons-vue';
import { propertyApi } from '../services';

const loading = ref(false);
const isAdvancedSearch = ref(false);
const tableData = ref([]);
const pagination = reactive({ pageNumber: 1, pageSize: 10, total: 0 });

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

const createRules = {
  approvedByUser: [{ required: true, message: '请输入审核员ID', trigger: 'blur' }],
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
  leaseType: [{ required: true, message: '请选择租赁类型', trigger: 'change' }],
  leaseTerm: [{ required: true, message: '请选择租期类型', trigger: 'change' }],
};

const statusText = (val) => statusMap?.[val]?.label ?? '未知';
const statusType = (val) => statusMap?.[val]?.type ?? 'info';
const leaseTypeText = (val) => leaseTypeMap?.[val] ?? '--';
const leaseTermText = (val) => leaseTermMap?.[val] ?? '--';

const toggleAdvanced = () => (isAdvancedSearch.value = !isAdvancedSearch.value);

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
const formatRooms = (row) => `${row?.bedrooms ?? '--'}室/${row?.bathrooms ?? '--'}卫`;

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
  createDialogVisible.value = true;
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

const handleView = (row) => {
  detailData.value = { ...row };
  detailVisible.value = true;
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
            style="min-width: 200px"
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
            <el-form-item label="租赁类型">
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
              <el-date-picker v-model="dateRanges.created" type="daterange" unlink-panels start-placeholder="开始日期" end-placeholder="结束日期" style="width: 100%;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="审核时间">
              <el-date-picker v-model="dateRanges.approved" type="daterange" unlink-panels start-placeholder="开始日期" end-placeholder="结束日期" style="width: 100%;" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="卧室数量">
              <el-space>
                <el-input-number v-model="searchForm.minBedrooms" :controls="false" placeholder="最少" />
                <span class="range-divider">-</span>
                <el-input-number v-model="searchForm.maxBedrooms" :controls="false" placeholder="最多" />
              </el-space>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="卫生间数量">
              <el-space>
                <el-input-number v-model="searchForm.minBathrooms" :controls="false" placeholder="最少" />
                <span class="range-divider">-</span>
                <el-input-number v-model="searchForm.maxBathrooms" :controls="false" placeholder="最多" />
              </el-space>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="可入住时间">
              <el-date-picker v-model="dateRanges.available" type="daterange" unlink-panels start-placeholder="开始日期" end-placeholder="结束日期" style="width: 100%;" />
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
        <el-table-column type="index" label="#" width="60" />
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
        <el-table-column label="租赁类型" width="120">
          <template #default="{ row }">
            {{ leaseTypeText(row.leaseType) }}
          </template>
        </el-table-column>
        <el-table-column label="租期" width="120">
          <template #default="{ row }">
            {{ leaseTermText(row.leaseTerm) }}
          </template>
        </el-table-column>
        <el-table-column prop="availableDate" label="可入住" min-width="140" show-overflow-tooltip />
        <el-table-column label="状态" width="120">
          <template #default="{ row }">
            <el-tag :type="statusType(row.status)" effect="plain">
              {{ statusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" fixed="right" width="120">
          <template #default="{ row }">
            <el-button type="primary" link :icon="View" @click="handleView(row)">查看详情</el-button>
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

    <el-drawer v-model="detailVisible" :title="detailTitle" size="45%" destroy-on-close>
      <div class="detail-grid">
        <section>
          <h4>基础信息</h4>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="房源名称">{{ detailData.propertyName || '--' }}</el-descriptions-item>
            <el-descriptions-item label="房源编码">{{ detailData.propertyCode || '--' }}</el-descriptions-item>
            <el-descriptions-item label="房间编号">{{ detailData.roomNumber || '--' }}</el-descriptions-item>
            <el-descriptions-item label="区域ID">{{ detailData.regionId ?? '--' }}</el-descriptions-item>
            <el-descriptions-item label="状态">
              <el-tag :type="statusType(detailData.status)" effect="plain">{{ statusText(detailData.status) }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="审核员ID">{{ detailData.approvedByUser ?? '--' }}</el-descriptions-item>
          </el-descriptions>
        </section>

        <section>
          <h4>租赁与价格</h4>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="租金">{{ formatPrice(detailData.rentPrice) }}</el-descriptions-item>
            <el-descriptions-item label="押金">{{ detailData.rentDeposit ?? '--' }}</el-descriptions-item>
            <el-descriptions-item label="物业费">{{ detailData.propertyFee ?? '--' }}</el-descriptions-item>
            <el-descriptions-item label="租赁类型">{{ leaseTypeText(detailData.leaseType) }}</el-descriptions-item>
            <el-descriptions-item label="租期">{{ leaseTermText(detailData.leaseTerm) }}</el-descriptions-item>
            <el-descriptions-item label="可入住">{{ detailData.availableDate || '--' }}</el-descriptions-item>
          </el-descriptions>
        </section>

        <section>
          <h4>户型与面积</h4>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="面积">{{ formatArea(detailData.area) }}</el-descriptions-item>
            <el-descriptions-item label="户型">{{ formatRooms(detailData) }}</el-descriptions-item>
            <el-descriptions-item label="人数上限">{{ detailData.maxTenants ?? '--' }}</el-descriptions-item>
            <el-descriptions-item label="经纬度">
              {{ detailData.latitude ?? '--' }} , {{ detailData.longitude ?? '--' }}
            </el-descriptions-item>
          </el-descriptions>
        </section>

        <section>
          <h4>位置信息</h4>
          <el-descriptions :column="1" border>
            <el-descriptions-item label="地址">{{ detailData.address || '--' }}</el-descriptions-item>
            <el-descriptions-item label="描述">{{ detailData.description || '--' }}</el-descriptions-item>
          </el-descriptions>
        </section>
      </div>
    </el-drawer>

    <el-dialog v-model="createDialogVisible" title="新增房源" width="720px">
      <el-form ref="createFormRef" :model="createForm" :rules="createRules" label-width="100px">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="房源名称" prop="propertyName">
              <el-input v-model="createForm.propertyName" placeholder="请输入房源名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="房源编码" prop="propertyCode">
              <el-input v-model="createForm.propertyCode" placeholder="例如 GZ-TNH-0502" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="房间编号" prop="roomNumber">
              <el-input v-model="createForm.roomNumber" placeholder="如 502-A" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="区域ID" prop="regionId">
              <el-input-number v-model="createForm.regionId" :controls="false" placeholder="可选" style="width: 100%;" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="24">
            <el-form-item label="地址" prop="address">
              <el-input v-model="createForm.address" placeholder="请输入地址" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="24">
            <el-form-item label="描述" prop="description">
              <el-input v-model="createForm.description" type="textarea" :rows="2" placeholder="请输入房源描述" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="面积(㎡)" prop="area">
              <el-input-number v-model="createForm.area" :controls="false" placeholder="面积" style="width: 100%;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="卧室数量" prop="bedrooms">
              <el-input-number v-model="createForm.bedrooms" :controls="false" placeholder="卧室" style="width: 100%;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="卫生间" prop="bathrooms">
              <el-input-number v-model="createForm.bathrooms" :controls="false" placeholder="卫生间" style="width: 100%;" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="最大入住" prop="maxTenants">
              <el-input-number v-model="createForm.maxTenants" :controls="false" placeholder="人数" style="width: 100%;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="租金(元/月)" prop="rentPrice">
              <el-input-number v-model="createForm.rentPrice" :controls="false" :precision="2" :step="100" placeholder="租金" style="width: 100%;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="押金" prop="rentDeposit">
              <el-input-number v-model="createForm.rentDeposit" :controls="false" :precision="2" :step="100" placeholder="押金" style="width: 100%;" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="物业费" prop="propertyFee">
              <el-input-number v-model="createForm.propertyFee" :controls="false" :precision="2" :step="10" placeholder="物业费" style="width: 100%;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="纬度" prop="latitude">
              <el-input-number v-model="createForm.latitude" :controls="false" :precision="6" placeholder="纬度 -90~90" style="width: 100%;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="经度" prop="longitude">
              <el-input-number v-model="createForm.longitude" :controls="false" :precision="6" placeholder="经度 -180~180" style="width: 100%;" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="8">
            <el-form-item label="租赁类型" prop="leaseType">
              <el-select v-model="createForm.leaseType" placeholder="请选择" style="width: 100%;">
                <el-option label="整租" :value="1" />
                <el-option label="合租" :value="2" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="租期类型" prop="leaseTerm">
              <el-select v-model="createForm.leaseTerm" placeholder="请选择" style="width: 100%;">
                <el-option label="月租" :value="0" />
                <el-option label="季租" :value="1" />
                <el-option label="半年" :value="2" />
                <el-option label="年租" :value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="审核员ID" prop="approvedByUser">
              <el-input-number v-model="createForm.approvedByUser" :controls="false" placeholder="请输入用户ID" style="width: 100%;" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button @click="createDialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="submitCreate">提 交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.property-page {
  display: flex;
  flex-direction: column;
  gap: 16px;
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
.pagination {
  margin-top: 12px;
  display: flex;
  justify-content: flex-end;
}
.detail-grid {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.detail-grid h4 {
  margin: 0 0 8px;
  font-weight: 600;
  color: #303133;
}
</style>
