<script setup>
import {ArrowDown, ArrowLeft, ArrowRight, ArrowUp} from "@element-plus/icons-vue";
defineOptions({ name: 'UserRoleIndexPage' });
import {ref, onMounted, computed, watch, nextTick} from 'vue';
import { ElMessage } from 'element-plus';
import { listUserRoles, listUsersNoRolesPaged, listRoles, assignUserRolesBatch } from '../services.js';
import {searchUsers} from "@/modules/user/services.js";

const loading = ref(false);
const userRoles = ref([]);
const drawerVisible =ref(false);

//用户列表相关数据
const drawerUsers=ref([]);
const drawerLoading=ref(false);
const drawerPage=ref(1);
const drawerPageSize=ref(20);
const drawerTotal=ref(0);

//角色列表相关数据
const drawRoleLoading=ref(false);

const createDualTableState = () => {
  const leftRows = ref([]);
  const rightRows = ref([]);
  const leftSelection = ref([]);
  const rightSelection = ref([]);
  const leftTableRef = ref();
  const rightTableRef = ref();

  const moveLeftToRight = () => {
    if (!leftSelection.value.length) {
      ElMessage.warning('请选择要下移的角色');
      return;
    }
    const selected = leftSelection.value.map(item => ({ ...item }));
    rightRows.value = [...rightRows.value, ...selected];
    const ids = new Set(selected.map(item => item.roleId));
    leftRows.value = leftRows.value.filter(item => !ids.has(item.roleId));
    leftSelection.value = [];
    leftTableRef.value?.clearSelection();
  };

  const moveRightToLeft = () => {
    if (!rightSelection.value.length) {
      ElMessage.warning('请选择要上移的角色');
      return;
    }
    const selected = rightSelection.value.map(item => ({ ...item }));
    leftRows.value = [...leftRows.value, ...selected];
    const ids = new Set(selected.map(item => item.roleId));
    rightRows.value = rightRows.value.filter(item => !ids.has(item.roleId));
    rightSelection.value = [];
    rightTableRef.value?.clearSelection();
  };

  return {
    leftRows,
    rightRows,
    leftSelection,
    rightSelection,
    leftTableRef,
    rightTableRef,
    moveLeftToRight,
    moveRightToLeft,
  };
};

const drawerDualState = createDualTableState();
const {
  leftRows: drawerRoles,
  rightRows: availableRoles,
  leftSelection: TopSelection,
  rightSelection: BottonSelection,
  leftTableRef: TopTableRef,
  rightTableRef: BottomTableRef,
  moveLeftToRight: moveTopToBottom,
  moveRightToLeft: moveBottomToTop,
} = drawerDualState;

const dialogDualState = createDualTableState();
const {
  leftRows: dialogLeftRoles,
  rightRows: dialogRightRoles,
  leftSelection: dialogLeftSelection,
  rightSelection: dialogRightSelection,
  leftTableRef: dialogLeftTableRef,
  rightTableRef: dialogRightTableRef,
  moveLeftToRight: moveDialogLeftToRight,
  moveRightToLeft: moveDialogRightToLeft,
} = dialogDualState;

const userTableRef=ref();

//模糊多条件查询
//存储选项
const filterOptions=[
  {label:'用户名',value:'userName'}
  ,{label: '真实姓名',value: 'realName'}
  ,{label: '邮箱',value: 'email'}
  ,{label: '电话',value: 'phone'}
  ,{label:'性别',value:'gender'}
]
//存储被选中的值，用于更新到前端，默认是数组第一个，必须要后声明
const filterKey=ref(filterOptions[0].value);

//性别
const GenderOptions=[{label:'男',value:'男'},{label: '女',value: '女'}]
const GenderKey=ref(GenderOptions[0].value);
const filterInput=ref('');
const showGenderSelect=computed(()=>filterKey.value === 'gender'); //当false，不显示，ture显示性别下拉列表

//分配用户角色：在抽屉里跨页记住勾选的用户
const selectedUsers = ref([]); // 全局缓存所有被勾选的用户
const suppressUserSelectionEvent = ref(false); // 避免翻页/恢复时清空缓存

//修改表单相关变量
const modifiDivlogVisiable=ref(false);
const modifUserRole=ref(null);//存储选中的用户

