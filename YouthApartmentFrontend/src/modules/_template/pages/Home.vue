<script setup>
defineOptions({ name: 'AdminHomePage' });
</script>

<template>
  <div class="home-page">
    <el-card class="welcome-card">
      <h2>后台首页</h2>
      <p>欢迎来到青年公寓后台管理系统。</p>
      <p>请从左侧菜单选择模块开始管理。</p>
    </el-card>

    <el-row :gutter="12" class="quick-panels">
      <el-col :span="6">
        <el-card><strong>用户</strong><div>查看和管理用户信息</div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><strong>角色</strong><div>配置角色与权限范围</div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><strong>权限</strong><div>维护权限点及说明</div></el-card>
      </el-col>
      <el-col :span="6">
        <el-card><strong>关联</strong><div>用户-角色与角色-权限关系</div></el-card>
      </el-col>
    </el-row>
  </div>
</template>

<style scoped>
.home-page { padding: 16px; }
.welcome-card { margin-bottom: 16px; }
.quick-panels :deep(.el-card) { min-height: 100px; display: flex; flex-direction: column; justify-content: center; align-items: center; }
</style>