import { createRouter, createWebHistory } from 'vue-router'
import Index from '../views/Index.vue'
import leave from '../views/Order/leave.vue'
import parts from '../views/CusSer/parts.vue'
import battery from '../views/CusSer/battery.vue'
import login from '../views/login.vue'
import register from '../views/register.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path:'/',component:()=>import('../views/login.vue')
    },
    {
      path:'/login',component:()=>import('../views/login.vue')
    },
    {
      path:'/register',component:()=>import('../views/register.vue')
    },
    {
      path: '/Index',
      name: 'Index',
      component: Index,
      children:[
        {
          path: '/about',
          name: 'about',
          component: () => import('../views/AboutView.vue')
        },
        {
          path: '/home/index',
          name: 'index',
          meta: { title: '首页' },
          component: () => import('../views/home/index.vue')
        },
        {
          path: '/Order/leave',
          name: 'Order',
          meta: { title: '订单' },
          
          children: [
            {
              path: '/Order/leave',
              name: 'leave',
              meta:{title:'新订单'},
              component: () => import('../views/Order/leave.vue')
            },
            
            {
              path: '/Order/OrderList',
              name: 'OrderList',
              meta:{title:'订单列表'},
              component: () => import('../views/Order/OrderList.vue')
            },
            {
              path: '/Order/parts',
              name: 'parts',
              meta:{title:'售后单'},
              component: () => import('../views/Order/parts.vue')
            },
          ]
        },
        {
          path: '/Product/Products',
          name: 'Product',
          meta: { title: '商品' },
          
          children: [
            {
              path: '/Product/Products',
              name: 'Products',
              meta:{title:'商品列表'},
              component: () => import('../views/Product/Products.vue')
            },
            {
              path: '/Product/Product-add',
              name: 'Product-add',
              meta:{title:'商品列表'},
              component: () => import('../views/Product/Product-add.vue')
            },
          ]
        },
        
        {
          path: '/CusSer/battery',
          name: 'CusSer',
          meta: { title: '统计分析' },
          // component: () => import('../views/CusSer/parts.vue'),
          
          children: [
            // {
            //   path: '/CusSer/parts',
            //   name: 'parts',
            //   meta:{title:'配件售后'},
            //   component: () => import('../views/CusSer/parts.vue')
            // },
            {
              path: '/CusSer/battery',
              name: 'battery',
              meta:{title:'统计分析'},
              component: () => import('../views/CusSer/battery.vue')
            },
          ]
        },
        {
          path: '/Manage/Worker',
          name: 'Manage',
          meta: { title: '人员管理' },
          // component: () => import('../views/CusSer/parts.vue'),
          
          children: [
            {
              path: '/Manage/Worker',
              name: 'Worker',
              meta:{title:'业务员管理'},
              component: () => import('../views/Manage/Worker.vue')
            },
            {
              path: '/Manage/Consumer',
              name: 'Consumer',
              meta:{title:'客户管理'},
              component: () => import('../views/Manage/Consumer.vue')
            },
            {
              path: '/Manage/Supplier',
              name: 'Supplier',
              meta:{title:'供应商管理'},
              component: () => import('../views/Manage/Supplier.vue')
            },
          ]
        },
        
      ]
    }
  ]
})

export default router
