<template>
    <el-card class="filter-container" shadow="never">
        <div>
            <i class="el-icon-search"></i>
            <span>筛选搜索</span>
            <el-button style="float: right" @click="searchBtn2" type="primary" size="small">
                查询结果
            </el-button>
            <el-button style="float: right;margin-right: 15px" @click="resetForm">
                重置
            </el-button>
        </div>
        <div style="margin-top: 15px">
            <el-form :inline="true" :model="listQuery" size="small" label-width="140px">
                <el-form-item label="输入搜索：">
                    <el-input style="width: 203px" v-model="keyword.input" placeholder="商品名称"></el-input>
                </el-form-item>
                <el-form-item label="商品编号：">
                    <el-input style="width: 203px" v-model="keyword.Proid" placeholder="商品编号"></el-input>
                </el-form-item>
                <el-form-item label="商品分类：">
                    <el-select v-model="cateName" @change="handleCateChange">
                        <el-option v-for="cate in ProCates" :key="cate.id" :label="cate.cateName" :value="cate.cateName" />
                    </el-select>
                </el-form-item>
                <!-- <el-form-item label="商品品牌：">
                    <el-select placeholder="请选择品牌" clearable>
                        <el-option v-for="item in brandOptions" :key="item.value" :label="item.label" :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item> -->
                <!-- <el-form-item label="上架状态：">
                    <el-select placeholder="全部" clearable>
                        <el-option v-for="item in publishStatusOptions" :key="item.value" :label="item.label"
                            :value="item.value">
                        </el-option>
                    </el-select>
                </el-form-item> -->
                <!-- <el-form-item label="审核状态：">
                    <el-select placeholder="全部" clearable>
                        <el-option
                v-for="item in verifyStatusOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value">
              </el-option>
                    </el-select>
                </el-form-item> -->
            </el-form>
        </div>
    </el-card>

    <el-card class="operate-container" shadow="never">
      <i class="el-icon-tickets"></i>
      <span>数据列表</span>
    </el-card>

    <div class="mt">

    
    <el-table :data="tableData" style="width: 100%" :row-class-name="tableRowClassName" @row-click="rowclick">
        <el-table-column prop="id" label="商品编号" width="180" />
        <el-table-column prop="productName" label="商品名称" width="180" />
        <el-table-column prop="productPrice" label="价格" width="180" />
        <el-table-column prop="productStock" label="库存" width="180" />
        <el-table-column prop="categoryid" label="分类" width="140"/>
        <el-table-column prop="supplierid" label="供货商" width="220"/>
        <el-table-column label="操作" >
            <template #default="{ row }">
                    <el-button type="primary"  @click="openDialog(row)"
                        :disabled="row.isRowEditable" :icon="Edit" circle></el-button>
                    <el-button type="danger"  @click="DelClick" :icon="Delete" circle></el-button>
                </template>
        </el-table-column>
    </el-table>
</div>
    <div class="pagination">
        <el-pagination v-model:current-page="searchQuery.currentPage" v-model:page-size="searchQuery.pageSize"
            :page-sizes="[10, 20, 30, 50]" :small="small" :disabled="disabled" :background="background"
            layout="total, sizes, prev, pager, next, jumper" :total="searchQuery.total" @size-change="handleSizeChange"
            @current-change="handleCurrentChange" />
    </div>


    <el-dialog v-model="dialogFormVisible" title="编辑页面" :style="{ 'z-index': 2, position: 'relative' }">
                        <el-form :model="form">
                            <el-form-item label="商品编号" :label-width="formLabelWidth">
                                <el-input v-model="form.id" autocomplete="off" />
                            </el-form-item>
                            <el-form-item label="商品名称" :label-width="formLabelWidth">
                                <el-input v-model="form.productName" autocomplete="off" />
                            </el-form-item>
                            <el-form-item label="价格" :label-width="formLabelWidth">
                                <el-input v-model="form.productPrice" autocomplete="off" />
                            </el-form-item>
                            <el-form-item label="库存" :label-width="formLabelWidth">
                                <el-input v-model="form.productStock" autocomplete="off" />
                            </el-form-item>
                            <el-form-item label="分类id" :label-width="formLabelWidth">
                                <el-input v-model="form.categoryid" autocomplete="off" />
                            </el-form-item>
                            <el-form-item label="供货商" :label-width="formLabelWidth">
                                <el-input v-model="form.supplierid" autocomplete="off" />
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
// import  zhCn  from "element-plus/lib/locale/lang/zh-cn"
import { ElMessage } from 'element-plus'
const searchQuery = ref({
    currentPage: 1,
    pageSize: 5,
    total: 10
})
import { Delete, Edit,} from '@element-plus/icons-vue'

