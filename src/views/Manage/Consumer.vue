<template>
  <div class="app-container">
    <el-card class="filter-container" shadow="never">
      <div>
        <i class="el-icon-search"></i>
        <span>筛选搜索</span>
        <el-button style="float:right" type="primary" @click="searchBtn" size="small">
          查询搜索
        </el-button>
        <el-button style="float:right;margin-right: 15px" size="small" @click="resetForm">
          重置
        </el-button>
      </div>
      <div style="margin-top: 15px">
        <el-form :inline="true" :model="listQuery" size="small" label-width="140px">
          <el-form-item label="输入搜索：">
            <el-input v-model="keyword.userName" class="input-width" placeholder="客户名称" clearable></el-input>
          </el-form-item>
          <el-form-item label="手机号：">
            <el-input style="width: 203px" v-model="keyword.phone" placeholder="手机号"></el-input>
          </el-form-item>
        </el-form>
      </div>
    </el-card>
    <el-card class="operate-container" shadow="never">
      <i class="el-icon-tickets"></i>
      <span>数据列表</span>
      <el-button size="mini" class="btn-add" @click="handleAdd()" style="margin-left: 90% "
        type="success">添加客户</el-button>
    </el-card>
    <div class="mt">
      <el-table :data="tableData" style="width: 100% ; position: relative; z-index: 1; "
        :row-class-name="tableRowClassName" @row-click="rowclick2">
        <el-table-column prop="id" label="客户编号" width="200" />
        <el-table-column prop="customerName" label="客户名称" width="200" />
        <el-table-column prop="customerPhone" label="联系方式" width="200" />
        <el-table-column prop="customerAddress" label="客户地址" width="200" />
        <el-table-column prop="dateAdded" label="合作时间" width="200">
          <template #default="scope">
            <div style="display: flex; align-items: center">
              <el-popover :width="300"
                popper-style="box-shadow: rgb(14 18 22 / 35%) 0px 10px 38px -10px, rgb(14 18 22 / 20%) 0px 10px 20px -15px; padding: 20px;">
                <template #reference>
                  {{ scope.row.dateAdded.substring(0, 10) }}
                </template>
                <template #default>
                  <div class="demo-rich-conent" style="display: flex; gap: 16px; flex-direction: column">
                    <div>
                      {{ scope.row.dateAdded }}
                    </div>
                  </div>
                </template>
              </el-popover>
            </div>
          </template>
        </el-table-column>
        <el-table-column prop="Operate" label="操作">
          <template #default="{ row }">
            <el-button type="primary" :icon="Edit" circle @click="openDialog(row)"></el-button>
            <el-button type="danger" :icon="Delete" circle @click="DelClick(row)"></el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>
    <div class="pagination">
      <el-pagination v-model:current-page="searchQuery.currentPage" v-model:page-size="searchQuery.pageSize"
        :page-sizes="[10, 15, 20, 30]" :small="small" :disabled="disabled" :background="background"
        layout="total, sizes, prev, pager, next, jumper" :total="searchQuery.total" @size-change="handleSizeChange"
        @current-change="handleCurrentChange" />
    </div>

    <el-dialog v-model="dialogFormVisible" title="修改供应商信息">
      <el-form :model="form">
        <el-form-item label="客户编号">
          <el-input v-model="form.id" disabled />
        </el-form-item>
        <el-form-item label="客户名称">
          <el-input v-model="form.customerName" />
        </el-form-item>
        <el-form-item label="客户电话">
          <el-input v-model="form.customerPhone" />
        </el-form-item>
        <el-form-item label="客户地址">
          <el-input v-model="form.customerAddress" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="closeDialog(row)">取消</el-button>
          <el-button type="primary" @click="closeDialogbtn(row)">
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>


    <el-dialog v-model="dialogFormInsertWorker" title="增加人员信息">
      <el-form :model="worker">
        <el-form-item label="客户名称">
          <el-input v-model="worker.customerName" />
        </el-form-item>
        <el-form-item label="客户电话">
          <el-input v-model="worker.customerPhone" />
        </el-form-item>
        <el-form-item label="客户地址">
          <el-input v-model="worker.customerAddress" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="cancelWorker(row)">取消</el-button>
          <el-button type="primary" @click="closeWorker(row)">
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>

  </div>
