<template>
    <body id="login">
        <el-form class="container" label-position="left" label-width="0px">
            <!-- <h5><a v-on:click="back" style="cursor:pointer; color:gray">&lt返回</a></h5> -->
            <h3 class="title">系统登录</h3>
            <el-form-item>
                <el-input type="text" auto-complete="off" placeholder="手机号" v-model="data.form.phone"></el-input>
            </el-form-item>
            <el-form-item>
                <el-input type="password" auto-complete="off" placeholder="密码" v-model="data.form.password"></el-input>
            </el-form-item>
            <el-form-item style="width: 100%">

                <el-button type="primary" class="login" @click="loginBtn">
                    登录
                </el-button>
                <el-button type="primary" class="register" @click="regBtn">
                    注册
                </el-button>
            </el-form-item>

        </el-form>
    </body>
</template>


<script setup>
import { reactive } from 'vue'
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus'
import jwtDecode from 'jwt-decode';

import axios from 'axios';




const router = useRouter()

const data = reactive({
    form: {
        phone: "",
        password: ""
    }
})


async function loginBtn() {
    try {
        const response = await axios.post('https://localhost:44341/api/Home/Login', {
            phone: data.form.phone,
            password: data.form.password
        });
        console.log(data.form.phone)
        console.log(data.form.password)
        console.log(response.data);
        // 后端返回的token在response.data.token中
        const token = response.data;
        // 将token保存到本地存储
        localStorage.setItem('token', token);
        // 解析token
        const decoded = jwtDecode(token);
        // 获取解析后的信息
        console.log(decoded)
        // const userId = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
        // const userName = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
        // const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        const userId = decoded.nameid;
        const userName = decoded.name;
        const userRole = decoded.role;

        console.log(userId); // 输出用户ID
        console.log(userName); // 输出用户名
        console.log(userRole); // 输出用户角色


        ElMessage({
            message: "登录成功",
            type: 'success',
        })

        // 跳转到用户信息页面
        router.push({ path: "/home/index" });
    } catch (error) {
        // 处理错误
        console.error('登录失败：', error);
    }
}


function regBtn() {
    router.push({ path: "/register" });
}


function back() {
    router.go(-1)
}


</script>

<style scoped>
#login {
    background-position: center;
    height: 100%;
    width: 100%;
    background-size: cover;
    position: fixed;


}

body {
    margin: 0px;
    padding: 0px;
}

.container {
    border-radius: 15px;
    background-clip: padding-box;
    margin: 90px auto;
    width: 350px;
    padding: 35px 35px 15px 35px;
    background: #fff;
    border: 1px solid #eaeaea;
    box-shadow: 0 0 25px #cac6c6;
}

.title {
    margin: 0px auto 40px auto;
    text-align: center;
    color: #505458;
    background-color: #fff;
}

.register {
    width: 40%;
    border: none;
    float: right;
    margin: 0;
}

.login {
    width: 40%;
    border: none;
    float: left;
    margin-right: 50px;
}

a {
    text-decoration: none;
}

.router-link-active {
    text-decoration: none;
}
</style>

