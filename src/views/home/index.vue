<template>
    <div class="module-box">
        <div @click="goto(v.url)" class="box" :style="`flex: 0 1 ${100 / lists.length}%;margin:5px;`"
            v-for="(v, i) in lists" :key="i">
            <div class="list-box">
                <i><el-icon>
                        <component :is="v.icon"></component>
                    </el-icon></i>
                <p>{{ v.title }}</p>
            </div>
        </div>
    </div>
    <div class="total-layout">
        <el-row :gutter="20">
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_home_order" class="total-icon">
                    <div class="total-title">今日订单总数</div>
                    <div class="total-value">共{{ todaycount }}条</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_money" class="total-icon">
                    <div class="total-title">今日销售总额</div>
                    <div class="total-value">￥{{ todayamount }}</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_home_order" class="total-icon">
                    <div class="total-title">昨日订单总数</div>
                    <div class="total-value">共{{ yesterdaycount }}条</div>
                </div>
            </el-col>
            <el-col :span="6">
                <div class="total-frame">
                    <img :src="img_money" class="total-icon">
                    <div class="total-title">昨日销售总额</div>
                    <div class="total-value">￥{{ yesterdayamount }}</div>
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
        <div style="flex: 0 1 40%;margin: 5px;">
            <el-card style=" height: 400px;">
                <div ref="chart2" style=" height: 300px;"></div>
            </el-card>
        </div>
    </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from "vue-router";  //导入路由
import axios from 'axios';
import * as echarts from 'echarts'
import {
    ChromeFilled,
    Opportunity,
    SwitchFilled,
    TrendCharts,
    VideoCameraFilled
} from '@element-plus/icons-vue'

import img_home_order from '@/assets/img/订单.png';
import img_money from '@/assets/img/人民币.png';

const router = useRouter();

const lists = ref([
    { title: '订单管理', icon: ChromeFilled, url: "/Order/leave" },
    { title: '商品管理', icon: Opportunity, url: "/Product/Products" },
    { title: '售后管理', icon: SwitchFilled, url: "/Order/parts" },
    { title: '人员管理', icon: TrendCharts, url: "/Manage/Consumer" },
    { title: '统计分析', icon: VideoCameraFilled, url: "/CusSer/battery" }
])

const yesterdayamount = ref('')
const todayamount = ref('')
const Total7day = ref([])
const todaycount = ref('')
const yesterdaycount = ref('')

const chart = ref()
const chart2 = ref()

const xAxisData = ref([])
const seriesData = ref([])

// onMounted(() => {
//     var myChart = echarts.init(chart.value);

//     var option = {
//         xAxis: {
//             type: 'category',
//             // data: [1,1,1,1]
//             data: xAxisData
//         },
//         yAxis: {
//             type: 'value'
//         },
//         series: [
//             {
//                 data:seriesData,
//                 // data: [2,2,2,2,2],
//                 type: 'bar'
//             }
//         ]
//     };

//     option && myChart.setOption(option);
// })




onMounted(async () => {
    Todayamount()
    dayamount7()
    Yesterdayamount()
    TodayCount()
    YesterdayCount()
    MonthCount()
    init2();
    init3()
})

const dayamount7 = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/GetDailyOrderTotal7');
        Total7day.value = response.data
        console.log(Total7day)
        console.log(response.data)
        xAxisData.value = Total7day.value.map(item => {
            const date = new Date(item.Date);
            const formattedDate = date.toLocaleDateString('en-US', {
                month: 'short', // 使用缩写的月份名称
                day: 'numeric'  // 显示数字形式的日期
            });
            return formattedDate;
        });// 获取日期数组
        seriesData.value = Total7day.value.map(item => item.Total); // 获取总数数组
        console.log(xAxisData)
        console.log(seriesData)
        init2()
        
    } catch (error) {
        console.error("出错了", error)
    }
}

const Todayamount = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/GetDailyOrderTotal');
        todayamount.value = response.data
        console.log(todayamount)
        console.log(response.data)

    } catch (error) {
        console.error("出错了:", error)
    }
}

