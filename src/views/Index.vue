<template>
    <div id="module">
        <el-container>
            <LeftMenu />

            <el-container>
                <el-header>
                    <breadcrumb />
                </el-header>

                <el-main>
                    <RouterView></RouterView>
                </el-main>
            </el-container>
        </el-container>
    </div>
</template>

<script setup>
import { ref, inject, onMounted } from 'vue'
import { Menu as IconMenu, Message, Setting } from '@element-plus/icons-vue'
import LeftMenu from '../components/LeftMenu.vue';
import breadcrumb from '../components/breadcrumb.vue';




</script>

<style>
/* 首页 */
.el-container {
    height: 100%;
}

.el-container .el-menu-item,
.el-container .el-sub-menu .el-sub-menu__title {
    background: #30377d;
    color: #f9f9f9;
}

.el-container .el-menu-item:hover,
.el-container .el-sub-menu .el-sub-menu__title:hover {
    color: #fff;
    background: #30377d;
}

.el-menu-item.is-active,
.el-sub-menu.is-active {
    color: #fff;
    background: #30377d;
}

.el-container .el-sub-menu.is-active.is-opened,
.el-container .el-sub-menu.is-active.is-opened .el-sub-menu__title {
    color: #f9f9f9;
    background: #20266d;
}

.el-container .el-header {
    /* border-bottom: 1px solid #f1f1f1; */
    background: #f5f5f8;
}

.el-container .el-header .exit {
    margin-top: 10px;
}

.aside {
    background: #30377d;
}

.aside .el-menu {
    border-right: solid 1px #f5f5f8;
    background: #1d1e24;
}

.el-submenu__title {
    color: #f5f5f8;
}

.el-main .el-card {
    border: none;
    background: #f5f5f8;
}

body {
   
    background-color: #f5f5f8;
    /* color: #fff; */
}</style>
