<script setup>
defineOptions({ name: 'UserRoleIndexPage' });
import {ref, onMounted, computed, watch} from 'vue';
import { ElMessage } from 'element-plus';
import { listUserRoles } from '../services.js';
import {listUsersNoRolesPaged} from "../services.js";

const loading = ref(false);
const userRoles = ref([]);
const drawerVisible =ref(false);

//用户列表相关数据
const drawerUsers=ref([]);
const drawerLoading=ref(false);
const drawerPage=ref(1);
const drawerPageSize=ref(10);
const drawerTotal=ref(0);

//模糊多条件查询
//存储选项
const filterOptions=[{label:'用户名',value:'userName'}
  ,{label: '邮箱',value: 'email'}
  ,{label: '电话',value: 'phone'}
  ,{label: '真实姓名',value: 'realName'}
  ,{label:'性别',value:'gender'}
]
//存储被选中的值，用于更新到前端，默认是数组第一个，必须要后声明
const filterKey=ref(filterOptions[0].value);

//性别
const GenderOptions=[{label:'男',value:'男'},{label: '女',value: '女'}]
const GenderKey=ref(GenderOptions[0].value);
const filterInput=ref('');
const showGenderSelect=computed(()=>filterKey.value === 'gender'); //当false，不显示，ture显示性别下拉列表
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

/*
const closeDrawer=()=>{
  drawerVisible.value=false;
}
*/

const fetchDrawerUsers=async ()=>{
  drawerLoading.value=true;
  try{
    const res=await listUsersNoRolesPaged({
      pageNumber:drawerPage.value,
      pageSize:drawerPageSize.value,
    });
    drawerUsers.value=res.items || [];
    drawerTotal.value=res.total || 0;
  }catch{
    ElMessage.error('获取用户列表失败');
  }finally {
    drawerLoading.value=false;
  }
}
//监听初次打开抽屉时，默认加载第1页，10行用户数据
watch(drawerVisible,(visible)=>{
  if(visible){
    drawerPage.value=1;
    drawerPageSize.value=10;
    fetchDrawerUsers();
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

  <el-drawer title="用户角色分配" v-model="drawerVisible" size="70%">
    <div class="drawer-panels">
      <div class="panel is-left">

      </div>
      <div class="panel is-right">
        <el-card class="box-card" >
          <div style="display: flex;">
            <el-select v-model="filterKey" @change="handelFilterKeyChange" style="width: 100px;">
              <el-option v-for="item in filterOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
            <el-input v-if="!showGenderSelect" placeholder="请输入查询条件" v-model="filterInput" style="width: 150px; margin-left: 10px;"/>
            <el-select v-else v-model="GenderKey"  style="width: 150px; margin-left: 10px;">
              <el-option v-for="item in GenderOptions" :key="item.value" :label="item.label" :value="item.value" style="align-items: center;"/>
            </el-select>
          </div>
          <div style="display: flex;">
            <el-button type="primary">分配</el-button>
            <el-button type="default">取消</el-button>
          </div>
        </el-card>

        <el-table class="tb-user" :data="drawerUsers" border stripe v-loading="drawerLoading" :header-cell-style="userTableHeaderStyle" :cell-style="userTableCellStyle">
          <el-table-column type="selection"  width="55" />
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
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        :background="true"
        @size-change="handleDrawerSizeChange"
        @current-change="handleDrawerCurrentChange"
      />
    </div>
  </el-drawer>



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
}
.tb-user{
  width: 95%;
  margin:10px auto 0;

}
.box-card{
  width: 95%;
  margin: 10px auto 0;
}



</style>