const Yesterdayamount = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/GetyesterdayTotal');
        yesterdayamount.value = response.data
        console.log(yesterdayamount)
        console.log(response.data)

    } catch (error) {
        console.error("出错了:", error)
    }
}

const TodayCount = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/GetDailyOrderCount');
        todaycount.value = response.data
        console.log(todaycount)
        console.log(response.data)

    } catch (error) {
        console.error("出错了:", error)
    }
}

const YesterdayCount = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/GetYesterdayCount');
        yesterdaycount.value = response.data
        console.log(yesterdaycount)
        console.log(response.data)

    } catch (error) {
        console.error("出错了:", error)
    }
}

const init2 = () => {

    const xAxisDatavalue = xAxisData.value
    console.log(xAxisDatavalue)
    const seriesDatavalue = seriesData.value
    console.log(seriesDatavalue)


    var myChart = echarts.init(chart.value);

    var option = {
        xAxis: {
            type: 'category',
            // data: [1,1,1,1]
            data: xAxisDatavalue
        },
        yAxis: {
            type: 'value'
        },
        series: [
            {
                data: seriesDatavalue,
                // data: [2,2,2,2,2],
                type: 'bar'
            }
        ]
    };

    option && myChart.setOption(option);

}


const monthCount = ref('')
const Catename = ref([])
const Ordercount = ref('')

const MonthCount = async () => {
    try {
        const response = await axios.get('https://localhost:44341/api/Redis/GetMonthCount');
        monthCount.value = response.data
        console.log(response.data)
        Catename.value = monthCount.value.map(item => item.CategoryName)// 获取配件分类名
        Ordercount.value = monthCount.value.map(item => item.OrderCount); // 获取个条数数组
        console.log(xAxisData)
        console.log(seriesData)
        console.log(Catename.value)
        init3()
    } catch (error) {
        console.error("出错了", error)
    }
}



const init3 = () => {
    const Catenamevalue = Catename.value
    console.log(Catenamevalue)
    const Ordercountvalue =  Ordercount.value
    console.log(Ordercountvalue)

    var data = Catename.value.map((catename, index) => ({
        value: Ordercountvalue[index],
        name: catename
    }));
    console.log(data)
    var myChart = echarts.init(chart2.value);

    var option = {
        title: {
            text: '本月统计',
            subtext: 'Fake Data',
            left: 'center',
            textStyle: {
            color:  'rgba(0, 0, 0,1)'
        }
        },
        tooltip: {
            trigger: 'item',
           
        },
        legend: {
            orient: 'vertical',
            left: 'left',
            textStyle: {
            color:  'rgba(#202021,1)'
        }
        },
        series: [
            {
                name: 'Access From',
                type: 'pie',
                radius: '50%',
                data:  data ,
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };

    option && myChart.setOption(option);
}





const goto = (url) => {
    router.push({ path: url });
}

</script>
<style>
.total-layout {
    margin-top: 20px;
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

.module-box {
  display:flex;  /*弹性布局*/
  justify-content: space-between;
 
}

.list-box{
  margin: 0 10px;
  padding: 20px;
  box-shadow: 0 3px 20px 2px rgb(0 0 0 / 30%);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #fff;
}
.list-box i {
  font-size: 40px;
}
.box:nth-child(1) .list-box {
  background-image:linear-gradient(#2277ce,#0356ab);
}
.box:nth-child(2) .list-box {
  background-image:linear-gradient(#ba5bd8,#9f3fbd);
}
.box:nth-child(3) .list-box{
  background-image:linear-gradient(#ea8052,#cf673a);
}
.box:nth-child(4) .list-box {
  background-image:linear-gradient(#84f395,#71c24a);
}
.box:nth-child(5) .list-box {
  background-image:linear-gradient(#A44A38,#902569);
}
.marginBottom {
  margin-bottom: 10px;
}
.pagination{
  margin: 10px 0;
}   
</style>