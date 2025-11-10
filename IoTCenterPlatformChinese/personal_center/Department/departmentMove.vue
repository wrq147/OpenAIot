<template>
    <view>
		<top leftIcon="icon-fanhui" :isleftBack="true"  backgroundColor="#ffffff"
			title="移动至部门" class="addPool-top"></top>
        <view class="uni-list">
			<radio-group @change="radioChange">
				<label class="uni-list-cell uni-list-cell-pd" v-for="(item, index) in deptList" :key="index">
					<view>
						<radio :value="item.deptId.toString()" />
					</view>
					<view class="deptInfo">
						<view class="iconfont icon-bumenguanli-bumentubiao"></view>
					    {{item.deptName}}
					</view>
				</label>
			</radio-group>
		</view>
		<view class="btn">
			<view class="btnInfo">
				<button type="primary" @click="save">确定</button>
			</view>
		</view>
	</view>
</template>
<script>
import { DeptamentList, deptMove } from "@/api/personalCenter";
export default {
	data() {
		return {
			deptList: [],
			parentId: '',
			current: 0,
			uids: [],
			deptId: ''
		}
	},
	onLoad(option) {
		console.log(option)
		if(option.uids) {
			this.uids = JSON.parse(option.uids);
			this.deptId = option.deptId;
			this.getDeptament();
		}
	},
	methods: {
		getDeptament() {
			DeptamentList().then(res => {
				if(res.code === 0) {
					this.deptList = res.data;
				}
			})
		},
		radioChange(e) {
			this.parentId = e.detail.value
		},
        save() {
            if(this.parentId) {
                deptMove({ uids: this.uids, ids: [], parentId: this.parentId, memDeptId: this.deptId }).then(response => {
                    if(response.code === 0) {
                        uni.showToast({
                            title: '移动成功',
                            icon:'success',
                            duration: 2000
                        });
                        uni.navigateBack({ delta: 1 });
                    } else {
                        uni.showToast({
                            title: response.message,
                            icon: 'none',
                            duration: 2000
                        });
                    }
                });
            } else {
                uni.showToast({
                    title: '请选择所要移动的部门',
                    icon: 'none',
                    duration: 2000
                });
            }
        }
	}
}
</script>
<style lang="less" scoped>
	page{
		background: #ffffff;
	}
	.uni-list{
		padding: 30rpx;
	}
	.uni-list-cell {
		display: flex;
		align-items: center;
		justify-content: flex-start;
		margin-bottom: 25rpx;
	}
	.deptInfo{
		display: flex;
		align-items: flex-start;
		margin-left: 18rpx;
	}
	.icon-bumenguanli-bumentubiao {
		width: 35rpx;
		height: 35rpx;
		margin-right: 14rpx;
		font-weight: normal;
	}
	.btn{
		position: fixed;
		bottom: 0;
		left: 0;
		width: 100%;
	}
	.btnInfo{
		display: flex;
		justify-content: space-around;
		padding: 20rpx;
	}
	.btn button{
		width: 100%;
	}
</style>