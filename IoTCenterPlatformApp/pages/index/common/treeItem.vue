<template>
  <view>
    <view class="contianer" v-for="(item, index) in data" :key="item.id">
      <view :class="['contianer-name', {active: localIsIndex === item.Id && showIcon === true }]" @click="navClick(item.Id, item.Name)">
	    {{ '- ' + item.Name }}
		<custom-icons v-if="localIsIndex === item.Id && showIcon === true" iconsName="icon-wancheng" iconsSize="30rpx" iconsColor="#2371FF"></custom-icons>
		<custom-icons v-if="showIcon === false" iconsName="icon-a-youjiantouhong" iconsSize="20rpx"	iconsColor="#999999"></custom-icons>
	  </view>
      <view v-if="item.Children !=null && item.Children.length > 0">
        <tree-item :data="item.Children" :isIndex="localIsIndex" :showIcon="showIcon" @navIsIndex="navIsIndex"></tree-item>
      </view>
    </view>
  </view>
</template>

<script>
export default {
  name: 'TreeItem',
  props: {
    data: {
      type: Array,
      default: () => []
    },
	isIndex: {
		type: String,
	},
	showIcon: {
		type: Boolean
	}
  },
  data() {
  	return {
  		rangeList: [],
  		localIsIndex: ''
  	}
	
  },
  watch: {
      isIndex(newValue) {
        this.localIsIndex = newValue
      }
    },
  methods: {
	navClick(id, text) {
	  	this.localIsIndex = id;
		this.$emit('navIsIndex', id, text)
	},
	navIsIndex(id, text) {
		this.$emit('navIsIndex', id, text)
	},
  }
};
</script>
<style lang="less" scoped>
	.contianer{
		padding-left: 20rpx;
	}
	.contianer-name{
		font-size: 30rpx;
		color: #333333;
		font-weight: bold;
		display: flex;
		align-items: center;
		justify-content: space-between;
		padding: 20rpx 0;
	}
	.contianer-name.active{
		color: #2371FF;
	}
</style>