const handleDialogLeftSelection = (rows) => {
  dialogLeftSelection.value = rows;
};
const handleDialogRightSelection = (rows) => {
  dialogRightSelection.value = rows;
};


const handleUserSelection = (rows) => {
  if (suppressUserSelectionEvent.value) return;
  syncUserSelection(rows); // Element Plus 会把当前页的勾选行传进来
};

const syncUserSelection = (rows) => {
  const currentIds = new Set(drawerUsers.value.map((user) => user.userId));
  const selectedIds = new Set(rows.map((row) => row.userId));

  // 1) 移除当前页里被取消勾选的用户（其他页保持不变）
  selectedUsers.value = selectedUsers.value.filter((user) => {
    if (currentIds.has(user.userId)) {
      return selectedIds.has(user.userId);
    }
    return true;
  });

  // 2) 把当前页新勾选的补进缓存
  rows.forEach((row) => {
    if (!selectedUsers.value.some((item) => item.userId === row.userId)) {
      selectedUsers.value.push(row);
    }
  });
};

const restoreUserSelection = () => {
  if (!userTableRef.value) return;
  suppressUserSelectionEvent.value = true;
  userTableRef.value.clearSelection();
  const selectedIds = new Set(selectedUsers.value.map((user) => user.userId));
  drawerUsers.value.forEach((user) => {
    if (selectedIds.has(user.userId)) {
      userTableRef.value.toggleRowSelection(user, true);
    }
  });
  suppressUserSelectionEvent.value = false;
};



const handelFilterKeyChange=(value)=>{
  if(value==='gender')
    filterInput.value=''; //切换到性别时，清空原输入框内容
  else
    GenderKey.value=GenderOptions[0].value; //切换到普通字段时，重置默认性别
}

//表格居中
const userTableHeaderStyle = () => ({ textAlign: 'center' });
const userTableCellStyle = () => ({ textAlign: 'center' });

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

const openDrawer = () =>{
  drawerVisible.value=true;
};
//核心逻辑：分配用户角色
const allocation = async () => {
  // 1) 基础校验：必须至少选中 1 个用户，且下方要有准备好的角色
  if (!selectedUsers.value.length) {
    ElMessage.warning('请先选择需要分配的用户');
    return;
  }
  if (!availableRoles.value.length) {
    ElMessage.warning('请先把角色移到「已分配角色」列表');
    return;
  }

  // 2) 抽取 userId / roleId 列表，符合 assignUserRolesBatch 的入参格式
  const userIds = selectedUsers.value.map((user) => user.userId);
  const roleIds = availableRoles.value.map((role) => role.roleId);

  // 3) 调用批量分配接口
  try {
    await assignUserRolesBatch({ userIds, roleIds });
    ElMessage.success('分配成功');

    // 4) 可选的收尾：刷新列表、关闭抽屉、清空勾选
    await fetchData();              // 重新加载「用户-角色映射」主表
    drawerVisible.value = false;    // 视情况决定是否立即收起抽屉
    selectedUsers.value = [];
    suppressUserSelectionEvent.value = true;
    userTableRef.value?.clearSelection();
    suppressUserSelectionEvent.value = false;
  } catch (error) {
    console.error(error);
    ElMessage.error('分配失败，请稍后重试');
  }
};

const closeDrawer=()=>{
  drawerVisible.value=false;
}

//加载抽屉里面的用户列表
const fetchDrawerUsers=async ()=>{
  drawerLoading.value=true;
  try{
    suppressUserSelectionEvent.value = true;
    const res=await listUsersNoRolesPaged({
      pageNumber:drawerPage.value,
      pageSize:drawerPageSize.value,
    });
    drawerUsers.value=res.items || [];
    drawerTotal.value=res.total || 0;
    await nextTick();
    restoreUserSelection();
    suppressUserSelectionEvent.value = false;
  }catch{
    suppressUserSelectionEvent.value = false;
    ElMessage.error('获取用户列表失败');
  }finally {
    drawerLoading.value=false;
  }
}
//监听初次打开抽屉时，默认加载第1页，10行用户数据，同时加载需要加载的数据
watch(drawerVisible,(visible)=>{
  if(visible){
    drawerPage.value=1;
    drawerPageSize.value=20;
    selectedUsers.value=[];
    nextTick(()=>{
      suppressUserSelectionEvent.value = true;
      userTableRef.value?.clearSelection();
      suppressUserSelectionEvent.value = false;
    });
    fetchDrawerUsers();
    fetchRoleDate();
  }else{
    selectedUsers.value=[];
    suppressUserSelectionEvent.value = true;
    userTableRef.value?.clearSelection();
    suppressUserSelectionEvent.value = false;
  }
});