const form = ref({});
const formLabelWidth = ref('120px');
const ProCates = ref([])
const cateName = ref('')
const tableData = ref([])
const ProCateid = ref('')
const Productid = ref(0)

const keyword = reactive({
    input: "",
    Proid:""
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
onMounted(async () => {
    Search();
    ProCate()
})
const ProCate = async () => {
    console.log(1111)
    try {
        const response = await axios.get('https://localhost:44341/api/Home/GetProCate');
        ProCates.value = response.data
        console.log(response.data)
        console.log(ProCates.value)
    } catch (error) {
        console.error('获取ProCates时出错：', error);
    }
}

const handleCateChange = () => {
    const selectedCate = ProCates.value.find(cate => cate.cateName === cateName.value);
    if (selectedCate) {
        console.log('Selected Category ID:', selectedCate.id);
        ProCateid.value = selectedCate.id
        console.log(ProCateid.value)
    }
};

const searchBtn2 = async () => {
    Search();
}

const Search = async () => {
    var service = axios.create({ baseURL: "https://localhost:44341/" })
    try {
        let response = await service.get(`/api/Home/GetGoodShow?Keyword=${encodeURIComponent(keyword.input)}&Proid=${keyword.Proid}&Cateid=${ProCateid.value}&PageIndex=${searchQuery.value.currentPage}&PageSize=${searchQuery.value.pageSize}`)
        let result = response.data
        if (result && result.list) {
            tableData.value = result.list
        }
        searchQuery.value.total = response.data.total;
        console.log("Total:", response.data.total);
        console.log(tableData)

    } catch (error) {
        console.error("Error", error)
    }
}




const rowclick = async (row) => {
    console.log(row.id)
    Productid.value = row.id
    console.log(Productid.value)
}
const DelClick = async () => {
    // const Orderid=row.id  
    console.log(Productid.value)
    try {
        const response = await axios.post('https://localhost:44341/api/Home/DelProduct',
            {
                id: parseInt(Productid.value)
            });
        console.log(Productid.value)

        console.log(response.data)
        // 删除成功后，刷新表格数据
        await Search();
    } catch (error) {
        console.error("Error fetching data:", error)
    }

}

const resetForm = () => {
  keyword.input = '';
  keyword.input='';
  ProCateid.value='';
  cateName.value='';
  Search();
}

// tableData.value.forEach(row => {
//     row.isRowEditable = false;
// });

// const openDialog = (row) => {
//     form.value = { ...row };
//     console.log(form)
//     console.log(form.value)
//     // 禁用其他行的编辑按钮
//     tableData.value.forEach(item => {
//         if (item !== row) {
//             item.isRowEditable = false;
//         }
//     });
//     row.dialogFormVisible = true;
// };


const dialogFormVisible = ref(false);

const openDialog = (row) => {
    dialogFormVisible.value = true;
    form.value = { ...row };
    console.log(form)
    console.log(form.value)
};

const closeDialog = (row) => {
    dialogFormVisible.value = false;
}

const closeDialogbtn = async (row) => {
    console.log(form.value)
    try {
        const token = localStorage.getItem('token');
        const response = await axios.post('https://localhost:44341/api/Home/Updateproduct',
            form.value,{
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            }
        );
        console.log(form.value)

        console.log(response.data)

        await Search();
        ElMessage({
            message: (response.data),
            type: 'success',
        })
        dialogFormVisible.value = false;
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
.mt{
    height: 400px; 
  overflow-y: auto;  
}


</style>