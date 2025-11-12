<script setup>
import { ref, computed } from 'vue';
import { useRoute } from 'vue-router';
import {
  User as UserIcon,
  Collection as RoleIcon,
  Lock as PermissionIcon,
  Link as UserRoleIcon,
  Connection as RolePermissionIcon,
  Files as BasicInfoIcon, // 新增图标
  Bell as AnnouncementIcon,
} from '@element-plus/icons-vue';

const collapsed = ref(false);
const route = useRoute();

const activeIndex = computed(() => {
  const { meta, path } = route;
  if (meta.activeMenu) {
    return meta.activeMenu;
  }
  return path;
});

const pageTitle = computed(() => route.meta?.title || '');
</script>

<template>
  <el-container class="admin-container">
    <el-aside :width="collapsed ? '64px' : '220px'" class="admin-aside">
      <div class="logo">青年公寓</div>
      <el-menu :router="true" :default-active="activeIndex" :collapse="collapsed" class="menu">
        <el-menu-item index="/admin/home">
          <el-icon><UserIcon /></el-icon>
          <span>首页</span>
        </el-menu-item>

        <el-sub-menu index="user-basic-info">
          <template #title>
            <el-icon><BasicInfoIcon /></el-icon>
            <span>基本信息</span>
          </template>
          <el-menu-item index="/admin/users">
            <el-icon><UserIcon /></el-icon>
            <span>用户管理</span>
          </el-menu-item>
          <el-menu-item index="/admin/roles">
            <el-icon><RoleIcon /></el-icon>
            <span>角色管理</span>
          </el-menu-item>
          <el-menu-item index="/admin/permissions">
            <el-icon><PermissionIcon /></el-icon>
            <span>权限管理</span>
          </el-menu-item>
          <el-menu-item index="/admin/user-role">
            <el-icon><UserRoleIcon /></el-icon>
            <span>用户-角色</span>
          </el-menu-item>
          <el-menu-item index="/admin/role-permission">
            <el-icon><RolePermissionIcon /></el-icon>
            <span>角色-权限</span>
          </el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="basic-services">
          <template #title>
            <el-icon><BasicInfoIcon /></el-icon>
            <span>基础服务管理</span>
          </template>
          <el-menu-item index="/admin/announcements">
            <el-icon><AnnouncementIcon /></el-icon>
            <span>公告管理</span>
          </el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="recycle-bin">
          <template #title>
            <el-icon><BasicInfoIcon /></el-icon>
            <span>回收站</span>
          </template>
          <el-menu-item index="/admin/announcements/recycle-bin">
            <el-icon><AnnouncementIcon /></el-icon>
            <span>公告回收站</span>
          </el-menu-item>
        </el-sub-menu>
      </el-menu>
    </el-aside>

    <el-container>
      <el-header class="admin-header">
        <div class="left">
          <el-button link @click="collapsed = !collapsed">
            {{ collapsed ? '展开菜单' : '收起菜单' }}
          </el-button>
          <el-breadcrumb separator="/" class="breadcrumbs">
            <el-breadcrumb-item>后台管理</el-breadcrumb-item>
            <el-breadcrumb-item v-if="pageTitle">{{ pageTitle }}</el-breadcrumb-item>
          </el-breadcrumb>
        </div>
      </el-header>
      <el-main class="admin-main">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<style scoped lang="scss">
.admin-container { height: 100vh; }

.admin-aside {
  background: #2F4050;
  color: #A7B1C2;

  :deep(.el-menu) {
    --el-menu-text-color: #A7B1C2;
    border-right: none;
    background: transparent;
    padding: 0;

    .el-menu-item,
    .el-sub-menu__title {
      height: 40px;
      line-height: 56px;
      margin: 0;
      border-radius: 0;
      &:hover {
        background-color: #293846;
        color: #fff;
      }
    }

    .el-menu-item {
      &.is-active {
        color: #fff;
        background-color: #293846;
        position: relative;
        &::before {
          content: '';
          position: absolute;
          left: 0;
          top: 0;
          height: 100%;
          width: 3px;
          background-color: #3C8DBC;
        }
      }
    }

    .el-sub-menu {
      position: relative;

      &.is-active {
        &::before {
          content: '';
          position: absolute;
          left: 0;
          top: 0;
          height: 100%;
          width: 3px;
          background-color: #3C8DBC;
        }

        > .el-sub-menu__title {
          color: #fff;
        }

        .el-menu {
          background-color: #293846;
        }
      }

      &.is-opened {
        .el-menu {
          background-color: #293846;
        }
      }
    }
  }
}

.logo {
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
}

.admin-header {
  display: flex;
  align-items: center;
  border-bottom: 1px solid var(--el-border-color);
  .left { display: flex; align-items: center; gap: 12px; }
}

.admin-main { background: #f5f7fa; }

/* Responsive */
@media (max-width: 768px) {
  .logo { height: 48px; font-size: 14px; }
  .admin-aside :deep(.el-menu) { padding: 4px 0; }
  .admin-aside :deep(.el-menu-item), .admin-aside :deep(.el-sub-menu__title) {
    height: 36px;
    margin: 2px 6px;
    padding: 0 10px;
    border-radius: 6px;
    :deep(.el-icon) { font-size: 16px; }
  }
}
</style>
