<template>
    <div class="total-layout">
        <el-row :gutter="20">
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_home_order" class="total-icon">
                    <div class="total-title">本月总营业额</div>
                    <div class="total-value">￥{{ MonthAmount }}</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_money" class="total-icon">
                    <div class="total-title">本月总订单数</div>
                    <div class="total-value">共{{ MonthTotal }}条</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_home_order" class="total-icon">
                    <div class="total-title">总成本价</div>
                    <div class="total-value">￥{{ TotalCost }}</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_money" class="total-icon">
                    <div class="total-title">本月总利润</div>
                    <div class="total-value" style="color: red;">￥{{ Profit }}</div>
                </div>
            </el-col>

        </el-row>
    </div>
    <div class="total-layout">
        <el-row :gutter="20">
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_home_order2" class="total-icon">
                    <div class="total-title">上月总营业额</div>
                    <div class="total-value">￥{{ LastMonthAmount }}</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_money2" class="total-icon">
                    <div class="total-title">上月总订单数</div>
                    <div class="total-value">共{{ LastMonthTotal }}条</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_home_order2" class="total-icon">
                    <div class="total-title">总成本价</div>
                    <div class="total-value">￥{{ LastTotalCost }}</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_money2" class="total-icon">
                    <div class="total-title">上月总利润</div>
                    <div class="total-value" style="color: red;">￥{{ LastProfit }}</div>
                </div>
            </el-col>

        </el-row>
    </div>
    <div class="module-box">
        <div style="flex: 0 1 60%;margin: 5px;">
            <el-card style=" height: 400px;">
                <div ref="chart" style=" height: 300px;"></div>
            </el-card>
        </div>
    </div>
</template>
<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from "vue-router";  //导入路由
import axios from 'axios';
import * as echarts from 'echarts'
import img_home_order from '@/assets/img/订单.png';
import img_money from '@/assets/img/人民币.png';
import img_home_order2 from '@/assets/img/订单2.png';
import img_money2 from '@/assets/img/人民币2.png';


const MonthAmount = ref('')
const MonthTotal = ref('')
const TotalCost = ref('')
const Profit = ref('')
const LastMonthAmount = ref('')
const LastMonthTotal = ref('')
const LastTotalCost = ref('')
const LastProfit = ref('')

const monthdata = ref([])
const Lastmonthdata = ref([])


const monthProTotal = ref([])
const Product = ref('')
const itemQuantity = ref('')


const chart = ref()
const chart2 = ref()


onMounted(async () => {
    MonthCount()
    LastMonthCount()
    init2()
    MonthProTotal()
})




const MonthCount = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/MonthAmount');
        monthdata.value = response.data
        console.log(response.data)
        console.log(monthdata.value)
        console.log(monthdata.value[0].totalCost)
        TotalCost.value = monthdata.value[0].totalCost
        MonthAmount.value = monthdata.value[0].totalRevenue
        MonthTotal.value = monthdata.value[0].totalOrders
        Profit.value = monthdata.value[0].profit
    } catch (error) {
        console.error("出错了", error)
    }
}
const LastMonthCount = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/LastMonthAmount');
        Lastmonthdata.value = response.data
        console.log(response.data)
        console.log(Lastmonthdata.value)
        LastTotalCost.value = Lastmonthdata.value[0].totalCost
        LastMonthAmount.value = Lastmonthdata.value[0].totalRevenue
        LastMonthTotal.value = Lastmonthdata.value[0].totalOrders
        LastProfit.value = Lastmonthdata.value[0].profit
    } catch (error) {
        console.error("出错了", error)
    }
}
const MonthProTotal = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/MonthProTotal');
        monthProTotal.value = response.data
        console.log(monthProTotal.value)
        console.log(response.data)
        Product.value = monthProTotal.value.map(item => item.itemName)// 获取配件名
        itemQuantity.value = monthProTotal.value.map(item => item.itemQuantity); // 获取数量
        // 倒序排列数据
        Product.value.reverse();
        itemQuantity.value.reverse();
        console.log(Product.value)
        console.log(11111111111111111111111)
        console.log(itemQuantity.value)
        init2()
    } catch (error) {
        console.error("出错了", error)
    }
}

const init2 = () => {


    var myChart = echarts.init(chart.value);

    var option = {
        title: {
            text: '本月热销商品'
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        legend: {},
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: {
            type: 'value',
            boundaryGap: [0, 0.01]
        },
        yAxis: {
            type: 'category',
            data: Product.value
        },
        series: [
            {
                name: '售出数量',
                type: 'bar',
                data: itemQuantity.value
            },

        ]
    };

    option && myChart.setOption(option);

}







</script>
<style>
.total-layout {
    margin-top: 20px;
    width: 97%;
}

.total-frame {
    border: 1px solid #9398a2;
    padding: 20px;
    height: 100px;
}

.total-icon {
    color: #101011;
    width: 60px;
    height: 60px;

}

.total-title {
    position: relative;
    font-size: 16px;
    color: #202021;
    left: 70px;
    top: -50px;
}

.total-value {
    position: relative;
    font-size: 18px;
    color: #202021;
    left: 70px;
    top: -40px;
}
</style>