<template>
  <div class="mts">
    <div class="mt-1" style="width: 35%; height: 300px; ">
      <div class="mt-4" style="width: 98%">
        <el-input v-model="keyword1.input" placeholder="搜索客户" class="input-with-select">
          <template #prepend>
            <el-icon>
              <Search />
            </el-icon>
          </template>
        </el-input>
        <el-button @click="searchBtn1" type="primary">搜索</el-button>
      </div>
      <div class="table-1" style="border-right: 1px solid #ccc;">
        <el-table :data="tableData1" style="width: 98%" :row-class-name="tableRowClassName" @row-click="rowclick1">
          <el-table-column prop="customerName" label="客户名" width="200" />
          <el-table-column prop="customerAddress" label="客户地址" />
        </el-table>
      </div>
    </div>
    <div class="mt-2" style="width: 65%; height: 300px;">
      <div class="mt-4" style="width: 100%;">
        <el-input v-model="keyword2.input" placeholder="搜索商品" class="input-with-select">
          <template #prepend>
            <el-icon>
              <Search />
            </el-icon>
          </template>
        </el-input>
        <el-button @click="searchBtn2" type="primary">搜索</el-button>
      </div>
      <div class="table-2">
        <el-table :data="tableData2" style="width: 100%" :row-class-name="tableRowClassName" @row-click="rowclick2">
          <el-table-column prop="productName" label="商品名" width="180" />
          <el-table-column prop="productPrice" label="价格" width="180" />
          <el-table-column prop="productStock" label="库存" />
        </el-table>
      </div>
    </div>
  </div>
  <div class="mt-3">
    <el-card class="operate-container" shadow="never">
      <el-text class="mx-1" style="size: 30px;">订单列表</el-text>
    </el-card>
  </div>


  <div class="form-table">
    <el-form class="form">
      <div class="customer-info">
        <label>当前客户：</label>
        <label>{{ customerName }}</label>
      </div>
      <div class="salesperson-info">
        <label>当前业务员：</label>
        <label>{{ currentUser }}</label>
      </div>
      <el-button class="button3" @click="orderbtn" type="primary">确定</el-button>
    </el-form>
    <div class="table-3">
      <el-table :data="tableData4" style="width: 100%">
        <el-table-column prop="productName" label="商品名称" width="250">
          <template #default="{ row }">
            <el-input v-model="row.productName" />
          </template>
        </el-table-column>
        <el-table-column prop="productPrice" label="商品价格" width="150">
          <template #default="{ row }">
            <el-input v-model="row.productPrice" />
          </template>
        </el-table-column>
        <el-table-column prop="ItemQuantity" label="商品数量" width="150">
          <template #default="{ row }">
            <el-input v-model="row.ItemQuantity" />
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="总额" width="150">
          <template #default="{ row }">
            <span>{{ Amount(row) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="状态" width="150">
          <template #default="{ row }">
            <el-select v-model="row.Status">
              <el-option label="出库" value="出库" />
              <el-option label="退货" value="退货" />
            </el-select>
          </template>
        </el-table-column>

        <el-table-column prop="Remark" label="备注" width="120">
          <template #default="{ row }">
            <el-input v-model="row.Remark" />
          </template>
        </el-table-column>
        <el-table-column fixed="right" label="操作" width="120">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="DelClick(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>
<script setup>
import { onMounted, ref, reactive, toRefs } from 'vue'
import axios from 'axios';
import jwtDecode from 'jwt-decode';
import { ElMessage } from 'element-plus';

const tableData1 = ref([])
const keyword1 = reactive({
  input: ""
})
const tableData2 = ref([])
const keyword2 = reactive({
  input: ""
})
const currentUser = ref('');

const tableData3 = ref([])
const tableData4 = ref([])
const customerName = ref("")

const pageIndex = ref(1);
const pageSize = ref(6);
const totalItems = ref(0);

onMounted(() => { //生命周期 挂载完成
  // 解析token
  const token = localStorage.getItem('token');
  const decoded = jwtDecode(token);
  // 获取解析后的信息
  console.log(decoded)
  const userId = decoded.nameid;
  const userName = decoded.name;
  const userRole = decoded.role;
  currentUser.value = userName


})


//搜索客户名
async function searchBtn1() {
  var service = axios.create({ baseURL: "https://localhost:44341/" })
  try {
    let response = await service.get(`/api/Home/Customername?customername=${encodeURIComponent(keyword1.input)}`)
    tableData1.value = response.data
    console.log(tableData1.value)
    console.log(response.data)
  } catch (error) {
    // 处理错误
    console.error('搜索失败：', error);
  }
}
//搜索商品
async function searchBtn2() {
  var service = axios.create({ baseURL: "https://localhost:44341/" })
  try {
    let response = await service.get(`/api/Home/GetGoodShow?Keyword=${encodeURIComponent(keyword2.input)}&PageIndex=${pageIndex.value}&PageSize=${pageSize.value}`)
    let result = response.data
    if (result && result.list) {
      tableData2.value = result.list
    }
    totalItems.value = response.data.total;
    console.log(keyword2.input)
    console.log(tableData2)

  } catch (error) {
    // 处理错误
    console.error('搜索失败：', error);
  }
}
function rowclick1(row, column, event) {
  console.log('点击的行数据:', row);
  tableData3.value.push(row);
  console.log(tableData3.value)
  customerName.value = row.customerName
  console.log(customerName.value)
}

function rowclick2(row, column, event) {
  console.log('点击的行数据:', row);
  // 使用深拷贝创建新的对象
  const newRow = { ...row };

  console.log(row.proId)
  tableData4.value.push(newRow);
  console.log(tableData4.value[0])
  console.log(tableData4.value[0].productName)
}

function Amount(row) {

  return row.productPrice * row.ItemQuantity;
}
async function orderbtn() {
  const newDataArray = tableData4.value.map((item) => {
    const itemQuantity = parseFloat(item.ItemQuantity);
    const productPrice = parseFloat(item.productPrice);
    if (isNaN(itemQuantity) || isNaN(productPrice)) {
      console.error(item.ItemQuantity, item.productPrice);
      return null;
    }
    return {
      CustomerName: customerName.value,
      ItemName: item.productName,
      ItemQuantity: item.ItemQuantity,
      ItemPrice: item.productPrice,
      Amount: item.ItemQuantity * item.productPrice,
      UserName: currentUser.value,
      Status: item.Status,
      Remark: item.Remark
    };
  });
  console.log(newDataArray);
  var service = axios.create({ baseURL: "https://localhost:44341/" })
  try {
    const addOrderPromise = newDataArray.some(item => item.Status === '出库')
      ? service.post('/api/Redis/AddOrder', newDataArray.filter(item => item.Status === '出库'))
      : Promise.resolve();

    const addPartAfterSalePromise = newDataArray.some(item => item.Status === '退货')
      ? service.post('/api/Home/AddPartAfterSale', newDataArray.filter(item => item.Status === '退货'))
      : Promise.resolve();

    // 并发执行两个请求
    //await Promise.all([addOrderPromise, addPartAfterSalePromise]);
    const [addOrderResponse, addPartAfterSaleResponse] = await Promise.all([
        addOrderPromise,
        addPartAfterSalePromise
    ]);
    console.log('API 请求成功');
    ElMessage({
      message: addOrderResponse.data,
      type: 'success',
    })

    tableData4.value = [];
    onMounted()


  } catch (error) {
    // 处理错误
    console.error('API 请求失败：', error);
  }
}

function DelClick(row) {
  const index = tableData4.value.indexOf(row);
  if (index !== -1) {
    tableData4.value.splice(index, 1);
  }
};

</script>

<style>
* {
  padding: 0;
  margin: 0;
}

.mts {
  display: flex;
  width: 100%;
  height: 240px;


}

.mt-4 {
  display: flex;
}

.form-table {
  width: 100%;
  /* margin-top: -100px; */

}

.table-1 {

  height: 200px;
  overflow-y: auto;

}

.table-2 {

  height: 200px;
  overflow-y: auto;

}

.table-3 {
  width: 84.4%;
  height: 400px;
  /* 设置固定高度 */
  overflow-y: auto;
  /* 启用纵向滚动 */
  /* position: fixed; */
}

.form {
  width: 100%;
  height: 30px;
  background-color: #ebeef5;
  color: black;
  /* margin-top: 20px; */
  justify-content: space-between;
  display: flex;

}

.customer-info,
.salesperson-info {
  width: calc(50% - 25px);
  box-sizing: border-box;
}


/* .button3 {
  margin-left: auto;
  background-color: #4CAF50;
  color: white;
  font-family: 'Arial', sans-serif;
  font-size: 16px;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  white-space: nowrap;
} */

.button3:hover {
  background-color: #45a049;

}
</style>