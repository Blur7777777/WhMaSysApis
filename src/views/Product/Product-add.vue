<template>
    <div class="mts">
        <h3 class="title">添加商品</h3>
        <el-form>
            <el-form-item label="商品编号">
                <el-input type="text" v-model="Product.form.proid"></el-input>
            </el-form-item>
            <el-form-item label="商品名称">
                <el-input type="text" v-model="Product.form.productName"></el-input>
            </el-form-item>
            <el-form-item label="来货价格">
                <el-input v-model="Product.form.productPrice" />
            </el-form-item>
            <el-form-item label="售出价格">
                <el-input v-model="Product.form.newproductPrice" />
            </el-form-item>
            <el-form-item label="商品库存">
                <el-input v-model="Product.form.productStock" />
            </el-form-item>
            <el-form-item label="分类">
                <el-select v-model="cateName" @change="handleCateChange">
                    <el-option v-for="cate in ProCates" :key="cate.id" :label="cate.cateName" :value="cate.cateName" />
                </el-select>
            </el-form-item>
            <el-form-item label="供货商">
                <el-select v-model="supplierName" @change="handleSupplierChange">
                    <el-option v-for="cates in SupCates" :key="cates.id" :label="cates.supplierName"
                        :value="cates.supplierName"></el-option>
                </el-select>
            </el-form-item>
            <el-form-item>
                <el-button @click="resetForm">重置</el-button>
                <el-button @click="Add" type="primary">添加</el-button>
            </el-form-item>
        </el-form>
    </div>
</template>
<script setup>
import { onMounted, ref, reactive } from 'vue'
import axios from 'axios';
import { ElMessage } from 'element-plus'

const ProCates = ref([])
const SupCates = ref([])

onMounted(async () => {
    ProCate();
    SupCate();
})

const cateName = ref('')
const supplierName = ref('')
const ProCateid = ref(0)
const SupCateid = ref(0)

const Product = reactive({
    form: {
        proid: "",
        productName: "",
        productPrice: "",
        newproductPrice: "",
        productStock: "",
        categoryid: "",
        supplierid: ""
    }
})

const ProCate = async () => {
    console.log(1111)
    try {
        const response = await axios.get('https://localhost:44341/api/Home/GetProCate');
        ProCates.value = response.data
        console.log(response.data)
        console.log(ProCates.value)
    } catch (error) {
        console.error('出错：', error);
    }
}
const SupCate = async () => {
    console.log(1111)
    try {
        const response = await axios.get('https://localhost:44341/api/Home/GetSupCate');

        SupCates.value = response.data
        console.log(response.data)
        console.log(SupCates.value)
    } catch (error) {
        console.error('出错：', error);
    }
}

const handleCateChange = () => {
    const selectedCate = ProCates.value.find(cate => cate.cateName === cateName.value);
    if (selectedCate) {
        console.log('ID:', selectedCate.id);
        ProCateid.value = selectedCate.id
        console.log(ProCateid.value)
    }
};
const handleSupplierChange = () => {
    const selectedSupplier = SupCates.value.find(supplier => supplier.supplierName === supplierName.value);
    if (selectedSupplier) {
        console.log('ID:', selectedSupplier.id);
        SupCateid.value = selectedSupplier.id
        console.log(SupCateid.value)
    }
};
const Add = async () => {
    //Product.form.proid = parseInt(Product.form.proid)
    Product.form.categoryid = String(ProCateid.value);
    Product.form.supplierid = String(SupCateid.value);

    Product.form.newproductPrice = parseFloat(Product.form.newproductPrice);
    Product.form.productPrice = parseFloat(Product.form.productPrice);
    // Product.form.productStock = parseInt(Product.form.productStock)
    Product.form.proid = Number(Product.form.proid);
    Product.form.productStock = String(Number(Product.form.productStock));
    Product.form.productName = String(Product.form.productName);

    console.log(Product.form)
    try {
        const response = await axios.post('https://localhost:44341/api/Home/AddProduct', Product.form);

        console.log('添加成功', response.data);
        location.reload()
        ElMessage({
            message: (response.data),
            type: 'success',
        })
    } catch (error) {
        console.error('添加失败', error);
    }
}
const resetForm = () => {
    Product.form.productName = "";
    Product.form.productPrice = "";
    Product.form.productStock = "";
    Product.form.categoryid = "";
    Product.form.supplierid = "";
}



//https://localhost:44341/api/Home/GetProCate

</script>
<style>
.mts {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 600px;
}

.el-form-item {
    margin-bottom: 30px;
}

.el-input,
.el-select {
    width: calc(100% - 20px);
    margin-left: 20px;
}

.el-button {
    margin-top: 30px;
    margin-bottom: 30px;
}
</style>