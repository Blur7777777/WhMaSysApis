<!-- 左菜单栏 -->
<template>
    <el-aside width="200px" class="aside">
        <el-menu :default-active="route.path" exact class="el-menu-vertical-demo" router>
            <el-menu-item class="menu" index="/index" style="padding: 0px;">
                <div style="width: 80%;padding-bottom: 30px;height: 80%; padding-left: 45px;">
                    <!-- <el-image id="myImage"  />  -->
                    仓库管理系统
                </div>
            </el-menu-item>
            <NavItem v-for="I in items" :key="I.url" :item="I" :basePath="I.url" />
        </el-menu>
    </el-aside>
</template>
<script setup>

import { reactive } from 'vue'
import { useRouter, useRoute } from "vue-router";  //导入路由
import NavItem from './NavItem.vue'  //子组件  


const route = useRoute();

const items = reactive([
    {
        text: '首页',
        url: '/home/index',
        icon: 'HomeFilled'
    },
    {
        text: '订单',
        url: '/',
        icon: 'HelpFilled',
        child: [
            {
                text: '新订单',
                url: '/Order/leave',
                icon: 'Operation',
            },
            {
                text: '订单列表',
                url: '/Order/OrderList',
                icon: 'Operation',
            },
            { text: '售后单', url: '/Order/parts', icon: 'Setting' },
        ]
    },

    {
        text: '商品',
        url: '/',
        icon: 'HelpFilled',
        child: [
            {
                text: '产品查询',
                url: '/Product/Products',
                icon: 'Grid',
            },
            {
                text: '添加产品',
                url: '/Product/Product-add',
                icon: 'Grid',
            },
        ]
    },

    {
        text: '统计分析',
        url: '/',
        icon: 'HelpFilled',
        child: [
            // { text: '配件售后', url: '/CusSer/parts', icon: 'Setting' },
            { text: '统计分析', url: '/CusSer/battery', icon: 'Setting' }
        ]
    },
    {
        text: '人员管理',
        url: '/',
        icon: 'Avatar',
        child: [
            { text: '业务员管理', url: '/Manage/Worker', icon: 'Avatar' },
            { text: '客户管理', url: '/Manage/Consumer', icon: 'Avatar' },
            { text: '供应商管理', url: '/Manage/Supplier', icon: 'Avatar' }
        ]
    },
])

</script>