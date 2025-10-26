<script setup>
defineOptions({ name: 'CommonForbiddenPage' });
</script>

<template>
  <div class="forbidden">
    <el-result icon="warning" title="403 无权访问" sub-title="请检查权限或联系管理员" />
  </div>
</template>

<style scoped>
.forbidden { padding: 48px; }
</style>