</template>
<script setup>
import { onMounted, ref, reactive } from 'vue'
import axios from 'axios';
import { ElMessage } from 'element-plus'
import { Delete, Edit, } from '@element-plus/icons-vue'

const searchQuery = ref({
  currentPage: 1,
  pageSize: 2,
  total: 10
})
const tableData = ref([])

const keyword = reactive({
  userName: "",
  phone: "",
})
//页分行
const handleSizeChange = (number) => {

  searchBtn()
}
//指定页
const handleCurrentChange = (number) => {

  searchBtn()
}
onMounted(async () => {
  searchBtn();
})


const searchBtn = async () => {
  var service = axios.create({ baseURL: "https://localhost:44341/" })
  try {
    let response = await service.get(`/api/Home/GetCustomer?CustomerName=${keyword.userName}&CustomerPhone=${keyword.phone}&PageIndex=${searchQuery.value.currentPage}&PageSize=${searchQuery.value.pageSize}`)
    let result = response.data
    if (result && result.list6) {
      tableData.value = result.list6
    }
    searchQuery.value.total = response.data.total;
    console.log("Total items:", response.data.total);
    console.log(tableData)

  } catch (error) {
    console.error("Error:", error)
  }
}

const form = ref({})
const worker = ref({})
const dialogFormVisible = ref(false);
const dialogFormInsertWorker = ref(false);
const openDialog = (row) => {
  dialogFormVisible.value = true;
  form.value = { ...row };
  console.log(form)
  console.log(form.value)

};

const closeDialog = () => {
  dialogFormVisible.value = false;
}
const closeDialogbtn = async (row) => {
  console.log(form.value)
  try {
    const token = localStorage.getItem('token');
    const response = await axios.post('https://localhost:44341/api/Home/UpdateCustomer',
      form.value,
      {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      }
    );
    console.log(form.value)

    console.log(response.data)
    ElMessage({
      message: (response.data),
      type: 'success',
    })
    closeDialog()
    // 删除成功后，刷新表格数据
    await searchBtn();
  } catch (error) {
    ElMessage({
      message: "暂无权限",
      type: 'success',
    })
    console.error("Error:", error)
  }
}

const DelClick = async (row) => {
  // const Orderid=row.id  
  console.log(row.id)
  try {
    const token = localStorage.getItem('token');
    const response = await axios.post('https://localhost:44341/api/Home/DeleteCustomer', {
      id: parseInt(row.id)
    },
      {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      });
    console.log(row.id)

    console.log(response.data)
    ElMessage({
      message: (response.data),
      type: 'success',
    })
    closeDialog()
    // 删除成功后，刷新表格数据
    await searchBtn();
  } catch (error) {
    ElMessage({
      message: "暂无权限",
      type: 'success',
    })
    console.error("Error:", error)
  }

}

const handleAdd = () => {
  dialogFormInsertWorker.value = true;

};
const cancelWorker = () => {
  dialogFormInsertWorker.value = false;
}

const closeWorker = async (row) => {
  console.log(worker.value)
  try {
    const token = localStorage.getItem('token');
    const response = await axios.post('https://localhost:44341/api/Home/Customer',
      worker.value,
      {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      }
    );
    console.log(worker.value)

    console.log(response.data)
    ElMessage({
      message: "添加成功",
      type: 'success',
    })
    cancelWorker()
    // 成功后，刷新表格数据
    await searchBtn();
  } catch (error) {
    ElMessage({
      message: "暂无权限",
      type: 'success',
    })
    console.error("Error :", error)
  }
}


const resetForm = () => {
  keyword.userName = '';
  keyword.phone = '';
  searchBtn();
}

</script>