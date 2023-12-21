<template>
    <el-card class="filter-container" shadow="never">
        <div>
            <i class="el-icon-search"></i>
            <span>筛选搜索</span>
            <el-button style="float: right" @click="searchBtn2" type="primary" size="small">
                查询结果
            </el-button>
            <el-button style="float: right;margin-right: 15px" size="small" @click="resetForm">
                重置
            </el-button>
        </div>
        <div style="margin-top: 15px">
            <el-form :inline="true" :model="listQuery" size="small" label-width="140px">
                <el-form-item label="输入搜索：">
                    <el-input style="width: 203px" v-model="keyword.itemName" placeholder="商品名称"></el-input>
                </el-form-item>
                <el-form-item label="客户名称：">
                    <el-input style="width: 203px" v-model="keyword.customerName" placeholder="客户名称"></el-input>
                </el-form-item>
                <el-form-item label="按时间查找：">
                    <el-date-picker class="input-width" v-model="keyword.date"  value-format="YYYY-MM-DD" type="date"
                        placeholder="请选择时间">
                    </el-date-picker>
                </el-form-item>
                <!-- <el-form-item label="商品分类：">
                    <el-select v-model="cateName">
                        <el-option v-for="cate in ProCates" :key="cate.id" :label="cate.cateName" :value="cate.cateName" />
                    </el-select>
                </el-form-item> -->
                <el-form-item label="业务员：">
                    <el-select placeholder="请选择业务员" v-model="userName" @click="handleCateChange">
                        <el-option v-for="cate in Users" :key="cate.id" :label="cate.userName" :value="cate.userName" />
                    </el-select>
                </el-form-item>
                <el-form-item label="订单编号">
                    <el-input style="width: 203px" v-model="keyword.id" placeholder="订单编号"></el-input>
                </el-form-item>

            </el-form>
        </div>
    </el-card>
    <el-card class="operate-container" shadow="never">
      <i class="el-icon-tickets"></i>
      <span>数据列表</span>
    </el-card>
    <div class="mt">
        <el-table :data="tableData" style="width: 100% ; position: relative; z-index: 1; "
            :row-class-name="tableRowClassName" @row-click="rowclick2">
            <el-table-column prop="id" label="订单编号" width="100" />
            <el-table-column prop="customerName" label="客户名称" width="180" />
            <el-table-column prop="itemName" label="商品名称" width="180" />
            <el-table-column prop="itemQuantity" label="数量" width="100" />
            <el-table-column prop="itemPrice" label="单价" width="80" />
            <el-table-column prop="amount" label="总额" width="100" />
            <el-table-column prop="userName" label="业务员" width="100" />
            <el-table-column prop="orderTime" label="出库时间" width="110">
                <template #default="scope">
                    <div style="display: flex; align-items: center">
                        <el-popover :width="300"
                            popper-style="box-shadow: rgb(14 18 22 / 35%) 0px 10px 38px -10px, rgb(14 18 22 / 20%) 0px 10px 20px -15px; padding: 20px;">
                            <template #reference>
                                {{ scope.row.orderTime.substring(0, 10) }}
                            </template>
                            <template #default>
                                <div class="demo-rich-conent" style="display: flex; gap: 16px; flex-direction: column">
                                    <div>
                                        {{ scope.row.orderTime }}
                                    </div>
                                </div>
                            </template>
                        </el-popover>
                    </div>
                </template>
            </el-table-column>
            <el-table-column prop="status" label="状态" width="100" />
            <el-table-column prop="remark" label="备注" width="100" />
            <el-table-column label="操作" width="130">
                <!-- <template #default="{ row }">
                    <el-button link type="primary" size="small"  @click="openDialog(row)" :disabled="row.isRowEditable">编辑</el-button>
                    <el-button link type="primary" size="small" @click="DelClick">删除</el-button>
                </template> -->
                <template #default="{ row }">
                    <el-button type="primary" :icon="Edit" circle @click="openDialog(row)"
                       ></el-button>
                    <el-button type="danger" :icon="Delete" circle  @click="DelClick(row)"></el-button>
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

    <el-dialog v-model="dialogFormVisible" title="编辑页面" :style="{ 'z-index': 2, position: 'relative' }">
        <el-form :model="form">
            <el-form-item label="订单编号" :label-width="formLabelWidth" >
                <el-input v-model="form.id" autocomplete="off" disabled/>
            </el-form-item>
            <el-form-item label="客户名称" :label-width="formLabelWidth">
                <el-input v-model="form.customerName" autocomplete="off" />
            </el-form-item>
            <el-form-item label="商品名称" :label-width="formLabelWidth">
                <el-input v-model="form.itemName" autocomplete="off" />
            </el-form-item>
            <el-form-item label="数量" :label-width="formLabelWidth">
                <el-input v-model="form.itemQuantity" autocomplete="off" />
            </el-form-item>
            <el-form-item label="单价" :label-width="formLabelWidth">
                <el-input v-model="form.itemPrice" autocomplete="off" />
            </el-form-item>
            <el-form-item label="总额" :label-width="formLabelWidth">
                <el-input v-model="form.amount" autocomplete="off" />
            </el-form-item>
            <el-form-item label="业务员" :label-width="formLabelWidth">
                <el-input v-model="form.userName" autocomplete="off" />
            </el-form-item>
            <el-form-item label="状态" :label-width="formLabelWidth">
                <el-input v-model="form.status" autocomplete="off" />
            </el-form-item>
            <el-form-item label="备注" :label-width="formLabelWidth">
                <el-input v-model="form.remark" autocomplete="off" />
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
</template>
<script setup>
import { onMounted, ref, reactive } from 'vue'
import axios from 'axios';
import { ElMessage } from 'element-plus'
import { Delete, Edit,} from '@element-plus/icons-vue'