//分页大小变化时
const handleDrawerSizeChange =(val)=>{
  drawerPageSize.value=val;
  drawerPage.value=1;
  fetchDrawerUsers();
}
//页码改变时
const handleDrawerCurrentChange=(val)=>{
  drawerPage.value=val;
  fetchDrawerUsers();
}

//监听上下两个表格的勾选变换，获取被勾选的行
const handelTopSelection=(rows)=>{
  TopSelection.value=rows;
}
const handelBottomSelection=(rows)=>{
  BottonSelection.value=rows;
}
//调用单条件查询api
const applySearch=async ()=>{
  //处理空的情况
  const InputKey=filterKey.value;
  //处理查询条件词，每次只构造一个，仅考虑两种情况，性别和其他
  let payload={};
  if(InputKey==='gender'){
    payload={gender:GenderKey.value};
  }else{
    const keyword=(filterInput.value|| '').trim();
    if(!keyword){
      ElMessage.warning('请输入查询条件');
      return;
    }
    payload={[InputKey]:keyword};
  }
  //拿出前面构造的单个条件，调用api接口
  try{
    drawerLoading.value=true;
    suppressUserSelectionEvent.value = true;
    const res=await searchUsers(payload);
    drawerUsers.value=Array.isArray(res)? res:[];
    drawerTotal.value=drawerUsers.value.length;//搜索结果不分页，直接展示所有结果
    drawerPage.value=1;
    await nextTick();
    restoreUserSelection();
    suppressUserSelectionEvent.value = false;
  }catch{
    suppressUserSelectionEvent.value = false;
    ElMessage.error('查询失败，请稍后重试');
  }finally {
    drawerLoading.value=false;
  }
}

//加载抽屉的角色列表
const fetchRoleDate=async ()=>{
  drawRoleLoading.value=true;
  try{
    drawerRoles.value= await listRoles();//调用role api接口，查询角色
    availableRoles.value=[];//初始时下面表格数据为空
  }catch{
    ElMessage.error('获取角色列表失败');
  }
  drawRoleLoading.value=false;
}

