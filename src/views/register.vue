<template>
    <body id="login">
        <el-form class="container" label-position="left" label-width="0px">
            <h5><a v-on:click="back" style="cursor:pointer; color:gray">&lt返回</a></h5>
            <h3 class="title">系统登录</h3>
            <el-tooltip content="手机为大陆手机号" effect="customized">
                <el-form-item>
                    <el-input type="text" auto-complete="off" placeholder="手机号" v-model="data.form.phone"></el-input>
                </el-form-item>
            </el-tooltip>
            <el-tooltip content="填写真实姓名" effect="customized">
                <el-form-item>
                    <el-input type="text" auto-complete="off" placeholder="用户名" v-model="data.form.username"></el-input>
                </el-form-item>
            </el-tooltip>
            <el-tooltip content="密码为3位数字" effect="customized">
                <el-form-item>
                    <el-input type="password" auto-complete="off" placeholder="密码" v-model="data.form.password"></el-input>
                </el-form-item>
            </el-tooltip>



            <el-form-item style="width: 100%">
                <el-button type="primary" class="register" @click="regBtn">注册</el-button>
            </el-form-item>

        </el-form>
    </body>
</template>

<script setup>
import { reactive, getCurrentInstance } from 'vue'
import { useRouter } from "vue-router";
import { ElMessage } from 'element-plus'
import axios from 'axios';


const data = reactive({
    form: {
        phone:"",
        username: "",
        password: ""
    }
})

const rotuer = useRouter();


async function regBtn() {
    try {
        const response = await axios.post('https://localhost:44341/api/Home/Reg', data.form);
        ElMessage({
            message: response.data,
            type: 'success',
        })
        console.log('注册成功', response.data);
        if (response.data=="注册成功") {
            rotuer.push({ path: "/login" });
        }
        
    } catch (error) {
        console.error('注册失败', error);
        ElMessage({
            message: response.data,
            type: 'success',
        })
    }
}



function back() {
    rotuer.go(-1)
}



</script>


<style>
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
    width: 100%;
    border: none;
    float: right;
    margin: 0;
}


.el-popper.is-customized {
    padding: 6px 12px;
    background: linear-gradient(90deg, rgb(159, 229, 151), rgb(204, 229, 129));
}

.el-popper.is-customized .el-popper__arrow::before {
    background: linear-gradient(45deg, #b2e68d, #bce689);
    right: 0;
}
</style>
  