<template>
    <el-col :span="12" :xs="24">
        <!-- <el-card class="box-card"> -->
        <div class="devInfoList">
          <!-- <div slot="header" class="clearfix"> -->
          <div>
            <ul class="list-ul">
              <li class="list-li-item">
                <div class="item_label">开发者id</div>
                <div class="item-right">{{ deveInfo.DevId }}</div>
              </li>
              <li class="list-li-item">
                <div class="item_label">开发者密钥</div>
                <div class="item-right">{{ deveInfo.SecKey }}</div>
              </li>
            </ul>
          </div>
          <!-- </el-card> -->
        </div>
      </el-col>
  </template>
  
  <script>
  export default {
    props: {
        deveInfo: {
            type: Object
        }
    },
    data() {
      return {
        
      }
    },
    computed: {},
    mounted() {
    },
    methods: {
      
    }
  };
  </script>
  <style lang="scss" scope>
  .devInfoList{
    .list-ul{
        .list-li-item{
            display: flex;
            align-items: center;
            .item_label{
                margin-right: 35px;
                width: 140px;
            }
        } 
    }
  }
  </style>