//加载对话框进行修改
const openModifiUserRole=async (row)=>{
  modifUserRole.value=row;
  if((drawerRoles.value.length+availableRoles.value.length)===0){
    await fetchRoleDate();
  }
  const assignedIds=new Set((row.roles??[]).map(role=>role.roleId));
  const allRoles=[...drawerRoles.value,...availableRoles.value];
  dialogLeftRoles.value=allRoles.filter(role=>!assignedIds.has(role.roleId)).map(role=>({...role}));
  dialogRightRoles.value=(row.roles??[]).map(role=>({...role}));
  dialogLeftSelection.value=[];
  dialogRightSelection.value=[];
  dialogLeftTableRef.value?.clearSelection();
  dialogRightTableRef.value?.clearSelection();
  modifiDivlogVisiable.value=true;
}

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
    <el-table :data="groupedRows" v-loading="loading" stripe :header-cell-style="userTableHeaderStyle" :cell-style="userTableCellStyle">
      <el-table-column prop="userId" label="用户ID" />
      <el-table-column prop="userName" label="姓名" />
      <el-table-column label="角色">
        <template #default="{ row }">
          <el-space wrap>
            <el-tag v-for="role in row.roles" :key="role.roleId" type="info" effect="plain" size="small">
              {{ role.roleName }}
            </el-tag>
          </el-space>
        </template>
      </el-table-column>
      <el-table-column label="操作">
        <template #default="{ row }">
          <el-button type="primary" @click="openModifiUserRole(row)" >修改</el-button>
        </template>
      </el-table-column>
    </el-table>
  </el-card>

  <el-drawer class="Top-drawer" title="用户角色分配" v-model="drawerVisible" size="70%">
    <div class="drawer-panels">
      <div class="panel is-left">
        <div class="left-top">
          <el-text size="large" style="font-weight: bold">待分配角色</el-text>
          <div class="left-top-table">
            <el-table @selection-change="handelTopSelection" ref="TopTableRef" :data="drawerRoles" v-loading="drawRoleLoading"
                      :header-cell-style="userTableHeaderStyle" :cell-style="userTableCellStyle"
                      style="width: 90%;margin: 20px auto 0; height: 85%;">
              <el-table-column fixed type="selection"  width="25" />
              <el-table-column prop="roleName" label="角色名"/>
            </el-table>
          </div>
        </div>
        <div class="left-middle">
          <el-button @click="moveBottomToTop" type="primary" style="width: 100px"><el-icon><ArrowUp /></el-icon></el-button>
          <el-button @click="moveTopToBottom" type="primary" style="width: 100px"><el-icon><ArrowDown /></el-icon></el-button>
        </div>
        <div class="left-bottom">
          <el-text size="large" style="font-weight: bold">已分配角色</el-text>
          <div class="left-bottom-table" >
            <el-table :data="availableRoles" @selection-change="handelBottomSelection" ref="BottomTableRef" style="width: 90%;margin: 20px auto 0; height: 85%;"
                      :header-cell-style="userTableHeaderStyle" :cell-style="userTableCellStyle">
              <el-table-column fixed type="selection"  width="25" />
              <el-table-column prop="roleName" label="已分配角色"/>
            </el-table>
          </div>
        </div>
      </div>
      <div class="panel is-right">
        <el-card class="box-card" >
          <div class="box-content">
            <div class="box-left">
              <el-select v-model="filterKey" @change="handelFilterKeyChange" style="width: 100px;">
                <el-option v-for="item in filterOptions" :key="item.value" :label="item.label" :value="item.value" />
              </el-select>
              <el-input v-if="!showGenderSelect" placeholder="请输入查询条件" v-model="filterInput" style="width: 150px; margin-left: 10px;"/>
              <el-select v-else v-model="GenderKey"  style="width: 150px; margin-left: 10px;">
                <el-option v-for="item in GenderOptions" :key="item.value" :label="item.label" :value="item.value" style="align-items: center;"/>
              </el-select>
              <el-button type="primary" @click="applySearch" style="margin-left: 10px">查询</el-button>
              <el-button type="default" @click="fetchDrawerUsers" style="margin-left: 10px">重置</el-button>
            </div>
            <div class="box-right">
              <el-button @click="allocation" type="primary">分配</el-button>
              <el-button @click="closeDrawer" type="default">取消</el-button>
            </div>
          </div>
        </el-card>
        <el-table ref="userTableRef" class="tb-user" :data="drawerUsers" border stripe v-loading="drawerLoading"
                  :header-cell-style="userTableHeaderStyle" :cell-style="userTableCellStyle"
                  @selection-change="handleUserSelection"
        style="height: 100%">
          <el-table-column fixed  type="selection"  width="55" />
          <el-table-column prop="userId" label="用户ID" />
          <el-table-column prop="userName" label="用户名"/>
          <el-table-column prop="email" label="邮箱" show-overflow-tooltip />
          <el-table-column prop="phone" label="电话" />
          <el-table-column prop="realName" label="姓名" show-overflow-tooltip />
          <el-table-column prop="idCard" label="身份证号" show-overflow-tooltip />
          <el-table-column prop="gender" label="性别" />
        </el-table>
      </div>
    </div>
    <div class="drawer-pagination">
      <el-pagination
        v-model:current-page="drawerPage"
        v-model:page-size="drawerPageSize"
        :total="drawerTotal"
        :page-sizes="[20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        :background="true"
        @size-change="handleDrawerSizeChange"
        @current-change="handleDrawerCurrentChange"
      />
    </div>
  </el-drawer>

  <el-dialog class="user-role-dialog" v-model="modifiDivlogVisiable" title="修改用户角色" width="65%">
    <el-form>
      <el-form-item label="用户：">
        <span>{{modifUserRole?.userName}}</span>
      </el-form-item>
    </el-form>
    <div class="role-dialog-columns">
      <div class="log-left">
        <el-text>待分配角色</el-text>
        <el-table
          ref="dialogLeftTableRef"
          :data="dialogLeftRoles"
          border
          stripe
          height="100%"
          :header-cell-style="userTableHeaderStyle"
          :cell-style="userTableCellStyle"
          @selection-change="handleDialogLeftSelection"
        >
          <el-table-column fixed type="selection" width="45" />
          <el-table-column prop="roleName" label="角色名" show-overflow-tooltip />
          <el-table-column prop="description" label="描述" show-overflow-tooltip />
        </el-table>
      </div>
      <div class="log-mid">
        <el-button type="primary" circle @click="moveDialogLeftToRight">
          <el-icon><ArrowRight /></el-icon>
        </el-button>
        <el-button type="primary" circle @click="moveDialogRightToLeft">
          <el-icon><ArrowLeft /></el-icon>
        </el-button>
      </div>
      <div class="log-right">
        <el-text>已分配角色</el-text>
        <el-table
          ref="dialogRightTableRef"
          :data="dialogRightRoles"
          border
          stripe
          height="100%"
          :header-cell-style="userTableHeaderStyle"
          :cell-style="userTableCellStyle"
          @selection-change="handleDialogRightSelection"
        >
          <el-table-column fixed type="selection" width="45" />
          <el-table-column prop="roleName" label="角色名" show-overflow-tooltip />
          <el-table-column prop="description" label="描述" show-overflow-tooltip />
        </el-table>
      </div>
    </div>
  </el-dialog>



