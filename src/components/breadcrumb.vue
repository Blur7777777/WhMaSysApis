<template>
  <div class="breadcrumb">
    <el-breadcrumb :separator-icon="ArrowRight">
      <el-breadcrumb-item v-for="I in lists" :key="I.path">
        <router-link :to="I.path">{{ I.meta.title }}</router-link>
      </el-breadcrumb-item>
      <div style="margin-left: 900px;" @click="drawers">当前业务员：{{ currentUser }}</div>
    </el-breadcrumb>
  </div>

  <el-drawer v-model="drawer" title="员工个人信息" :with-header="false" class="drawer">
    <div>
      <h2>基本信息</h2>
      <div class="form-message">
        <el-form-item label="姓名">
          <P>{{ worker.userName }} </P>
        </el-form-item>
        <el-form-item label="职位">
          <P> {{ worker.role }}</P>
        </el-form-item>
        <el-form-item label="联系方式">
          <el-input style="width: 203px" v-model="worker.phone" placeholder="待完善"></el-input>
        </el-form-item>
        <el-form-item label="性别">
          <el-input style="width: 203px" v-model="worker.sex" placeholder="待完善"></el-input>
        </el-form-item>
        <el-form-item label="出生日期">
          <el-input style="width: 203px" v-model="worker.birthday" placeholder="待完善"></el-input>
        </el-form-item>
        <el-form-item label="民族">
          <el-input style="width: 203px" v-model="worker.nation" placeholder="待完善"></el-input>
        </el-form-item>
        <el-form-item label="身份证号码">
          <el-input style="width: 203px" v-model="worker.idnumber" placeholder="待完善"></el-input>
        </el-form-item>
        <el-form-item label="学历">
          <el-input style="width: 203px" v-model="worker.education" placeholder="待完善"></el-input>
        </el-form-item>
        <el-form-item label="个人地址">
          <el-input style="width: 203px" v-model="worker.address" placeholder="待完善"></el-input>
        </el-form-item>
      </div>
    </div>
    <div class="form-btn">
      <el-button type="primary" @click="updatepassword">修改密码</el-button>
      <el-button type="primary" @click="updatephone(row)">更新修改</el-button>
      <el-button type="primary" @click="logout">退出登录</el-button>
    </div>

  </el-drawer>

  <el-dialog v-model="dialogFormInsertWorker" title="修改密码">
    <el-form :model="workerpwd">、
      <el-form-item label="当前账号">
        <el-input v-model="worker.phone" />
      </el-form-item>
      <el-form-item label="旧密码">
        <el-input v-model="workerpwd.password" />
      </el-form-item>
      <el-form-item label="新密码">
        <el-input v-model="workerpwd.newpassword" />
      </el-form-item>
      <el-form-item label="再次输入新密码">
        <el-input v-model="workerpwd.newpasswords" />
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="openDialog(row)">取消</el-button>
                <el-button type="primary" @click="closeWorker(row)">
                    确定
                </el-button>
      </span>
    </template>
  </el-dialog>


</template>
  
<script  setup>
import { ArrowRight } from '@element-plus/icons-vue'
import { useRoute, useRouter } from "vue-router";
import jwtDecode from 'jwt-decode';
import { ref, onMounted, watch, provide } from 'vue';
import axios from 'axios';
import { ElMessage } from 'element-plus'
const route = useRoute();
const router = useRouter()
const lists = ref([]);
const currentUser = ref('');
const currentUserID = ref('')
const drawer = ref(false)
const worker = ref({
    userName: '',
    role: '',
    phone: '',
    sex: '',
    birthday: '',
    nation: '',
    idnumber: '',
    education: '',
    address: '',
    password:''
});

const workerpwd=ref({
  newpassword:'',
  newpasswords:'',
  password:''
})

const dialogFormInsertWorker= ref(false);
const updatepassword = (row) => {
  dialogFormInsertWorker.value = true;
   console.log(worker.value.id)

};
const closeWorker = async () => {
    try {
      // 验证密码并执行更新 旧密码在后端验证
      if (workerpwd.value.newpassword !== workerpwd.value.newpasswords) {
        
        console.log("2次新密码不同")
      }
      else{
        const response = await axios.post('https://localhost:44341/api/Home/Updatapassword', {
        id:worker.value.id,
        oldPassword: workerpwd.value.password,
        newPassword: workerpwd.value.newpassword
      });
      
      // 处理响应，例如显示成功消息
      console.log(response.data);
      
      // 关闭对话框
      dialogFormInsertWorker.value = false;
      }
      
    } catch (error) {
      // 处理错误，例如显示错误消息
      console.error('更新失败', error);
    }
  };

  
const openDialog = () => {
  dialogFormInsertWorker.value = false;
}

//监听函数
watch(route, (to, from) => {
  getBreadcrumb(to.matched);
});
onMounted(() => { //生命周期 挂载完成
  getBreadcrumb(route.matched);
  // 解析token
  const token = localStorage.getItem('token');
  const decoded = jwtDecode(token);
  // 获取解析后的信息
  console.log(decoded)
  const userId = decoded.nameid;
  const userName = decoded.name;
  const userRole = decoded.role;
  currentUser.value = userName
  currentUserID.value = userId
})
function getBreadcrumb(matched) {
  lists.value = matched;
}

const logout = () => {
  // 清除token
  localStorage.removeItem('token');
  // 跳转到登录页面
  router.push('/');
}



const drawers = async () => {
  drawer.value = true
  console.log(currentUserID.value)
  try {
    const response = await axios.get(`https://localhost:44341/api/Home/GetUserById?id=${parseInt(currentUserID.value)}`);

    console.log(response.data)
    worker.value=response.data
    console.log( worker.value)

  } catch (error) {
    console.error("Error fetching data:", error)
  }
}



const updatephone = async (row) => {
 
 console.log(currentUserID.value)
 console.log( worker.value)

 try {
        const response = await axios.post('https://localhost:44341/api/Home/UpdateWorker',
        worker.value
        );
        console.log(worker.value)

        console.log(response.data)
        ElMessage({
            message: (response.data),
            type: 'success',
        })
     
       
       
    } catch (error) {
        console.error("Error fetching data:", error)
    }
}


</script>
<style>
/* 面包屑 */
.breadcrumb {
  padding: 20px;
  display: inline-block;

}

.breadcrumb ul li {
  display: inline-block;
}

.breadcrumb ul li span {
  padding: 0 5px;
}

.el-carousel__item img {
  color: #0e0e0e;
  font-size: 14px;
  opacity: 0.75;
  line-height: 200px;
  margin: 0;
  max-width: 100%;
}

/* #module .breadcrumb .el-breadcrumb__item .el-breadcrumb__inner ,
#module .breadcrumb .el-breadcrumb__item .el-breadcrumb__inner a {
  color: #0a0a0a;
} */

.el-carousel__item:nth-child(2n) {
  background-color: #050505;
}

.el-carousel__item:nth-child(2n+1) {
  background-color: #0c0c0c;
}

.drawer {
  text-align: center;
}

.drawer .el-drawer__title {

  font-size: 24px;
  color: #333;
  margin-bottom: 20px;
}

.form-message {
  margin-top: 50px;

}

.form-btn {
  margin-top: 55px;
}
</style>