const tableData = ref([])



const formLabelWidth = ref('120px');
const ProCates = ref([])
const cateName = ref('')
const Users = ref([])
const userName = ref('')
const keyword = reactive({
    customerName: "",
    id: "",
    itemName: "",
    userName: "",
    date:""
})
console.log(userName)

const handleCateChange = () => {
    keyword.userName = userName
    console.log(keyword.userName)
    console.log(userName)
};


const ProCate = async () => {
    console.log(1111)
    try {
        const response = await axios.get('https://localhost:44341/api/Home/GetProCate');
        ProCates.value = response.data
        console.log(response.data)
        console.log(ProCates.value)
    } catch (error) {
        console.error('获取 ProCates 时出错：', error);
    }
}
const GetUsers = async () => {
    console.log(1111)
    try {
        const response = await axios.get('https://localhost:44341/api/Home/GetUser');
        Users.value = response.data
        console.log(response.data)
        console.log(Users.value)

    } catch (error) {
        console.error('获取 ProCates 时出错：', error);
    }
}
onMounted(async () => {
    ProCate()
    GetUsers()
    Search();
})

const resetForm = () => {
    keyword.customerName = '';
    keyword.itemName = '';
    keyword.id = '';
    keyword.userName = '';
    keyword.date='';
    Search();
}


const form = ref({});

const dialogFormVisible = ref(false);

const openDialog = (row) => {
    dialogFormVisible.value = true;
    form.value = { ...row };
    console.log(form)
    console.log(form.value)

};

const closeDialog = () => {
    dialogFormVisible.value = false;
}





const Orderid = ref([])

const searchQuery = ref({
    currentPage: 1,
    pageSize: 10,
    total: 10
})

//页分行
const handleSizeChange = (number) => {
    Search();
    searchBtn2()
}
//指定页
const handleCurrentChange = (number) => {
    Search();
    searchBtn2()
}


const searchBtn2 = async () => {
    Search();
}
const rowclick2 = async (row) => {
    console.log(row.id)
    Orderid.value = row.id
    console.log(Orderid.value)

}
const DelClick = async (row) => {
    try {
        const token = localStorage.getItem('token');
        const response = await axios.post('https://localhost:44341/api/Home/DelOrder', 
        {id: parseInt(row.id),},
      {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
      }
        );
        console.log(response.data)
        ElMessage({
            message: (response.data),
            type: 'success',
        })
        closeDialog()
        // 删除成功后，刷新表格数据
        await Search();
    } catch (error) {
        ElMessage({
            message: "暂无权限",
            type: 'success',
        })
        console.error("Error :", error)
    }

}

const Search = async () => {
    var service = axios.create({ baseURL: "https://localhost:44341/" })
    try {
        const orderTimeParam = keyword.date ? `&orderTime=${encodeURIComponent(keyword.date)}` : '';
        let response = await service.get(`/api/Home/GetOrderShow?id=${encodeURIComponent(keyword.id)}&userName=${encodeURIComponent(keyword.userName)}${orderTimeParam}&itemName=${encodeURIComponent(keyword.itemName)}&customerName=${encodeURIComponent(keyword.customerName)}&PageIndex=${searchQuery.value.currentPage}&PageSize=${searchQuery.value.pageSize}`)
        let result = response.data
        if (result && result.list2) {
            tableData.value = result.list2
        }
        searchQuery.value.total = response.data.total;
        console.log("Total items:", response.data.total);
        console.log(tableData)

    } catch (error) {
        console.error("Error fetching data:", error)
    }
}
const closeDialogbtn = async (row) => {
    console.log(form.value)
    try {
        const token = localStorage.getItem('token');
        const response = await axios.post('https://localhost:44341/api/Home/UpdateOrder',
            form.value,{
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
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
        await Search();
    } catch (error) {
        ElMessage({
            message: "暂无权限",
            type: 'success',
        })
        console.error("Error", error)
    }
}

</script>
<style>
.el-table .warning-row {
    --el-table-tr-bg-color: var(--el-color-warning-light-9);
}

.el-table .success-row {
    --el-table-tr-bg-color: var(--el-color-success-light-9);
}

.pagination {
    text-align: center;
    position: fixed;
}

.mt {
    height: 400px;
    overflow-y: auto;
}
</style>