</template>

<style scoped>
.card-header { display: flex; justify-content: space-between; align-items: center; }
.drawer-panels {display: flex;gap: 16px;height: 95%}
.drawer-panels .panel{ flex: 1; border-style: solid;border-width: 1px; border-color: #e0e0e0;border-radius:15px;}
.drawer-panels .panel.is-right{ flex:2;display: flex; flex-direction: column;align-items: center;}
.drawer-pagination {
  margin-top:auto;
  display: flex;
  padding-top:15px;
  justify-content: center;
}
.panel .is-right{
  display: flex;
  flex-direction: column;
  flex: 2;
}
.is-left{
  display: flex;
  flex-direction: column;
  flex: 2;
}
.tb-user{
  width: 95%;
  margin:10px auto 0;
  flex: 1;
}
.box-card{
  width: 95%;
  margin: 10px auto 0;
  box-shadow: none;
}
.box-content{
  display: flex;
  align-items: center;
  justify-content: space-between; /*两个div撑开*/
}
.box-left{
  display: flex;
  align-items: center;
}
.box-right{
  display: flex;
  align-items: center;
}


.left-top {
  width: 90%;
  height: 45%;
  flex: 1;
  margin:10px auto 10px auto;
  border-style: solid;border-width: 1px; border-color: #e0e0e0;border-radius:15px;

  display: flex;
  flex-direction: column;
}
/*去掉表格底部线条*/
.left-top :deep(.el-table__inner-wrapper::before){
  height: 0;
}

.left-bottom :deep(.el-table__inner-wrapper::before){
  height: 0;
}

.left-top :deep(.el-table){
  flex: 1;
  width: 100%;
}

.left-top-table{
  flex: 1;
  width: 80%;
  height: 90%;
  margin:10px auto 10px auto;
  border-style: solid;border-width: 1px; border-color: #e0e0e0;border-radius:15px;
}

.left-bottom-table{
  flex: 1;
  width: 80%;
  height: 90%;
  margin:10px auto 10px auto;
  border-style: solid;border-width: 1px; border-color: #e0e0e0;border-radius:15px;
}

.left-middle{
  display: flex;                /* 启用 flex 布局 */
  flex-direction:row;       /* 水平堆叠两个按钮 */
  align-items: center;          /* 水平居中 */
  justify-content: center;      /* 垂直居中 */
  gap: 50px;
}

.left-bottom{
  width: 90%;
  height: 45%;
  flex: 1;
  margin:10px auto 10px auto;
  border-style: solid;border-width: 1px; border-color: #e0e0e0;border-radius:15px;

  display: flex;
  flex-direction: column;
}

.Top-drawer :deep(.el-drawer){
  font-weight: bold;
}

.log-mid :deep(.el-button + .el-button) {
  margin-left: 0;
}


</style>
